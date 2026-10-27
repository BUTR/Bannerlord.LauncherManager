using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Bannerlord.LauncherManager.Native.Utils;

/// <summary>
/// JSON in and out through UTF-8 in native memory, freed as soon as it's read. JsonSerializer's string overloads rent
/// their UTF-8 buffers from ArrayPool&lt;byte&gt;.Shared, up to 3 bytes per char rounded up to a power of two, 4 MB for
/// a 300 module list. The pool keeps an array per size for every core and thread that rented one, trimmed only after a
/// minute unused and a full GC, which an idle process doesn't run: the native library kept ~12 MB of them. GC arrays
/// instead would land in the large object heap, the WebAssembly runtime's memory grew by the segments they took.
/// Shared by the native library and the WebAssembly runtime
/// </summary>
public static unsafe class NativeJson
{
    public delegate TResult Utf8Reader<out TResult>(ReadOnlySpan<byte> utf8);

    /// <exception cref="JsonException">Invalid JSON</exception>
    public static TValue? Deserialize<TValue>(ReadOnlySpan<char> json, JsonTypeInfo<TValue> jsonTypeInfo)
    {
        var length = Encoding.UTF8.GetByteCount(json);
        var utf8 = (byte*) NativeMemory.Alloc((nuint) Math.Max(length, 1));
        try
        {
            Encoding.UTF8.GetBytes(json, new Span<byte>(utf8, length));
            return JsonSerializer.Deserialize(new ReadOnlySpan<byte>(utf8, length), jsonTypeInfo);
        }
        finally
        {
            NativeMemory.Free(utf8);
        }
    }

    /// <summary>
    /// Writes the value's JSON as UTF-8 into native memory and hands it to the reader, the memory is freed after
    /// </summary>
    public static TResult Serialize<TValue, TResult>(TValue value, JsonTypeInfo<TValue> jsonTypeInfo, Utf8Reader<TResult> read)
    {
        using var buffer = new NativeBufferWriter();
        using (var writer = new Utf8JsonWriter(buffer, new JsonWriterOptions { Encoder = jsonTypeInfo.Options.Encoder, Indented = jsonTypeInfo.Options.WriteIndented, SkipValidation = true }))
        {
            JsonSerializer.Serialize(writer, value, jsonTypeInfo);
        }
        return read(buffer.WrittenSpan);
    }

    /// <summary>
    /// Writes the value's JSON as null-terminated UTF-8 into native memory and hands the memory over, the caller frees
    /// it with NativeMemory.Free
    /// </summary>
    public static byte* SerializeToNative<TValue>(TValue value, JsonTypeInfo<TValue> jsonTypeInfo, out int length)
    {
        using var buffer = new NativeBufferWriter();
        using (var writer = new Utf8JsonWriter(buffer, new JsonWriterOptions { Encoder = jsonTypeInfo.Options.Encoder, Indented = jsonTypeInfo.Options.WriteIndented, SkipValidation = true }))
        {
            JsonSerializer.Serialize(writer, value, jsonTypeInfo);
        }
        return buffer.Detach(out length);
    }

    /// <summary>
    /// An IBufferWriter over native memory, freed on Dispose. Utf8JsonWriter asks for Memory, so it's its own
    /// MemoryManager; a Memory handed out stays valid across a grow, it reads the current buffer
    /// </summary>
    private sealed class NativeBufferWriter : MemoryManager<byte>, IBufferWriter<byte>
    {
        private byte* _buffer = (byte*) NativeMemory.Alloc(16 * 1024);
        private int _capacity = 16 * 1024;
        private int _written;

        public ReadOnlySpan<byte> WrittenSpan => new(_buffer, _written);

        void IBufferWriter<byte>.Advance(int count) => _written += count;

        Memory<byte> IBufferWriter<byte>.GetMemory(int sizeHint)
        {
            Ensure(sizeHint);
            return CreateMemory(_written, _capacity - _written);
        }

        Span<byte> IBufferWriter<byte>.GetSpan(int sizeHint)
        {
            Ensure(sizeHint);
            return new Span<byte>(_buffer + _written, _capacity - _written);
        }

        private void Ensure(int sizeHint)
        {
            sizeHint = Math.Max(sizeHint, 1);
            if (_capacity - _written >= sizeHint)
                return;

            var capacity = Math.Max(_capacity * 2, _written + sizeHint);
            _buffer = (byte*) NativeMemory.Realloc(_buffer, (nuint) capacity);
            _capacity = capacity;
        }

        /// <summary>
        /// The written bytes and a null terminator, shrunk to them: the caller owns them, Dispose frees nothing
        /// </summary>
        public byte* Detach(out int length)
        {
            Ensure(1);
            _buffer[_written] = 0;
            var buffer = (byte*) NativeMemory.Realloc(_buffer, (nuint) _written + 1);
            length = _written;
            _buffer = null;
            return buffer;
        }

        public override Span<byte> GetSpan() => new(_buffer, _capacity);

        public override MemoryHandle Pin(int elementIndex = 0) => new(_buffer + elementIndex);

        public override void Unpin() { }

        protected override void Dispose(bool disposing)
        {
            NativeMemory.Free(_buffer);
            _buffer = null;
        }
    }
}
