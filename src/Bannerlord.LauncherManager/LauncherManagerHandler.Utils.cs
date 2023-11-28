using Bannerlord.LauncherManager.Models;

using System;
using System.Linq;

namespace Bannerlord.LauncherManager;

partial class LauncherManagerHandler
{
    private string _currentExecutable = Constants.BannerlordExecutable;
    private string? _currentGameMode = "/singleplayer"; // We only support singleplayer
    private string? _currentLoadOrder;
    private string? _currentSaveFile;
    private bool _continueLastSaveFile;

    private GameStore? _store;

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
    /// </summary>
    public void SetGameStore(GameStore store) => _store = store;

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
            string.IsNullOrEmpty(_currentSaveFile) ? string.Empty : $"/continuesave \"{_currentSaveFile}\"",
            _continueLastSaveFile ? "/continuegame" : string.Empty
        };

        var installPath = GetInstallPath();
        var platform = GetPlatform(installPath, _store ??= GetStore(installPath));
        var win64Executable = System.IO.Path.Combine(Constants.BinFolder, Constants.Win64Configuration, _currentExecutable);
        var xboxExecutable = System.IO.Path.Combine(Constants.BinFolder, Constants.XboxConfiguration, _currentExecutable);
        var binDirectories = ReadDirectoryFileList(System.IO.Path.Combine(installPath, Constants.BinFolder)) ?? Array.Empty<string>();
        var hasWin64 = binDirectories.Any(x => System.IO.Path.GetFileName(x).Equals(Constants.Win64Configuration, StringComparison.OrdinalIgnoreCase));
        var hasXbox = binDirectories.Any(x => System.IO.Path.GetFileName(x).Equals(Constants.XboxConfiguration, StringComparison.OrdinalIgnoreCase));
        var hasWin64Executable = hasWin64 && ReadDirectoryFileList(System.IO.Path.Combine(installPath, Constants.BinFolder, Constants.Win64Configuration))?.Any(x => System.IO.Path.GetFileName(x).Equals(_currentExecutable)) == true;
        var hasXboxExecutable = hasXbox && ReadDirectoryFileList(System.IO.Path.Combine(installPath, Constants.BinFolder, Constants.XboxConfiguration))?.Any(x => System.IO.Path.GetFileName(x).Equals(_currentExecutable)) == true;
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
    public void SetGameParameterContinueLastSaveFile(bool value)
    {
        _continueLastSaveFile = value;
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

    /// <summary>
    /// External<br/>
    /// </summary>
    public GameStore GetStore(string installPath)
    {
        var nativeFiles = ReadDirectoryFileList(System.IO.Path.Combine(installPath, Constants.ModulesFolder, Constants.NativeModule)) ?? Array.Empty<string>();
        if (nativeFiles.Any(x => x.EndsWith("gdk.target")))
            return GameStore.Xbox;
        if (nativeFiles.Any(x => x.EndsWith("epic.target")))
            return GameStore.Epic;
        if (nativeFiles.Any(x => x.EndsWith("gog.target")))
            return GameStore.GOG;
        if (nativeFiles.Any(x => x.EndsWith("steam.target")))
            return GameStore.Steam;

        return GameStore.Unknown;
    }

    /// <summary>
    /// External<br/>
    /// </summary>
    public GamePlatform GetPlatform()
    {
        var installPath = GetInstallPath();
        return GetPlatform(installPath, _store ?? GetStore(installPath));
    }

    /// <summary>
    /// Internal<br/>
    /// </summary>
    internal GamePlatform GetPlatform(string installPath, GameStore store)
    {
        // TODO:
        GamePlatform GetForUnknownStore()
        {
            var internalStore = GetStore(installPath);
            
            var binDirectories = ReadDirectoryList(System.IO.Path.Combine(installPath, Constants.BinFolder)) ?? Array.Empty<string>();
            var hasWin64 = binDirectories.Any(x => x.Contains(Constants.Win64Configuration));
            var hasXbox = binDirectories.Any(x => x.Contains(Constants.XboxConfiguration));
            
            return (hasXbox, hasWin64) switch
            {
                (true, false) when internalStore == GameStore.Xbox => GamePlatform.Xbox,
                (false, true) when internalStore == GameStore.Steam => GamePlatform.Win64,
                (false, true) when internalStore == GameStore.GOG => GamePlatform.Win64,
                (false, true) when internalStore == GameStore.Epic => GamePlatform.Win64,

                (true, false) => GamePlatform.Xbox,
                (false, true) => GamePlatform.Win64,
                (true, true) => GamePlatform.Unknown,
                (false, false) => GamePlatform.Unknown,
            };
        }

        return FromStore(store) is var platform && platform == GamePlatform.Unknown ? GetForUnknownStore() : platform;
    }

    public static GamePlatform FromStore(GameStore store) => store switch
    {
        GameStore.Steam => GamePlatform.Win64,
        GameStore.GOG => GamePlatform.Win64,
        GameStore.Epic => GamePlatform.Win64,
        GameStore.Xbox => GamePlatform.Xbox,
        _ => GamePlatform.Unknown,
    };

    public static string GetConfigurationByPlatform(GamePlatform platform) => platform switch
    {
        GamePlatform.Win64 => Constants.Win64Configuration,
        GamePlatform.Xbox => Constants.XboxConfiguration,
        _ => string.Empty
    };
}