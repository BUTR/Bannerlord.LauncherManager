using System.IO;

namespace Bannerlord.LauncherManager.Utils;

public static class IssuesChecker
{
    /// <summary>
    /// External<br/>
    /// </summary>
    public static void CheckForRootHarmony(this LauncherManagerHandler launcherManagerHandler)
    {
        var folder = launcherManagerHandler.GetInstallPath();
        if (!string.IsNullOrEmpty(folder)) return;

        var harmonyFilePath = Path.Combine(folder, Constants.BinFolder, Constants.Win64Configuration, "0Harmony.dll");
        if (launcherManagerHandler.ReadFileContent(harmonyFilePath, 0, -1) is not { } data) return;

        launcherManagerHandler.ShowWarning("{=dDprK7Mz}WARNING!", "{=tqjPGPtP}BUTRLoader detected 0Harmony.dll inside the game's root bin folder!{NL}This could lead to issues, remove it?", string.Empty, result =>
        {
            if (!result) return;

            launcherManagerHandler.WriteFileContent($"{harmonyFilePath}.bak", data);
            launcherManagerHandler.WriteFileContent(harmonyFilePath, null);
        });
    }
}