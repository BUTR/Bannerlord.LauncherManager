namespace Bannerlord.VortexExtension.Models
{
    public sealed record Reference
    {
        public string? FileMD5 { get; set; }
        public int? FileSize { get; set; }
        public string? GameId { get; set; }
        public string? VersionMatch { get; set; }
        public string? LogicalFileName { get; set; }
        public string? FileExpression { get; set; }
    }
}