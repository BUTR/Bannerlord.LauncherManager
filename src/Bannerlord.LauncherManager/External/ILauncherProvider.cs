using Bannerlord.LauncherManager.Models;

using System.Collections.Generic;

namespace Bannerlord.LauncherManager.External;

public interface ILauncherProvider
{
    void SetGameParameters(string executable, IReadOnlyList<string> gameParameters);

    IModuleViewModel[]? GetAllModuleViewModels();
    IModuleViewModel[]? GetModuleViewModels();
    void SetModuleViewModels(IReadOnlyList<IModuleViewModel> moduleViewModels);
    
    LauncherOptions GetOptions();
    LauncherState GetState();
}