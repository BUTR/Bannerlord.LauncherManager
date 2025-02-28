using Bannerlord.LauncherManager.External;
using Bannerlord.LauncherManager.Models;
using Bannerlord.LauncherManager.Native.Models;

using BUTR.NativeAOT.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.Native.Adapters;

internal sealed class LoadOrderStateProvider : ILoadOrderStateProvider
{
    private readonly unsafe param_ptr* _pOwner;
    private readonly N_GetAllModuleViewModels _getAllModuleViewModels;
    private readonly N_GetModuleViewModels _getModuleViewModels;
    private readonly N_SetModuleViewModels _setModuleViewModels;

    public unsafe LoadOrderStateProvider(
        param_ptr* pOwner,
        N_GetAllModuleViewModels getAllModuleViewModels,
        N_GetModuleViewModels getModuleViewModels,
        N_SetModuleViewModels setModuleViewModels)
    {
        _pOwner = pOwner;
        _getAllModuleViewModels = getAllModuleViewModels;
        _getModuleViewModels = getModuleViewModels;
        _setModuleViewModels = setModuleViewModels;
    }

    public async Task<IModuleViewModel[]?> GetAllModuleViewModelsAsync()
    {
        var tcs = new TaskCompletionSource<IModuleViewModel[]?>(TaskCreationOptions.RunContinuationsAsynchronously);
        GetAllModuleViewModelsNative(tcs);
        return await tcs.Task;
    }

    public async Task<IModuleViewModel[]?> GetModuleViewModelsAsync()
    {
        var tcs = new TaskCompletionSource<IModuleViewModel[]?>(TaskCreationOptions.RunContinuationsAsynchronously);
        GetModuleViewModelsNative(tcs);
        return await tcs.Task;
    }

    public async Task SetModuleViewModelsAsync(IReadOnlyList<IModuleViewModel> moduleViewModels)
    {
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        SetModuleViewModelsNative(moduleViewModels, tcs);
        await tcs.Task;
    }


    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void GetAllModuleViewModelsNativeCallback(param_ptr* pOwner, return_value_json* pResult)
    {
        Logger.LogCallbackInput(pResult);

        if (pOwner == null)
        {
            Logger.LogException(new ArgumentNullException(nameof(pOwner)));
            return;
        }

        if (GCHandle.FromIntPtr((IntPtr) pOwner) is not { Target: TaskCompletionSource<IModuleViewModel[]?> tcs } handle)
        {
            Logger.LogException(new InvalidOperationException("Invalid GCHandle."));
            return;
        }

        using var result = SafeStructMallocHandle.Create(pResult, true);
        try
        {
            var moduleViewModels = result.ValueAsJson(Bindings.CustomSourceGenerationContext.IReadOnlyListModuleViewModel)?
                .Where(x => x != null!)
                .Cast<IModuleViewModel>()
                .OrderBy(x => x.Index)
                .ToArray();
            tcs.TrySetResult(moduleViewModels);

            Logger.LogOutput();
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            tcs.TrySetException(e);
        }
        finally
        {
            handle.Free();
        }
    }
    private unsafe void GetAllModuleViewModelsNative(TaskCompletionSource<IModuleViewModel[]?> tcs)
    {
        Logger.LogInput();

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);

        try
        {
            using var result = SafeStructMallocHandle.Create(_getAllModuleViewModels(_pOwner, (param_ptr*) GCHandle.ToIntPtr(handle), &GetAllModuleViewModelsNativeCallback), true);
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
    public static unsafe void GetModuleViewModelsNativeCallback(param_ptr* pOwner, return_value_json* pResult)
    {
        Logger.LogCallbackInput(pResult);

        if (pOwner == null)
        {
            Logger.LogException(new ArgumentNullException(nameof(pOwner)));
            return;
        }

        if (GCHandle.FromIntPtr((IntPtr) pOwner) is not { Target: TaskCompletionSource<IModuleViewModel[]?> tcs } handle)
        {
            Logger.LogException(new InvalidOperationException("Invalid GCHandle."));
            return;
        }

        using var result = SafeStructMallocHandle.Create(pResult, true);
        try
        {
            var moduleViewModels = result.ValueAsJson(Bindings.CustomSourceGenerationContext.IReadOnlyListModuleViewModel)?
                .Where(x => x != null!)
                .Cast<IModuleViewModel>()
                .OrderBy(x => x.Index)
                .ToArray();
            tcs.TrySetResult(moduleViewModels);

            Logger.LogOutput();
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            tcs.TrySetException(e);
        }
        finally
        {
            handle.Free();
        }
    }
    private unsafe void GetModuleViewModelsNative(TaskCompletionSource<IModuleViewModel[]?> tcs)
    {
        Logger.LogInput();

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);

        try
        {
            using var result = SafeStructMallocHandle.Create(_getModuleViewModels(_pOwner, (param_ptr*) GCHandle.ToIntPtr(handle), &GetModuleViewModelsNativeCallback), true);
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
    public static unsafe void SetModuleViewModelsNativeCallback(param_ptr* pOwner, return_value_void* pResult)
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
    private unsafe void SetModuleViewModelsNative(IReadOnlyList<IModuleViewModel> moduleViewModels, TaskCompletionSource tcs)
    {
        Logger.LogInput();

        var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);

        var moduleViewModelsCasted = moduleViewModels.OfType<ModuleViewModel>().ToList();
        fixed (char* pModuleViewModels = BUTR.NativeAOT.Shared.Utils.SerializeJson(moduleViewModelsCasted, Bindings.CustomSourceGenerationContext.IReadOnlyListModuleViewModel))
        {
            try
            {
                using var result = SafeStructMallocHandle.Create(_setModuleViewModels(_pOwner, (param_json*) pModuleViewModels, (param_ptr*) GCHandle.ToIntPtr(handle), &SetModuleViewModelsNativeCallback), true);
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