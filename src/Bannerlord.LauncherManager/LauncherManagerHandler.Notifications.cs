using Bannerlord.LauncherManager.Localization;
using Bannerlord.LauncherManager.Models;

using System;

namespace Bannerlord.LauncherManager;

public partial class LauncherManagerHandler
{
    /// <summary>
    /// Internal<br/>
    /// </summary>
    protected internal void ShowHint(BUTRTextObject message) => SendNotification(Guid.NewGuid().ToString(), NotificationType.Hint, message.ToString(), 0);

    /// <summary>
    /// Internal<br/>
    /// </summary>
    protected internal void ShowHint(string message) => SendNotification(Guid.NewGuid().ToString(), NotificationType.Hint, message, 0);
}