using Bannerlord.LauncherManager.Models;

using System;
using System.Collections.Generic;

namespace Bannerlord.LauncherManager.External.UI;

public interface IDialogProvider
{
    /// <summary>
    /// Creates an interactable dialog
    /// </summary>
    /// <param name="type"></param>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="filters"></param>
    /// <param name="onResult"></param>
    void SendDialog(DialogType type, string title, string message, IReadOnlyList<DialogFileFilter> filters, Action<string> onResult);
}