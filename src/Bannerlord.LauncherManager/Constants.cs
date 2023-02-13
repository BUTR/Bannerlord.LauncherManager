using System.IO;

namespace Bannerlord.LauncherManager;

public static class Constants
{
    public static string BannerlordExecutable = Path.Combine("bin", "Win64_Shipping_Client", "Bannerlord.exe");
    public static string BLSEExecutable = Path.Combine("bin", "Win64_Shipping_Client", "Bannerlord.BLSE.exe");
    public static string SubModuleName = "SubModule.xml";
    public static string ModulesFolder = "Modules";
}