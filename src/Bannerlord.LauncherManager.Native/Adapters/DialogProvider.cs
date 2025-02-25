using Bannerlord.LauncherManager.External.UI;
using Bannerlord.LauncherManager.Models;

using BUTR.NativeAOT.Shared;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.Native.Adapters;

internal sealed class DialogProvider : IDialogProvider
{
    private readonly unsafe param_ptr* _pOwner;
    private readonly N_SendDialogDelegate _sendDialog;

    public unsafe DialogProvider(param_ptr* pOwner, N_SendDialogDelegate sendDialog)
    {
        _pOwner = pOwner;
        _sendDialog = sendDialog;
    }

    public async Task<string> SendDialogAsync(DialogType type, string title, string message, IReadOnlyList<DialogFileFilter> filters)
    {
        var tcs = new TaskCompletionSource<string>();
        SendDialogNative(type.ToStringFast().ToLowerInvariant(), title, message, filters, tcs);
        return await tcs.Task;
    }
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void SendDialogNativeCallback(param_ptr* pOwner, return_value_string* pResult)
    {
        Logger.LogCallbackInput(pResult);

        if (pOwner == null)
        {
            Logger.LogException(new ArgumentNullException(nameof(pOwner)));
            return;
        }
        
        if (GCHandle.FromIntPtr((IntPtr) pOwner) is not { Target: TaskCompletionSource<string?> tcs } handle)
        {
            Logger.LogException(new InvalidOperationException("Invalid GCHandle."));
            return;
        }
        
        using var result = SafeStructMallocHandle.Create(pResult, true);
        result.SetAsString(tcs);
        handle.Free();

        Logger.LogOutput();
    }
    
    private unsafe void SendDialogNative(ReadOnlySpan<char> type, ReadOnlySpan<char> title, ReadOnlySpan<char> message, IReadOnlyList<DialogFileFilter> filters, TaskCompletionSource<string> tcs)
    {
        Logger.LogInput();

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);

        fixed (char* pType = type)
        fixed (char* pTitle = title)
        fixed (char* pFilters = BUTR.NativeAOT.Shared.Utils.SerializeJson(filters, Bindings.CustomSourceGenerationContext.IReadOnlyListDialogFileFilter))
        fixed (char* pMessage = message)
        {
            try
            {
                using var result = SafeStructMallocHandle.Create(_sendDialog(_pOwner, (param_string*) pType, (param_string*) pTitle, (param_string*) pMessage, (param_json*) pFilters, (param_ptr*) GCHandle.ToIntPtr(handle), &SendDialogNativeCallback), true);
                result.ValueAsVoid();
                
                Logger.LogOutput();
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                tcs.TrySetException(e);
                handle.Free();
            }
        }
    }
}