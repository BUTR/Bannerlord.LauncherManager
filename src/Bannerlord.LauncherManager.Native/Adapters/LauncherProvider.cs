using Bannerlord.LauncherManager.External;
using Bannerlord.LauncherManager.Models;
using Bannerlord.LauncherManager.Native.Models;

using BUTR.NativeAOT.Shared;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Bannerlord.LauncherManager.Native.Adapters;

internal sealed unsafe class LauncherProvider : ILauncherProvider
{
    private readonly param_ptr* _pOwner;
    private readonly N_SetGameParametersDelegate _setGameParameters;
    private readonly N_GetAllModuleViewModels _getAllModuleViewModels;
    private readonly N_GetModuleViewModels _getModuleViewModels;
    private readonly N_SetModuleViewModels _setModuleViewModels;
    private readonly N_GetOptions _getOptions;
    private readonly N_GetState _getState;

    public LauncherProvider(
        param_ptr* pOwner,
        N_SetGameParametersDelegate setGameParameters,
        N_GetAllModuleViewModels getAllModuleViewModels,
        N_GetModuleViewModels getModuleViewModels,
        N_SetModuleViewModels setModuleViewModels,
        N_GetOptions getOptions,
        N_GetState getState)
    {
        _pOwner = pOwner;
        _setGameParameters = setGameParameters;
        _getAllModuleViewModels = getAllModuleViewModels;
        _getModuleViewModels = getModuleViewModels;
        _setModuleViewModels = setModuleViewModels;
        _getOptions = getOptions;
        _getState = getState;
    }
        
    public void SetGameParameters(string executable, IReadOnlyList<string> gameParameters) => SetGameParametersNative(executable, gameParameters);
    public IModuleViewModel[]? GetAllModuleViewModels() => GetAllModuleViewModelsNative();
    public IModuleViewModel[]? GetModuleViewModels() => GetModuleViewModelsNative();
    public void SetModuleViewModels(IReadOnlyList<IModuleViewModel> moduleViewModels) => SetModuleViewModelsNative(moduleViewModels);
    public LauncherOptions GetOptions() => GetOptionsNative();
    public LauncherState GetState() => GetStateNative();

    private void SetGameParametersNative(ReadOnlySpan<char> executable, IReadOnlyList<string> gameParameters)
    {
        Logger.LogInput();

        fixed (char* pExecutable = executable)
        fixed (char* pGameParameters = BUTR.NativeAOT.Shared.Utils.SerializeJson(gameParameters, Bindings.CustomSourceGenerationContext.IReadOnlyListString))
        {
            Logger.LogPinned(pExecutable, pGameParameters);

            using var result = SafeStructMallocHandle.Create(_setGameParameters(_pOwner, (param_string*) pExecutable, (param_json*) pGameParameters), true);
            result.ValueAsVoid();
        }

        Logger.LogOutput();
    }
        
    private IModuleViewModel[]? GetAllModuleViewModelsNative()
    {
        Logger.LogInput();

        using var result = SafeStructMallocHandle.Create(_getAllModuleViewModels(_pOwner), true);
        if (result.IsNull) return null;

        var returnResult = result.ValueAsJson(Bindings.CustomSourceGenerationContext.IReadOnlyListModuleViewModel)?.OrderBy(x => x.Index).ToArray<IModuleViewModel>();
        Logger.LogOutput(returnResult);
        return returnResult;
    }

    private IModuleViewModel[]? GetModuleViewModelsNative()
    {
        Logger.LogInput();

        using var result = SafeStructMallocHandle.Create(_getModuleViewModels(_pOwner), true);
        if (result.IsNull) return null;

        var returnResult = result.ValueAsJson(Bindings.CustomSourceGenerationContext.IReadOnlyListModuleViewModel)?.OrderBy(x => x.Index).ToArray<IModuleViewModel>();
        Logger.LogOutput(returnResult);
        return returnResult;
    }

    private void SetModuleViewModelsNative(IReadOnlyList<IModuleViewModel> moduleViewModels)
    {
        Logger.LogInput();

        fixed (char* pModuleViewModels = BUTR.NativeAOT.Shared.Utils.SerializeJson((IReadOnlyList<ModuleViewModel>) moduleViewModels, Bindings.CustomSourceGenerationContext.IReadOnlyListModuleViewModel))
        {
            Logger.LogPinned(pModuleViewModels);

            using var result = SafeStructMallocHandle.Create(_setModuleViewModels(_pOwner, (param_json*) pModuleViewModels), true);

        }
        Logger.LogOutput();
    }

    private LauncherOptions GetOptionsNative()
    {
        Logger.LogInput();

        using var result = SafeStructMallocHandle.Create(_getOptions(_pOwner), true);
        if (result.IsNull) return LauncherOptions.Empty;

        var returnResult = result.ValueAsJson(Bindings.CustomSourceGenerationContext.LauncherOptions)!;
        Logger.LogOutput(returnResult);
        return returnResult;
    }

    private LauncherState GetStateNative()
    {
        Logger.LogInput();

        using var result = SafeStructMallocHandle.Create(_getState(_pOwner), true);
        if (result.IsNull) return LauncherState.Empty;

        var returnResult = result.ValueAsJson(Bindings.CustomSourceGenerationContext.LauncherState)!;
        Logger.LogOutput(returnResult);
        return returnResult;
    }
}