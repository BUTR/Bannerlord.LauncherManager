using Bannerlord.VortexExtension.Models;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Bannerlord.VortexExtension
{
    public partial class VortexExtensionHandler
    {
        private bool _callbacksRegistered;
        private GetActiveProfileDelegate D_GetActiveProfile;
        private GetProfileByIdDelegate D_GetProfileById;
        private GetActiveGameIdDelegate D_GetActiveGameId;
        private SetGameParametersDelegate D_SetGameParameters;
        private GetLoadOrderDelegate D_GetLoadOrder;
        private SetLoadOrderDelegate D_SetLoadOrder;
        private TranslateStringDelegate D_TranslateString;
        private SendNotificationDelegate D_SendNotification;
        private GetInstallPathDelegate D_GetInstallPath;
        private ReadFileContentDelegate D_ReadFileContent;
        private ReadDirectoryFileListDelegate D_ReadDirectoryFileList;

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

        private void ThrowIfNoCallbacksRegistered()
        {
            if (!_callbacksRegistered)
                throw new CallbacksNotRegisteredException();
        }
    }
}