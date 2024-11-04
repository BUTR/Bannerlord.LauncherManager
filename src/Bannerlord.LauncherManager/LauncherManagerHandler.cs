#if !NETSTANDARD2_1_OR_GREATER
using Bannerlord.LauncherManager.Extensions;
#endif
using Bannerlord.LauncherManager.External;
using Bannerlord.LauncherManager.External.UI;
using Bannerlord.LauncherManager.Models;
using Bannerlord.LauncherManager.Utils;
using Bannerlord.ModuleManager;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Bannerlord.LauncherManager;

public partial class LauncherManagerHandler
{
    private bool _isInitialized;
    protected ILauncherStateProvider LauncherStateProvider { get; private set; } = default!;
    protected IGameInfoProvider GameInfoProvider { get; private set; } = default!;
    protected IFileSystemProvider FileSystemProvider { get; private set; } = default!;
    protected IDialogProvider DialogProvider { get; private set; } = default!;
    protected INotificationProvider NotificationProvider { get; private set; } = default!;
    protected ILoadOrderStateProvider LoadOrderStateProvider { get; private set; } = default!;

    public LauncherManagerHandler()
    {
        _providers =
        [
            new MainModuleProvider(this),
            new SteamModuleProvider(this),
            new VortexModuleProvider(this)
        ];
    }

    protected void Initialize(ILauncherStateProvider launcherStateProvider, IGameInfoProvider gameInfoProvider, IFileSystemProvider fileSystemProvider,
        IDialogProvider dialogProvider, INotificationProvider notificationProvider, ILoadOrderStateProvider loadOrderStateProvider)
    {
        if (_isInitialized)
            throw new LauncherManagerInitializedTwiceException();

        _isInitialized = true;
        LauncherStateProvider = launcherStateProvider;
        GameInfoProvider = gameInfoProvider;
        FileSystemProvider = fileSystemProvider;
        DialogProvider = dialogProvider;
        NotificationProvider = notificationProvider;
        LoadOrderStateProvider = loadOrderStateProvider;
    }

    public LauncherManagerHandler(ILauncherStateProvider launcherStateProvider, IGameInfoProvider gameInfoProvider, IFileSystemProvider fileSystemProvider,
        IDialogProvider dialogProvider, INotificationProvider notificationProvider, ILoadOrderStateProvider loadOrderStateProvider) : this()
    {
        Initialize(launcherStateProvider, gameInfoProvider, fileSystemProvider, dialogProvider, notificationProvider, loadOrderStateProvider);
    }

    /// <summary>
    /// External<br/>
    /// </summary>
    public void RefreshModules()
    {
        _modules = null;
        _allModules = null;
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
    public IReadOnlyList<IModuleViewModel> GetViewModelsFromModules(IEnumerable<ModuleInfoExtended> modules)
    {
        if (GetAllModuleViewModels() is not { } viewModels)
            return Array.Empty<IModuleViewModel>();

        return modules.Select(moduleInfoExtended => viewModels.First(x => x.ModuleInfoExtended == moduleInfoExtended)).ToList();
    }
}