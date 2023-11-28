using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Bannerlord.LauncherManager.Models;

public record LauncherState
{
    public static readonly LauncherState Empty = new()
    {
        IsSingleplayer = true
    };

    public required bool IsSingleplayer { get; set; }

    public LauncherState() { }

    [SetsRequiredMembers, JsonConstructor]
    public LauncherState(bool isSingleplayer)
    {
        IsSingleplayer = isSingleplayer;
    }
}