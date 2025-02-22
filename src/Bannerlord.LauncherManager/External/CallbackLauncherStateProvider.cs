using Bannerlord.LauncherManager.Models;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.External;

public sealed class CallbackLauncherStateProvider : ILauncherStateProvider
{
    private readonly Action<string, IReadOnlyList<string>, Action> _setGameParameters;

    private readonly Action<Action<LauncherOptions>> _getOptions;
    private readonly Action<Action<LauncherState>> _getState;

    public CallbackLauncherStateProvider(Action<string, IReadOnlyList<string>, Action> setGameParameters, Action<Action<LauncherOptions>> getOptions, Action<Action<LauncherState>> getState)
    {
        _setGameParameters = setGameParameters;
        _getOptions = getOptions;
        _getState = getState;
    }

    public Task SetGameParametersAsync(string executable, IReadOnlyList<string> gameParameters)
    {
        var tcs = new TaskCompletionSource<object?>();
        _setGameParameters(executable, gameParameters, () => tcs.TrySetResult(null));
        return tcs.Task;
    }

    public Task<LauncherOptions> GetOptionsAsync()
    {
        var tcs = new TaskCompletionSource<LauncherOptions>();
        _getOptions((result) => tcs.TrySetResult(result));
        return tcs.Task;
    }

    public Task<LauncherState> GetStateAsync()
    {
        var tcs = new TaskCompletionSource<LauncherState>();
        _getState((result) => tcs.TrySetResult(result));
        return tcs.Task;
    }
}