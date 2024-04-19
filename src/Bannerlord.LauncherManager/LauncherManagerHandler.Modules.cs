using Bannerlord.LauncherManager.Models;
using Bannerlord.LauncherManager.Utils;
using Bannerlord.ModuleManager;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bannerlord.LauncherManager;

partial class LauncherManagerHandler
{
    // Not real modules, we declare this way our launcher capabilities
    protected static IEnumerable<ModuleInfoExtended> GetLauncherFeatures() =>
        FeatureIds.LauncherFeatures.Select(x => new ModuleInfoExtended { Id = x, IsSingleplayerModule = true });

    private List<ModuleInfoExtendedWithMetadata>? _modules;

    /// <summary>
    /// Internal<br/>
    /// All installed Modules
    /// </summary>
    protected Dictionary<string, ModuleInfoExtended> ExtendedModuleInfoCache = new();

    private readonly IModulePathProvider[] _providers;

    /// <summary>
    /// Internal<br/>
    /// </summary>
    protected internal IReadOnlyList<ModuleInfoExtendedWithMetadata> GetModules() => _modules ??= ReloadModules().ToList();

    /// <summary>
    /// Internal<br/>
    /// </summary>
    protected virtual IEnumerable<ModuleInfoExtendedWithMetadata> ReloadModules()
    {
        foreach (var modulePathProvider in _providers)
        {
            foreach (var modulePath in modulePathProvider.GetModulePaths())
            {
                var subModulePath = Path.Combine(modulePath, Constants.SubModuleName);
                if (ReadFileContent(subModulePath, 0, -1) is { } data)
                {
                    ModuleInfoExtended? moduleInfoExtended;
                    try
                    {
                        moduleInfoExtended = ModuleInfoExtended.FromXml(ReaderUtils.Read(data));
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"modulePath: {modulePath}, content: {Convert.ToBase64String(data)}", e);
                    }
                    yield return new ModuleInfoExtendedWithMetadata(moduleInfoExtended, modulePathProvider.ModuleProviderType, subModulePath);
                }
            }
        }

    }

    /// <summary>
    /// Internal<br/>
    /// </summary>
    protected internal IEnumerable<string> GetModulePaths() => _providers.SelectMany(provider => provider.GetModulePaths());

}