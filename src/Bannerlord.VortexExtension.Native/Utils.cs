using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Bannerlord.VortexExtension.Native
{
    internal static class Utils
    {
        public static unsafe char* SerializeJsonCopy<TValue>(TValue value, JsonTypeInfo<TValue> jsonTypeInfo)
        {
            return Copy(JsonSerializer.Serialize<TValue>(value, jsonTypeInfo));
        }

        public static unsafe string SerializeJson<TValue>(TValue value, JsonTypeInfo<TValue> jsonTypeInfo)
        {
            return JsonSerializer.Serialize<TValue>(value, jsonTypeInfo);
        }


        public static TValue DeserializeJson<TValue>(SafeStringMallocHandle json, JsonTypeInfo<TValue> jsonTypeInfo)
        {
            if (json.DangerousGetHandle() == IntPtr.Zero)
            {
                throw new JsonDeserializationException($"Received null parameter! Type: {typeof(TValue)};");
            }

            return DeserializeJson<TValue>((ReadOnlySpan<char>) json, jsonTypeInfo);
        }

        public static unsafe TValue DeserializeJson<TValue>(param_json* json, JsonTypeInfo<TValue> jsonTypeInfo)
        {
            if (json is null)
            {
                throw new JsonDeserializationException($"Received null parameter! Type: {typeof(TValue)};");
            }

            return DeserializeJson<TValue>(param_json.ToSpan(json), jsonTypeInfo);
        }

        private static TValue DeserializeJson<TValue>([StringSyntax(StringSyntaxAttribute.Json)] ReadOnlySpan<char> json, JsonTypeInfo<TValue> jsonTypeInfo)
        {
            try
            {
                if (JsonSerializer.Deserialize<TValue>(json, jsonTypeInfo) is not { } result)
                {
                    throw new JsonDeserializationException($"Received null! Type: {typeof(TValue)}; Json:{json};");
                }

                return result;
            }
            catch (JsonException e)
            {
                throw new JsonDeserializationException($"Failed to deserialize! Type: {typeof(TValue)}; Json:{json};", e);
            }
        }


        public static unsafe char* Copy(in ReadOnlySpan<char> str)
        {
            var size = (uint) ((str.Length + 1) * 2);

            var dst = (char*) NativeMemory.Alloc(new UIntPtr(size));
            str.CopyTo(new Span<char>(dst, str.Length));
            dst[str.Length] = '\0';
            return dst;
        }

        public static unsafe TValue* Create<TValue>(TValue value) where TValue : unmanaged
        {
            var size = Unsafe.SizeOf<TValue>();
            var dst = (TValue*) NativeMemory.Alloc(new UIntPtr((uint) size));
            MemoryMarshal.Write(new Span<byte>(dst, size), ref value);
            return dst;
        }
    }
}