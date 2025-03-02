using Bannerlord.LauncherManager.Models;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager;

partial class LauncherManagerHandler
{
    /// <summary>
    /// Internal<br/>
    /// </summary>
    protected internal async Task<bool> ShowWarningAsync(string title, string contentPrimary, string contentSecondary)
    {
        var resultRaw = await SendDialogAsync(DialogType.Warning, title, string.Join("--CONTENT-SPLIT--", contentPrimary, contentSecondary), Array.Empty<DialogFileFilter>());
        return bool.TryParse(resultRaw, out var result) && result;
    }

    /// <summary>
    /// Internal<br/>
    /// </summary>
    protected internal async Task<string> ShowFileOpenAsync(string title, IReadOnlyList<DialogFileFilter> filters)
    {
        return await SendDialogAsync(DialogType.FileOpen, title, string.Empty, filters);
    }

    /// <summary>
    /// Internal<br/>
    /// </summary>
    protected internal async Task<string> ShowFileSaveAsync(string title, string fileName, IReadOnlyList<DialogFileFilter> filters)
    {
        return await SendDialogAsync(DialogType.FileSave, title, fileName, filters);
    }
}