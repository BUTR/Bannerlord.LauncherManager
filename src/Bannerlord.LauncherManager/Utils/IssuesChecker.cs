using System.IO;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.Utils;

public static class IssuesChecker
{
    /// <summary>
    /// External<br/>
    /// </summary>
    public static async Task CheckForRootHarmonyAsync(this LauncherManagerHandler launcherManagerHandler)
    {
        var folder = await launcherManagerHandler.GetInstallPathAsync();
        if (!string.IsNullOrEmpty(folder)) return;

        var harmonyFilePath = Path.Combine(folder, Constants.BinFolder, Constants.Win64Configuration, "0Harmony.dll");
        if (await launcherManagerHandler.ReadFileContentAsync(harmonyFilePath, 0, -1) is not { } data) return;

        var result = await launcherManagerHandler.ShowWarningAsync("{=dDprK7Mz}WARNING!", "{=tqjPGPtP}BUTRLoader detected 0Harmony.dll inside the game's root bin folder!{NL}This could lead to issues, remove it?", string.Empty);
        if (!result) return;

        await launcherManagerHandler.WriteFileContentAsync($"{harmonyFilePath}.bak", data);
        await launcherManagerHandler.WriteFileContentAsync(harmonyFilePath, null);
    }
}