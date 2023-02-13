using Bannerlord.LauncherManager.Localization;
using Bannerlord.LauncherManager.Models;

using BUTR.NativeAOT.Shared;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Bannerlord.LauncherManager.Native;

internal sealed unsafe class LauncherManagerHandlerNative : LauncherManagerHandler, IDisposable
{
    public static LauncherManagerHandlerNative? FromPointer(void* ptr) => GCHandle.FromIntPtr(new IntPtr(ptr)).Target as LauncherManagerHandlerNative;

    public N_SetGameParametersDelegate D_SetGameParameters = (_, _, _) => throw new CallbacksNotRegisteredException();
    public N_GetLoadOrderDelegate D_GetLoadOrder = (_) => throw new CallbacksNotRegisteredException();
    public N_SetLoadOrderDelegate D_SetLoadOrder = (_, _) => throw new CallbacksNotRegisteredException();
    public N_SendNotificationDelegate D_SendNotification = (_, _, _, _, _) => throw new CallbacksNotRegisteredException();
    public N_SendDialogDelegate D_SendDialog = (_, _, _, _, _, _, _) => throw new CallbacksNotRegisteredException();
    public N_GetInstallPathDelegate D_GetInstallPath = (_) => throw new CallbacksNotRegisteredException();
    public N_ReadFileContentDelegate D_ReadFileContent = (_, _) => throw new CallbacksNotRegisteredException();
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

    // TODO:
    public override SaveMetadata? GetSaveMetadata(string fileName, byte[] data)
    {
        return base.GetSaveMetadata(fileName, data);
    }

    // TODO:
    public override SaveMetadata[] GetSaveFiles()
    {
        return base.GetSaveFiles();
    }

    // TODO:
    public override string? GetSaveFilePath(string saveFile)
    {
        return base.GetSaveFilePath(saveFile);
    }
}