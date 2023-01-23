using Microsoft.VisualBasic.CompilerServices;

namespace Bannerlord.VortexExtension.Models
{
    public enum LockedState
    {
        @true,
        @false,
        always,
        never
    }

    public sealed record LoadOrderEntry
    {
        public long Pos { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; } = string.Empty;
        //public LockedState? Locked { get; set; }
        public string? ModId { get; set; }
        public object? Data { get; set; }

        public override string ToString() => $"{Name} - {Pos}";
    }

    public sealed record LoadOrderDisplayItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ImgUrl { get; set; } = string.Empty;
        public string? Prefix { get; set; }
        public string? Data { get; set; }
        public bool? Locked { get; set; }
        public bool? External { get; set; }
        public bool? Official { get; set; }
        public string? Message { get; set; }
    }
}