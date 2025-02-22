using Bannerlord.LauncherManager.External;
using Bannerlord.LauncherManager.Models;

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

    public async Task SetGameParametersAsync(string executable, IReadOnlyList<string> gameParameters)
    {
        var tcs = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
        SetGameParametersNative(executable, gameParameters, tcs);
        await tcs.Task;
    }

    public async Task<LauncherOptions> GetOptionsAsync()
    {
        var tcs = new TaskCompletionSource<LauncherOptions>(TaskCreationOptions.RunContinuationsAsynchronously);
        GetOptionsNative(tcs);
        return await tcs.Task;
    }

    public async Task<LauncherState> GetStateAsync()
    {
        var tcs = new TaskCompletionSource<LauncherState>(TaskCreationOptions.RunContinuationsAsynchronously);
        GetStateNative(tcs);
        return await tcs.Task;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void SetGameParametersNativeCallback(param_ptr* pOwner)
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
    private unsafe void SetGameParametersNative(ReadOnlySpan<char> executable, IReadOnlyList<string> gameParameters, TaskCompletionSource<object?> tcs)
    {
        Logger.LogInput();

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);

        fixed (char* pExecutable = executable)
        fixed (char* pGameParameters = BUTR.NativeAOT.Shared.Utils.SerializeJson(gameParameters, Bindings.CustomSourceGenerationContext.IReadOnlyListString) ?? string.Empty)
        {
            Logger.LogPinned(pExecutable, pGameParameters);

            try
            {
                using var result = SafeStructMallocHandle.Create(_setGameParameters(_pOwner, (param_string*) pExecutable, (param_json*) pGameParameters, (param_ptr*) GCHandle.ToIntPtr(handle), &SetGameParametersNativeCallback), true);
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

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void GetOptionsNativeCallback(param_ptr* pOwner, param_json* pResult)
    {
        Logger.LogInput(pOwner, pResult);

        if (pOwner == null)
            return;
        
        var handle = GCHandle.FromIntPtr((IntPtr) pOwner);
        var tcs = default(TaskCompletionSource<LauncherOptions>);
        try
        {
            var result = BUTR.NativeAOT.Shared.Utils.DeserializeJson(pResult, Bindings.CustomSourceGenerationContext.LauncherOptions);

            tcs = (TaskCompletionSource<LauncherOptions>) handle.Target!;
            tcs.TrySetResult(result);
            
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
    private unsafe void GetOptionsNative(TaskCompletionSource<LauncherOptions> tcs)
    {
        Logger.LogInput();

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);
        
        try
        {
            using var result = SafeStructMallocHandle.Create(_getOptions(_pOwner, (param_ptr*) GCHandle.ToIntPtr(handle), &GetOptionsNativeCallback), true);
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

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void GetStateNativeCallback(param_ptr* pOwner, param_json* pResult)
    {
        Logger.LogInput(pOwner, pResult);

        if (pOwner == null)
            return;
        
        var handle = GCHandle.FromIntPtr((IntPtr) pOwner);
        var tcs = default(TaskCompletionSource<LauncherState>);
        try
        {
            var result = BUTR.NativeAOT.Shared.Utils.DeserializeJson(pResult, Bindings.CustomSourceGenerationContext.LauncherState);

            tcs = (TaskCompletionSource<LauncherState>) handle.Target!;
            tcs.TrySetResult(result);
            
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
    private unsafe void GetStateNative(TaskCompletionSource<LauncherState> tcs)
    {
        Logger.LogInput();

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);
        
        try
        {
            using var result = SafeStructMallocHandle.Create(_getState(_pOwner, (param_ptr*) GCHandle.ToIntPtr(handle), &GetStateNativeCallback), true);
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