using Bannerlord.LauncherManager.Models;

using System;

namespace Bannerlord.LauncherManager.External.UI;

public sealed class CallbackNotificationProvider : INotificationProvider
{
    private readonly Action<string, NotificationType, string, uint> _sendNotification;

    public CallbackNotificationProvider(Action<string, NotificationType, string, uint> sendNotification)
    {
        _sendNotification = sendNotification;
    }

    public void SendNotification(string id, NotificationType type, string message, uint displayMs) => _sendNotification(id, type, message, displayMs);
}