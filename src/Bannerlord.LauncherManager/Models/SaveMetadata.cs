using Bannerlord.ModuleManager;

using System;
using System.Collections.Generic;

namespace Bannerlord.LauncherManager.Models;

public class TWSaveMetadata
{
    [Newtonsoft.Json.JsonProperty("List")]
    [System.Text.Json.Serialization.JsonPropertyName("List")]
    public Dictionary<string, string> List { get; set; } = new();
}

public class SaveMetadata : Dictionary<string, string>
{
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public string Name => this[nameof(Name)];

    [Newtonsoft.Json.JsonConstructor]
    [System.Text.Json.Serialization.JsonConstructor]
    public SaveMetadata() { }
    public SaveMetadata(string name)
    {
        this[nameof(Name)] = name;
    }
    public SaveMetadata(string name, TWSaveMetadata metadata) : base(metadata.List)
    {
        this[nameof(Name)] = name;
    }

    public string[] GetModules() =>
        TryGetValue("Modules", out var text) ? text.Split(';') : Array.Empty<string>();

    public ApplicationVersion GetModuleVersion(string moduleName) =>
        TryGetValue($"Module_{moduleName}", out var versionRaw) ? ApplicationVersion.TryParse(versionRaw, out var versionVar) ? versionVar : ApplicationVersion.Empty : ApplicationVersion.Empty;

    public int GetChangeSet() =>
        TryGetValue("ApplicationVersion", out var av) && av?.Split('.') is { Length: 4 } split && int.TryParse(split[3], out var cs) ? cs : -1;
}