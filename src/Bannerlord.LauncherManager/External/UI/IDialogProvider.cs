using Bannerlord.LauncherManager.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.External.UI;

public interface IDialogProvider
{
    /// <summary>
    /// Creates an interactable dialog
    /// </summary>
    Task<string> SendDialogAsync(DialogType type, string title, string message, IReadOnlyList<DialogFileFilter> filters);
}