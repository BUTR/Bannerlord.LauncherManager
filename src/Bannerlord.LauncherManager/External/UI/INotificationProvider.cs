using Bannerlord.LauncherManager.Models;

namespace Bannerlord.LauncherManager.External.UI;

public interface INotificationProvider
{
    /// <summary>
    /// Sends a UI notification
    /// </summary>
    /// <param name="id"></param>
    /// <param name="type"></param>
    /// <param name="message"></param>
    /// <param name="displayMs"></param>
    void SendNotification(string id, NotificationType type, string message, uint displayMs);
}