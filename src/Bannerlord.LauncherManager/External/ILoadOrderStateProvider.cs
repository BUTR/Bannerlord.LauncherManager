using Bannerlord.LauncherManager.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.External;

public interface ILoadOrderStateProvider
{
    /// <summary>
    /// Return all available ViewModels
    /// </summary>
    /// <returns></returns>
    Task<IModuleViewModel[]?> GetAllModuleViewModelsAsync();

    /// <summary>
    /// Returns the displayed ViewModels
    /// </summary>
    /// <returns></returns>
    Task<IModuleViewModel[]?> GetModuleViewModelsAsync();

    /// <summary>
    /// Sets the displayed ViewModels
    /// </summary>
    /// <param name="moduleViewModels"></param>
    Task SetModuleViewModelsAsync(IReadOnlyList<IModuleViewModel> moduleViewModels);
}