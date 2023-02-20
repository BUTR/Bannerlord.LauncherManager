using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Bannerlord.LauncherManager.Models;

public record CopyInstallInstruction : IInstallInstruction
{
    public InstallInstructionType Type { get; set; } = InstallInstructionType.Copy;
    public required string ModId { get; set; }
    public required string Source { get; set; }
    public required string Destination { get; set; }

    public CopyInstallInstruction() { }
    [SetsRequiredMembers, JsonConstructor]
    public CopyInstallInstruction(string modId, string source, string destination) => (ModId, Source, Destination) = (modId, source, destination);
}

public record NoneInstallInstruction : IInstallInstruction
{
    public InstallInstructionType Type { get; set; }

    public NoneInstallInstruction() { }
    [SetsRequiredMembers, JsonConstructor]
    public NoneInstallInstruction(InstallInstructionType type) => (Type) = (type);
}

public enum InstallInstructionType
{
    None,
    Copy,
    Attribute
}

public interface IInstallInstruction
{
    InstallInstructionType Type { get; }
}

public record InstallResult
{
    public static readonly InstallResult AsInvalid = new()
    {
        Instructions = new()
    };

    public required List<IInstallInstruction> Instructions { get; set; }

    public InstallResult() { }
    [SetsRequiredMembers, JsonConstructor]
    public InstallResult(List<IInstallInstruction> instructions) => (Instructions) = (instructions);
}