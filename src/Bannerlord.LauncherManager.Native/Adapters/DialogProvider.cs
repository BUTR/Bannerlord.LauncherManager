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
    public static unsafe void SendDialogNativeCallback(param_ptr* pOwner, param_string* pResult)
    {
        Logger.LogInput(pOwner, pResult);

        if (pOwner == null)
            return;
        
        var handle = GCHandle.FromIntPtr((IntPtr) pOwner);
        var tcs = default(TaskCompletionSource<string>);
        try
        {
            var result = new string(param_string.ToSpan(pResult));

            tcs = (TaskCompletionSource<string>) handle.Target!;
            tcs.TrySetResult(result);
            
            Logger.LogOutput(result);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            tcs?.TrySetException(e);
        }
        finally
        {
            handle.Free();
        }
    }
    
    private unsafe void SendDialogNative(ReadOnlySpan<char> type, ReadOnlySpan<char> title, ReadOnlySpan<char> message, IReadOnlyList<DialogFileFilter> filters, TaskCompletionSource<string> tcs)
    {
        Logger.LogInput();

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);

        fixed (char* pType = type)
        fixed (char* pTitle = title)
        fixed (char* pFilters = BUTR.NativeAOT.Shared.Utils.SerializeJson(filters, Bindings.CustomSourceGenerationContext.IReadOnlyListDialogFileFilter) ?? string.Empty)
        fixed (char* pMessage = message)
        {
            Logger.LogPinned(pType, pTitle, pFilters, pMessage);

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