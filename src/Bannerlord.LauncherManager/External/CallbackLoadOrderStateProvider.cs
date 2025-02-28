using Bannerlord.LauncherManager.Models;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.External;

public sealed class CallbackLoadOrderStateProvider : ILoadOrderStateProvider
{
    private readonly Action<Action<IModuleViewModel[]?>> _getAllModuleViewModels;
    private readonly Action<Action<IModuleViewModel[]?>> _getModuleViewModels;
    private readonly Action<IReadOnlyList<IModuleViewModel>, Action> _setModuleViewModels;

    public CallbackLoadOrderStateProvider(Action<Action<IModuleViewModel[]?>> getAllModuleViewModels, Action<Action<IModuleViewModel[]?>> getModuleViewModels, Action<IReadOnlyList<IModuleViewModel>, Action> setModuleViewModels)
    {
        _getAllModuleViewModels = getAllModuleViewModels;
        _getModuleViewModels = getModuleViewModels;
        _setModuleViewModels = setModuleViewModels;
    }

    public async Task<IModuleViewModel[]?> GetAllModuleViewModelsAsync()
    {
        var tcs = new TaskCompletionSource<IModuleViewModel[]?>();
        try
        {
            _getAllModuleViewModels(result => tcs.TrySetResult(result));
        }
        catch (Exception ex)
        {
            tcs.TrySetException(ex);
        }
        return await tcs.Task;
    }

    public async Task<IModuleViewModel[]?> GetModuleViewModelsAsync()
    {
        var tcs = new TaskCompletionSource<IModuleViewModel[]?>();
        try
        {
            _getModuleViewModels(result => tcs.TrySetResult(result));
        }
        catch (Exception ex)
        {
            tcs.TrySetException(ex);
        }
        return await tcs.Task;
    }

    public async Task SetModuleViewModelsAsync(IReadOnlyList<IModuleViewModel> moduleViewModels)
    {
        var tcs = new TaskCompletionSource<object?>();
        try
        {
            _setModuleViewModels(moduleViewModels, () => tcs.TrySetResult(null));
        }
        catch (Exception ex)
        {
            tcs.TrySetException(ex);
        }
        await tcs.Task;
    }
}