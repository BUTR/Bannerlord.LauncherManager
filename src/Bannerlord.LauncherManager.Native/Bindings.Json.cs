using Bannerlord.LauncherManager.Native.Utils;

using BUTR.NativeAOT.Shared;

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.Native;

// JSON crosses as UTF-8: a parameter as null-terminated param_data, a result as return_value_data. The addon reads a JS
// string as UTF-8 and the serializer reads UTF-8, nothing converts in between. A result's bytes go to V8 as they are:
// the serializer escapes everything outside ASCII, so they're Latin-1 too, which V8 keeps as an external string.
// Through native memory instead of BUTR.NativeAOT.Shared's string and ArrayPool based helpers, see NativeJson
public static unsafe partial class Bindings
{
    /// <summary>
    /// The JSON of a value as a return_value_data, its memory goes to the receiver, see SerializeJsonNative
    /// </summary>
    internal static return_value_data* JsonValue<TValue>(TValue? value, JsonTypeInfo<TValue> jsonTypeInfo, bool isOwner) where TValue : class
    {
        var json = SerializeJsonNative(value, jsonTypeInfo, out var length);
        return return_value_data.AsValue(json, length, isOwner);
    }

    /// <summary>
    /// JSON the caller made, an ASCII string, as a return_value_data
    /// </summary>
    internal static return_value_data* JsonValue(string json, bool isOwner)
    {
        var utf8 = (byte*) Allocator.Alloc((nuint) json.Length + 1);
        var length = Encoding.UTF8.GetBytes(json, new Span<byte>(utf8, json.Length));
        utf8[length] = 0;
        return return_value_data.AsValue(utf8, length, isOwner);
    }

    /// <summary>
    /// The JSON of a value in native memory, null-terminated UTF-8 the receiver frees with common_dealloc
    /// </summary>
    internal static byte* SerializeJsonNative<TValue>(TValue? value, JsonTypeInfo<TValue> jsonTypeInfo, out int length) where TValue : class
    {
        if (value is null)
        {
            length = 0;
            return null;
        }

        var json = NativeJson.SerializeToNative(value, jsonTypeInfo, out length);
#if TRACK_ALLOCATIONS
        // The Allocator frees only what it tracks
        var tracked = (byte*) Allocator.Alloc((nuint) length + 1);
        new ReadOnlySpan<byte>(json, length + 1).CopyTo(new Span<byte>(tracked, length + 1));
        NativeMemory.Free(json);
        return tracked;
#else
        return json;
#endif
    }

    /// <summary>
    /// The JSON of a value in native memory for a call that only reads it, freed when the handle is disposed
    /// </summary>
    internal static SafeDataMallocHandle SerializeJsonNativeHandle<TValue>(TValue? value, JsonTypeInfo<TValue> jsonTypeInfo) where TValue : class
    {
        var json = SerializeJsonNative(value, jsonTypeInfo, out var length);
        return SafeDataMallocHandle.Create(json, length, true);
    }

    /// <summary>
    /// A JSON parameter, read where it is
    /// </summary>
    [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(nameof(json))]
    internal static TValue? DeserializeJsonNative<TValue>(param_data* json, JsonTypeInfo<TValue> jsonTypeInfo, [CallerMemberName] string? caller = null) where TValue : class =>
        json is null ? null : DeserializeJsonNative(MemoryMarshal.CreateReadOnlySpanFromNullTerminated((byte*) json), jsonTypeInfo, caller);

    /// <summary>
    /// SafeStructMallocHandle.ValueAsJson for a JSON result
    /// </summary>
    internal static TValue? ValueAsJsonNative<TValue>(SafeStructMallocHandle<return_value_data> result, JsonTypeInfo<TValue> jsonTypeInfo, [CallerMemberName] string? caller = null) where TValue : class
    {
        var ptr = result.Value;
        if (ptr->Error is null)
        {
            using var json = new SafeDataMallocHandle(ptr->Value, ptr->Length, result.IsOwner);
            return json.IsInvalid ? null : DeserializeJsonNative(json.ToSpan(), jsonTypeInfo, caller);
        }

        using var hError = new SafeStringMallocHandle(ptr->Error, result.IsOwner);
        throw new NativeCallException(new string(hError));
    }

    /// <summary>
    /// SafeStructMallocHandle.SetAsJson for a JSON result
    /// </summary>
    internal static void SetAsJsonNative<TValue>(SafeStructMallocHandle<return_value_data> result, TaskCompletionSource<TValue?> tcs, JsonTypeInfo<TValue> jsonTypeInfo, [CallerMemberName] string? caller = null) where TValue : class
    {
        try
        {
            tcs.TrySetResult(ValueAsJsonNative(result, jsonTypeInfo, caller));
        }
        catch (Exception e)
        {
            tcs.TrySetException(e);
        }
    }

    private static TValue? DeserializeJsonNative<TValue>(ReadOnlySpan<byte> json, JsonTypeInfo<TValue> jsonTypeInfo, string? caller)
    {
        try
        {
            return JsonSerializer.Deserialize(json, jsonTypeInfo);
        }
        catch (JsonException e)
        {
            throw new JsonDeserializationException($"Failed to deserialize! Caller: {caller}, Type: {typeof(TValue)}; Json: {Encoding.UTF8.GetString(json)};", e);
        }
    }
}
