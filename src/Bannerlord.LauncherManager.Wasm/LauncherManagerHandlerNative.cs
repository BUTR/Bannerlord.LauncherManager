using Bannerlord.LauncherManager.Localization;
using Bannerlord.LauncherManager.Models;
using Bannerlord.LauncherManager.Utils;
using Bannerlord.LauncherManager.Wasm;
using Bannerlord.LauncherManager.Wasm.Adapters;

using FetchBannerlordVersion;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.Native;

/// <summary>
/// The WebAssembly counterpart of the native handler. It keeps the name, so the native partials (MLVScan, obfuscation) are shared
/// </summary>
internal sealed partial class LauncherManagerHandlerNative : LauncherManagerHandler
{
    private static readonly string[] BinFolders = ["Win64_Shipping_Client", "Gaming.Desktop.x64_Shipping_Client"];

    private readonly string _savePath;

    public LauncherManagerHandlerNative(JsProviders providers, string documentsPath) :
        base(providers, providers, providers, providers, providers, providers)
    {
        _savePath = Path.Combine(documentsPath, "Mount and Blade II Bannerlord", "Game Saves");
    }

    public new async Task SetGameParameterLoadOrderAsync(LoadOrder loadOrder) => await base.SetGameParameterLoadOrderAsync(loadOrder);

    public new async Task<IReadOnlyList<ModuleInfoExtendedWithMetadata>> GetModulesAsync() => await base.GetModulesAsync();
    public new async Task<IReadOnlyList<ModuleInfoExtendedWithMetadata>> GetAllModulesAsync() => await base.GetAllModulesAsync();

    public new async Task<IEnumerable<IModuleViewModel>?> GetModuleViewModelsAsync() => await base.GetModuleViewModelsAsync();

    public new async Task ShowHintAsync(BUTRTextObject message) => await base.ShowHintAsync(message);
    public new async Task ShowHintAsync(string message) => await base.ShowHintAsync(message);

    public new async Task<string> SendDialogAsync(DialogType type, string title, string message, IReadOnlyList<DialogFileFilter> filters) => await base.SendDialogAsync(type, title, message, filters);

    public override Task<SaveMetadata?> GetSaveMetadataAsync(string fileName, byte[] data)
    {
        var length = BitConverter.ToInt32(data.AsSpan(0, 4));
        if (length > 5 * 1024 * 1024) return Task.FromResult<SaveMetadata?>(null); // 5MB JSON? Orly?
        if (length > data.Length - 4) return Task.FromResult<SaveMetadata?>(null);

        try
        {
            var metadata = JsonSerializer.Deserialize(Encoding.UTF8.GetString(data.AsSpan(4, length)), Bindings.CustomSourceGenerationContext.TWSaveMetadata);
            if (metadata == null) return Task.FromResult<SaveMetadata?>(null);
            return Task.FromResult<SaveMetadata?>(new SaveMetadata(Path.GetFileName(fileName), metadata));
        }
        catch (JsonException)
        {
            return Task.FromResult<SaveMetadata?>(null);
        }
    }

    public override async Task<IReadOnlyList<SaveMetadata>> GetSaveFilesAsync()
    {
        // The saves' reads overlap, see ReadAhead
        return await (await ReadDirectoryFileListAsync(_savePath) ?? []).ToAsyncEnumerable()
            .SelectInOrderAsync(ReadSaveFileMetadataAsync)
            .OfType<SaveMetadata>()
            .ToArrayAsync();
    }

    private async Task<SaveMetadata?> ReadSaveFileMetadataAsync(string filePath)
    {
        if (await ReadFileContentAsync(filePath, 0, 4) is not { } lengthData) return null;
        var length = BitConverter.ToInt32(lengthData, 0);
        if (length > 5 * 1024 * 1024) return null; // 5MB JSON? Orly?
        if (await ReadFileContentAsync(filePath, 4, length) is not { } jsonData) return null;

        try
        {
            var metadata = JsonSerializer.Deserialize(jsonData, Bindings.CustomSourceGenerationContext.TWSaveMetadata);
            return metadata is not null ? new SaveMetadata(Path.GetFileNameWithoutExtension(filePath), metadata) : null;
        }
        catch (JsonException) { return null; }
    }

    public override Task<string?> GetSaveFilePathAsync(string saveFile)
    {
        return Task.FromResult<string?>(Path.Combine(_savePath, $"{saveFile}.sav"));
    }

    public override async Task<string> GetGameVersionAsync()
    {
        using var game = await StageVersionFilesAsync();
        return Fetcher.GetVersion(game.Path, Constants.TaleWorldsLibrary);
    }

    public override async Task<int> GetChangesetAsync()
    {
        using var game = await StageVersionFilesAsync();
        return Fetcher.GetChangeSet(game.Path, Constants.TaleWorldsLibrary);
    }

    // The files Fetcher reads, from the first bin folder it would pick
    private async Task<StagedGameFolder> StageVersionFilesAsync()
    {
        var gamePath = await GetInstallPathAsync();
        var binDirectories = await ReadDirectoryListAsync(Path.Combine(gamePath, Constants.BinFolder)) ?? [];
        var binFolder = BinFolders.FirstOrDefault(x => binDirectories.Any(y => Path.GetFileName(y).Equals(x, StringComparison.OrdinalIgnoreCase))) ?? BinFolders[0];
        var binPath = Path.Combine(gamePath, Constants.BinFolder, binFolder);

        var files = await ReadDirectoryFileListAsync(binPath) ?? [];
        bool HasFile(string name) => files.Any(x => Path.GetFileName(x).Equals(name, StringComparison.OrdinalIgnoreCase));

        var libAssembly = HasFile(Constants.TaleWorldsLibrary) ? await ReadFileContentAsync(Path.Combine(binPath, Constants.TaleWorldsLibrary), 0, -1) : null;
        var versionXml = HasFile("Version.xml") ? await ReadFileContentAsync(Path.Combine(binPath, "Version.xml"), 0, -1) : null;
        return StagedGameFolder.Create(libAssembly, versionXml, Constants.TaleWorldsLibrary);
    }
}
