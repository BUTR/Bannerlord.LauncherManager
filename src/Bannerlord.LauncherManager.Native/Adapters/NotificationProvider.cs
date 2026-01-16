using Bannerlord.LauncherManager.External.UI;
using Bannerlord.LauncherManager.Models;
using Bannerlord.LauncherManager.Native.Extensions;

using BUTR.NativeAOT.Shared;

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.Native.Adapters;

internal sealed unsafe class NotificationProvider : INotificationProvider
{
    private readonly param_ptr* _pOwner;
    private readonly N_SendNotificationDelegate _sendNotification;

    public NotificationProvider(param_ptr* pOwner, N_SendNotificationDelegate sendNotification)
    {
        _pOwner = pOwner;
        _sendNotification = sendNotification;
    }

    public Task SendNotificationAsync(string id, NotificationType type, string message, uint displayMs)
    {
#if DEBUG
        //using var logger = LogMethod(id.ToFormattable(), type.ToFormattable(), displayMs);
        using var logger = LogMethod(id.ToFormattable(), displayMs);
#else
        using var logger = LogMethod();
#endif

        try
        {
            var tcs = new TaskCompletionSource();
            SendNotificationNative(id, type.ToStringFast().ToLowerInvariant(), message, displayMs, tcs);
            return tcs.Task;
        }
        catch (Exception e)
        {
            logger.LogException(e);
            throw;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static void SendNotificationCallback(param_ptr* pOwner, return_value_void* pResult)
    {
#if DEBUG
        using var logger = LogCallbackMethod(pResult);
#else
        using var logger = LogCallbackMethod();
#endif

        try
        {
            if (pOwner == null)
            {
                logger.LogException(new ArgumentNullException(nameof(pOwner)));
                return;
            }

            if (GCHandle.FromIntPtr((IntPtr) pOwner) is not { Target: TaskCompletionSource tcs } handle)
            {
                logger.LogException(new InvalidOperationException("Invalid GCHandle."));
                return;
            }

            using var result = SafeStructMallocHandle.Create(pResult, true);
            logger.LogResult(result);
            result.SetAsVoid(tcs);
            handle.Free();
        }
        catch (Exception e)
        {
            logger.LogException(e);
            throw;
        }
    }

    private void SendNotificationNative(ReadOnlySpan<char> id, ReadOnlySpan<char> type, ReadOnlySpan<char> message, uint displayMs, TaskCompletionSource tcs)
    {
#if DEBUG
        using var logger = LogMethod(displayMs);
#else
        using var logger = SilentLogMethod();
#endif

        fixed (char* pId = id)
        fixed (char* pType = type)
        fixed (char* pMessage = message)
        {
            var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);

            try
            {
                using var result = SafeStructMallocHandle.Create(_sendNotification(_pOwner, (param_string*) pId, (param_string*) pType, (param_string*) pMessage, displayMs, (param_ptr*) GCHandle.ToIntPtr(handle), &SendNotificationCallback), true);
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