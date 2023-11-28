using Bannerlord.LauncherManager.Models;

namespace Bannerlord.LauncherManager.External;

public interface ILoadOrderProvider
{
    LoadOrder LoadLoadOrder();
    void SaveLoadOrder(LoadOrder loadOrder);
}