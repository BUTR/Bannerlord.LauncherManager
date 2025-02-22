using Bannerlord.LauncherManager.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager;

partial class LauncherManagerHandler
{
    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal async Task RefreshGameParametersAsync(string executable, IReadOnlyList<string> gameParameters)
    {
        ThrowIfNotInitialized();
        await LauncherStateProvider.SetGameParametersAsync(executable, gameParameters);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal async Task SendNotificationAsync(string id, NotificationType type, string message, uint displayMs)
    {
        ThrowIfNotInitialized();
        await NotificationProvider.SendNotificationAsync(id, type, message, displayMs);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal async Task<string> SendDialogAsync(DialogType type, string title, string message, IReadOnlyList<DialogFileFilter> filters)
    {
        ThrowIfNotInitialized();
        return await DialogProvider.SendDialogAsync(type, title, message, filters);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal async Task<string> GetInstallPathAsync()
    {
        ThrowIfNotInitialized();
        return await GameInfoProvider.GetInstallPathAsync();
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal async Task<byte[]?> ReadFileContentAsync(string filePath, int offset, int length)
    {
        ThrowIfNotInitialized();
        return await FileSystemProvider.ReadFileContentAsync(filePath, offset, length);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal async Task WriteFileContentAsync(string filePath, byte[]? data)
    {
        ThrowIfNotInitialized();
        await FileSystemProvider.WriteFileContentAsync(filePath, data);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal async Task<string[]?> ReadDirectoryFileListAsync(string directoryPath)
    {
        ThrowIfNotInitialized();
        return await FileSystemProvider.ReadDirectoryFileListAsync(directoryPath);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal async Task<string[]?> ReadDirectoryListAsync(string directoryPath)
    {
        ThrowIfNotInitialized();
        return await FileSystemProvider.ReadDirectoryListAsync(directoryPath);
    }

    /// <summary>
    /// Callback<br/>
    /// Returns all available ViewModels
    /// </summary>
    protected internal async Task<IModuleViewModel[]?> GetAllModuleViewModelsAsync()
    {
        ThrowIfNotInitialized();
        return await LoadOrderStateProvider.GetAllModuleViewModelsAsync();
    }

    /// <summary>
    /// Callback<br/>
    /// Returns the current shown sorted ViewModels
    /// </summary>
    protected internal async Task<IEnumerable<IModuleViewModel>?> GetModuleViewModelsAsync()
    {
        ThrowIfNotInitialized();
        return await LoadOrderStateProvider.GetModuleViewModelsAsync();
    }

    /// <summary>
    /// Callback<br/>
    /// Sets the current shown sorted ViewModels
    /// </summary>
    protected internal async Task SetModuleViewModelsAsync(IReadOnlyList<IModuleViewModel> orderedModules)
    {
        ThrowIfNotInitialized();

        await LoadOrderStateProvider.SetModuleViewModelsAsync(orderedModules);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal async Task<LauncherOptions> GetOptionsAsync()
    {
        ThrowIfNotInitialized();
        return await LauncherStateProvider.GetOptionsAsync();
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal async Task<LauncherState> GetStateAsync()
    {
        ThrowIfNotInitialized();
        return await LauncherStateProvider.GetStateAsync();
    }

    private void ThrowIfNotInitialized()
    {
        if (!_isInitialized)
            throw new LauncherManagerNotInitializedException();
    }
}