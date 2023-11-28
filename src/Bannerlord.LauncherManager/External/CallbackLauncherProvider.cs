using Bannerlord.LauncherManager.Models;

using System;
using System.Collections.Generic;

namespace Bannerlord.LauncherManager.External;

public sealed class CallbackLauncherProvider : ILauncherProvider
{
    private readonly Action<string, IReadOnlyList<string>> _setGameParameters;
    private readonly Func<IModuleViewModel[]?> _getAllModuleViewModels;
    private readonly Func<IModuleViewModel[]?> _getModuleViewModels;
    private readonly Action<IReadOnlyList<IModuleViewModel>> _setModuleViewModels;
    private readonly Func<LauncherOptions> _getOptions;
    private readonly Func<LauncherState> _getState;

    public CallbackLauncherProvider(Action<string, IReadOnlyList<string>> setGameParameters, Func<IModuleViewModel[]?> getAllModuleViewModels, Func<IModuleViewModel[]?> getModuleViewModels, Action<IReadOnlyList<IModuleViewModel>> setModuleViewModels, Func<LauncherOptions> getOptions, Func<LauncherState> getState)
    {
        _setGameParameters = setGameParameters;
        _getAllModuleViewModels = getAllModuleViewModels;
        _getModuleViewModels = getModuleViewModels;
        _setModuleViewModels = setModuleViewModels;
        _getOptions = getOptions;
        _getState = getState;
    }

    public void SetGameParameters(string executable, IReadOnlyList<string> gameParameters) => _setGameParameters(executable, gameParameters);
    public IModuleViewModel[]? GetAllModuleViewModels() => _getAllModuleViewModels();
    public IModuleViewModel[]? GetModuleViewModels() => _getModuleViewModels();
    public void SetModuleViewModels(IReadOnlyList<IModuleViewModel> moduleViewModels) => _setModuleViewModels(moduleViewModels);
    public LauncherOptions GetOptions() => _getOptions();
    public LauncherState GetState() => _getState();
}