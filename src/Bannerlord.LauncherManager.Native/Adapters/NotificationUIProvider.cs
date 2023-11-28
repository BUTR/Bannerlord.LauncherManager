using Bannerlord.LauncherManager.External;
using Bannerlord.LauncherManager.Models;

using BUTR.NativeAOT.Shared;

using System;

namespace Bannerlord.LauncherManager.Native.Adapters;

internal sealed unsafe class NotificationUIProvider : INotificationUIProvider
{
    private readonly param_ptr* _pOwner;
    private readonly N_SendNotificationDelegate _sendNotification;

    public NotificationUIProvider(param_ptr* pOwner, N_SendNotificationDelegate sendNotification)
    {
        _pOwner = pOwner;
        _sendNotification = sendNotification;
    }

    public void SendNotification(string id, NotificationType type, string message, uint displayMs) => SendNotification(id, type.ToStringFast().ToLowerInvariant(), message, displayMs);

    private void SendNotification(ReadOnlySpan<char> id, ReadOnlySpan<char> type, ReadOnlySpan<char> message, uint displayMs)
    {
        Logger.LogInput(displayMs);

        fixed (char* pId = id)
        fixed (char* pType = type)
        fixed (char* pMessage = message)
        {
            Logger.LogPinned(pId, pType, pMessage);

            using var result = SafeStructMallocHandle.Create(_sendNotification(_pOwner, (param_string*) pId, (param_string*) pType, (param_string*) pMessage, displayMs), true);
            result.ValueAsVoid();
        }

        Logger.LogOutput();
    }
}