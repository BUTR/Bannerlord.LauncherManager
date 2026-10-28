#ifndef VE_COMPRESSION_IMPL_GUARD_HPP_
#define VE_COMPRESSION_IMPL_GUARD_HPP_

#include "Bindings.Compression.hpp"
#include "Logger.hpp"
#include "Utils.Generic.hpp"
#include <cstdint>
#include <isal/igzip_lib.h>
#include <limits>
#include <memory>
#include <napi.h>
#include <string>

using namespace Napi;
using namespace Utils;

namespace Bindings::Compression
{
// A stored block's header: its type byte, then LEN and NLEN
constexpr size_t StoredBlockHeaderSize = 5;
constexpr size_t StoredBlockMaxSize = 65535;

/**
 * Compresses one chunk of a raw deflate stream with ISA-L level 1, on libuv's thread pool. A chunk is compressed
 * without the ones before it: a chunk that isn't the last ends byte aligned without a final block (FULL_FLUSH), so the
 * chunks' outputs joined in order are one deflate stream. Memory stays bounded by the chunk, and chunks of one stream
 * can be compressed at the same time
 */
class DeflateRawChunkWorker : public AsyncWorker
{
    const Promise::Deferred deferred_;
    // Keeps the input alive while the worker reads it, JS must not change it until the promise settles
    const ObjectReference inputReference_;
    const uint8_t *const input_;
    const size_t inputSize_;
    const bool last_;
    std::unique_ptr<uint8_t[]> output_;
    size_t outputSize_ = 0;

  public:
    // Napi::Env: in the worker, Env is AsyncWorker::Env()
    DeflateRawChunkWorker(const Napi::Env env, const Uint8Array input, const bool last)
        : AsyncWorker(env), deferred_(Promise::Deferred::New(env)), inputReference_(Persistent(input.As<Object>())),
          input_(input.Data()), inputSize_(input.ByteLength()), last_(last)
    {
    }

    Promise GetPromise() const
    {
        return deferred_.Promise();
    }

  protected:
    void Execute() override
    {
        // The stream (80 KB) and the level's hash tables, reused by the pool's thread for its next chunks
        thread_local std::unique_ptr<isal_zstream> stream;
        thread_local std::unique_ptr<uint8_t[]> levelBuffer;
        if (!stream)
        {
            stream = std::make_unique<isal_zstream>();
            levelBuffer = std::make_unique<uint8_t[]>(ISAL_DEF_LVL1_DEFAULT);
        }

        // ISA-L stores what doesn't compress, its output is at most the input with stored blocks' headers
        const auto capacity = inputSize_ + (inputSize_ / StoredBlockMaxSize + 1) * StoredBlockHeaderSize + 64;
        output_ = std::make_unique<uint8_t[]>(capacity);

        isal_deflate_stateless_init(stream.get());
        stream->level = 1;
        stream->level_buf = levelBuffer.get();
        stream->level_buf_size = ISAL_DEF_LVL1_DEFAULT;
        stream->next_in = const_cast<uint8_t *>(input_);
        stream->avail_in = static_cast<uint32_t>(inputSize_);
        stream->next_out = output_.get();
        stream->avail_out = static_cast<uint32_t>(capacity);
        stream->end_of_stream = last_ ? 1 : 0;
        stream->flush = last_ ? NO_FLUSH : FULL_FLUSH;

        const auto result = isal_deflate_stateless(stream.get());
        if (result != COMP_OK)
        {
            SetError("ISA-L deflate failed with " + std::to_string(result));
            return;
        }
        outputSize_ = capacity - stream->avail_out;
    }

    void OnOK() override
    {
        // V8's sandbox can't take native memory, the output is copied into the buffer once
        deferred_.Resolve(Buffer<uint8_t>::Copy(Env(), output_.get(), outputSize_));
    }

    void OnError(const Error &error) override
    {
        deferred_.Reject(error.Value());
    }
};

Value DeflateRawChunkAsync(const CallbackInfo &info)
{
    LoggerScope logger(__FUNCTION__);
    return WithExceptionHandling(logger, [&]() {
        const auto env = info.Env();
        if (!info[0].IsTypedArray() || info[0].As<TypedArray>().TypedArrayType() != napi_uint8_array)
        {
            throw TypeError::New(env, "The chunk must be a Uint8Array");
        }
        const auto input = info[0].As<Uint8Array>();
        if (input.ByteLength() > std::numeric_limits<uint32_t>::max() / 2)
        {
            throw RangeError::New(env, "The chunk is too large");
        }
        const auto last = info[1].ToBoolean().Value();

        auto *const worker = new DeflateRawChunkWorker(env, input, last);
        const auto promise = worker->GetPromise();
        worker->Queue();
        return static_cast<Value>(promise);
    });
}

Object Init(const Env env, const Object exports)
{
    exports.Set("deflateRawChunkAsync", Function::New(env, DeflateRawChunkAsync));

    return exports;
}

} // namespace Bindings::Compression
#endif
