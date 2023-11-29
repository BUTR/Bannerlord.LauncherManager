﻿#if !NETSTANDARD2_1_OR_GREATER
using Bannerlord.LauncherManager.Extensions;
#endif
using Bannerlord.LauncherManager.External;
using Bannerlord.LauncherManager.Models;
using Bannerlord.LauncherManager.Utils;
using Bannerlord.ModuleManager;

using System.Collections.Generic;
using System.Linq;

namespace Bannerlord.LauncherManager;

public partial class LauncherManagerHandler
{
    protected ILauncherProvider LauncherProvider { get; }
    protected IGameInfoProvider GameInfoProvider { get; }
    protected ILoadOrderProvider LoadOrderProvider { get; }
    protected IFileSystemProvider FileSystemProvider { get; }
    protected IDialogUIProvider DialogUIProvider { get; }
    protected INotificationUIProvider NotificationUIProvider { get; }

    public LauncherManagerHandler(
        ILauncherProvider launcherProvider,
        IGameInfoProvider gameInfoProvider,
        ILoadOrderProvider loadOrderProvider,
        IFileSystemProvider fileSystemProvider,
        IDialogUIProvider dialogUIProvider,
        INotificationUIProvider notificationUIProvider)
    {
        LauncherProvider = launcherProvider;
        GameInfoProvider = gameInfoProvider;
        LoadOrderProvider = loadOrderProvider;
        FileSystemProvider = fileSystemProvider;
        DialogUIProvider = dialogUIProvider;
        NotificationUIProvider = notificationUIProvider;
        _providers = new IModulePathProvider[]
        {
            new MainModuleProvider(this),
            new SteamModuleProvider(this)
        };
    }

    /// <summary>
    /// External<br/>
    /// </summary>
    public void RefreshModules()
    {
        _modules = null;
        ExtendedModuleInfoCache = GetLauncherFeatures().Concat(GetModules()).ToDictionary(x => x.Id, x => x);
    }

    /// <summary>
    /// Internal<br/>
    /// </summary>
    public IReadOnlyCollection<ModuleInfoExtended> GetFromLoadOrder(LoadOrder loadOrder)
    {
        var ids = loadOrder.Select(x => x.Key).ToHashSet();
        return GetModules().Where(x => ids.Contains(x.Id)).ToList();
    }

    /// <summary>
    /// Internal<br/>
    /// </summary>
    public LoadOrder GetFromViewModel(IEnumerable<IModuleViewModel> modules) => new(modules);

    /// <summary>
    /// Internal<br/>
    /// </summary>
    public LoadOrder GetFromModules(IEnumerable<ModuleInfoExtended> modules) => new(modules);
}