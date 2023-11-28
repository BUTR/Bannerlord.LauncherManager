using Bannerlord.LauncherManager.Models;

using System;
using System.Collections.Generic;

namespace Bannerlord.LauncherManager;

partial class LauncherManagerHandler
{
    /// <summary>
    /// Internal<br/>
    /// </summary>
    protected internal void ShowWarning(string title, string contentPrimary, string contentSecondary, Action<bool> onResult)
    {
        SendDialog(DialogType.Warning, title, string.Join("--CONTENT-SPLIT--", contentPrimary, contentSecondary), Array.Empty<DialogFileFilter>(), resultRaw =>
        {
            onResult(bool.TryParse(resultRaw, out var result) && result);
        });
    }

    /// <summary>
    /// Internal<br/>
    /// </summary>
    protected internal void ShowFileOpen(string title, IReadOnlyList<DialogFileFilter> filters, Action<string> onResult)
    {
        SendDialog(DialogType.FileOpen, title, string.Empty, filters, onResult);
    }

    /// <summary>
    /// Internal<br/>
    /// </summary>
    protected internal void ShowFileSave(string title, string fileName, IReadOnlyList<DialogFileFilter> filters, Action<string> onResult)
    {
        SendDialog(DialogType.FileSave, title, fileName, filters, onResult);
    }
}