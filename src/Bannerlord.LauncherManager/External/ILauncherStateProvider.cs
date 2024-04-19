using Bannerlord.LauncherManager.Models;

using System.Collections.Generic;

namespace Bannerlord.LauncherManager.External;

public interface ILauncherStateProvider
{
    void SetGameParameters(string executable, IReadOnlyList<string> gameParameters);

    LauncherOptions GetOptions();
    LauncherState GetState();
}