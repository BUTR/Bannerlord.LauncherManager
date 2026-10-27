using Bannerlord.LauncherManager.Native;
using Bannerlord.LauncherManager.Native.Utils;
using Bannerlord.ModuleManager;

using System;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization.Metadata;

using static Bannerlord.LauncherManager.Native.Bindings;

namespace Bannerlord.LauncherManager.Wasm;

/// <summary>
/// JSON in and out of the exports, with the native bindings' serializer options
/// </summary>
internal static class Json
{
    // Through UTF-8 in native memory, see NativeJson: ArrayPool<byte>.Shared trims from a finalizer that never runs here
    // and kept every buffer it lent, GC arrays grew the heap by large object segments. WebAssembly memory doesn't
    // shrink, what malloc frees is reused
    public static T Deserialize<T>(string json, JsonTypeInfo<T> typeInfo) =>
        DeserializeOrNull(json, typeInfo) ?? throw new ArgumentException($"Expected {typeof(T).Name}, got null");

    public static T? DeserializeOrNull<T>(string json, JsonTypeInfo<T> typeInfo) => NativeJson.Deserialize(json, typeInfo);

    public static string Serialize<T>(T value, JsonTypeInfo<T> typeInfo) =>
        NativeJson.Serialize(value, typeInfo, static utf8 => Encoding.UTF8.GetString(utf8));

    /// <param name="modules">The JSON of the modules, or "#" and the handle of a list kept by ModuleLists</param>
    public static ModuleInfoExtended[] DeserializeModules(string modules) =>
        ModuleLists.FromArgument(modules) ?? Deserialize(modules, CustomSourceGenerationContext.ModuleInfoExtendedArray).Where(x => x != null!).ToArray();

    public static ModuleInfoExtended DeserializeModule(string json) =>
        Deserialize(json, CustomSourceGenerationContext.ModuleInfoExtended);
}
