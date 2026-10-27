using Bannerlord.LauncherManager.Localization;
using Bannerlord.LauncherManager.Models;
using Bannerlord.LauncherManager.Native;
using Bannerlord.LauncherManager.Native.Models;
using Bannerlord.LauncherManager.Utils;
using Bannerlord.LauncherManager.Wasm.Adapters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;

using static Bannerlord.LauncherManager.Native.Bindings;

namespace Bannerlord.LauncherManager.Native
{
    public static partial class Bindings
    {
        public record OrderByLoadOrderResultNative(bool Result, IReadOnlyList<string>? Issues, IReadOnlyList<ModuleViewModel>? OrderedModuleViewModels);
    }
}

namespace Bannerlord.LauncherManager.Wasm
{
    /// <summary>
    /// LauncherManager instances, by the handler id the TS wrapper finds their callbacks with.
    /// The async methods complete through resolve and reject, see <see cref="AsyncExport"/>
    /// </summary>
    public static partial class LauncherManagerExports
    {
        private static readonly Dictionary<int, LauncherManagerHandlerNative> _handlers = new();
        private static int _nextHandler = 1;

        // Every export starts here, before its first await
        private static LauncherManagerHandlerNative Get(int handler)
        {
            MicrotaskSynchronizationContext.Install();
            return _handlers.TryGetValue(handler, out var value) ? value : throw new InvalidOperationException("Handler is null or wrong!");
        }

        [JSExport]
        public static int CreateHandler(string documentsPath)
        {
            var handler = _nextHandler++;
            _handlers[handler] = new LauncherManagerHandlerNative(new JsProviders(handler), documentsPath);
            return handler;
        }

        [JSExport]
        public static void DisposeHandler(int handler) => _handlers.Remove(handler);

        [JSExport]
        public static void CheckForRootHarmonyAsync(int handler, int call) =>
            AsyncExport.Run(call, () => Get(handler).CheckForRootHarmonyAsync());

        [JSExport]
        public static void GetGamePlatformAsync(int handler, int call) =>
            AsyncExport.RunString(call, async () => (await Get(handler).GetPlatformAsync()).ToStringFast());

        [JSExport]
        public static void GetGameVersionAsync(int handler, int call) =>
            AsyncExport.RunString(call, () => Get(handler).GetGameVersionAsync());

        [JSExport]
        public static void GetModulesAsync(int handler, int call) =>
            AsyncExport.RunString(call, async () => Json.Serialize(await Get(handler).GetModulesAsync(), CustomSourceGenerationContext.IReadOnlyListModuleInfoExtendedWithMetadata));

        [JSExport]
        public static void GetAllModulesAsync(int handler, int call) =>
            AsyncExport.RunString(call, async () => Json.Serialize(await Get(handler).GetAllModulesAsync(), CustomSourceGenerationContext.IReadOnlyListModuleInfoExtendedWithMetadata));

        [JSExport]
        public static void GetSaveFilePathAsync(int handler, string saveFile, int call) =>
            AsyncExport.RunString(call, () => Get(handler).GetSaveFilePathAsync(saveFile));

        [JSExport]
        public static void GetSaveFilesAsync(int handler, int call) =>
            AsyncExport.RunString(call, async () => Json.Serialize(await Get(handler).GetSaveFilesAsync(), CustomSourceGenerationContext.IReadOnlyListSaveMetadata));

        [JSExport]
        public static void GetSaveMetadataAsync(int handler, string saveFile, byte[] data, int call) =>
            AsyncExport.RunString(call, async () => Json.Serialize(await Get(handler).GetSaveMetadataAsync(saveFile, data), CustomSourceGenerationContext.SaveMetadata));

        [JSExport]
        public static string InstallModule(int handler, string files, string moduleInfos)
        {
            var filesArray = Json.Deserialize(files, CustomSourceGenerationContext.StringArray).Where(x => x != null!).ToArray();
            var moduleInfosArray = Json.Deserialize(moduleInfos, CustomSourceGenerationContext.ModuleInfoExtendedWithMetadataArray).Where(x => x != null!).ToArray();
            return Json.Serialize(Get(handler).InstallModuleContent(filesArray, moduleInfosArray), CustomSourceGenerationContext.InstallResult);
        }

        [JSExport]
        public static void IsObfuscatedAsync(int handler, string module, int call) =>
            AsyncExport.RunBoolean(call, () => Get(handler).IsObfuscatedAsync(Json.Deserialize(module, CustomSourceGenerationContext.ModuleInfoExtendedWithMetadata)));

        [JSExport]
        public static void ScanModAsync(int handler, string module, int call) =>
            AsyncExport.RunString(call, async () => Json.Serialize(await Get(handler).ScanModAsync(Json.Deserialize(module, CustomSourceGenerationContext.ModuleInfoExtendedWithMetadata)), CustomSourceGenerationContext.ListScanResultDto));

        [JSExport]
        public static bool IsSorting(int handler) => Get(handler).IsSorting;

        [JSExport]
        public static void ModuleListHandlerExportAsync(int handler, int call) =>
            AsyncExport.Run(call, () => new ModuleListHandler(Get(handler)).ExportAsync());

        [JSExport]
        public static void ModuleListHandlerExportSaveFileAsync(int handler, string saveFile, int call) =>
            AsyncExport.Run(call, () => new ModuleListHandler(Get(handler)).ExportSaveFileAsync(saveFile));

        [JSExport]
        public static void ModuleListHandlerImportAsync(int handler, int call) =>
            AsyncExport.RunBoolean(call, () => new ModuleListHandler(Get(handler)).ImportAsync());

        [JSExport]
        public static void ModuleListHandlerImportSaveFileAsync(int handler, string saveFile, int call) =>
            AsyncExport.RunBoolean(call, () => new ModuleListHandler(Get(handler)).ImportSaveFileAsync(saveFile));

        [JSExport]
        public static void OrderByLoadOrderAsync(int handler, string loadOrder, int call) =>
            AsyncExport.RunString(call, async () =>
            {
                var launcherManager = Get(handler);
                var order = Json.Deserialize(loadOrder, CustomSourceGenerationContext.LoadOrder);
                var isSelected = order.ToDictionary(x => x.Key, x => x.Value.IsSelected);
                var (result, issues, orderedModuleViewModels) = await launcherManager.TryOrderByLoadOrderAsync(
                    order.OrderBy(x => x.Value.Index).Select(x => x.Key),
                    x => isSelected.TryGetValue(x, out var value) && value);
                var native = new OrderByLoadOrderResultNative(result, issues, orderedModuleViewModels?.Select(x => new ModuleViewModel(x.ModuleInfoExtended, x.IsValid)
                {
                    IsSelected = x.IsSelected,
                    IsDisabled = x.IsDisabled,
                    Index = x.Index,
                }).ToArray());
                return Json.Serialize(native, CustomSourceGenerationContext.OrderByLoadOrderResultNative);
            });

        [JSExport]
        public static void RefreshModulesAsync(int handler, int call) =>
            AsyncExport.Run(call, () => Get(handler).RefreshModulesAsync());

        [JSExport]
        public static void RefreshGameParametersAsync(int handler, int call) =>
            AsyncExport.Run(call, () => Get(handler).RefreshGameParametersAsync());

        [JSExport]
        public static void SetGameParameterExecutableAsync(int handler, string executable, int call) =>
            AsyncExport.Run(call, () => Get(handler).SetGameParameterExecutableAsync(executable));

        [JSExport]
        public static void SetGameParameterLoadOrderAsync(int handler, string loadOrder, int call) =>
            AsyncExport.Run(call, () => Get(handler).SetGameParameterLoadOrderAsync(Json.Deserialize(loadOrder, CustomSourceGenerationContext.LoadOrder)));

        [JSExport]
        public static void SetGameParameterSaveFileAsync(int handler, string saveFile, int call) =>
            AsyncExport.Run(call, () => Get(handler).SetGameParameterSaveFileAsync(saveFile));

        [JSExport]
        public static void SetGameParameterContinueLastSaveFileAsync(int handler, bool value, int call) =>
            AsyncExport.Run(call, () => Get(handler).SetGameParameterContinueLastSaveFileAsync(value));

        [JSExport]
        public static void SetGameStore(int handler, string gameStore) => Get(handler).SetGameStore(Enum.Parse<GameStore>(gameStore));

        [JSExport]
        public static void SortAsync(int handler, int call) =>
            AsyncExport.Run(call, () => Get(handler).SortAsync());

        [JSExport]
        public static void SortHelperChangeModulePositionAsync(int handler, string moduleViewModel, int insertIndex, int call) =>
            AsyncExport.RunBoolean(call, async () =>
            {
                var launcherManager = Get(handler);
                var target = Json.Deserialize(moduleViewModel, CustomSourceGenerationContext.ModuleViewModel);
                var modules = (await launcherManager.GetModuleViewModelsAsync())?.ToArray() ?? [];
                var lookup = modules.ToDictionary(x => x.ModuleInfoExtended.Id, x => x);
                var (result, issues) = SortHelper.ChangeModulePosition(modules, lookup, target, insertIndex);
                if (issues is { Count: > 0 })
                    await launcherManager.ShowHintAsync(new BUTRTextObject("{=sP1a61KE}Failed to place the module to the desired position! Placing to the nearest available!{NL}Reason:{NL}{REASONS}").SetTextVariable("REASONS", string.Join("\n", issues)).ToString());
                return result;
            });

        [JSExport]
        public static void SortHelperToggleModuleSelectionAsync(int handler, string moduleViewModel, int call) =>
            AsyncExport.RunString(call, async () =>
            {
                var target = Json.Deserialize(moduleViewModel, CustomSourceGenerationContext.ModuleViewModel);
                var modules = (await Get(handler).GetModuleViewModelsAsync())?.ToArray() ?? [];
                var lookup = modules.ToDictionary(x => x.ModuleInfoExtended.Id, x => x);
                SortHelper.ToggleModuleSelection(modules, lookup, target);
                return Json.Serialize(target, CustomSourceGenerationContext.ModuleViewModel);
            });

        [JSExport]
        public static void SortHelperValidateModuleAsync(int handler, string moduleViewModel, int call) =>
            AsyncExport.RunString(call, async () =>
            {
                var target = Json.Deserialize(moduleViewModel, CustomSourceGenerationContext.ModuleViewModel);
                var modules = (await Get(handler).GetModuleViewModelsAsync())?.ToArray() ?? [];
                var lookup = modules.ToDictionary(x => x.ModuleInfoExtended.Id, x => x);
                return Json.Serialize(SortHelper.ValidateModule(modules, lookup, target).ToArray(), CustomSourceGenerationContext.StringArray);
            });

        [JSExport]
        public static string TestModule(int handler, string files) =>
            Json.Serialize(Get(handler).TestModuleContent(Json.Deserialize(files, CustomSourceGenerationContext.StringArray)), CustomSourceGenerationContext.SupportedResult);

        [JSExport]
        public static void DialogTestWarningAsync(int handler, int call) =>
            AsyncExport.RunString(call, () => Get(handler).SendDialogAsync(DialogType.Warning, "Test Title", "Test Message", Array.Empty<DialogFileFilter>()));

        [JSExport]
        public static void DialogTestFileOpenAsync(int handler, int call) =>
            AsyncExport.RunString(call, () => Get(handler).SendDialogAsync(DialogType.FileOpen, "Test Title", "Test Message", [new DialogFileFilter("Test Filter", ["*.test"])]));
    }
}
