using Bannerlord.LauncherManager.Models;

using System.Collections.Generic;

namespace Bannerlord.LauncherManager.External;

public interface ILoadOrderStateProvider
{
    /// <summary>
    /// Return all available ViewModels
    /// </summary>
    /// <returns></returns>
    IModuleViewModel[]? GetAllModuleViewModels();

    /// <summary>
    /// Returns the displayed ViewModels
    /// </summary>
    /// <returns></returns>
    IModuleViewModel[]? GetModuleViewModels();

    /// <summary>
    /// Sets the displayed ViewModels
    /// </summary>
    /// <param name="moduleViewModels"></param>
    void SetModuleViewModels(IReadOnlyList<IModuleViewModel> moduleViewModels);
}