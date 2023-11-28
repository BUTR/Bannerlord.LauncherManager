using Bannerlord.LauncherManager.Models;

namespace Bannerlord.LauncherManager.External;

public interface INotificationUIProvider
{
    void SendNotification(string id, NotificationType type, string message, uint displayMs);
}