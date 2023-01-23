using Bannerlord.ModuleManager;
using Bannerlord.VortexExtension.Models;

using System;
using System.Text.Json.Serialization;

namespace Bannerlord.VortexExtension.Native
{
    [JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default)]

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
    internal partial class SourceGenerationContext : JsonSerializerContext { }
}