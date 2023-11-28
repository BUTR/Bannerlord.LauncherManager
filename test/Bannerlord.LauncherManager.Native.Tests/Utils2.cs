using Bannerlord.LauncherManager.Models;

using BUTR.NativeAOT.Shared;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Text.Unicode;

namespace Bannerlord.LauncherManager.Native.Tests;

public class InstallInstructionJsonConverter : JsonConverter<IInstallInstruction>
{
    public override IInstallInstruction Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var readerCopy = reader;
        var data = JsonDocument.ParseValue(ref reader);
        return (Enum.TryParse<InstallInstructionType>(data.RootElement.GetProperty("type").GetString(), out var val) ? val : InstallInstructionType.None) switch
        {
            InstallInstructionType.Copy => JsonSerializer.Deserialize<CopyInstallInstruction>(ref readerCopy, options)!,
            InstallInstructionType.ModuleInfo => JsonSerializer.Deserialize<ModuleInfoInstallInstruction>(ref readerCopy, options)!,
            _ => new NoneInstallInstruction()
        };
    }

    public override void Write(Utf8JsonWriter writer, IInstallInstruction value, JsonSerializerOptions options) => throw new NotSupportedException();
}

internal static partial class Utils2
{
#if DEBUG
    public const string DllPath = "../../../../../src/Bannerlord.LauncherManager.Native/bin/Debug/net8.0/win-x64/native/Bannerlord.LauncherManager.Native.dll";
#else
        public const string DllPath = "../../../../../src/Bannerlord.LauncherManager.Native/bin/Release/net8.0/win-x64/native/Bannerlord.LauncherManager.Native.dll";
#endif


    [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe partial void* common_alloc(nuint size);
    [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe partial void common_dealloc(void* ptr);
    [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe partial int common_alloc_alive_count();

    private static readonly JsonSerializerOptions Options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        IgnoreReadOnlyFields = true,
        IgnoreReadOnlyProperties = true,
        IncludeFields = false,
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin),
        Converters =
        {
            new InstallInstructionJsonConverter(),
            new JsonStringEnumConverter(),
        }
    };
    internal static readonly SourceGenerationContext CustomSourceGenerationContext = new(Options);

    public static unsafe void LibrarySetAllocator() => Allocator.SetCustom(&common_alloc, &common_dealloc);
    public static int LibraryAliveCount() => common_alloc_alive_count();

    public static unsafe SafeStringMallocHandle ToString(string value) => BUTR.NativeAOT.Shared.Utils.Copy(value, true);
    public static unsafe ReadOnlySpan<char> ToSpan(param_string* value) => new SafeStringMallocHandle((char*) value, false).ToSpan();
    public static SafeStringMallocHandle ToJson<T>(T value) where T : class? => BUTR.NativeAOT.Shared.Utils.SerializeJsonCopy(value, (JsonTypeInfo<T>) CustomSourceGenerationContext.GetTypeInfo(typeof(T))!, true);

    public static unsafe T? GetResult<T>(return_value_json* ret) where T : class
    {
        using var result = SafeStructMallocHandle.Create(ret, true);
        return result.ValueAsJson((JsonTypeInfo<T>) CustomSourceGenerationContext.GetTypeInfo(typeof(T))!);
    }
    public static unsafe string GetResult(return_value_string* ret)
    {
        using var result = SafeStructMallocHandle.Create(ret, true);
        using var str = result.ValueAsString();
        return str.ToSpan().ToString();
    }
    public static unsafe bool GetResult(return_value_bool* ret)
    {
        using var result = SafeStructMallocHandle.Create(ret, true);
        return result.ValueAsBool();
    }
    public static unsafe int GetResult(return_value_int32* ret)
    {
        using var result = SafeStructMallocHandle.Create(ret, true);
        return result.ValueAsInt32();
    }
    public static unsafe uint GetResult(return_value_uint32* ret)
    {
        using var result = SafeStructMallocHandle.Create(ret, true);
        return result.ValueAsUInt32();
    }
    public static unsafe void GetResult(return_value_void* ret)
    {
        using var result = SafeStructMallocHandle.Create(ret, true);
        result.ValueAsVoid();
    }
    public static unsafe void* GetResult(return_value_ptr* ret)
    {
        using var result = SafeStructMallocHandle.Create(ret, true);
        return result.ValueAsPointer();
    }
}