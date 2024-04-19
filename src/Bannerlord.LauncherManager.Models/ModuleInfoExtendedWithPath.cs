using System.Collections.Generic;
using Bannerlord.ModuleManager;

namespace Bannerlord.LauncherManager.Models;

public record ModuleInfoExtendedWithPath : ModuleInfoExtended
{
    public string Path { get; set; } = string.Empty;

    public ModuleInfoExtendedWithPath() { }
    public ModuleInfoExtendedWithPath(string path, string id, string name, bool isOfficial, ApplicationVersion version, bool isSingleplayerModule, bool isMultiplayerModule,
        IReadOnlyList<SubModuleInfoExtended> subModules, IReadOnlyList<DependentModule> dependentModules, IReadOnlyList<DependentModule> modulesToLoadAfterThis,
        IReadOnlyList<DependentModule> incompatibleModules, IReadOnlyList<DependentModuleMetadata> dependentModuleMetadatas, string url)
    {
        Path = path;
        Id = id;
        Name = name;
        IsOfficial = isOfficial;
        Version = version;
        IsSingleplayerModule = isSingleplayerModule;
        IsMultiplayerModule = isMultiplayerModule;
        SubModules = subModules;
        DependentModules = dependentModules;
        ModulesToLoadAfterThis = modulesToLoadAfterThis;
        IncompatibleModules = incompatibleModules;
        DependentModuleMetadatas = dependentModuleMetadatas;
        Url = url;
    }
    public ModuleInfoExtendedWithPath(ModuleInfoExtended module, string path) : base(module)
    {
        Path = path;
    }
}