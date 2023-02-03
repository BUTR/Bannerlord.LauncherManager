using System.Collections.Generic;

namespace Bannerlord.LauncherManager.Models
{
    public sealed record Profile
    {
        public string Id { get; set; } = string.Empty;
        public string GameId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Dictionary<string, ProfileMod> ModState { get; set; } = new();
        public ulong LastActivated { get; set; }
        public bool? PendingRemove { get; set; }
        //public Dictionary<string, object>? Features { get; set; } = new();
    }
}