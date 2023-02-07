using FetchBannerlordVersion;

using System;

namespace Bannerlord.LauncherManager
{
    public partial class LauncherManagerHandler
    {
        private string? _currentGameMode;
        private string? _currentLoadOrder;
        private string? _currentSaveFile;
        
        public string GetGameVersion()
        {
            var gamePath = GetInstallPath();
            return Fetcher.GetVersion(gamePath.ToString(), "TaleWorlds.Library.dll");
        }

        /// <summary>
        /// Sets the launch arguments for the Bannerlord executable
        /// </summary>
        public void RefreshGameParameters()
        {
            var parameters = new[]
            {
                _currentGameMode ?? string.Empty,
                _currentLoadOrder ?? string.Empty,
                _currentSaveFile ?? string.Empty,
            };

            RefreshGameParameters(Constants.BannerlordExecutable.AsSpan(), parameters);
            //RefreshGameParameters(modules.Any(x => x is { Key: "Bannerlord.BLSE", Value.Enabled: true })
            //    ? Constants.BLSEExecutable.AsSpan()
            //    : Constants.BannerlordExecutable.AsSpan(), parameters);
        }
        
        public void SetCurrentSaveFile(string saveName)
        {
            _currentSaveFile = saveName;
            RefreshGameParameters();
        }
    }
}