using Bannerlord.LauncherManager.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Bannerlord.LauncherManager;

partial class LauncherManagerHandler
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsDirectorySeparator(char c) => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar;

    private static bool EndsInDirectorySeparator(string? path) =>
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

    private sealed record FileWithModuleInfo(string File, string ModuleBasePath, ModuleInfoExtendedWithMetadata ModuleInfo);

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
    public InstallResult InstallModuleContent(string[] files, ModuleInfoExtendedWithMetadata[] moduleInfos)
    {
        var lowerSubModuleName = Constants.SubModuleName.ToLower();
        if (!files.Any(x => x.ToLower().Contains(lowerSubModuleName)))
            return InstallResult.AsInvalid;

        files = files.Where(x => !EndsInDirectorySeparator(x)).ToArray();

        var filesWithModuleInfos = files.Where(x => x.EndsWith(Constants.SubModuleName, StringComparison.OrdinalIgnoreCase)).SelectMany(file =>
        {
            var moduleInfo = moduleInfos.FirstOrDefault(x => x.Path == file);
            if (moduleInfo is null) return new List<FileWithModuleInfo>();

            var subModuleFileIdx = file.IndexOf(Constants.SubModuleName, StringComparison.OrdinalIgnoreCase);
            var moduleBasePath = file.Substring(0, subModuleFileIdx);
            return files.Where(y => y.StartsWith(moduleBasePath)).Select(y => new FileWithModuleInfo(y, moduleBasePath, moduleInfo));
        }).ToArray();

        var moduleInfoInstructions = filesWithModuleInfos.Select(x => x.ModuleInfo).Distinct().Select(x => new ModuleInfoInstallInstruction(x)).ToArray();
        var copyInstructions = filesWithModuleInfos.SelectMany(GetCopyInstructions).ToArray();
        var copyStoreInstructions = filesWithModuleInfos.SelectMany(GetStoreCopyInstructions).ToArray();

        return new InstallResult
        {
            Instructions = copyInstructions.Concat(copyStoreInstructions).Concat(moduleInfoInstructions).ToList()
        };
    }

    private IEnumerable<IInstallInstruction> GetCopyInstructions(FileWithModuleInfo filesWithModuleInfo)
    {
        var file = filesWithModuleInfo.File;
        var moduleBasePath = filesWithModuleInfo.ModuleBasePath;
        var moduleInfo = filesWithModuleInfo.ModuleInfo;

        var modulePath = file.Substring(moduleBasePath.Length);
        if (modulePath.StartsWith(Constants.BinFolder, StringComparison.OrdinalIgnoreCase)) yield break;

        var destination = Path.Combine(Constants.ModulesFolder, moduleInfo.Id, file.Substring(moduleBasePath.Length));
        yield return new CopyInstallInstruction
        {
            ModuleId = moduleInfo.Id,
            Source = file,
            Destination = destination,
        };
    }

    private IEnumerable<IInstallInstruction> GetStoreCopyInstructions(FileWithModuleInfo filesWithModuleInfo)
    {
        var file = filesWithModuleInfo.File;
        var moduleBasePath = filesWithModuleInfo.ModuleBasePath;
        var moduleInfo = filesWithModuleInfo.ModuleInfo;

        var modulePath = file.Substring(moduleBasePath.Length);
        if (!modulePath.StartsWith(Constants.BinFolder, StringComparison.OrdinalIgnoreCase)) yield break;
        var binPath = modulePath.Substring(4);

        var destination = Path.Combine(Constants.ModulesFolder, moduleInfo.Id, file.Substring(moduleBasePath.Length));
        if (binPath.StartsWith(Constants.Win64Configuration, StringComparison.OrdinalIgnoreCase))
        {
            yield return new CopyStoreInstallInstruction
            {
                Store = GameStore.Steam,
                ModuleId = moduleInfo.Id,
                Source = file,
                Destination = destination,
            };
            yield return new CopyStoreInstallInstruction
            {
                Store = GameStore.GOG,
                ModuleId = moduleInfo.Id,
                Source = file,
                Destination = destination,
            };
            yield return new CopyStoreInstallInstruction
            {
                Store = GameStore.Epic,
                ModuleId = moduleInfo.Id,
                Source = file,
                Destination = destination,
            };
        }
        else if (binPath.StartsWith(Constants.XboxConfiguration, StringComparison.OrdinalIgnoreCase))
        {
            yield return new CopyStoreInstallInstruction
            {
                Store = GameStore.Xbox,
                ModuleId = moduleInfo.Id,
                Source = file,
                Destination = destination,
            };
        }
    }
}