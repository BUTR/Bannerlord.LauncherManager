﻿using Bannerlord.LauncherManager.Localization;
using Bannerlord.LauncherManager.Models;
using Bannerlord.LauncherManager.Utils;
using Bannerlord.ModuleManager;

using System.Collections.Generic;
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
        var modules = GetModuleViewModels()?.Select(x => x.ModuleInfoExtended) ?? [];
        var sorted = SortHelper.AutoSort(modules);
        var sortedViewModels = GetViewModelsFromModules(sorted);
        SetModuleViewModels(sortedViewModels);

        SendNotification("sort-finished", NotificationType.Info, new BUTRTextObject("{=J7dh36Dy}Finished sorting!").ToString(), 3000);
        IsSorting = false;
    }
}