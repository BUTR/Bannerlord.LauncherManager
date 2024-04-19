using Bannerlord.LauncherManager.Models;

using System;
using System.Collections.Generic;

namespace Bannerlord.LauncherManager.External.UI;

public sealed class CallbackDialogProvider : IDialogProvider
{
    private readonly Action<DialogType, string, string, IReadOnlyList<DialogFileFilter>, Action<string>> _sendDialog;

    public CallbackDialogProvider(Action<DialogType, string, string, IReadOnlyList<DialogFileFilter>, Action<string>> sendDialog)
    {
        _sendDialog = sendDialog;
    }

    public void SendDialog(DialogType type, string title, string message, IReadOnlyList<DialogFileFilter> filters, Action<string> onResult) => _sendDialog(type, title, message, filters, onResult);
}