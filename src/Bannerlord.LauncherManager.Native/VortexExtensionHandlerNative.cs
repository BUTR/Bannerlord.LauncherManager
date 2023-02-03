using BUTR.NativeAOT.Shared;

using System;
using System.Runtime.InteropServices;

namespace Bannerlord.VortexExtension.Native
{
    internal sealed unsafe class VortexExtensionHandlerNative : VortexExtensionHandler, IDisposable
    {
        public static VortexExtensionHandlerNative? FromPointer(void* ptr) => GCHandle.FromIntPtr(new IntPtr(ptr)).Target as VortexExtensionHandlerNative;

        public N_GetActiveProfileDelegate D_GetActiveProfile = (_) => throw new CallbacksNotRegisteredException();
        public N_GetProfileByIdDelegate D_GetProfileById = (_, _) => throw new CallbacksNotRegisteredException();
        public N_GetActiveGameIdDelegate D_GetActiveGameId = (_) => throw new CallbacksNotRegisteredException();
        public N_SetGameParametersDelegate D_SetGameParameters = (_, _, _, _) => throw new CallbacksNotRegisteredException();
        public N_GetLoadOrderDelegate D_GetLoadOrder = (_) => throw new CallbacksNotRegisteredException();
        public N_SetLoadOrderDelegate D_SetLoadOrder = (_, _) => throw new CallbacksNotRegisteredException();
        public N_TranslateStringDelegate D_TranslateString = (_, _, _) => throw new CallbacksNotRegisteredException();
        public N_SendNotificationDelegate D_SendNotification = (_, _, _, _, _) => throw new CallbacksNotRegisteredException();
        public N_GetInstallPathDelegate D_GetInstallPath = (_) => throw new CallbacksNotRegisteredException();
        public N_ReadFileContentDelegate D_ReadFileContent = (_, _) => throw new CallbacksNotRegisteredException();
        public N_ReadDirectoryFileList D_ReadDirectoryFileList = (_, _) => throw new CallbacksNotRegisteredException();
        public N_ReadDirectoryList D_ReadDirectoryList = (_, _) => throw new CallbacksNotRegisteredException();

        public param_ptr* OwnerPtr { get; }
        public VoidPtr* HandlePtr { get; }

        public VortexExtensionHandlerNative(param_ptr* pOwner)
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

        ~VortexExtensionHandlerNative()
        {
            ReleaseUnmanagedResources();
        }
    }
}