using Bannerlord.LauncherManager.Models;

using System;
using System.Collections.Generic;

namespace Bannerlord.LauncherManager.External;

public sealed class CallbackLauncherStateProvider : ILauncherStateProvider
{
    private readonly Action<string, IReadOnlyList<string>> _setGameParameters;

    private readonly Func<LauncherOptions> _getOptions;
    private readonly Func<LauncherState> _getState;

    public CallbackLauncherStateProvider(Action<string, IReadOnlyList<string>> setGameParameters, Func<LauncherOptions> getOptions, Func<LauncherState> getState)
    {
        _setGameParameters = setGameParameters;
        _getOptions = getOptions;
        _getState = getState;
    }

    public void SetGameParameters(string executable, IReadOnlyList<string> gameParameters) => _setGameParameters(executable, gameParameters);
    public LauncherOptions GetOptions() => _getOptions();
    public LauncherState GetState() => _getState();
}