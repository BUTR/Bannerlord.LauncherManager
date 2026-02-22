using Bannerlord.LauncherManager.Models;
using Bannerlord.LauncherManager.Native.Utils;

using MLVScan.Models.Dto;
using MLVScan.Services;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.Native;

partial class LauncherManagerHandlerNative
{
    private static readonly HashSet<string> ExcludedAssemblies =
    [
        "0Harmony.dll",

        "Mono.Cecil.dll",
        "Mono.Cecil.Mdb.dll",
        "Mono.Cecil.Pdb.dll",
        "Mono.Cecil.Rocks.dll",

        "MonoMod.Backports.dll",
        "MonoMod.Core.dll",
        "MonoMod.Iced.dll",
        "MonoMod.ILHelpers.dll",
        "MonoMod.Utils.dll",
    ];

    private async Task<List<ScanResultDto>> ScanFolder(string folder)
    {
        var result = new List<ScanResultDto>();
        var modScanner = new ModScanner(folder);
        var files = await FileSystemProvider.ReadDirectoryFileListAsync(folder);
        var assemblies = files?.Where(x => Path.GetExtension(x).Equals(".dll", System.StringComparison.OrdinalIgnoreCase)) ?? [];
        foreach (var assembly in assemblies)
        {
            if (ExcludedAssemblies.Contains(Path.GetFileName(assembly)))
                continue;

            if (await FileSystemProvider.ReadFileContentAsync(assembly, 0, -1) is { } data)
            {
                var findings = modScanner.ScanMod(data, assembly).Select(x =>
                {
                    x.Location = Path.GetFileName(assembly);
                    return x;
                });
                result.AddRange(ScanResultMapper.ToDto(findings, assembly, data));
            }
        }

        return result;
    }

    public async Task<List<ScanResultDto>> ScanModAsync(ModuleInfoExtendedWithMetadata moduleInfoExtended)
    {
        if (moduleInfoExtended.IsOfficial)
            return [];

        var result = new List<ScanResultDto>();

        var moduleDir = Path.GetDirectoryName(moduleInfoExtended.Path)!;
        var asmWin = Path.Combine(moduleDir, Constants.BinFolder, Constants.Win64Configuration);
        var asmXbox = Path.Combine(moduleDir, Constants.BinFolder, Constants.XboxConfiguration);

        var platform = await GetPlatformAsync();
        switch (platform)
        {
            case GamePlatform.Win64:
            {
                result.AddRange(await ScanFolder(asmWin));
                break;
            }
            case GamePlatform.Xbox:
            {
                result.AddRange(await ScanFolder(asmXbox));
                break;
            }
        }

        return result;
    }
}