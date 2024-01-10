using Bannerlord.LauncherManager.External;
using Bannerlord.LauncherManager.External.UI;
using Bannerlord.LauncherManager.Models;
using Bannerlord.LauncherManager.Native.Models;

using BUTR.NativeAOT.Shared;

using System.Collections.Generic;
using System.Linq;

namespace Bannerlord.LauncherManager.Native.Adapters;

internal sealed unsafe class LoadOrderStateProvider : ILoadOrderStateProvider
{
    private readonly param_ptr* _pOwner;
    private readonly N_GetAllModuleViewModels _getAllModuleViewModels;
    private readonly N_GetModuleViewModels _getModuleViewModels;
    private readonly N_SetModuleViewModels _setModuleViewModels;

    public LoadOrderStateProvider(
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
        
    public IModuleViewModel[]? GetAllModuleViewModels() => GetAllModuleViewModelsNative();
    public IModuleViewModel[]? GetModuleViewModels() => GetModuleViewModelsNative();
    public void SetModuleViewModels(IReadOnlyList<IModuleViewModel> moduleViewModels) => SetModuleViewModelsNative(moduleViewModels);

        
    private IModuleViewModel[]? GetAllModuleViewModelsNative()
    {
        Logger.LogInput();

        using var result = SafeStructMallocHandle.Create(_getAllModuleViewModels(_pOwner), true);
        if (result.IsNull) return null;

        var returnResult = result.ValueAsJson(Bindings.CustomSourceGenerationContext.IReadOnlyListModuleViewModel)?.OrderBy(x => x.Index).Cast<IModuleViewModel>().ToArray();
        Logger.LogOutput(returnResult);
        return returnResult;
    }

    private IModuleViewModel[]? GetModuleViewModelsNative()
    {
        Logger.LogInput();

        using var result = SafeStructMallocHandle.Create(_getModuleViewModels(_pOwner), true);
        if (result.IsNull) return null;

        var returnResult = result.ValueAsJson(Bindings.CustomSourceGenerationContext.IReadOnlyListModuleViewModel)?.OrderBy(x => x.Index).Cast<IModuleViewModel>().ToArray();
        Logger.LogOutput(returnResult);
        return returnResult;
    }

    private void SetModuleViewModelsNative(IReadOnlyList<IModuleViewModel> moduleViewModels)
    {
        Logger.LogInput();

        var moduleViewModelsCasted = moduleViewModels.OfType<ModuleViewModel>().ToList();
        fixed (char* pModuleViewModels = BUTR.NativeAOT.Shared.Utils.SerializeJson(moduleViewModelsCasted, Bindings.CustomSourceGenerationContext.IReadOnlyListModuleViewModel))
        {
            Logger.LogPinned(pModuleViewModels);

            using var result = SafeStructMallocHandle.Create(_setModuleViewModels(_pOwner, (param_json*) pModuleViewModels), true);

        }
        Logger.LogOutput();
    }
}