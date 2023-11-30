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
        LauncherStateProvider.SetGameParameters(executable, gameParameters);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal LoadOrder LoadLoadOrder()
    {
        return LoadOrderPersistenceProvider.LoadLoadOrder();
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal void SaveLoadOrder(LoadOrder loadOrder)
    {
        LoadOrderPersistenceProvider.SaveLoadOrder(loadOrder);

        SetGameParameterLoadOrder(loadOrder);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal void SendNotification(string id, NotificationType type, string message, uint displayMs)
    {
        NotificationProvider.SendNotification(id, type, message, displayMs);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal void SendDialog(DialogType type, string title, string message, IReadOnlyList<DialogFileFilter> filters, Action<string> onResult)
    {
        DialogProvider.SendDialog(type, title, message, filters, onResult);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal string GetInstallPath()
    {
        return GameInfoProvider.GetInstallPath();
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal byte[]? ReadFileContent(string filePath, int offset, int length)
    {
        return FileSystemProvider.ReadFileContent(filePath, offset, length);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal void WriteFileContent(string filePath, byte[]? data)
    {
        FileSystemProvider.WriteFileContent(filePath, data);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal string[]? ReadDirectoryFileList(string directoryPath)
    {
        return FileSystemProvider.ReadDirectoryFileList(directoryPath);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal string[]? ReadDirectoryList(string directoryPath)
    {
        return FileSystemProvider.ReadDirectoryList(directoryPath);
    }

    /// <summary>
    /// Callback<br/>
    /// Returns all available ViewModels
    /// </summary>
    protected internal IModuleViewModel[]? GetAllModuleViewModels()
    {
        return LoadOrderStateProvider.GetAllModuleViewModels();
    }

    /// <summary>
    /// Callback<br/>
    /// Returns the current shown sorted ViewModels
    /// </summary>
    protected internal IModuleViewModel[]? GetModuleViewModels()
    {
        return LoadOrderStateProvider.GetModuleViewModels();
    }

    /// <summary>
    /// Callback<br/>
    /// Sets the current shown sorted ViewModels
    /// </summary>
    protected internal void SetModuleViewModels(IReadOnlyList<IModuleViewModel> orderedModules)
    {
        LoadOrderStateProvider.SetModuleViewModels(orderedModules);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal LauncherOptions GetOptions()
    {
        return LauncherStateProvider.GetOptions();
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal LauncherState GetState()
    {
        return LauncherStateProvider.GetState();
    }
}