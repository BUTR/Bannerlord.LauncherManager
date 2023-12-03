using Bannerlord.LauncherManager.Models;

using System;
using System.Collections.Generic;

namespace Bannerlord.LauncherManager;

partial class LauncherManagerHandler
{
    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal void RefreshGameParameters(string executable, IReadOnlyList<string> gameParameters)
    {
        ThrowIfNotInitialized();
        LauncherStateProvider.SetGameParameters(executable, gameParameters);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal LoadOrder LoadLoadOrder()
    {
        ThrowIfNotInitialized();
        return LoadOrderPersistenceProvider.LoadLoadOrder();
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal void SaveLoadOrder(LoadOrder loadOrder)
    {
        ThrowIfNotInitialized();
        LoadOrderPersistenceProvider.SaveLoadOrder(loadOrder);

        SetGameParameterLoadOrder(loadOrder);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal void SendNotification(string id, NotificationType type, string message, uint displayMs)
    {
        ThrowIfNotInitialized();
        NotificationProvider.SendNotification(id, type, message, displayMs);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal void SendDialog(DialogType type, string title, string message, IReadOnlyList<DialogFileFilter> filters, Action<string> onResult)
    {
        ThrowIfNotInitialized();
        DialogProvider.SendDialog(type, title, message, filters, onResult);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal string GetInstallPath()
    {
        ThrowIfNotInitialized();
        return GameInfoProvider.GetInstallPath();
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal byte[]? ReadFileContent(string filePath, int offset, int length)
    {
        ThrowIfNotInitialized();
        return FileSystemProvider.ReadFileContent(filePath, offset, length);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal void WriteFileContent(string filePath, byte[]? data)
    {
        ThrowIfNotInitialized();
        FileSystemProvider.WriteFileContent(filePath, data);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal string[]? ReadDirectoryFileList(string directoryPath)
    {
        ThrowIfNotInitialized();
        return FileSystemProvider.ReadDirectoryFileList(directoryPath);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal string[]? ReadDirectoryList(string directoryPath)
    {
        ThrowIfNotInitialized();
        return FileSystemProvider.ReadDirectoryList(directoryPath);
    }

    /// <summary>
    /// Callback<br/>
    /// Returns all available ViewModels
    /// </summary>
    protected internal IModuleViewModel[]? GetAllModuleViewModels()
    {
        ThrowIfNotInitialized();
        return LoadOrderStateProvider.GetAllModuleViewModels();
    }

    /// <summary>
    /// Callback<br/>
    /// Returns the current shown sorted ViewModels
    /// </summary>
    protected internal IModuleViewModel[]? GetModuleViewModels()
    {
        ThrowIfNotInitialized();
        return LoadOrderStateProvider.GetModuleViewModels();
    }

    /// <summary>
    /// Callback<br/>
    /// Sets the current shown sorted ViewModels
    /// </summary>
    protected internal void SetModuleViewModels(IReadOnlyList<IModuleViewModel> orderedModules)
    {
        ThrowIfNotInitialized();
        LoadOrderStateProvider.SetModuleViewModels(orderedModules);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal LauncherOptions GetOptions()
    {
        ThrowIfNotInitialized();
        return LauncherStateProvider.GetOptions();
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal LauncherState GetState()
    {
        ThrowIfNotInitialized();
        return LauncherStateProvider.GetState();
    }

    private void ThrowIfNotInitialized()
    {
        if (!_isInitialized)
            throw new LauncherManagerNotInitializedException();
    }
}