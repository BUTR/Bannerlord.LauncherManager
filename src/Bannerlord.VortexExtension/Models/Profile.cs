using System.Collections.Generic;

namespace Bannerlord.VortexExtension.Models
{
    public sealed record Profile
    {
        public string Id { get; set; }
        public string GameId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, ProfileMod> ModState { get; set; } = new();
        public ulong LastActivated { get; set; }
        public bool? PendingRemove { get; set; }
        //public Dictionary<string, object>? Features { get; set; } = new();
    }
}