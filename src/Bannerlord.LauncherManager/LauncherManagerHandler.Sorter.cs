using Bannerlord.LauncherManager.Localization;
using Bannerlord.LauncherManager.Models;
using Bannerlord.ModuleManager;

using System;
using System.Linq;

namespace Bannerlord.LauncherManager;

partial class LauncherManagerHandler
{
    //private const int SORTING = 1;
    // private const int NOTSORTING = 0;
    // if (Interlocked.CompareExchange(ref IsSorting, SORTING, NOTSORTING) == NOTSORTING)
    public bool IsSorting;

    /// <summary>
    /// External<br/>
    /// </summary>
    public void Sort()
    {
        IsSorting = true;
        var modules = GetModuleViewModels()?.Select(x => x.ModuleInfoExtended).ToArray() ?? Array.Empty<ModuleInfoExtendedWithPath>();
        var sorted = ModuleSorter.Sort(modules);
        SaveLoadOrder(GetFromModules(sorted));

        SendNotification("sort-finished", NotificationType.Info, new BUTRTextObject("{=J7dh36Dy}Finished sorting!").ToString(), 3000);
        IsSorting = false;
    }
}