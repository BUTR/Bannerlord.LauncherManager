using Bannerlord.ModuleManager;

using System;
using System.Collections.Generic;

namespace Bannerlord.LauncherManager.Models;

public class SaveMetadata : Dictionary<string, string>
{
    public string Name => this[nameof(Name)];

    public SaveMetadata(string name)
    {
        this[nameof(Name)] = name;
    }

    public string[] GetModules()
    {
        if (!TryGetValue("Modules", out var text))
        {
            return Array.Empty<string>();
        }
        return text.Split(';');
    }
    public ApplicationVersion GetModuleVersion(string moduleName)
    {
        if (TryGetValue($"Module_{moduleName}", out var versionRaw))
        {
            return ApplicationVersion.TryParse(versionRaw, out var versionVar) ? versionVar : ApplicationVersion.Empty;
        }
        return ApplicationVersion.Empty;
    }

    public int GetChangeSet() =>
        TryGetValue("ApplicationVersion", out var av) && av?.Split('.') is { Length: 4 } split && int.TryParse(split[3], out var cs) ? cs : -1;
}