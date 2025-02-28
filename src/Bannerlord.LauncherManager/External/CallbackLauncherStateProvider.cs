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
        try
        {
            _setGameParameters(executable, gameParameters, () => tcs.TrySetResult(null));
        }
        catch (Exception ex)
        {
            tcs.TrySetException(ex);
        }
        return tcs.Task;
    }

    public Task<LauncherOptions> GetOptionsAsync()
    {
        var tcs = new TaskCompletionSource<LauncherOptions>();
        try
        {
            _getOptions(result => tcs.TrySetResult(result));
        }
        catch (Exception ex)
        {
            tcs.TrySetException(ex);
        }
        return tcs.Task;
    }

    public Task<LauncherState> GetStateAsync()
    {
        var tcs = new TaskCompletionSource<LauncherState>();
        try
        {
            _getState(result => tcs.TrySetResult(result));
        }
        catch (Exception ex)
        {
            tcs.TrySetException(ex);
        }
        return tcs.Task;
    }
}