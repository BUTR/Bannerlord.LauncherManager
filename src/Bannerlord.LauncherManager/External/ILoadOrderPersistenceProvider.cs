using Bannerlord.LauncherManager.Models;

namespace Bannerlord.LauncherManager.External;

public interface ILoadOrderPersistenceProvider
{
    /// <summary>
    /// Loads the LoadOrder from a persistent storage
    /// </summary>
    /// <returns></returns>
    LoadOrder LoadLoadOrder();
    
    /// <summary>
    /// Saves the LoadOrder to a persistent storage
    /// </summary>
    /// <param name="loadOrder"></param>
    void SaveLoadOrder(LoadOrder loadOrder);
}