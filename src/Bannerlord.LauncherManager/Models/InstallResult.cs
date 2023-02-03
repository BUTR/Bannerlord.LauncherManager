using System.Collections.Generic;

namespace Bannerlord.VortexExtension.Models
{
    public sealed record InstallResult
    {
        public static InstallResult AsInvalid { get; } = new()
        {
            Instructions = new()
        };

        public List<InstallInstruction> Instructions { get; set; } = new();

        public override string ToString() => $"Instructions: {string.Join(", ", Instructions)}";
    }
}