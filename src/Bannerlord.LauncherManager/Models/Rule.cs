namespace Bannerlord.LauncherManager.Models
{
    public sealed record Rule
    {
        public RuleType Type { get; set; }
        public Reference Reference { get; set; } = new();
        public string? Comment { get; set; }
    }
}