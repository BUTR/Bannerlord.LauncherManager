namespace Bannerlord.LauncherManager.Models
{
    public sealed record ProfileMod
    {
        public bool Enabled { get; set; }
        public ulong EnabledTime { get; set; }
    }
}