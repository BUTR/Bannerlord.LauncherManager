using Bannerlord.LauncherManager.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bannerlord.LauncherManager.Utils;

internal interface IModulePathProvider
{
    ModuleProviderType ModuleProviderType { get; }
    IAsyncEnumerable<string> GetModulePaths();
}

internal class MainModuleProvider : IModulePathProvider
{
    public ModuleProviderType ModuleProviderType => ModuleProviderType.Default;

    private readonly LauncherManagerHandler _handler;

    public MainModuleProvider(LauncherManagerHandler handler)
    {
        _handler = handler;
    }


    public async IAsyncEnumerable<string> GetModulePaths()
    {
        var installPath = await _handler.GetInstallPathAsync();
        var directories = await _handler.ReadDirectoryListAsync(Path.Combine(installPath, Constants.ModulesFolder));
        if (directories is null) throw new DirectoryNotFoundException($"installPath: {installPath}. Modules directory not found!");
        //if (directories is null) yield break;
        foreach (var moduleFolder in directories)
        {
            var files = await _handler.ReadDirectoryFileListAsync(moduleFolder) ?? [];
            if (files.All(x => x != "__folder_managed_by_vortex"))
                yield return moduleFolder;
        }
    }
}

internal class SteamModuleProvider : IModulePathProvider
{
    public ModuleProviderType ModuleProviderType => ModuleProviderType.Steam;

    private readonly LauncherManagerHandler _handler;

    public SteamModuleProvider(LauncherManagerHandler handler)
    {
        _handler = handler;
    }

    public async IAsyncEnumerable<string> GetModulePaths()
    {
        var installPath = await _handler.GetInstallPathAsync();
        if (!installPath.ToLower().Contains("steamapps") || !installPath.ToLower().Contains("common"))
            yield break;

        var steamApps = installPath.Substring(0, installPath.IndexOf("common", StringComparison.Ordinal));
        var workshopDir = Path.Combine(steamApps, "workshop", "content", "261550");

        var directories = await _handler.ReadDirectoryListAsync(workshopDir);
        // Optional
        //if (directories is null) throw new DirectoryNotFoundException($"installPath: {installPath}. Steam's Modules directory not found!");
        if (directories is null) yield break;
        foreach (var moduleFolder in directories)
        {
            yield return moduleFolder;
        }
    }
}

internal class VortexModuleProvider : IModulePathProvider
{
    public ModuleProviderType ModuleProviderType => ModuleProviderType.Vortex;

    private readonly LauncherManagerHandler _handler;

    public VortexModuleProvider(LauncherManagerHandler handler)
    {
        _handler = handler;
    }


    public async IAsyncEnumerable<string> GetModulePaths()
    {
        var installPath = await _handler.GetInstallPathAsync();

        var directories = await _handler.ReadDirectoryListAsync(Path.Combine(installPath, Constants.ModulesFolder));
        if (directories is null) yield break;

        foreach (var moduleFolder in directories)
        {
            var files = await _handler.ReadDirectoryFileListAsync(moduleFolder) ?? [];
            if (files.Any(x => x == "__folder_managed_by_vortex"))
                yield return moduleFolder;
        }
    }
}