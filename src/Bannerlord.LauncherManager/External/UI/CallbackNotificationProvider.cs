using Bannerlord.LauncherManager.Models;

using System;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.External.UI;

public sealed class CallbackNotificationProvider : INotificationProvider
{
    private readonly Action<string, NotificationType, string, uint> _sendNotification;

    public CallbackNotificationProvider(Action<string, NotificationType, string, uint> sendNotification)
    {
        _sendNotification = sendNotification;
    }

    public Task SendNotificationAsync(string id, NotificationType type, string message, uint displayMs)
    {
        _sendNotification(id, type, message, displayMs);
        return Task.CompletedTask;
    }
}