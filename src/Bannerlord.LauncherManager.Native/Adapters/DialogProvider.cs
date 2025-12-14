using Bannerlord.LauncherManager.External.UI;
using Bannerlord.LauncherManager.Models;
using Bannerlord.LauncherManager.Native.Extensions;

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

    public Task<string> SendDialogAsync(DialogType type, string title, string message, IReadOnlyList<DialogFileFilter> filters)
    {
#if DEBUG
        //using var logger = LogMethod(type.ToFormattable(), title.ToFormattable(), message.ToFormattable());
        using var logger = LogMethod(title.ToFormattable(), message.ToFormattable());
#else
        using var logger = LogMethod();
#endif

        try
        {
            var tcs = new TaskCompletionSource<string>();
            SendDialogNative(type.ToStringFast().ToLowerInvariant(), title, message, filters, tcs);
            return tcs.Task;
        }
        catch (Exception e)
        {
            logger.LogException(e);
            throw;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void SendDialogNativeCallback(param_ptr* pOwner, return_value_string* pResult)
    {
#if DEBUG
        using var logger = LogCallbackMethod(pResult);
#else
        using var logger = LogCallbackMethod(pResult);
#endif

        try
        {
            if (pOwner == null)
            {
                logger.LogException(new ArgumentNullException(nameof(pOwner)));
                return;
            }

            if (GCHandle.FromIntPtr((IntPtr) pOwner) is not { Target: TaskCompletionSource<string?> tcs } handle)
            {
                logger.LogException(new InvalidOperationException("Invalid GCHandle."));
                return;
            }

            using var result = SafeStructMallocHandle.Create(pResult, true);
            logger.LogResult(result);
            result.SetAsString(tcs);
            handle.Free();
        }
        catch (Exception e)
        {
            logger.LogException(e);
            throw;
        }
    }

    private unsafe void SendDialogNative(ReadOnlySpan<char> type, ReadOnlySpan<char> title, ReadOnlySpan<char> message, IReadOnlyList<DialogFileFilter> filters, TaskCompletionSource<string> tcs)
    {
#if DEBUG
        using var logger = LogMethod(type.ToString().ToFormattable(), message.ToString().ToFormattable());
#else
        using var logger = LogMethod();
#endif

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);

        fixed (char* pType = type)
        fixed (char* pTitle = title)
        fixed (char* pFilters = BUTR.NativeAOT.Shared.Utils.SerializeJson(filters, Bindings.CustomSourceGenerationContext.IReadOnlyListDialogFileFilter))
        fixed (char* pMessage = message)
        {
            try
            {
                using var result = SafeStructMallocHandle.Create(_sendDialog(_pOwner, (param_string*) pType, (param_string*) pTitle, (param_string*) pMessage, (param_json*) pFilters, (param_ptr*) GCHandle.ToIntPtr(handle), &SendDialogNativeCallback), true);
                logger.LogResult(result);
                result.ValueAsVoid();
            }
            catch (Exception e)
            {
                logger.LogException(e);
                tcs.TrySetException(e);
                handle.Free();
            }
        }
    }
}