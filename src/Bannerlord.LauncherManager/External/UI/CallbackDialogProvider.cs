using Bannerlord.LauncherManager.Models;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.External.UI;

public sealed class CallbackDialogProvider : IDialogProvider
{
    private readonly Action<DialogType, string, string, IReadOnlyList<DialogFileFilter>, Action<string>> _sendDialog;

    public CallbackDialogProvider(Action<DialogType, string, string, IReadOnlyList<DialogFileFilter>, Action<string>> sendDialog)
    {
        _sendDialog = sendDialog;
    }

    public async Task<string> SendDialogAsync(DialogType type, string title, string message, IReadOnlyList<DialogFileFilter> filters)
    {
        var tcs = new TaskCompletionSource<string>();
        try
        {
            _sendDialog(type, title, message, filters, result => tcs.TrySetResult(result));
        }
        catch (Exception ex)
        {
            tcs.TrySetException(ex);
        }
        return await tcs.Task;
    }
}