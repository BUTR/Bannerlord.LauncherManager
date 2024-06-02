using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Bannerlord.LauncherManager.Models;

public record LauncherOptions
{
    public static readonly LauncherOptions Empty = new()
    {
        BetaSorting = false
    };

    public required bool BetaSorting { get; set; }

    public LauncherOptions() { }

    [SetsRequiredMembers, JsonConstructor]
    public LauncherOptions(bool betaSorting)
    {
        BetaSorting = betaSorting;
    }
}