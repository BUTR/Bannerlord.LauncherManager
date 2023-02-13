using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Bannerlord.LauncherManager.Models;

public record CopyInstallInstruction : IInstallInstruction
{
    public InstallInstructionType Type { get; set; } = InstallInstructionType.Copy;
    public required string Source { get; set; }
    public required string Destination { get; set; }

    public CopyInstallInstruction() { }
    [SetsRequiredMembers, JsonConstructor]
    public CopyInstallInstruction(string source, string destination) => (Source, Destination) = (source, destination);
}
public record AttributeInstallInstruction : IInstallInstruction
{
    public InstallInstructionType Type { get; set; } = InstallInstructionType.Attribute;
    public required string Key { get; set; }
    public required List<string> Value { get; set; }

    public AttributeInstallInstruction() { }
    [SetsRequiredMembers, JsonConstructor]
    public AttributeInstallInstruction(string key, List<string> value) => (Key, Value) = (key, value);
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