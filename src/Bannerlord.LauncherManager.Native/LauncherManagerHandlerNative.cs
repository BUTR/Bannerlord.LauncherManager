using Bannerlord.LauncherManager.Localization;
using Bannerlord.LauncherManager.Models;

using BUTR.NativeAOT.Shared;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace Bannerlord.LauncherManager.Native;

internal sealed unsafe class LauncherManagerHandlerNative : LauncherManagerHandler, IDisposable
{
    private static readonly string SavePath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Mount and Blade II Bannerlord", "Game Saves", "Native");
    
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

    private bool TryReadSaveMetadata(string fileName, Span<byte> data, out int length, [NotNullWhen(true)] out SaveMetadata? saveMetadata)
    {
        length = BitConverter.ToInt32(data.Slice(0, 4));
        if (length > data.Length)
        {
            length = length + 4;
            saveMetadata = null;
            return false;
        }
        saveMetadata = JsonSerializer.Deserialize(data.Slice(4), Bindings.CustomSourceGenerationContext.SaveMetadata);
        if (saveMetadata is null) return false;
        
        saveMetadata["Name"] = fileName;
        return true;
    }
    
    public override SaveMetadata? GetSaveMetadata(string fileName, byte[] data)
    {
        if (TryReadSaveMetadata(fileName, data, out _, out var saveMetadata))
            return saveMetadata;
        return null;
    }

    public override SaveMetadata[] GetSaveFiles()
    {
        IEnumerable<SaveMetadata> GetSaveFilesInternal()
        {
            foreach (var file in ReadDirectoryFileList(SavePath) ?? Array.Empty<string>())
            {
                if (ReadFileContent(file, 0, 1024) is not { } data) continue;
                if (!TryReadSaveMetadata(file, data, out var length, out var saveMetadata))
                {
                    Span<byte> dataFull = new byte[length];
                    data.CopyTo(dataFull);
                    if (ReadFileContent(file, 1024, length - 1024) is not { } dataExtended) continue;
                    dataExtended.CopyTo(dataFull.Slice(1024));
                    if (!TryReadSaveMetadata(file, data, out _, out saveMetadata)) continue;
                }
                yield return saveMetadata;
            }
        }
        return GetSaveFilesInternal().ToArray();
    }

    public override string? GetSaveFilePath(string saveFile) => Path.Combine(SavePath, $"{saveFile}.sav");
}