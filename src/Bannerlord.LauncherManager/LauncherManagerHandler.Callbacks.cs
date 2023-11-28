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
        _launcherUProvider.SetGameParameters(executable, gameParameters);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal LoadOrder LoadLoadOrder()
    {
        return _loadOrderProvider.LoadLoadOrder();
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal void SaveLoadOrder(LoadOrder loadOrder)
    {
        _loadOrderProvider.SaveLoadOrder(loadOrder);

        SetGameParameterLoadOrder(loadOrder);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal void SendNotification(string id, NotificationType type, string message, uint displayMs)
    {
        _notificationUIProvider.SendNotification(id, type, message, displayMs);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal void SendDialog(DialogType type, string title, string message, IReadOnlyList<DialogFileFilter> filters, Action<string> onResult)
    {
        _dialogUIProvider.SendDialog(type, title, message, filters, onResult);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal string GetInstallPath()
    {
        return _gameInfoProvider.GetInstallPath();
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal byte[]? ReadFileContent(string filePath, int offset, int length)
    {
        return _fileSystemProvider.ReadFileContent(filePath, offset, length);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal void WriteFileContent(string filePath, byte[]? data)
    {
        _fileSystemProvider.WriteFileContent(filePath, data);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal string[]? ReadDirectoryFileList(string directoryPath)
    {
        return _fileSystemProvider.ReadDirectoryFileList(directoryPath);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal string[]? ReadDirectoryList(string directoryPath)
    {
        return _fileSystemProvider.ReadDirectoryList(directoryPath);
    }

    /// <summary>
    /// Callback<br/>
    /// Returns all available ViewModels
    /// </summary>
    protected internal IModuleViewModel[]? GetAllModuleViewModels()
    {
        return _launcherUProvider.GetAllModuleViewModels();
    }

    /// <summary>
    /// Callback<br/>
    /// Returns the current shown sorted ViewModels
    /// </summary>
    protected internal IModuleViewModel[]? GetModuleViewModels()
    {
        return _launcherUProvider.GetModuleViewModels();
    }

    /// <summary>
    /// Callback<br/>
    /// Sets the current shown sorted ViewModels
    /// </summary>
    protected internal void SetModuleViewModels(IReadOnlyList<IModuleViewModel> orderedModules)
    {
        _launcherUProvider.SetModuleViewModels(orderedModules);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal LauncherOptions GetOptions()
    {
        return _launcherUProvider.GetOptions();
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal LauncherState GetState()
    {
        return _launcherUProvider.GetState();
    }
}