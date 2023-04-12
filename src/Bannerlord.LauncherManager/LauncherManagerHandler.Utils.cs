using Bannerlord.LauncherManager.Models;

using System;
using System.Linq;

namespace Bannerlord.LauncherManager;

public partial class LauncherManagerHandler
{
    private enum GameStore { Steam, GOG, Epic, Xbox, Unknown }
    private enum GamePlatform { Win64, Xbox, Unknown }

    private (GamePlatform, GameStore) GetPlatformAndStore(string installPath)
    {
        var binDirectories = ReadDirectoryList(System.IO.Path.Combine(installPath, "bin")) ?? Array.Empty<string>();
        var hasWin64 = binDirectories.Any(x => x.Contains(Constants.Win64Configuration));
        var hasXbox = binDirectories.Any(x => x.Contains(Constants.XboxConfiguration));

        var modulesDirectories = ReadDirectoryFileList(System.IO.Path.Combine(installPath, Constants.ModulesFolder)) ?? Array.Empty<string>();
        var hasNativeModule = modulesDirectories.Any(x => x.EndsWith("Native", StringComparison.OrdinalIgnoreCase));

        var nativeFiles = hasNativeModule ? ReadDirectoryFileList(System.IO.Path.Combine(installPath, Constants.ModulesFolder, "Native")) ?? Array.Empty<string>() : Array.Empty<string>();
        var hasGdk = nativeFiles.Any(x => x.EndsWith("gdk.target"));
        var hasEpic = nativeFiles.Any(x => x.EndsWith("epic.target"));
        var hasGog = nativeFiles.Any(x => x.EndsWith("gog.target"));
        var hasSteam = nativeFiles.Any(x => x.EndsWith("steam.target"));

        return (hasXbox, hasWin64) switch
        {
            (true, false) when hasGdk => (GamePlatform.Xbox, GameStore.Xbox),
            (false, true) when hasSteam => (GamePlatform.Win64, GameStore.Steam),
            (false, true) when hasGog => (GamePlatform.Win64, GameStore.GOG),
            (false, true) when hasEpic => (GamePlatform.Win64, GameStore.Epic),
            (true, false) => (GamePlatform.Xbox, GameStore.Unknown),
            (false, true) => (GamePlatform.Win64, GameStore.Unknown),
            (true, true) => (GamePlatform.Unknown, GameStore.Unknown),
            (false, false) => (GamePlatform.Unknown, GameStore.Unknown),
        };
    }

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

        var installPath = GetInstallPath();
        var (platform, _) = GetPlatformAndStore(installPath);
        var win64Executable = System.IO.Path.Combine("bin", Constants.Win64Configuration, _currentExecutable);
        var xboxExecutable = System.IO.Path.Combine("bin", Constants.XboxConfiguration, _currentExecutable);
        var binDirectories = ReadDirectoryFileList(System.IO.Path.Combine(installPath, "bin")) ?? Array.Empty<string>();
        var hasWin64 = binDirectories.Any(x => x.EndsWith(Constants.Win64Configuration, StringComparison.OrdinalIgnoreCase));
        var hasXbox = binDirectories.Any(x => x.EndsWith(Constants.XboxConfiguration, StringComparison.OrdinalIgnoreCase));
        var hasWin64Executable = hasWin64 && ReadDirectoryFileList(System.IO.Path.Combine(installPath, "bin", Constants.Win64Configuration))?.Any(x => x.EndsWith(_currentExecutable)) == true;
        var hasXboxExecutable = hasXbox && ReadDirectoryFileList(System.IO.Path.Combine(installPath, "bin", Constants.XboxConfiguration))?.Any(x => x.EndsWith(_currentExecutable)) == true;
        var fullExecutablePath = platform switch
        {
            GamePlatform.Win64 when hasWin64Executable => win64Executable,
            GamePlatform.Xbox when hasXboxExecutable => xboxExecutable,
            _ => hasWin64Executable ? win64Executable : hasXboxExecutable ? xboxExecutable : string.Empty
        };

        RefreshGameParameters(fullExecutablePath, parameters);
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