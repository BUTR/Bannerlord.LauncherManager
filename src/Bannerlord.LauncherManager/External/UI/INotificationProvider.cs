using Bannerlord.LauncherManager.Models;

using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.External.UI;

public interface INotificationProvider
{
    /// <summary>
    /// Sends a UI notification
    /// </summary>
    Task SendNotificationAsync(string id, NotificationType type, string message, uint displayMs);
}