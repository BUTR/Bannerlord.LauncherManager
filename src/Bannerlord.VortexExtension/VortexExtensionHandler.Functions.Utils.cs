using Bannerlord.VortexExtension.Models;

using System.Linq;

using FetchBannerlordVersion;

namespace Bannerlord.VortexExtension
{
    public partial class VortexExtensionHandler
    {
        public string GetGameVersion()
        {
            var gamePath = GetInstallPath();
            return Fetcher.GetVersion(gamePath.ToString(), "TaleWorlds.Library.dll");
        }

        /// <summary>
        /// Sets the launch arguements for the Bannerlord executable
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

            SetGameParameters(Constants.BannerlordExecutable, parameters);
        }
    }
}