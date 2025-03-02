using Bannerlord.LauncherManager.Models;
using Bannerlord.ModuleManager;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Bannerlord.LauncherManager.Utils;

public static class SortHelper
{
    /// <summary>
    /// Given a list of modules provided as <paramref name="source"/>, automatically
    /// sorts the modules such that they form a valid load order based on module metadata.
    /// </summary>
    public static IEnumerable<ModuleInfoExtended> AutoSort(IEnumerable<ModuleInfoExtended> source)
    {
        var orderedModules = source
            .OrderByDescending(x => x.IsOfficial)
            .ThenBy(x => x.Id, new AlphanumComparatorFast())
            .ToArray();

        return ModuleSorter.TopologySort(orderedModules, module => ModuleUtilities.GetDependencies(orderedModules, module));
    }

    /// <summary>
    /// External<br/>
    /// </summary>
    public static void ToggleModuleSelection<TModuleViewModel>(IEnumerable<TModuleViewModel> moduleVMs, IDictionary<string, TModuleViewModel> lookup, TModuleViewModel moduleVM)
        where TModuleViewModel : class, IModuleViewModel
    {
        var modules = moduleVMs.Select(x => x.ModuleInfoExtended).ToList();
        var ctx = new DefaultModuleContext<TModuleViewModel>(lookup);

        if (moduleVM.IsSelected)
            ModuleUtilities.DisableModule(modules, moduleVM.ModuleInfoExtended, ctx.GetIsSelected, ctx.SetIsSelected, ctx.GetIsDisabled, ctx.SetIsDisabled);
        else
            ModuleUtilities.EnableModule(modules, moduleVM.ModuleInfoExtended, ctx.GetIsSelected, ctx.SetIsSelected, ctx.GetIsDisabled, ctx.SetIsDisabled);
    }

    /// <summary>
    /// External<br/>
    /// </summary>
    public static IEnumerable<string> ValidateModule<TModuleViewModel>(IEnumerable<TModuleViewModel> moduleVMs, IDictionary<string, TModuleViewModel> lookup, TModuleViewModel moduleVM)
        where TModuleViewModel : class, IModuleViewModel
    {
        var modules = moduleVMs.Select(x => x.ModuleInfoExtended).Concat(FeatureIds.LauncherFeatures.Select(x => new ModuleInfoExtended { Id = x })).ToList();
        var ctx = new DefaultModuleContext<TModuleViewModel>(lookup);

        return ModuleUtilities.ValidateModule(modules, moduleVM.ModuleInfoExtended, ctx.GetIsSelected, ctx.GetIsValid).Select(ModuleIssueRenderer.Render);
    }

    /// <summary>
    /// External<br/>
    /// </summary>
    public static void SortByDefault(List<IModuleViewModel> modules)
    {
        static IEnumerable<ModuleInfoExtended> Sort(IEnumerable<ModuleInfoExtended> source)
        {
            var orderedModules = source
                .OrderByDescending(x => x.IsOfficial)
                .ThenBy(x => x.Id, new AlphanumComparatorFast())
                .ToArray();

            return ModuleSorter.TopologySort(orderedModules, module => ModuleUtilities.GetDependencies(orderedModules, module));
        }

        var sorted = Sort(modules.Select(x => x.ModuleInfoExtended)).Select((x, i) => new { Item = x.Id, Index = i }).ToDictionary(x => x.Item, x => x.Index);
        modules.Sort(new ByIndexComparer<IModuleViewModel>(x => sorted.TryGetValue(x.ModuleInfoExtended.Id, out var idx) ? idx : -1));
        //var sorted = Sort(Modules2.Select(x => x.ModuleInfoExtended)).Select(x => x.Id).ToList();
        //SortBy(sorted);
    }

    /// <summary>
    /// External<br/>
    /// </summary>
    public static ChangeModulePositionResult ChangeModulePosition<TModuleViewModel>(IList<TModuleViewModel> moduleVMs, IDictionary<string, TModuleViewModel> lookup, TModuleViewModel targetModuleVM, int insertIndex)
        where TModuleViewModel : IModuleViewModel
    {
        static float Clamp(float value, float min, float max)
        {
            if (min > max)
            {
                throw new Exception();
            }

            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }

            return value;
        }

        if (insertIndex >= moduleVMs.IndexOf(targetModuleVM)) insertIndex--;
        insertIndex = (int) Clamp(insertIndex, 0f, moduleVMs.Count - 1);
        var currentModuleIndex = moduleVMs.IndexOf(targetModuleVM);
        if (insertIndex == -1 || currentModuleIndex == -1) return new(false, []);

        var modules = moduleVMs.Select(x => x.ModuleInfoExtended).ToList();
        var issuesReported = false;
        var issues = new List<string>();
        while (insertIndex != currentModuleIndex)
        {
            modules.RemoveAt(currentModuleIndex);
            modules.Insert(insertIndex, targetModuleVM.ModuleInfoExtended);
            var loadOrderValidationIssues = LoadOrderChecker.IsLoadOrderCorrect(modules.Where(x => lookup[x.Id] is { IsValid: true }).ToList()).ToList();
            if (loadOrderValidationIssues.Count == 0)
            {
                moduleVMs.RemoveAt(currentModuleIndex);
                moduleVMs.Insert(insertIndex, targetModuleVM);
                return new(true, issues);
            }

            if (!issuesReported)
            {
                issuesReported = true;
                issues.AddRange(loadOrderValidationIssues);
            }

            // Do it until we find the nearest acceptable index or stop if we fail
            modules.RemoveAt(insertIndex);
            modules.Insert(currentModuleIndex, targetModuleVM.ModuleInfoExtended);

            if (currentModuleIndex < insertIndex) insertIndex--;
            if (currentModuleIndex > insertIndex) insertIndex++;
        }

        return new(false, issues);
    }
}