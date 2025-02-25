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
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
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
    public static unsafe void SetGameParametersNativeCallback(param_ptr* pOwner, return_value_void* pResult)
    {
        Logger.LogCallbackInput(pResult);

        if (pOwner == null)
        {
            Logger.LogException(new ArgumentNullException(nameof(pOwner)));
            return;
        }
        
        if (GCHandle.FromIntPtr((IntPtr) pOwner) is not { Target: TaskCompletionSource tcs } handle)
        {
            Logger.LogException(new InvalidOperationException("Invalid GCHandle."));
            return;
        }
        
        using var result = SafeStructMallocHandle.Create(pResult, true);
        result.SetAsVoid(tcs);
        handle.Free();

        Logger.LogOutput();
    }
    private unsafe void SetGameParametersNative(ReadOnlySpan<char> executable, IReadOnlyList<string> gameParameters, TaskCompletionSource tcs)
    {
        Logger.LogInput();

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);

        fixed (char* pExecutable = executable)
        fixed (char* pGameParameters = BUTR.NativeAOT.Shared.Utils.SerializeJson(gameParameters, Bindings.CustomSourceGenerationContext.IReadOnlyListString))
        {
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
    public static unsafe void GetOptionsNativeCallback(param_ptr* pOwner, return_value_json* pResult)
    {
        Logger.LogCallbackInput(pResult);

        if (pOwner == null)
        {
            Logger.LogException(new ArgumentNullException(nameof(pOwner)));
            return;
        }
        
        if (GCHandle.FromIntPtr((IntPtr) pOwner) is not { Target: TaskCompletionSource<LauncherOptions?> tcs } handle)
        {
            Logger.LogException(new InvalidOperationException("Invalid GCHandle."));
            return;
        }
        
        using var result = SafeStructMallocHandle.Create(pResult, true);
        result.SetAsJson(tcs, Bindings.CustomSourceGenerationContext.LauncherOptions);
        handle.Free();

        Logger.LogOutput();
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
    public static unsafe void GetStateNativeCallback(param_ptr* pOwner, return_value_json* pResult)
    {
        Logger.LogCallbackInput(pResult);

        if (pOwner == null)
        {
            Logger.LogException(new ArgumentNullException(nameof(pOwner)));
            return;
        }
        
        if (GCHandle.FromIntPtr((IntPtr) pOwner) is not { Target: TaskCompletionSource<LauncherState?> tcs } handle)
        {
            Logger.LogException(new InvalidOperationException("Invalid GCHandle."));
            return;
        }
        
        using var result = SafeStructMallocHandle.Create(pResult, true);
        result.SetAsJson(tcs, Bindings.CustomSourceGenerationContext.LauncherState);
        handle.Free();

        Logger.LogOutput();
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