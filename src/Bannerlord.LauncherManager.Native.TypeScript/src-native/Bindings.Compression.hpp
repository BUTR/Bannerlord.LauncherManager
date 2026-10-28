#ifndef VE_COMPRESSION_GUARD_HPP_
#define VE_COMPRESSION_GUARD_HPP_

#include <napi.h>

using namespace Napi;

namespace Bindings::Compression
{
Value DeflateRawChunkAsync(const CallbackInfo &info);

Object Init(const Env env, const Object exports);
} // namespace Bindings::Compression
#endif
