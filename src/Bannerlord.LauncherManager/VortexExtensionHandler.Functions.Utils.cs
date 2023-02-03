using Bannerlord.LauncherManager.Models;

using FetchBannerlordVersion;

using System.Linq;

namespace Bannerlord.LauncherManager
{
    public partial class VortexExtensionHandler
    {
        public string GetGameVersion()
        {
            var gamePath = GetInstallPath();
            return Fetcher.GetVersion(gamePath.ToString(), "TaleWorlds.Library.dll");
        }

        /// <summary>
        /// Sets the launch arguments for the Bannerlord executable
        /// </summary>
        public void RefreshGameParams(LoadOrder modules)
        {
            // We only support singleplayer
            const string gameMode = "singleplayer";

            var parameters = new[]
            {
                $"/{gameMode}",
                $"_MODULES_*{string.Join("*", modules.Where(x => x.Value.Enabled).OrderBy(x => x.Value.Pos).Select(x => x.Key))}*_MODULES_"
            };

            SetGameParameters(modules.Any(x => x is { Key: "Bannerlord.BLSE", Value.Enabled: true })
                ? Constants.BLSEExecutable
                : Constants.BannerlordExecutable, parameters);
        }
    }
}