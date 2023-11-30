#if !NETSTANDARD2_1_OR_GREATER
using Bannerlord.LauncherManager.Extensions;
#endif
using Bannerlord.LauncherManager.External;
using Bannerlord.LauncherManager.External.UI;
using Bannerlord.LauncherManager.Models;
using Bannerlord.LauncherManager.Utils;
using Bannerlord.ModuleManager;

using System.Collections.Generic;
using System.Linq;

namespace Bannerlord.LauncherManager;

public partial class LauncherManagerHandler
{
    protected ILauncherStateProvider LauncherStateProvider { get; }
    protected IGameInfoProvider GameInfoProvider { get; }
    protected ILoadOrderPersistenceProvider LoadOrderPersistenceProvider { get; }
    protected IFileSystemProvider FileSystemProvider { get; }
    protected IDialogProvider DialogProvider { get; }
    protected INotificationProvider NotificationProvider { get; }
    protected ILoadOrderStateProvider LoadOrderStateProvider { get; }

    public LauncherManagerHandler(
        ILauncherStateProvider launcherStateProvider,
        IGameInfoProvider gameInfoProvider,
        ILoadOrderPersistenceProvider loadOrderPersistenceProvider,
        IFileSystemProvider fileSystemProvider,
        IDialogProvider dialogProvider,
        INotificationProvider notificationProvider,
        ILoadOrderStateProvider loadOrderStateProvider)
    {
        LauncherStateProvider = launcherStateProvider;
        GameInfoProvider = gameInfoProvider;
        LoadOrderPersistenceProvider = loadOrderPersistenceProvider;
        FileSystemProvider = fileSystemProvider;
        DialogProvider = dialogProvider;
        NotificationProvider = notificationProvider;
        LoadOrderStateProvider = loadOrderStateProvider;
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