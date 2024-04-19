using Bannerlord.LauncherManager.Models;
using Bannerlord.ModuleManager;

using System.Text.Json.Serialization;

namespace Bannerlord.LauncherManager.Native.Tests;

[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Default,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    IgnoreReadOnlyFields = true,
    IgnoreReadOnlyProperties = true,
    IncludeFields = false,
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(ApplicationVersion))]
[JsonSerializable(typeof(SubModuleInfoExtended))]
[JsonSerializable(typeof(ModuleInfoExtended))]
[JsonSerializable(typeof(ModuleInfoExtended[]))]
[JsonSerializable(typeof(ModuleInfoExtendedWithPath))]
[JsonSerializable(typeof(ModuleInfoExtendedWithPath[]))]
[JsonSerializable(typeof(ModuleInfoExtendedWithMetadata))]
[JsonSerializable(typeof(ModuleInfoExtendedWithMetadata[]))]
[JsonSerializable(typeof(ModuleSorterOptions))]
[JsonSerializable(typeof(ModuleIssue[]))]
[JsonSerializable(typeof(DependentModuleMetadata[]))]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(InstallResult))]
[JsonSerializable(typeof(ModuleInfoInstallInstruction))]
[JsonSerializable(typeof(CopyInstallInstruction))]
[JsonSerializable(typeof(NoneInstallInstruction))]
[JsonSerializable(typeof(SaveMetadata[]))]
internal partial class SourceGenerationContext : JsonSerializerContext;