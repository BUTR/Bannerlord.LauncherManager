using System.Collections.Generic;

namespace Bannerlord.VortexExtension.Models
{
    public sealed record SupportedResult
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
                "Bannerlord.BUTRLoader.dll"
            }
        };

        public bool Supported { get; set; }
        public List<string> RequiredFiles { get; set; } = new();

        public override string ToString() => $"Supported: {Supported}; RequiredFiles: {string.Join(", ", RequiredFiles)}";
    }
}