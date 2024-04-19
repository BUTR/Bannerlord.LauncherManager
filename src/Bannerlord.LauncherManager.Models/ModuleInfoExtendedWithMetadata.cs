using Bannerlord.ModuleManager;

using System.Collections.Generic;

namespace Bannerlord.LauncherManager.Models;

public record ModuleInfoExtendedWithMetadata : ModuleInfoExtended
{
    public ModuleProviderType ModuleProviderType { get; set; }
    public string Path { get; set; } = string.Empty;

    public ModuleInfoExtendedWithMetadata() { }
    public ModuleInfoExtendedWithMetadata(ModuleProviderType moduleProviderType, string path, string id, string name, bool isOfficial, ApplicationVersion version, bool isSingleplayerModule, bool isMultiplayerModule,
        IReadOnlyList<SubModuleInfoExtended> subModules, IReadOnlyList<DependentModule> dependentModules, IReadOnlyList<DependentModule> modulesToLoadAfterThis,
        IReadOnlyList<DependentModule> incompatibleModules, IReadOnlyList<DependentModuleMetadata> dependentModuleMetadatas, string url)
    {
        ModuleProviderType = moduleProviderType;
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
    public ModuleInfoExtendedWithMetadata(ModuleInfoExtended module, ModuleProviderType moduleProviderType, string path) : base(module)
    {
        ModuleProviderType = moduleProviderType;
        Path = path;
    }
}