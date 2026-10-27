using Bannerlord.LauncherManager.Models;
using Bannerlord.LauncherManager.Native;
using Bannerlord.LauncherManager.Native.Utils;
using Bannerlord.LauncherManager.Wasm.Adapters;
using Bannerlord.ModuleManager;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;

using static Bannerlord.LauncherManager.Native.Bindings;

namespace Bannerlord.LauncherManager.Wasm;

/// <summary>
/// BannerlordModuleManager. Objects cross as JSON strings, the TS wrapper parses them
/// </summary>
public static partial class ModuleManagerExports
{
    private static readonly ApplicationVersionComparer _applicationVersionComparer = new();

    /// <summary>
    /// Parses a module list and keeps it until ReleaseModules, the module list parameters then take "#" and the handle
    /// instead of the JSON, see the TS wrapper's ModuleValidator
    /// </summary>
    [JSExport]
    public static int RegisterModules(string modules) =>
        ModuleLists.Register(Json.Deserialize(modules, CustomSourceGenerationContext.ModuleInfoExtendedArray).Where(x => x != null!).ToArray());

    [JSExport]
    public static void ReleaseModules(int handle) => ModuleLists.Release(handle);

    /// <summary>
    /// The positions of the result's modules in the source, see ModuleLists.Positions, or the modules when it can't
    /// </summary>
    private static string Positions(ModuleInfoExtended[] source, IList<ModuleInfoExtended> result) =>
        ModuleLists.Positions(source, result.ToArray()) ?? Json.Serialize(result.ToArray(), CustomSourceGenerationContext.ModuleInfoExtendedArray);

    [JSExport]
    public static string Sort(string source)
    {
        var modules = Json.DeserializeModules(source);
        return Positions(modules, ModuleSorter.Sort(modules));
    }

    [JSExport]
    public static string SortWithOptions(string source, string options)
    {
        var sorterOptions = Json.Deserialize(options, CustomSourceGenerationContext.ModuleSorterOptions);
        var modules = Json.DeserializeModules(source);
        return Positions(modules, ModuleSorter.Sort(modules, sorterOptions));
    }

    [JSExport]
    public static bool AreAllDependenciesOfModulePresent(string source, string module) =>
        ModuleUtilities.AreDependenciesPresent(Json.DeserializeModules(source), Json.DeserializeModule(module));

    [JSExport]
    public static string GetDependentModulesOf(string source, string module)
    {
        var modules = Json.DeserializeModules(source);
        return Positions(modules, ModuleUtilities.GetDependencies(modules, Json.DeserializeModule(module)).ToArray());
    }

    [JSExport]
    public static string GetDependentModulesOfWithOptions(string source, string module, string options)
    {
        var sorterOptions = Json.Deserialize(options, CustomSourceGenerationContext.ModuleSorterOptions);
        var modules = Json.DeserializeModules(source);
        return Positions(modules, ModuleUtilities.GetDependencies(modules, Json.DeserializeModule(module), sorterOptions).ToArray());
    }

    /// <summary>
    /// The validation manager is the TS wrapper's for this call, see ModuleManagerCallbacks
    /// </summary>
    [JSExport]
    public static string ValidateModule(string modules, string targetModule)
    {
        var result = ModuleUtilities.ValidateModule(Json.DeserializeModules(modules), Json.DeserializeModule(targetModule),
            module => Call(ModuleManagerCallbacks.IsSelected, module.Id)).ToArray();
        return Json.Serialize(result, CustomSourceGenerationContext.ModuleIssueArray);
    }

    [JSExport]
    public static string ValidateLoadOrder(string modules, string targetModule)
    {
        var result = ModuleUtilities.ValidateLoadOrder(Json.DeserializeModules(modules), Json.DeserializeModule(targetModule)).ToArray();
        return Json.Serialize(result, CustomSourceGenerationContext.ModuleIssueArray);
    }

    /// <summary>
    /// The enable/disable manager is the TS wrapper's for this call, see ModuleManagerCallbacks
    /// </summary>
    [JSExport]
    public static void EnableModule(string modules, string targetModule)
    {
        ModuleUtilities.EnableModule(Json.DeserializeModules(modules), Json.DeserializeModule(targetModule),
            module => Call(ModuleManagerCallbacks.GetSelected, module.Id),
            (module, value) => Call(ModuleManagerCallbacks.SetSelected, module.Id, value),
            module => Call(ModuleManagerCallbacks.GetDisabled, module.Id),
            (module, value) => Call(ModuleManagerCallbacks.SetDisabled, module.Id, value));
    }

    /// <summary>
    /// The enable/disable manager is the TS wrapper's for this call, see ModuleManagerCallbacks
    /// </summary>
    [JSExport]
    public static void DisableModule(string modules, string targetModule)
    {
        ModuleUtilities.DisableModule(Json.DeserializeModules(modules), Json.DeserializeModule(targetModule),
            module => Call(ModuleManagerCallbacks.GetSelected, module.Id),
            (module, value) => Call(ModuleManagerCallbacks.SetSelected, module.Id, value),
            module => Call(ModuleManagerCallbacks.GetDisabled, module.Id),
            (module, value) => Call(ModuleManagerCallbacks.SetDisabled, module.Id, value));
    }

    [JSExport]
    public static string? GetModuleInfo(string xml)
    {
        var result = ModuleInfoExtended.FromXml(ReaderUtils2.Read(xml));
        return result is null ? null : Json.Serialize(result, CustomSourceGenerationContext.ModuleInfoExtended);
    }

    [JSExport]
    public static string? GetModuleInfoWithPath(string xml, string path)
    {
        if (ModuleInfoExtended.FromXml(ReaderUtils2.Read(xml)) is not { } module) return null;
        return Json.Serialize(new ModuleInfoExtendedWithPath(module, path), CustomSourceGenerationContext.ModuleInfoExtendedWithPath);
    }

    [JSExport]
    public static string? GetModuleInfoWithMetadata(string xml, string moduleProvider, string path)
    {
        if (ModuleInfoExtended.FromXml(ReaderUtils2.Read(xml)) is not { } module) return null;
        var type = Enum.TryParse<ModuleProviderType>(moduleProvider, out var e) ? e : ModuleProviderType.Default;
        return Json.Serialize(new ModuleInfoExtendedWithMetadata(module, type, path), CustomSourceGenerationContext.ModuleInfoExtendedWithMetadata);
    }

    [JSExport]
    public static string GetSubModuleInfo(string xml)
    {
        var result = SubModuleInfoExtended.FromXml(ReaderUtils2.Read(xml)) ?? throw new InvalidOperationException("Invalid xml content!");
        return Json.Serialize(result, CustomSourceGenerationContext.SubModuleInfoExtended);
    }

    [JSExport]
    public static string ParseApplicationVersion(string content)
    {
        var result = ApplicationVersion.TryParse(content, out var v) ? v : ApplicationVersion.Empty;
        return Json.Serialize(result, CustomSourceGenerationContext.ApplicationVersion);
    }

    [JSExport]
    public static int CompareVersions(string x, string y) => _applicationVersionComparer.Compare(
        Json.Deserialize(x, CustomSourceGenerationContext.ApplicationVersion),
        Json.Deserialize(y, CustomSourceGenerationContext.ApplicationVersion));

    [JSExport]
    public static string GetDependenciesAll(string module) =>
        Json.Serialize(Json.DeserializeModule(module).DependenciesAllDistinct().ToArray(), CustomSourceGenerationContext.DependentModuleMetadataArray);

    [JSExport]
    public static string GetDependenciesToLoadBeforeThis(string module) =>
        Json.Serialize(Json.DeserializeModule(module).DependenciesLoadBeforeThisDistinct().ToArray(), CustomSourceGenerationContext.DependentModuleMetadataArray);

    [JSExport]
    public static string GetDependenciesToLoadAfterThis(string module) =>
        Json.Serialize(Json.DeserializeModule(module).DependenciesLoadAfterThisDistinct().ToArray(), CustomSourceGenerationContext.DependentModuleMetadataArray);

    [JSExport]
    public static string GetDependenciesIncompatibles(string module) =>
        Json.Serialize(Json.DeserializeModule(module).DependenciesIncompatiblesDistinct().ToArray(), CustomSourceGenerationContext.DependentModuleMetadataArray);

    // A throwing JS callback counts as false, like the native bindings
    private static bool Call(Func<string, bool> callback, string? moduleId)
    {
        using var logger = LogMethod();
        try
        {
            return callback(moduleId ?? string.Empty);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return false;
        }
    }

    private static void Call(Action<string, bool> callback, string? moduleId, bool value)
    {
        using var logger = LogMethod();
        try
        {
            callback(moduleId ?? string.Empty, value);
        }
        catch (Exception e)
        {
            logger.LogException(e);
        }
    }
}
