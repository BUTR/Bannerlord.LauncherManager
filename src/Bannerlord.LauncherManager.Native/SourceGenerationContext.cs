using Bannerlord.ModuleManager;
using Bannerlord.LauncherManager.Models;

using System;
using System.Text.Json.Serialization;

namespace Bannerlord.LauncherManager.Native
{
    [JsonSourceGenerationOptions(
        GenerationMode = JsonSourceGenerationMode.Default,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        IgnoreReadOnlyFields = true,
        IgnoreReadOnlyProperties = true,
        IncludeFields = false,
        WriteIndented = false,
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]

    [JsonSerializable(typeof(String[]))]

    [JsonSerializable(typeof(SupportedResult))]
    [JsonSerializable(typeof(InstallResult))]
    [JsonSerializable(typeof(Profile))]
    [JsonSerializable(typeof(LoadOrder))]
    //[JsonSerializable(typeof(DiscoveredTool))]

    [JsonSerializable(typeof(ApplicationVersion))]
    [JsonSerializable(typeof(SubModuleInfoExtended))]
    [JsonSerializable(typeof(ModuleInfoExtended))]
    [JsonSerializable(typeof(ModuleInfoExtended[]))]
    [JsonSerializable(typeof(ModuleSorterOptions))]
    [JsonSerializable(typeof(ModuleIssue[]))]
    [JsonSerializable(typeof(DependentModuleMetadata[]))]
    internal partial class SourceGenerationContext : JsonSerializerContext { }
}