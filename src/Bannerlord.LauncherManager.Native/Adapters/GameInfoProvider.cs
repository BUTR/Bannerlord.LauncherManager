using Bannerlord.LauncherManager.External;

using BUTR.NativeAOT.Shared;

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.Native.Adapters;

internal sealed class GameInfoProvider : IGameInfoProvider
{
    private readonly unsafe param_ptr* _pOwner;
    private readonly N_GetInstallPathDelegate _getInstallPath;

    public unsafe GameInfoProvider(param_ptr* pOwner, N_GetInstallPathDelegate getInstallPath)
    {
        _pOwner = pOwner;
        _getInstallPath = getInstallPath;
    }

    public async Task<string> GetInstallPathAsync()
    {
        var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        GetInstallPathNative(tcs);
        return await tcs.Task;
    }
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void GetInstallPathNativeCallback(param_ptr* pOwner, param_string* pResult)
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

    private unsafe void GetInstallPathNative(TaskCompletionSource<string> tcs)
    {
        Logger.LogInput();

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);
        
        try
        {
            using var result = SafeStructMallocHandle.Create(_getInstallPath(_pOwner, (param_ptr*) GCHandle.ToIntPtr(handle), &GetInstallPathNativeCallback), true);
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