using System.Collections.Generic;

namespace Bannerlord.LauncherManager.Models;

public record SupportedResult
{
    public static SupportedResult AsNotSupported { get; } = new()
    {
        Supported = false,
        RequiredFiles = new()
    };
    public static SupportedResult AsSupported { get; } = new()
    {
        Supported = true,
        RequiredFiles = new()
    };
    public static SupportedResult AsSupportedWithBUTRLoader { get; } = new()
    {
        Supported = true,
        RequiredFiles = new()
        {
            "Bannerlord.BUTRLoader.dll",
            "TaleWorlds.MountAndBlade.Launcher.exe.config",
        }
    };
    public static SupportedResult AsSupportedWithBLSE { get; } = new()
    {
        Supported = true,
        RequiredFiles = new()
        {
            "Bannerlord.BLSE.exe"
        }
    };

    public bool Supported { get; set; }
    public List<string> RequiredFiles { get; set; } = new();
}