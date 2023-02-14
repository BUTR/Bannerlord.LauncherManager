using Bannerlord.LauncherManager.Models;

using System;
using System.Collections.Generic;

namespace Bannerlord.LauncherManager;

public partial class LauncherManagerHandler
{
    private bool _callbacksRegistered;
    private SetGameParametersDelegate D_SetGameParameters = (_, _) => throw new CallbacksNotRegisteredException();
    private GetLoadOrderDelegate D_LoadLoadOrder = () => throw new CallbacksNotRegisteredException();
    private SetLoadOrderDelegate D_SaveLoadOrder = (_) => throw new CallbacksNotRegisteredException();
    private SendNotificationDelegate D_SendNotification = (_, _, _, _) => throw new CallbacksNotRegisteredException();
    private SendDialogDelegate D_SendDialog = (_, _, _, _, _) => throw new CallbacksNotRegisteredException();
    private GetInstallPathDelegate D_GetInstallPath = () => throw new CallbacksNotRegisteredException();
    private ReadFileContentDelegate D_ReadFileContent = (_, _, _) => throw new CallbacksNotRegisteredException();
    private WriteFileContentDelegate D_WriteFileContent = (_, _) => throw new CallbacksNotRegisteredException();
    private ReadDirectoryFileListDelegate D_ReadDirectoryFileList = (_) => throw new CallbacksNotRegisteredException();
    private ReadDirectoryListDelegate D_ReadDirectoryList = (_) => throw new CallbacksNotRegisteredException();
    private GetModuleViewModelsDelegate D_GetModuleViewModels = () => throw new CallbacksNotRegisteredException();
    private SetModuleViewModelsDelegate D_SetModuleViewModels = (_) => throw new CallbacksNotRegisteredException();
    private GetOptionsDelegate D_GetOptions = () => throw new CallbacksNotRegisteredException();
    private GetStateDelegate D_GetState = () => throw new CallbacksNotRegisteredException();

    /// <summary>
    /// External<br/>
    /// </summary>
    public void RegisterCallbacks(SetGameParametersDelegate setGameParameters
        , GetLoadOrderDelegate loadLoadOrder
        , SetLoadOrderDelegate saveLoadOrder
        , SendNotificationDelegate sendNotification
        , SendDialogDelegate sendDialog
        , GetInstallPathDelegate getInstallPath
        , ReadFileContentDelegate readFileContent
        , WriteFileContentDelegate writeFileContent
        , ReadDirectoryFileListDelegate readDirectoryFileList
        , ReadDirectoryListDelegate readDirectoryList
        , GetModuleViewModelsDelegate getModuleViewModels
        , SetModuleViewModelsDelegate setModuleViewModels
        , GetOptionsDelegate getOptions
        , GetStateDelegate getState
    )
    {
        D_SetGameParameters = setGameParameters;
        D_LoadLoadOrder = loadLoadOrder;
        D_SaveLoadOrder = saveLoadOrder;
        D_SendNotification = sendNotification;
        D_SendDialog = sendDialog;
        D_GetInstallPath = getInstallPath;
        D_ReadFileContent = readFileContent;
        D_WriteFileContent = writeFileContent;
        D_ReadDirectoryFileList = readDirectoryFileList;
        D_ReadDirectoryList = readDirectoryList;
        D_GetModuleViewModels = getModuleViewModels;
        D_SetModuleViewModels = setModuleViewModels;
        D_GetOptions = getOptions;
        D_GetState = getState;
        _callbacksRegistered = true;
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal void RefreshGameParameters(string executable, IReadOnlyList<string> gameParameters)
    {
        ThrowIfNoCallbacksRegistered();

        D_SetGameParameters(executable, gameParameters);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal LoadOrder LoadLoadOrder()
    {
        ThrowIfNoCallbacksRegistered();

        return D_LoadLoadOrder();
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal void SaveLoadOrder(LoadOrder loadOrder)
    {
        ThrowIfNoCallbacksRegistered();

        D_SaveLoadOrder(loadOrder);

        SetGameParameterLoadOrder(loadOrder);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal void SendNotification(string id, NotificationType type, string message, uint displayMs)
    {
        ThrowIfNoCallbacksRegistered();

        D_SendNotification(id, type, message, displayMs);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal void SendDialog(DialogType type, string title, string message, IReadOnlyList<DialogFileFilter> filters, Action<string> onResult)
    {
        ThrowIfNoCallbacksRegistered();

        D_SendDialog(type, title, message, filters, onResult);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal string GetInstallPath()
    {
        ThrowIfNoCallbacksRegistered();

        return D_GetInstallPath();
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal byte[]? ReadFileContent(string filePath, int offset, int length)
    {
        ThrowIfNoCallbacksRegistered();

        return D_ReadFileContent(filePath, offset, length);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal void WriteFileContent(string filePath, byte[]? data)
    {
        ThrowIfNoCallbacksRegistered();

        D_WriteFileContent(filePath, data);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal string[]? ReadDirectoryFileList(string directoryPath)
    {
        ThrowIfNoCallbacksRegistered();

        return D_ReadDirectoryFileList(directoryPath);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal string[]? ReadDirectoryList(string directoryPath)
    {
        ThrowIfNoCallbacksRegistered();

        return D_ReadDirectoryList(directoryPath);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal IModuleViewModel[]? GetModuleViewModels()
    {
        ThrowIfNoCallbacksRegistered();

        return D_GetModuleViewModels();
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal void SetModuleViewModels(IReadOnlyList<IModuleViewModel> orderedModules)
    {
        ThrowIfNoCallbacksRegistered();

        D_SetModuleViewModels(orderedModules);
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal LauncherOptions GetOptions()
    {
        ThrowIfNoCallbacksRegistered();

        return D_GetOptions();
    }

    /// <summary>
    /// Callback<br/>
    /// </summary>
    protected internal LauncherState GetState()
    {
        ThrowIfNoCallbacksRegistered();

        return D_GetState();
    }

    private void ThrowIfNoCallbacksRegistered()
    {
        if (!_callbacksRegistered)
            throw new CallbacksNotRegisteredException();
    }
}