using Bannerlord.LauncherManager.External.UI;
using Bannerlord.LauncherManager.Models;

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
        var tcs = new TaskCompletionSource<object?>();
        SendNotificationNative(id, type.ToStringFast().ToLowerInvariant(), message, displayMs, tcs);
        return tcs.Task;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static void SendNotificationCallback(param_ptr* pOwner)
    {
        Logger.LogInput(pOwner);

        if (pOwner == null)
            return;
        
        var handle = GCHandle.FromIntPtr((IntPtr) pOwner);
        var tcs = default(TaskCompletionSource<object?>);
        try
        {
            tcs = (TaskCompletionSource<object?>) handle.Target!;
            tcs.TrySetResult(null);
            
            Logger.LogOutput();
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

    private void SendNotificationNative(ReadOnlySpan<char> id, ReadOnlySpan<char> type, ReadOnlySpan<char> message, uint displayMs, TaskCompletionSource<object?> tcs)
    {
        Logger.LogInput(displayMs);

        fixed (char* pId = id)
        fixed (char* pType = type)
        fixed (char* pMessage = message)
        {
            Logger.LogPinned(pId, pType, pMessage);

            var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);
        
            try
            {
                using var result = SafeStructMallocHandle.Create(_sendNotification(_pOwner, (param_string*) pId, (param_string*) pType, (param_string*) pMessage, displayMs, (param_ptr*) GCHandle.ToIntPtr(handle), &SendNotificationCallback), true);
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