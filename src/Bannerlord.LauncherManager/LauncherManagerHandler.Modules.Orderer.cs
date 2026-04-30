using Bannerlord.LauncherManager.Extensions;
using Bannerlord.LauncherManager.Models;
using Bannerlord.LauncherManager.Utils;
using Bannerlord.ModuleManager;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager;

partial class LauncherManagerHandler
{
    protected internal static bool IsVisible(bool isSingleplayer, ModuleInfoExtended moduleInfo) =>
        moduleInfo.IsNative() || !isSingleplayer && moduleInfo.IsMultiplayerModule || isSingleplayer && moduleInfo.IsSingleplayerModule;

    /// <summary>
    /// External<br/>
    /// </summary>
    public async Task<OrderByLoadOrderResult> TryOrderByLoadOrderAsync(
        IEnumerable<string> loadOrder,
        Func<string, bool> isModuleSelected)
    {
        var options = await GetOptionsAsync();

        return options.BetaSorting
            ? await TryOrderByLoadOrderBetaAsync(loadOrder, isModuleSelected)
            : await TryOrderByLoadOrderTWAsync(loadOrder, isModuleSelected);
    }

    /// <summary>
    /// Internal<br/>
    /// </summary>
    protected internal async Task<OrderByLoadOrderResult> TryOrderByLoadOrderTWAsync(IEnumerable<string> loadOrder, Func<string, bool> isModuleSelected, bool overwriteWhenFailure = false)
    {
        var moduleViewModels = await GetAllModuleViewModelsAsync() ?? [];
        var moduleViewModelLookup = moduleViewModels.ToDictionary(x => x.ModuleInfoExtended.Id, x => x);

        var state = await GetStateAsync();

        // Seed the input list with the user's saved order, then append the rest from cache
        var seenIds = new HashSet<string>();
        var originalOrderedModules = new List<ModuleInfoExtendedWithMetadata>();
        foreach (var id in loadOrder)
        {
            if (!ExtendedModuleInfoCache.TryGetValue(id, out var moduleInfoExtended)) continue;
            if (!IsVisible(state.IsSingleplayer, moduleInfoExtended)) continue;
            if (moduleInfoExtended is not ModuleInfoExtendedWithMetadata moduleInfoExtendedWithPath) continue;
            if (!seenIds.Add(id)) continue;

            originalOrderedModules.Add(moduleInfoExtendedWithPath);
        }

        foreach (var moduleInfoExtended in ExtendedModuleInfoCache.Values)
        {
            if (!IsVisible(state.IsSingleplayer, moduleInfoExtended)) continue;
            if (moduleInfoExtended is not ModuleInfoExtendedWithMetadata moduleInfoExtendedWithPath) continue;
            if (!seenIds.Add(moduleInfoExtended.Id)) continue;

            originalOrderedModules.Add(moduleInfoExtendedWithPath);
        }

        return BuildOrderResult(originalOrderedModules, moduleViewModelLookup, isModuleSelected, overwriteWhenFailure);
    }

    /// <summary>
    /// Internal<br/>
    /// Beta sorting: strictly validates the user's saved order against module dependencies and
    /// fails loudly if it is invalid (unlike the TW path, which silently auto-corrects). When the
    /// saved order is valid, falls back to a single-pass topological sort with the user's order
    /// as the tiebreaker among dependency-equivalent choices.
    /// </summary>
    protected internal async Task<OrderByLoadOrderResult> TryOrderByLoadOrderBetaAsync(IEnumerable<string> loadOrder, Func<string, bool> isModuleSelected, bool overwriteWhenFailure = false)
    {
        var moduleViewModels = await GetAllModuleViewModelsAsync() ?? [];
        var moduleViewModelLookup = moduleViewModels.ToDictionary(x => x.ModuleInfoExtended.Id, x => x);

        var state = await GetStateAsync();

        var visibleModules = new List<ModuleInfoExtendedWithMetadata>();
        foreach (var moduleInfoExtended in ExtendedModuleInfoCache.Values)
        {
            if (!IsVisible(state.IsSingleplayer, moduleInfoExtended)) continue;
            if (moduleInfoExtended is not ModuleInfoExtendedWithMetadata moduleInfoExtendedWithPath) continue;

            visibleModules.Add(moduleInfoExtendedWithPath);
        }

        // Strict pre-validation: surface dependency violations in the user's saved order
        // instead of silently rewriting it. Skipped when the caller asks to force-overwrite.
        if (!overwriteWhenFailure)
        {
            var seen = new HashSet<string>();
            var userOrderedSelected = new List<ModuleInfoExtended>();
            foreach (var id in loadOrder)
            {
                if (!seen.Add(id)) continue;
                if (!ExtendedModuleInfoCache.TryGetValue(id, out var m)) continue;
                if (!IsVisible(state.IsSingleplayer, m)) continue;
                if (!isModuleSelected(id)) continue;
                userOrderedSelected.Add(m);
            }

            var userOrderIssues = LoadOrderChecker.IsLoadOrderCorrect(userOrderedSelected).ToList();
            if (userOrderIssues.Count != 0)
            {
                var fallbackViewModels = visibleModules
                    .Where(x => moduleViewModelLookup.ContainsKey(x.Id))
                    .Select(x => moduleViewModelLookup[x.Id])
                    .ToList();
                return new(false, userOrderIssues, fallbackViewModels);
            }
        }

        var userPriority = new Dictionary<string, int>();
        foreach (var id in loadOrder)
        {
            if (!userPriority.ContainsKey(id))
                userPriority[id] = userPriority.Count;
        }

        // Order the input so TopologySort's DFS uses user preference as the tiebreaker among
        // dependency-equivalent choices. Modules absent from the user's order fall back to the
        // default ordering (official first, then alphanumeric by id).
        var alphanum = new AlphanumComparatorFast();
        visibleModules.Sort((a, b) =>
        {
            var aHasUser = userPriority.TryGetValue(a.Id, out var aP);
            var bHasUser = userPriority.TryGetValue(b.Id, out var bP);
            if (aHasUser && bHasUser) return aP.CompareTo(bP);
            if (aHasUser) return -1;
            if (bHasUser) return 1;
            if (a.IsOfficial != b.IsOfficial) return b.IsOfficial.CompareTo(a.IsOfficial);
            return alphanum.Compare(a.Id, b.Id);
        });

        return BuildOrderResult(visibleModules, moduleViewModelLookup, isModuleSelected, overwriteWhenFailure);
    }

    private static OrderByLoadOrderResult BuildOrderResult(
        List<ModuleInfoExtendedWithMetadata> orderedInput,
        IDictionary<string, IModuleViewModel> moduleViewModelLookup,
        Func<string, bool> isModuleSelected,
        bool overwriteWhenFailure)
    {
        var rawOrderedModules = ModuleSorter.TopologySort<ModuleInfoExtended>(orderedInput, x => ModuleUtilities.GetDependencies(orderedInput, x))
            .OfType<ModuleInfoExtendedWithMetadata>()
            .ToList();

        var existingOrderedModules = rawOrderedModules
            .Where(x => moduleViewModelLookup.ContainsKey(x.Id))
            .ToList();

        var existingOrderedViewModels = existingOrderedModules
            .Select(x => moduleViewModelLookup[x.Id])
            .ToList();

        var loadOrderValidationIssues = LoadOrderChecker.IsLoadOrderCorrect(
            existingOrderedModules.Where(x => moduleViewModelLookup[x.Id].IsSelected).ToList()).ToList();
        if (!overwriteWhenFailure && loadOrderValidationIssues.Count != 0)
            return new(false, loadOrderValidationIssues, existingOrderedViewModels);

        foreach (var moduleVM in existingOrderedViewModels)
        {
            if (isModuleSelected(moduleVM.ModuleInfoExtended.Id) != moduleVM.IsSelected)
                SortHelper.ToggleModuleSelection(existingOrderedViewModels, moduleViewModelLookup, moduleVM);
        }

        var idx = 0;
        foreach (var viewModel in existingOrderedViewModels)
            viewModel.Index = idx++;

        return new(true, loadOrderValidationIssues.Count > 0 ? loadOrderValidationIssues : null, existingOrderedViewModels);
    }
}
