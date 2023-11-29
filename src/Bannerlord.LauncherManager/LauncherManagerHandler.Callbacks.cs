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
        LauncherProvider.SetGameParameters(executable, gameParameters);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal LoadOrder LoadLoadOrder()
    {
        return LoadOrderProvider.LoadLoadOrder();
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal void SaveLoadOrder(LoadOrder loadOrder)
    {
        LoadOrderProvider.SaveLoadOrder(loadOrder);

        SetGameParameterLoadOrder(loadOrder);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal void SendNotification(string id, NotificationType type, string message, uint displayMs)
    {
        NotificationUIProvider.SendNotification(id, type, message, displayMs);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal void SendDialog(DialogType type, string title, string message, IReadOnlyList<DialogFileFilter> filters, Action<string> onResult)
    {
        DialogUIProvider.SendDialog(type, title, message, filters, onResult);
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
        return LauncherProvider.GetAllModuleViewModels();
    }

    /// <summary>
    /// Callback<br/>
    /// Returns the current shown sorted ViewModels
    /// </summary>
    protected internal IModuleViewModel[]? GetModuleViewModels()
    {
        return LauncherProvider.GetModuleViewModels();
    }

    /// <summary>
    /// Callback<br/>
    /// Sets the current shown sorted ViewModels
    /// </summary>
    protected internal void SetModuleViewModels(IReadOnlyList<IModuleViewModel> orderedModules)
    {
        LauncherProvider.SetModuleViewModels(orderedModules);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal LauncherOptions GetOptions()
    {
        return LauncherProvider.GetOptions();
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal LauncherState GetState()
    {
        return LauncherProvider.GetState();
    }
}