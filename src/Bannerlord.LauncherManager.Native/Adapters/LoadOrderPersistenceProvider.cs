using Bannerlord.LauncherManager.External;
using Bannerlord.LauncherManager.Models;

using BUTR.NativeAOT.Shared;

namespace Bannerlord.LauncherManager.Native.Adapters;

internal sealed unsafe class LoadOrderPersistenceProvider : ILoadOrderPersistenceProvider
{
    private readonly param_ptr* _pOwner;
    private readonly N_GetLoadOrderDelegate _getLoadOrder;
    private readonly N_SetLoadOrderDelegate _setLoadOrder;

    public LoadOrderPersistenceProvider(param_ptr* pOwner, N_GetLoadOrderDelegate getLoadOrder, N_SetLoadOrderDelegate setLoadOrder)
    {
        _pOwner = pOwner;
        _getLoadOrder = getLoadOrder;
        _setLoadOrder = setLoadOrder;
    }

    public LoadOrder LoadLoadOrder() => LoadLoadOrderNative();
    public void SaveLoadOrder(LoadOrder loadOrder) => SaveLoadOrderNative(loadOrder);

    private LoadOrder LoadLoadOrderNative()
    {
        Logger.LogInput();

        using var result = SafeStructMallocHandle.Create(_getLoadOrder(_pOwner), true);

        var returnResult = result.ValueAsJson(Bindings.CustomSourceGenerationContext.LoadOrder)!;
        Logger.LogOutput(returnResult);
        return returnResult;
    }

    private void SaveLoadOrderNative(LoadOrder loadOrder)
    {
        Logger.LogInput();

        fixed (char* pLoadOrder = BUTR.NativeAOT.Shared.Utils.SerializeJson(loadOrder, Bindings.CustomSourceGenerationContext.LoadOrder))
        {
            Logger.LogPinned(pLoadOrder);

            using var result = SafeStructMallocHandle.Create(_setLoadOrder(_pOwner, (param_json*) pLoadOrder), true);
            result.ValueAsVoid();
        }

        Logger.LogOutput();
    }
}