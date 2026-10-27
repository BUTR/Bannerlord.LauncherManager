using Bannerlord.LauncherManager.External;
using Bannerlord.LauncherManager.External.UI;
using Bannerlord.LauncherManager.Models;
using Bannerlord.LauncherManager.Native.Models;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Bannerlord.LauncherManager.Native.Bindings;

namespace Bannerlord.LauncherManager.Wasm.Adapters;

/// <summary>
/// The providers of a LauncherManager instance, backed by the JS callbacks of its handler id
/// </summary>
internal sealed class JsProviders :
    ILauncherStateProvider, IGameInfoProvider, IFileSystemProvider, IDialogProvider, INotificationProvider, ILoadOrderStateProvider
{
    private readonly int _handler;

    public JsProviders(int handler) => _handler = handler;

    public Task SetGameParametersAsync(string executable, IReadOnlyList<string> gameParameters) =>
        JsCallbacks.SetGameParameters(_handler, executable, Json.Serialize(gameParameters, CustomSourceGenerationContext.IReadOnlyListString));

    public async Task<LauncherOptions> GetOptionsAsync() =>
        Json.Deserialize(await JsCallbacks.GetOptions(_handler) ?? "null", CustomSourceGenerationContext.LauncherOptions);

    public async Task<LauncherState> GetStateAsync() =>
        Json.Deserialize(await JsCallbacks.GetState(_handler) ?? "null", CustomSourceGenerationContext.LauncherState);

    public async Task<string> GetInstallPathAsync() => await JsCallbacks.GetInstallPath(_handler) ?? string.Empty;

    public async Task<byte[]?> ReadFileContentAsync(string filePath, int offset, int length) =>
        await JsCallbacks.ReadFileContent(_handler, filePath, offset, length) as byte[];

    public Task WriteFileContentAsync(string filePath, byte[]? data) => JsCallbacks.WriteFileContent(_handler, filePath, data);

    public async Task<string[]?> ReadDirectoryFileListAsync(string directoryPath) =>
        DeserializeStrings(await JsCallbacks.ReadDirectoryFileList(_handler, directoryPath));

    public async Task<string[]?> ReadDirectoryListAsync(string directoryPath) =>
        DeserializeStrings(await JsCallbacks.ReadDirectoryList(_handler, directoryPath));

    public async Task<string> SendDialogAsync(DialogType type, string title, string message, IReadOnlyList<DialogFileFilter> filters)
    {
        var json = Json.Serialize(filters, CustomSourceGenerationContext.IReadOnlyListDialogFileFilter);
        return await JsCallbacks.SendDialog(_handler, type.ToStringFast().ToLowerInvariant(), title, message, json) ?? string.Empty;
    }

    public Task SendNotificationAsync(string id, NotificationType type, string message, uint displayMs) =>
        JsCallbacks.SendNotification(_handler, id, type.ToStringFast().ToLowerInvariant(), message, displayMs);

    public async Task<IModuleViewModel[]?> GetAllModuleViewModelsAsync() =>
        DeserializeModuleViewModels(await JsCallbacks.GetAllModuleViewModels(_handler));

    public async Task<IModuleViewModel[]?> GetModuleViewModelsAsync() =>
        DeserializeModuleViewModels(await JsCallbacks.GetModuleViewModels(_handler));

    public Task SetModuleViewModelsAsync(IReadOnlyList<IModuleViewModel> moduleViewModels)
    {
        var json = Json.Serialize(moduleViewModels.OfType<ModuleViewModel>().ToList(), CustomSourceGenerationContext.IReadOnlyListModuleViewModel);
        return JsCallbacks.SetModuleViewModels(_handler, json);
    }

    private static string[]? DeserializeStrings(string? json) =>
        json is null ? null : Json.DeserializeOrNull(json, CustomSourceGenerationContext.StringArray);

    private static IModuleViewModel[]? DeserializeModuleViewModels(string? json)
    {
        if (json is null) return null;
        return Json.DeserializeOrNull(json, CustomSourceGenerationContext.IReadOnlyListModuleViewModel)?
            .Where(x => x != null!)
            .Cast<IModuleViewModel>()
            .OrderBy(x => x.Index)
            .ToArray();
    }
}
