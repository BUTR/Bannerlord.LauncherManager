using Bannerlord.LauncherManager.Models;

using System;

namespace Bannerlord.LauncherManager
{
    public partial class VortexExtensionHandler
    {
        private bool _callbacksRegistered;
        private GetActiveProfileDelegate D_GetActiveProfile = () => throw new CallbacksNotRegisteredException();
        private GetProfileByIdDelegate D_GetProfileById = (_) => throw new CallbacksNotRegisteredException();
        private GetActiveGameIdDelegate D_GetActiveGameId = () => throw new CallbacksNotRegisteredException();
        private SetGameParametersDelegate D_SetGameParameters = (_, _) => throw new CallbacksNotRegisteredException();
        private GetLoadOrderDelegate D_GetLoadOrder = () => throw new CallbacksNotRegisteredException();
        private SetLoadOrderDelegate D_SetLoadOrder = (_) => throw new CallbacksNotRegisteredException();
        private TranslateStringDelegate D_TranslateString = (_) => throw new CallbacksNotRegisteredException();
        private SendNotificationDelegate D_SendNotification = (_, _, _, _) => throw new CallbacksNotRegisteredException();
        private GetInstallPathDelegate D_GetInstallPath = () => throw new CallbacksNotRegisteredException();
        private ReadFileContentDelegate D_ReadFileContent = (_) => throw new CallbacksNotRegisteredException();
        private ReadDirectoryFileListDelegate D_ReadDirectoryFileList = (_) => throw new CallbacksNotRegisteredException();
        private ReadDirectoryListDelegate D_ReadDirectoryList = (_) => throw new CallbacksNotRegisteredException();

        public void RegisterCallbacks(
            GetActiveProfileDelegate getActiveProfile
            , GetProfileByIdDelegate getProfileById
            , GetActiveGameIdDelegate getActiveGameId
            , SetGameParametersDelegate setGameParameters
            , GetLoadOrderDelegate getLoadOrder
            , SetLoadOrderDelegate setLoadOrder
            , TranslateStringDelegate translateString
            , SendNotificationDelegate sendNotification
            , GetInstallPathDelegate getInstallPath
            , ReadFileContentDelegate readFileContent
            , ReadDirectoryFileListDelegate readDirectoryFileList
            , ReadDirectoryListDelegate readDirectoryList
        )
        {
            D_GetActiveProfile = getActiveProfile;
            D_GetProfileById = getProfileById;
            D_GetActiveGameId = getActiveGameId;
            D_SetGameParameters = setGameParameters;
            D_GetLoadOrder = getLoadOrder;
            D_SetLoadOrder = setLoadOrder;
            D_TranslateString = translateString;
            D_SendNotification = sendNotification;
            D_GetInstallPath = getInstallPath;
            D_ReadFileContent = readFileContent;
            D_ReadDirectoryFileList = readDirectoryFileList;
            D_ReadDirectoryList = readDirectoryList;
            _callbacksRegistered = true;
        }

        public Profile GetActiveProfile()
        {
            ThrowIfNoCallbacksRegistered();

            return D_GetActiveProfile();
        }

        public Profile GetProfileById(ReadOnlySpan<char> profileId)
        {
            ThrowIfNoCallbacksRegistered();

            return D_GetProfileById(profileId);
        }

        public ReadOnlySpan<char> GetActiveGameId()
        {
            ThrowIfNoCallbacksRegistered();

            return D_GetActiveGameId();
        }

        public void SetGameParameters(ReadOnlySpan<char> executable, string[] gameParameters)
        {
            ThrowIfNoCallbacksRegistered();

            D_SetGameParameters(executable, gameParameters);
        }

        public LoadOrder GetLoadOrder()
        {
            ThrowIfNoCallbacksRegistered();

            return D_GetLoadOrder();
        }

        public void SetLoadOrder(LoadOrder loadOrder)
        {
            ThrowIfNoCallbacksRegistered();

            D_SetLoadOrder(loadOrder);
            RefreshGameParams(loadOrder);
        }

        public ReadOnlySpan<char> TranslateString(ReadOnlySpan<char> text)
        {
            ThrowIfNoCallbacksRegistered();

            return D_TranslateString(text);
        }

        public void SendNotification(ReadOnlySpan<char> id, ReadOnlySpan<char> type, ReadOnlySpan<char> message, uint displayMs)
        {
            ThrowIfNoCallbacksRegistered();

            D_SendNotification(id, type, message, displayMs);
        }

        public ReadOnlySpan<char> GetInstallPath()
        {
            ThrowIfNoCallbacksRegistered();

            return D_GetInstallPath();
        }

        public string? ReadFileContent(ReadOnlySpan<char> filePath)
        {
            ThrowIfNoCallbacksRegistered();

            return D_ReadFileContent(filePath);
        }

        public string[]? ReadDirectoryFileList(ReadOnlySpan<char> directoryPath)
        {
            ThrowIfNoCallbacksRegistered();

            return D_ReadDirectoryFileList(directoryPath);
        }

        public string[]? ReadDirectoryList(ReadOnlySpan<char> directoryPath)
        {
            ThrowIfNoCallbacksRegistered();

            return D_ReadDirectoryList(directoryPath);
        }

        private void ThrowIfNoCallbacksRegistered()
        {
            if (!_callbacksRegistered)
                throw new CallbacksNotRegisteredException();
        }
    }
}