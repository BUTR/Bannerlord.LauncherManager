using Bannerlord.LauncherManager.Models;

using System.Linq;

namespace Bannerlord.LauncherManager;

public partial class LauncherManagerHandler
{
    private string _currentExecutable = Constants.BannerlordExecutable;
    private string? _currentGameMode = "/singleplayer"; // We only support singleplayer
    private string? _currentLoadOrder;
    private string? _currentSaveFile;

    /// <summary>
    /// External<br/>
    /// </summary>
    public virtual string GetGameVersion() => string.Empty;

    /// <summary>
    /// External<br/>
    /// </summary>
    public virtual int GetChangeset() => -1;

    /// <summary>
    /// External<br/>
    /// Sets the launch arguments for the Bannerlord executable
    /// </summary>
    public void RefreshGameParameters()
    {
        var parameters = new[]
        {
            _currentGameMode ?? string.Empty,
            _currentLoadOrder ?? string.Empty,
            string.IsNullOrEmpty(_currentSaveFile) ? string.Empty : $"/continuesave {_currentSaveFile}",
        };

        RefreshGameParameters(_currentExecutable, parameters);
        //RefreshGameParameters(modules.Any(x => x is { Key: "Bannerlord.BLSE", Value.Enabled: true })
        //    ? Constants.BLSEExecutable.AsSpan()
        //    : Constants.BannerlordExecutable.AsSpan(), parameters);
    }

    /// <summary>
    /// Internal<br/>
    /// </summary>
    internal void SetGameParameterLoadOrder(LoadOrder loadOrder)
    {
        var activeLoadOrder = loadOrder.Where(x => x.Value.IsSelected).Select(x => x.Key).ToArray();
        _currentLoadOrder = activeLoadOrder.Length > 0 ? $"_MODULES_*{string.Join("*", activeLoadOrder)}*_MODULES_" : string.Empty;
        RefreshGameParameters();
    }

    /// <summary>
    /// External<br/>
    /// </summary>
    public void SetGameParameterExecutable(string executable)
    {
        _currentExecutable = executable;
        RefreshGameParameters();
    }

    /// <summary>
    /// External<br/>
    /// </summary>
    public void SetGameParameterSaveFile(string? saveName)
    {
        _currentSaveFile = saveName;
        RefreshGameParameters();
    }
}