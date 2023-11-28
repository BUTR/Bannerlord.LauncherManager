using Bannerlord.LauncherManager.Models;

using System;

namespace Bannerlord.LauncherManager.External;

public sealed class CallbackLoadOrderProvider : ILoadOrderProvider
{
    private readonly Func<LoadOrder> _loadLoadOrder;
    private readonly Action<LoadOrder> _saveLoadOrder;

    public CallbackLoadOrderProvider(Func<LoadOrder> loadLoadOrder, Action<LoadOrder> saveLoadOrder)
    {
        _loadLoadOrder = loadLoadOrder;
        _saveLoadOrder = saveLoadOrder;
    }

    public LoadOrder LoadLoadOrder() => _loadLoadOrder();
    public void SaveLoadOrder(LoadOrder loadOrder) => _saveLoadOrder(loadOrder);
}