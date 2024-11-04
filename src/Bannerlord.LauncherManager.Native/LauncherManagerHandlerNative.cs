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

namespace Bannerlord.LauncherManager.Native;

internal sealed unsafe class LauncherManagerHandlerNative : LauncherManagerHandler, IDisposable
{
    private static readonly string SavePath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Mount and Blade II Bannerlord", "Game Saves");

    public static LauncherManagerHandlerNative? FromPointer(void* ptr) => GCHandle.FromIntPtr(new IntPtr(ptr)).Target as LauncherManagerHandlerNative;

    public param_ptr* OwnerPtr { get; }
    public VoidPtr* HandlePtr { get; }

    public LauncherManagerHandlerNative(param_ptr* pOwner,
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

    private void ReleaseUnmanagedResources()
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

    public new void SetGameParameterLoadOrder(LoadOrder loadOrder) => base.SetGameParameterLoadOrder(loadOrder);


    public new IReadOnlyList<ModuleInfoExtendedWithMetadata> GetModules() => base.GetModules();
    public new IReadOnlyList<ModuleInfoExtendedWithMetadata> GetAllModules() => base.GetAllModules();

    public new IEnumerable<IModuleViewModel>? GetModuleViewModels() => base.GetModuleViewModels();

    public new void ShowHint(BUTRTextObject message) => base.ShowHint(message);
    public new void ShowHint(string message) => base.ShowHint(message);

    public new void SendDialog(DialogType type, string title, string message, IReadOnlyList<DialogFileFilter> filters, Action<string> onResult) => base.SendDialog(type, title, message, filters, onResult);

    public override SaveMetadata? GetSaveMetadata(string fileName, ReadOnlySpan<byte> data)
    {
        var length = BitConverter.ToInt32(data.Slice(0, 4));
        if (length > 5 * 1024 * 1024) return null; // 5MB JSON? Orly?
        if (length > data.Length - 4) return null;

        try
        {
            var metadata = JsonSerializer.Deserialize(Encoding.UTF8.GetString(data.Slice(4, length)), Bindings.CustomSourceGenerationContext.TWSaveMetadata);
            if (metadata == null) return null;
            return new SaveMetadata(Path.GetFileName(fileName), metadata);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public override SaveMetadata[] GetSaveFiles()
    {
        IEnumerable<SaveMetadata> GetSaveFilesInternal()
        {
            foreach (var filePath in ReadDirectoryFileList(SavePath) ?? [])
            {
                if (ReadFileContent(filePath, 0, 4) is not { } lengthData) continue;
                var length = BitConverter.ToInt32(lengthData, 0);
                if (length > 5 * 1024 * 1024) continue; // 5MB JSON? Orly?
                if (ReadFileContent(filePath, 4, length) is not { } jsonData) continue;

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
        return GetSaveFilesInternal().ToArray();
    }

    public override string GetSaveFilePath(string saveFile) => Path.Combine(SavePath, $"{saveFile}.sav");


    public override string GetGameVersion()
    {
        var gamePath = GetInstallPath();
        return Fetcher.GetVersion(gamePath, Constants.TaleWorldsLibrary);
    }

    public override int GetChangeset()
    {
        var gamePath = GetInstallPath();
        return Fetcher.GetChangeSet(gamePath, Constants.TaleWorldsLibrary);
    }
}