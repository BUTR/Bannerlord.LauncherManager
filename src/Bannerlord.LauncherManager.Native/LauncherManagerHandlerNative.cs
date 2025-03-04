using Bannerlord.LauncherManager.External;
using Bannerlord.LauncherManager.External.UI;
using Bannerlord.LauncherManager.Localization;
using Bannerlord.LauncherManager.Models;

using BUTR.NativeAOT.Shared;

using FetchBannerlordVersion;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.Native;

internal sealed partial class LauncherManagerHandlerNative : LauncherManagerHandler, IDisposable
{
    private static readonly string SavePath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Mount and Blade II Bannerlord", "Game Saves");

    public static unsafe LauncherManagerHandlerNative? FromPointer(void* ptr) => GCHandle.FromIntPtr(new IntPtr(ptr)).Target as LauncherManagerHandlerNative;

    public unsafe param_ptr* OwnerPtr { get; }
    public unsafe VoidPtr* HandlePtr { get; }

    public unsafe LauncherManagerHandlerNative(param_ptr* pOwner,
        ILauncherStateProvider launcherStateUProvider,
        IGameInfoProvider gameInfoProvider,
        IFileSystemProvider fileSystemProvider,
        IDialogProvider dialogProvider,
        INotificationProvider notificationProvider,
        ILoadOrderStateProvider loadOrderStateProvider) :
        base(launcherStateUProvider, gameInfoProvider, fileSystemProvider, dialogProvider, notificationProvider, loadOrderStateProvider)
    {
        OwnerPtr = pOwner;
        HandlePtr = (VoidPtr*) GCHandle.ToIntPtr(GCHandle.Alloc(this, GCHandleType.Normal)).ToPointer();
    }

    private unsafe void ReleaseUnmanagedResources()
    {
        var handle = GCHandle.FromIntPtr(new IntPtr(HandlePtr));
        if (handle.IsAllocated) handle.Free();
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~LauncherManagerHandlerNative()
    {
        ReleaseUnmanagedResources();
    }

    public new async Task SetGameParameterLoadOrderAsync(LoadOrder loadOrder) => await base.SetGameParameterLoadOrderAsync(loadOrder);


    public new async Task<IReadOnlyList<ModuleInfoExtendedWithMetadata>> GetModulesAsync() => await base.GetModulesAsync();
    public new async Task<IReadOnlyList<ModuleInfoExtendedWithMetadata>> GetAllModulesAsync() => await base.GetAllModulesAsync();

    public new async Task<IEnumerable<IModuleViewModel>?> GetModuleViewModelsAsync() => await base.GetModuleViewModelsAsync();

    public new async Task ShowHintAsync(BUTRTextObject message) => await base.ShowHintAsync(message);
    public new async Task ShowHintAsync(string message) => await base.ShowHintAsync(message);

    public new async Task<string> SendDialogAsync(DialogType type, string title, string message, IReadOnlyList<DialogFileFilter> filters) => await base.SendDialogAsync(type, title, message, filters);

    public override Task<SaveMetadata?> GetSaveMetadataAsync(string fileName, ReadOnlyMemory<byte> data)
    {
        var length = BitConverter.ToInt32(data.Slice(0, 4).Span);
        if (length > 5 * 1024 * 1024) return Task.FromResult<SaveMetadata?>(null); // 5MB JSON? Orly?
        if (length > data.Length - 4) return Task.FromResult<SaveMetadata?>(null);

        try
        {
            var metadata = JsonSerializer.Deserialize(Encoding.UTF8.GetString(data.Slice(4, length).Span), Bindings.CustomSourceGenerationContext.TWSaveMetadata);
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
        async IAsyncEnumerable<SaveMetadata> GetSaveFilesInternal()
        {
            foreach (var filePath in await ReadDirectoryFileListAsync(SavePath) ?? [])
            {
                if (await ReadFileContentAsync(filePath, 0, 4) is not { } lengthData) continue;
                var length = BitConverter.ToInt32(lengthData, 0);
                if (length > 5 * 1024 * 1024) continue; // 5MB JSON? Orly?
                if (await ReadFileContentAsync(filePath, 4, length) is not { } jsonData) continue;

                SaveMetadata? saveMetadata;
                try
                {
                    var metadata = JsonSerializer.Deserialize(jsonData, Bindings.CustomSourceGenerationContext.TWSaveMetadata);
                    saveMetadata = metadata is not null ? new SaveMetadata(Path.GetFileNameWithoutExtension(filePath), metadata) : null;
                }
                catch (JsonException) { saveMetadata = null; }
                if (saveMetadata is null) continue;

                yield return saveMetadata;
            }
        }
        return await GetSaveFilesInternal().ToArrayAsync();
    }

    public override Task<string?> GetSaveFilePathAsync(string saveFile)
    {
        return Task.FromResult<string?>(Path.Combine(SavePath, $"{saveFile}.sav"));
    }


    public override async Task<string> GetGameVersionAsync()
    {
        var gamePath = await GetInstallPathAsync();
        return Fetcher.GetVersion(gamePath, Constants.TaleWorldsLibrary);
    }

    public override async Task<int> GetChangesetAsync()
    {
        var gamePath = await GetInstallPathAsync();
        return Fetcher.GetChangeSet(gamePath, Constants.TaleWorldsLibrary);
    }
}