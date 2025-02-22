using Bannerlord.LauncherManager.Models;
using Bannerlord.LauncherManager.Native.Models;
using Bannerlord.ModuleManager;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Bannerlord.LauncherManager.Native;

[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Default,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    IgnoreReadOnlyFields = true,
    IgnoreReadOnlyProperties = true,
    IncludeFields = false,
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]

[JsonSerializable(typeof(Bindings.OrderByLoadOrderResult))]

[JsonSerializable(typeof(IReadOnlyList<DialogFileFilter>))]

[JsonSerializable(typeof(TWSaveMetadata))]
[JsonSerializable(typeof(SaveMetadata))]
[JsonSerializable(typeof(IReadOnlyList<SaveMetadata>))]
[JsonSerializable(typeof(SaveMetadata[]))]

[JsonSerializable(typeof(String[]))]
[JsonSerializable(typeof(List<String>))]

[JsonSerializable(typeof(InstallResult))]
[JsonSerializable(typeof(ModuleInfoInstallInstruction))]
[JsonSerializable(typeof(CopyInstallInstruction))]
[JsonSerializable(typeof(CopyStoreInstallInstruction))]
[JsonSerializable(typeof(NoneInstallInstruction))]

[JsonSerializable(typeof(SupportedResult))]
[JsonSerializable(typeof(LoadOrder))]

[JsonSerializable(typeof(ModuleViewModel))]
[JsonSerializable(typeof(ModuleViewModel[]))]
[JsonSerializable(typeof(IReadOnlyList<ModuleViewModel>))]
[JsonSerializable(typeof(LauncherOptions))]
[JsonSerializable(typeof(LauncherState))]

[JsonSerializable(typeof(ApplicationVersion))]
[JsonSerializable(typeof(SubModuleInfoExtended))]
[JsonSerializable(typeof(ModuleInfoExtended))]
[JsonSerializable(typeof(ModuleInfoExtended[]))]
[JsonSerializable(typeof(ModuleInfoExtendedWithPath))]
[JsonSerializable(typeof(ModuleInfoExtendedWithPath[]))]
[JsonSerializable(typeof(ModuleInfoExtendedWithMetadata))]
[JsonSerializable(typeof(IReadOnlyList<ModuleInfoExtendedWithMetadata>))]
[JsonSerializable(typeof(ModuleInfoExtendedWithMetadata[]))]
[JsonSerializable(typeof(ModuleSorterOptions))]
[JsonSerializable(typeof(ModuleIssue[]))]
[JsonSerializable(typeof(DependentModuleMetadata[]))]
internal partial class SourceGenerationContext : JsonSerializerContext;