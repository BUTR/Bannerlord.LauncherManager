using Bannerlord.ModuleManager;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Bannerlord.LauncherManager.Models;

public record CopyInstallInstruction : IInstallInstruction
{
    public InstallInstructionType Type { get; set; } = InstallInstructionType.Copy;
    public required string ModuleId { get; set; }
    public required string Source { get; set; }
    public required string Destination { get; set; }

    public CopyInstallInstruction() { }
    [SetsRequiredMembers, JsonConstructor]
    public CopyInstallInstruction(string moduleId, string source, string destination) => (ModuleId, Source, Destination) = (moduleId, source, destination);
}
public record ModuleInfoInstallInstruction : IInstallInstruction
{
    public InstallInstructionType Type { get; set; } = InstallInstructionType.ModuleInfo;
    public required ModuleInfoExtended ModuleInfo { get; set; }

    public ModuleInfoInstallInstruction() { }
    [SetsRequiredMembers, JsonConstructor]
    public ModuleInfoInstallInstruction(ModuleInfoExtended moduleInfo) => (ModuleInfo) = (moduleInfo);
}

public record NoneInstallInstruction : IInstallInstruction
{
    public InstallInstructionType Type { get; set; }

    public NoneInstallInstruction() { }
    [SetsRequiredMembers, JsonConstructor]
    public NoneInstallInstruction(InstallInstructionType type) => (Type) = (type);
}

[NetEscapades.EnumGenerators.EnumExtensions]
public enum InstallInstructionType
{
    None,
    Copy,
    ModuleInfo,
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