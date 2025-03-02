using Bannerlord.LauncherManager.Models;
using Bannerlord.LauncherManager.Utils;
using Bannerlord.ModuleManager;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager;

partial class LauncherManagerHandler
{
    // Not real modules, we declare this way our launcher capabilities
    protected static IEnumerable<ModuleInfoExtended> GetLauncherFeatures() =>
        FeatureIds.LauncherFeatures.Select(x => new ModuleInfoExtended { Id = x, IsSingleplayerModule = true });

    private List<ModuleInfoExtendedWithMetadata>? _modules;
    private List<ModuleInfoExtendedWithMetadata>? _allModules;

    /// <summary>
    /// Internal<br/>
    /// All installed Modules
    /// </summary>
    protected Dictionary<string, ModuleInfoExtended> ExtendedModuleInfoCache = new();

    private readonly IModulePathProvider[] _providers;

    /// <summary>
    /// Internal<br/>
    /// </summary>
    protected internal async Task<IReadOnlyList<ModuleInfoExtendedWithMetadata>> GetModulesAsync()
    {
        if (_allModules is null || _modules is null)
        {
            _allModules = await ReloadModulesAsync().ToListAsync();
            _modules = _allModules.GroupBy(x => x.Id).Select(x => x.First()).ToList();
        }

        return _modules;
    }

    /// <summary>
    /// Internal<br/>
    /// </summary>
    protected internal async Task<IReadOnlyList<ModuleInfoExtendedWithMetadata>> GetAllModulesAsync()
    {
        if (_allModules is null || _modules is null)
        {
            _allModules = await ReloadModulesAsync().ToListAsync();
            _modules = _allModules.GroupBy(x => x.Id).Select(x => x.First()).ToList();
        }

        return _allModules;
    }

    /// <summary>
    /// Internal<br/>
    /// </summary>
    protected virtual async IAsyncEnumerable<ModuleInfoExtendedWithMetadata> ReloadModulesAsync()
    {
        foreach (var modulePathProvider in _providers)
        {
            await foreach (var modulePath in modulePathProvider.GetModulePaths())
            {
                var subModulePath = Path.Combine(modulePath, Constants.SubModuleName);
                if (await ReadFileContentAsync(subModulePath, 0, -1) is { } data)
                {
                    ModuleInfoExtended? moduleInfoExtended;
                    try
                    {
                        moduleInfoExtended = ModuleInfoExtended.FromXml(ReaderUtils.Read(data));
                    }
                    catch (Exception e)
                    {
                        await SendNotificationAsync("module-parsing-error-exception", NotificationType.Error, $"Failed to read SubModule.xml at path '{subModulePath}'!\n{e}", 3000);
                        continue;
                    }
                    if (string.IsNullOrEmpty(moduleInfoExtended.Id))
                    {
                        await SendNotificationAsync("module-parsing-error-invalid-module-id", NotificationType.Error, $"SubModule.xml has an invalid Id at path '{subModulePath}'!", 3000);
                        continue;
                    }

                    yield return new ModuleInfoExtendedWithMetadata(moduleInfoExtended, modulePathProvider.ModuleProviderType, subModulePath);
                }
            }
        }

    }

    /// <summary>
    /// Internal<br/>
    /// </summary>
    protected internal IAsyncEnumerable<string> GetModulePathsAsync() => _providers.ToAsyncEnumerable().SelectMany(provider => provider.GetModulePaths());
}