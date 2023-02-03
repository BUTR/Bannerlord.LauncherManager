using System;
using System.Collections.Generic;
using System.IO;

namespace Bannerlord.LauncherManager
{
    public interface IModulePathProvider
    {
        IEnumerable<string> GetModulePaths();
    }

    public class MainModuleProvider : IModulePathProvider
    {
        private readonly VortexExtensionHandler _handler;

        public MainModuleProvider(VortexExtensionHandler handler)
        {
            _handler = handler;
        }

        public IEnumerable<string> GetModulePaths()
        {
            var installPath = _handler.GetInstallPath().ToString();
            var directories = _handler.ReadDirectoryList(Path.Combine(installPath, Constants.ModulesFolder));
            if (directories is null) throw new DirectoryNotFoundException($"installPath: {installPath}. Modules directory not found!");
            //if (directories is null) yield break;
            foreach (var moduleFolder in directories)
            {
                yield return moduleFolder;
            }
        }
    }

    public class SteamModuleProvider : IModulePathProvider
    {
        private readonly VortexExtensionHandler _handler;

        public SteamModuleProvider(VortexExtensionHandler handler)
        {
            _handler = handler;
        }

        public IEnumerable<string> GetModulePaths()
        {
            var installPath = _handler.GetInstallPath().ToString();
            if (!installPath.ToLower().Contains("steamapps", StringComparison.OrdinalIgnoreCase))
                yield break;

            var steamApps = installPath.Substring(0, installPath.IndexOf("common", StringComparison.Ordinal));
            var workshopDir = Path.Combine(steamApps, "workshop", "content", "261550");

            var directories = _handler.ReadDirectoryFileList(workshopDir);
            if (directories is null) throw new DirectoryNotFoundException($"installPath: {installPath}. Steam's Modules directory not found!");
            //if (directories is null) yield break;
            foreach (var moduleFolder in directories)
            {
                yield return moduleFolder;
            }
        }
    }
}