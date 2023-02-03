namespace Bannerlord.VortexExtension.Models
{
    public sealed record ProfileMod
    {
        public bool Enabled { get; set; }
        public ulong EnabledTime { get; set; }
    }
}