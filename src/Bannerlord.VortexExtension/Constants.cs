using System.IO;

namespace Bannerlord.VortexExtension
{
    public static class Constants
    {
        public static string I18Namespace = "game-mount-and-blade2";

        public static string GameID = "mountandblade2bannerlord";
        public static string BannerlordExecutable = Path.Combine("bin", "Win64_Shipping_Client", "Bannerlord.exe");
        public static string BLSEExecutable = Path.Combine("bin", "Win64_Shipping_Client", "Bannerlord.BLSE.exe");
        public static string SubModuleName = "SubModule.xml";
        public static string ModulesFolder = "Modules";
    }
}