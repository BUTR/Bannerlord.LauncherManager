using Bannerlord.LauncherManager.Models;
using Bannerlord.LauncherManager.Utils;
using Bannerlord.ModuleManager;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Bannerlord.LauncherManager;

public partial class LauncherManagerHandler
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsDirectorySeparator(char c) => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar;

    private static bool EndsInDirectorySeparator([NotNullWhen(true)] string? path) =>
        !string.IsNullOrEmpty(path) && IsDirectorySeparator(path![path.Length - 1]);


    /// <summary>
    /// External<br/>
    /// Checks the mods content and verifies if it is a valid bannerlord mod
    /// </summary>
    public SupportedResult TestModuleContent(string[] files)
    {
        var lowerSubModuleName = Constants.SubModuleName.ToLower();
        if (!files.Any(x => x.ToLower().Contains(lowerSubModuleName)))
            return SupportedResult.AsNotSupported;

        return SupportedResult.AsSupported;
    }

    /// <summary>
    /// External<br/>
    /// Two ways to handle installation:<br/>
    /// 1. The root folder is the game's root folder.<br/>
    ///     We can install mods to both /Modules and /bin folders<br/>
    /// 2. The root folder is the game's /Modules folder<br/>
    ///     The /bin mods won't be handled by us and instead they will be
    ///     copied by a fallback mechanism. Vortex or Vanilla will just copy the
    ///     content as is  to the game's root folder<br/>
    /// </summary>
    public InstallResult InstallModuleContent(string[] files, string destinationPath)
    {
        var (platform, _) = GetPlatformAndStore(GetInstallPath());
        
        var lowerSubModuleName = Constants.SubModuleName.ToLower();
        if (!files.Any(x => x.ToLower().Contains(lowerSubModuleName)))
            return InstallResult.AsInvalid;

        files = files.Where(x => !EndsInDirectorySeparator(x)).ToArray();

        var instructions = files.Where(x => x.EndsWith(Constants.SubModuleName, StringComparison.OrdinalIgnoreCase)).Select(file =>
        {
            var path = Path.Combine(destinationPath, file);
            var data = ReadFileContent(path, 0, -1);
            if (data is null) return new List<IInstallInstruction>();
            var module = ModuleInfoExtended.FromXml(ReaderUtils.Read(data));
            if (module is null) return new List<IInstallInstruction>();

            var subModuleFileIdx = file.IndexOf(Constants.SubModuleName, StringComparison.OrdinalIgnoreCase);
            var moduleBasePath = file.Substring(0, subModuleFileIdx);
            return files.Where(y => y.StartsWith(moduleBasePath)).Where(y =>
            {
                var modulePath = y.Substring(moduleBasePath.Length);
                if (!modulePath.StartsWith("bin", StringComparison.OrdinalIgnoreCase)) return true;
                var binPath = modulePath.Substring(4);
                // Avoid unnecessary bin folder copy
                return platform switch
                {
                    GamePlatform.Win64 => binPath.StartsWith(Constants.Win64Configuration, StringComparison.OrdinalIgnoreCase),
                    GamePlatform.Xbox => binPath.StartsWith(Constants.XboxConfiguration, StringComparison.OrdinalIgnoreCase),
                    _ => true,
                };
            }).Select(file2 => new CopyInstallInstruction
            {
                ModuleId = module.Id,
                Source = file2,
                Destination = Path.Combine(Constants.ModulesFolder, module.Id, file2.Substring(subModuleFileIdx))
            }).Concat(new IInstallInstruction[]
            {
                new ModuleInfoInstallInstruction(module),
            }).ToList();
        }).SelectMany(x => x).ToList();

        return new InstallResult { Instructions = instructions };
    }
}