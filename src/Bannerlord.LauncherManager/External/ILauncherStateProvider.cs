using Bannerlord.LauncherManager.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.External;

public interface ILauncherStateProvider
{
    Task SetGameParametersAsync(string executable, IReadOnlyList<string> gameParameters);

    Task<LauncherOptions> GetOptionsAsync();
    Task<LauncherState> GetStateAsync();
}