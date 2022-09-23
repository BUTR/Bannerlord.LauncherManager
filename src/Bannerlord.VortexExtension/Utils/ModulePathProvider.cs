using System.Collections.Generic;
using System.IO;

namespace Bannerlord.VortexExtension
{
    public interface IModulePathProvider
    {
        IEnumerable<string> GetModulePaths();
    }

    public class MainModuleProvider: IModulePathProvider
    {
        private readonly VortexExtensionHandler _handler;

        public MainModuleProvider(VortexExtensionHandler handler)
        {
            _handler = handler;
        }

        public IEnumerable<string> GetModulePaths()
        {
            var installPath = _handler.GetInstallPath().ToString();
            foreach (var moduleFolder in _handler.ReadDirectoryFileList(Path.Combine(installPath, Constants.ModulesFolder)))
            {
                yield return moduleFolder;
            }
        }
    }

    public class SteamModuleProvider: IModulePathProvider
    {
        private readonly VortexExtensionHandler _handler;

        public SteamModuleProvider(VortexExtensionHandler handler)
        {
            _handler = handler;
        }

        public IEnumerable<string> GetModulePaths()
        {
            var installPath = _handler.GetInstallPath().ToString();
            if (!installPath.ToLower().Contains("steamapps"))
                yield break;

            var steamApps = installPath.Substring(0, installPath.IndexOf("common"));
            var workshopDir = Path.Combine(steamApps, "workshop", "content", "261550");

            foreach (var moduleFolder in _handler.ReadDirectoryFileList(workshopDir))
            {
                yield return moduleFolder;
            }
        }
    }
}