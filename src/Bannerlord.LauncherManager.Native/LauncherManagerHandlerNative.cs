using Bannerlord.LauncherManager.Localization;
using Bannerlord.LauncherManager.Models;

using BUTR.NativeAOT.Shared;

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

    public N_SetGameParametersDelegate D_SetGameParameters = (_, _, _) => throw new CallbacksNotRegisteredException();
    public N_GetLoadOrderDelegate D_GetLoadOrder = (_) => throw new CallbacksNotRegisteredException();
    public N_SetLoadOrderDelegate D_SetLoadOrder = (_, _) => throw new CallbacksNotRegisteredException();
    public N_SendNotificationDelegate D_SendNotification = (_, _, _, _, _) => throw new CallbacksNotRegisteredException();
    public N_SendDialogDelegate D_SendDialog = (_, _, _, _, _, _, _) => throw new CallbacksNotRegisteredException();
    public N_GetInstallPathDelegate D_GetInstallPath = (_) => throw new CallbacksNotRegisteredException();
    public N_ReadFileContentDelegate D_ReadFileContent = (_, _, _, _) => throw new CallbacksNotRegisteredException();
    public N_WriteFileContentDelegate D_WriteFileContent = (_, _, _, _) => throw new CallbacksNotRegisteredException();
    public N_ReadDirectoryFileList D_ReadDirectoryFileList = (_, _) => throw new CallbacksNotRegisteredException();
    public N_ReadDirectoryList D_ReadDirectoryList = (_, _) => throw new CallbacksNotRegisteredException();
    public N_GetModuleViewModels D_GetModuleViewModels = (_) => throw new CallbacksNotRegisteredException();
    public N_SetModuleViewModels D_SetModuleViewModels = (_, _) => throw new CallbacksNotRegisteredException();
    public N_GetOptions D_GetOptions = (_) => throw new CallbacksNotRegisteredException();
    public N_GetState D_GetState = (_) => throw new CallbacksNotRegisteredException();

    public param_ptr* OwnerPtr { get; }
    public VoidPtr* HandlePtr { get; }

    public LauncherManagerHandlerNative(param_ptr* pOwner)
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

    public new IReadOnlyList<ModuleInfoExtendedWithPath> GetModules() => base.GetModules();

    public new void ShowHint(BUTRTextObject message) => base.ShowHint(message);
    public new void ShowHint(string message) => base.ShowHint(message);

    public override SaveMetadata? GetSaveMetadata(string fileName, byte[] data)
    {
        var length = BitConverter.ToInt32(data.AsSpan(0, 4));
        if (length > 5 * 1024 * 1024) return null; // 5MB JSON? Orly?
        if (length > data.Length - 4) return null;

        try
        {
            var metadata = JsonSerializer.Deserialize(Encoding.UTF8.GetString(data.AsSpan(4, length)), Bindings.CustomSourceGenerationContext.TWSaveMetadata);
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
            foreach (var filePath in ReadDirectoryFileList(SavePath) ?? Array.Empty<string>())
            {
                if (ReadFileContent(filePath, 0, 4) is not { } lengthData) continue;
                var length = BitConverter.ToInt32(lengthData, 0);
                if (length > 5 * 1024 * 1024) continue; // 5MB JSON? Orly?
                if (ReadFileContent(filePath, 4, length) is not { } jsonData) continue;

                SaveMetadata? saveMetadata;
                try
                {
                    var metadata = JsonSerializer.Deserialize(jsonData, Bindings.CustomSourceGenerationContext.TWSaveMetadata);
                    saveMetadata = metadata is not null ? new SaveMetadata(JsonSerializer.Serialize(metadata, Bindings.CustomSourceGenerationContext.TWSaveMetadata), metadata) : null;
                }
                catch (JsonException) { saveMetadata = null; }
                if (saveMetadata is null) continue;

                yield return saveMetadata;
            }
        }
        return GetSaveFilesInternal().ToArray();
    }

    public override string? GetSaveFilePath(string saveFile) => Path.Combine(SavePath, $"{saveFile}.sav");
}