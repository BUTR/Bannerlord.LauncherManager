using System;
using System.IO;

namespace Bannerlord.VortexExtension.Native
{
    public static class Logger
    {
        public static void Log(string message)
        {
#if DEBUG
            var path = "D:\\Git\\Bannerlord.VortexExtension\\src\\Bannerlord.VortexExtension.Native\\bin\\Release\\net7.0\\win-x64\\native\\test.log";
            File.AppendAllText(path, $"{message}{Environment.NewLine}");
#endif
        }
    }
}