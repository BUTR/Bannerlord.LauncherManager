using Bannerlord.LauncherManager.Localization;
using Bannerlord.LauncherManager.Models;

using System;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager;

partial class LauncherManagerHandler
{
    /// <summary>
    /// Internal<br/>
    /// </summary>
    protected internal async Task ShowHintAsync(BUTRTextObject message) => await SendNotificationAsync(Guid.NewGuid().ToString(), NotificationType.Hint, message.ToString(), 0);

    /// <summary>
    /// Internal<br/>
    /// </summary>
    protected internal async Task ShowHintAsync(string message) => await SendNotificationAsync(Guid.NewGuid().ToString(), NotificationType.Hint, message, 0);
}