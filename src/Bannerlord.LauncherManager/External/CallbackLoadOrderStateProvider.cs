using Bannerlord.LauncherManager.Models;

using System;
using System.Collections.Generic;

namespace Bannerlord.LauncherManager.External;

public sealed class CallbackLoadOrderStateProvider : ILoadOrderStateProvider
{
    private readonly Func<IModuleViewModel[]?> _getAllModuleViewModels;
    private readonly Func<IModuleViewModel[]?> _getModuleViewModels;
    private readonly Action<IReadOnlyList<IModuleViewModel>> _setModuleViewModels;

    public CallbackLoadOrderStateProvider(Func<IModuleViewModel[]?> getAllModuleViewModels, Func<IModuleViewModel[]?> getModuleViewModels, Action<IReadOnlyList<IModuleViewModel>> setModuleViewModels)
    {
        _getAllModuleViewModels = getAllModuleViewModels;
        _getModuleViewModels = getModuleViewModels;
        _setModuleViewModels = setModuleViewModels;
    }

    public IModuleViewModel[]? GetAllModuleViewModels() => _getAllModuleViewModels();
    public IModuleViewModel[]? GetModuleViewModels() => _getModuleViewModels();
    public void SetModuleViewModels(IReadOnlyList<IModuleViewModel> moduleViewModels) => _setModuleViewModels(moduleViewModels);
}