using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Bannerlord.LauncherManager.Models;

public class DialogFileFilter
{
    public required string Name { get; set; }
    public required IReadOnlyList<string> Extensions { get; set; }

    public DialogFileFilter() { }
    [SetsRequiredMembers, JsonConstructor]
    public DialogFileFilter(string name, IReadOnlyList<string> extensions) => (Name, Extensions) = (name, extensions);
}