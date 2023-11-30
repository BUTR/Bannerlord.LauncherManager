using Bannerlord.LauncherManager.External;
using Bannerlord.LauncherManager.Models;

using BUTR.NativeAOT.Shared;

using System;
using System.Collections.Generic;

namespace Bannerlord.LauncherManager.Native.Adapters;

internal sealed unsafe class LauncherStateProvider : ILauncherStateProvider
{
    private readonly param_ptr* _pOwner;
    private readonly N_SetGameParametersDelegate _setGameParameters;
    private readonly N_GetOptions _getOptions;
    private readonly N_GetState _getState;

    public LauncherStateProvider(
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
        
    public void SetGameParameters(string executable, IReadOnlyList<string> gameParameters) => SetGameParametersNative(executable, gameParameters);
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