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

    public Task<string> GetInstallPathAsync()
    {
#if DEBUG
        using var logger = LogMethod();
#else
        using var logger = LogMethod();
#endif

        try
        {
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            GetInstallPathNative(tcs);
            return tcs.Task;
        }
        catch (Exception e)
        {
            logger.LogException(e);
            throw;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void GetInstallPathNativeCallback(param_ptr* pOwner, return_value_string* pResult)
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

    private unsafe void GetInstallPathNative(TaskCompletionSource<string> tcs)
    {
#if DEBUG
        using var logger = LogMethod();
#else
        using var logger = SilentLogMethod();
#endif

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);

        try
        {
            using var result = SafeStructMallocHandle.Create(_getInstallPath(_pOwner, (param_ptr*) GCHandle.ToIntPtr(handle), &GetInstallPathNativeCallback), true);
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