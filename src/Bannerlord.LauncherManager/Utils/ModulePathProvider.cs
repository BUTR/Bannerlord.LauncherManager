﻿using Bannerlord.LauncherManager.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bannerlord.LauncherManager.Utils;

internal interface IModulePathProvider
{
    ModuleProviderType ModuleProviderType { get; }
    IEnumerable<string> GetModulePaths();
}

internal class MainModuleProvider : IModulePathProvider
{
    public ModuleProviderType ModuleProviderType => ModuleProviderType.Default;

    private readonly LauncherManagerHandler _handler;

    public MainModuleProvider(LauncherManagerHandler handler)
    {
        _handler = handler;
    }


    public IEnumerable<string> GetModulePaths()
    {
        var installPath = _handler.GetInstallPath();
        var directories = _handler.ReadDirectoryList(Path.Combine(installPath, Constants.ModulesFolder));
        if (directories is null) throw new DirectoryNotFoundException($"installPath: {installPath}. Modules directory not found!");
        //if (directories is null) yield break;
        foreach (var moduleFolder in directories)
        {
            var files = _handler.ReadDirectoryFileList(moduleFolder) ?? [];
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

    public IEnumerable<string> GetModulePaths()
    {
        var installPath = _handler.GetInstallPath();
        if (!installPath.ToLower().Contains("steamapps") || !installPath.ToLower().Contains("common"))
            yield break;

        var steamApps = installPath.Substring(0, installPath.IndexOf("common", StringComparison.Ordinal));
        var workshopDir = Path.Combine(steamApps, "workshop", "content", "261550");

        var directories = _handler.ReadDirectoryList(workshopDir);
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


    public IEnumerable<string> GetModulePaths()
    {
        var installPath = _handler.GetInstallPath();

        var directories = _handler.ReadDirectoryList(Path.Combine(installPath, Constants.ModulesFolder));
        if (directories is null) yield break;

        foreach (var moduleFolder in directories)
        {
            var files = _handler.ReadDirectoryFileList(moduleFolder) ?? [];
            if (files.Any(x => x == "__folder_managed_by_vortex"))
                yield return moduleFolder;
        }
    }
}