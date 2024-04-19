using Bannerlord.LauncherManager.External.UI;
using Bannerlord.LauncherManager.Models;

using BUTR.NativeAOT.Shared;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bannerlord.LauncherManager.Native.Adapters;

internal sealed unsafe class DialogProvider : IDialogProvider
{
    private readonly param_ptr* _pOwner;
    private readonly N_SendDialogDelegate _sendDialog;

    public DialogProvider(param_ptr* pOwner, N_SendDialogDelegate sendDialog)
    {
        _pOwner = pOwner;
        _sendDialog = sendDialog;
    }

    public void SendDialog(DialogType type, string title, string message, IReadOnlyList<DialogFileFilter> filters, Action<string> onResult)
    {
        SendDialogNative(type.ToStringFast().ToLowerInvariant(), title, message, filters, onResult);
    }

    private void SendDialogNative(ReadOnlySpan<char> type, ReadOnlySpan<char> title, ReadOnlySpan<char> message, IReadOnlyList<DialogFileFilter> filters, Action<string> onResult)
    {
        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static void Callback(param_ptr* p_ptr, param_string* p_result)
        {
            Logger.LogInput($"{nameof(SendDialogNative)}_{nameof(Callback)}");
            var result = new string(param_string.ToSpan(p_result));
            Logger.LogOutput(result, $"{nameof(SendDialogNative)}_{nameof(Callback)}");

            var handle = GCHandle.FromIntPtr(new IntPtr(p_ptr));
            try
            {
                var callbackStorage = handle.Target as CallbackStorage;
                callbackStorage?.SetResult(result);
            }
            catch (Exception e)
            {
                Logger.LogException(e, $"{nameof(SendDialogNative)}_{nameof(Callback)}");
            }
            finally
            {
                handle.Free();
            }
        }

        Logger.LogInput();

        var pCallbackStorage = GCHandle.ToIntPtr(GCHandle.Alloc(new CallbackStorage(result => onResult((string) result)), GCHandleType.Normal)).ToPointer();
        fixed (char* pType = type)
        fixed (char* pTitle = title)
        fixed (char* pFilters = BUTR.NativeAOT.Shared.Utils.SerializeJson(filters, Bindings.CustomSourceGenerationContext.IReadOnlyListDialogFileFilter))
        fixed (char* pMessage = message)
        {
            Logger.LogPinned(pType, pTitle, pFilters, pMessage);

            using var result = SafeStructMallocHandle.Create(_sendDialog(_pOwner, (param_string*) pType, (param_string*) pTitle, (param_string*) pMessage, (param_json*) pFilters, (param_ptr*) pCallbackStorage, &Callback), true);
            result.ValueAsVoid();

            Logger.LogOutput();
        }
    }
}