namespace Bannerlord.VortexExtension.Models
{
    public sealed record InstallInstruction
    {
        public InstructionType Type { get; set; }
        public string? Path { get; set; }
        public string? Source { get; set; }
        public string? Destination { get; set; }
        public string? Section { get; set; }
        public string? Key { get; set; }
        public object? Value { get; set; }
        public string? SubmoduleType { get; set; }
        public string? Data { get; set; }
        public Rule? Rule { get; set; }

        public override string ToString() =>
            $"Type: {Type}; Path: {Path}; Source: {Source}; Destination: {Destination}; Section: {Section}; Key: {Key}; " +
            $"Value: {Value}; SubmoduleType: {SubmoduleType};  Data: {Data};  Rule: {Rule}; ";

    }
}