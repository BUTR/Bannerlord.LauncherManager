using Bannerlord.LauncherManager.External;
using Bannerlord.LauncherManager.Models;
using Bannerlord.LauncherManager.Native.Extensions;

using BUTR.NativeAOT.Shared;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.Native.Adapters;

internal sealed class LauncherStateProvider : ILauncherStateProvider
{
    private readonly unsafe param_ptr* _pOwner;
    private readonly N_SetGameParametersDelegate _setGameParameters;
    private readonly N_GetOptions _getOptions;
    private readonly N_GetState _getState;

    public unsafe LauncherStateProvider(
        param_ptr* pOwner,
        N_SetGameParametersDelegate setGameParameters,
        N_GetOptions getOptions,
        N_GetState getState)
    {
        _pOwner = pOwner;
        _setGameParameters = setGameParameters;
        _getOptions = getOptions;
        _getState = getState;
    }

    public Task SetGameParametersAsync(string executable, IReadOnlyList<string> gameParameters)
    {
#if DEBUG
        using var logger = Logger.LogMethod(executable.ToFormattable());
#else
        using var logger = Logger.LogMethod();
#endif

        try
        {
            var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            SetGameParametersNative(executable, gameParameters, tcs);
            return tcs.Task;
        }
        catch (Exception e)
        {
            logger.LogException(e);
            throw;
        }
    }

    public Task<LauncherOptions> GetOptionsAsync()
    {
#if DEBUG
        using var logger = Logger.LogMethod();
#else
        using var logger = Logger.LogMethod();
#endif

        try
        {
            var tcs = new TaskCompletionSource<LauncherOptions>(TaskCreationOptions.RunContinuationsAsynchronously);
            GetOptionsNative(tcs);
            return tcs.Task;
        }
        catch (Exception e)
        {
            logger.LogException(e);
            throw;
        }
    }

    public Task<LauncherState> GetStateAsync()
    {
#if DEBUG
        using var logger = Logger.LogMethod();
#else
        using var logger = Logger.LogMethod();
#endif

        try
        {
            var tcs = new TaskCompletionSource<LauncherState>(TaskCreationOptions.RunContinuationsAsynchronously);
            GetStateNative(tcs);
            return tcs.Task;
        }
        catch (Exception e)
        {
            logger.LogException(e);
            throw;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void SetGameParametersNativeCallback(param_ptr* pOwner, return_value_void* pResult)
    {
#if DEBUG
        using var logger = Logger.LogCallbackMethod(pResult);
#else
        using var logger = Logger.LogCallbackMethod(pResult);
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
    private unsafe void SetGameParametersNative(ReadOnlySpan<char> executable, IReadOnlyList<string> gameParameters, TaskCompletionSource tcs)
    {
#if DEBUG
        using var logger = Logger.LogMethod();
#else
        using var logger = Logger.LogMethod();
#endif

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);

        fixed (char* pExecutable = executable)
        fixed (char* pGameParameters = BUTR.NativeAOT.Shared.Utils.SerializeJson(gameParameters, Bindings.CustomSourceGenerationContext.IReadOnlyListString))
        {
            try
            {
                using var result = SafeStructMallocHandle.Create(_setGameParameters(_pOwner, (param_string*) pExecutable, (param_json*) pGameParameters, (param_ptr*) GCHandle.ToIntPtr(handle), &SetGameParametersNativeCallback), true);
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

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void GetOptionsNativeCallback(param_ptr* pOwner, return_value_json* pResult)
    {
#if DEBUG
        using var logger = Logger.LogCallbackMethod(pResult);
#else
        using var logger = Logger.LogCallbackMethod(pResult);
#endif

        try
        {
            if (pOwner == null)
            {
                logger.LogException(new ArgumentNullException(nameof(pOwner)));
                return;
            }

            if (GCHandle.FromIntPtr((IntPtr) pOwner) is not { Target: TaskCompletionSource<LauncherOptions?> tcs } handle)
            {
                logger.LogException(new InvalidOperationException("Invalid GCHandle."));
                return;
            }

            using var result = SafeStructMallocHandle.Create(pResult, true);
            logger.LogResult(result);
            result.SetAsJson(tcs, Bindings.CustomSourceGenerationContext.LauncherOptions);
            handle.Free();
        }
        catch (Exception e)
        {
            logger.LogException(e);
            throw;
        }
    }
    private unsafe void GetOptionsNative(TaskCompletionSource<LauncherOptions> tcs)
    {
#if DEBUG
        using var logger = Logger.LogMethod();
#else
        using var logger = Logger.LogMethod();
#endif

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);

        try
        {
            using var result = SafeStructMallocHandle.Create(_getOptions(_pOwner, (param_ptr*) GCHandle.ToIntPtr(handle), &GetOptionsNativeCallback), true);
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

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void GetStateNativeCallback(param_ptr* pOwner, return_value_json* pResult)
    {
#if DEBUG
        using var logger = Logger.LogCallbackMethod(pResult);
#else
        using var logger = Logger.LogCallbackMethod(pResult);
#endif

        try
        {
            if (pOwner == null)
            {
                logger.LogException(new ArgumentNullException(nameof(pOwner)));
                return;
            }

            if (GCHandle.FromIntPtr((IntPtr) pOwner) is not { Target: TaskCompletionSource<LauncherState?> tcs } handle)
            {
                logger.LogException(new InvalidOperationException("Invalid GCHandle."));
                return;
            }

            using var result = SafeStructMallocHandle.Create(pResult, true);
            logger.LogResult(result);
            result.SetAsJson(tcs, Bindings.CustomSourceGenerationContext.LauncherState);
            handle.Free();
        }
        catch (Exception e)
        {
            logger.LogException(e);
            throw;
        }
    }
    private unsafe void GetStateNative(TaskCompletionSource<LauncherState> tcs)
    {
#if DEBUG
        using var logger = Logger.LogMethod();
#else
        using var logger = Logger.LogMethod();
#endif

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);

        try
        {
            using var result = SafeStructMallocHandle.Create(_getState(_pOwner, (param_ptr*) GCHandle.ToIntPtr(handle), &GetStateNativeCallback), true);
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