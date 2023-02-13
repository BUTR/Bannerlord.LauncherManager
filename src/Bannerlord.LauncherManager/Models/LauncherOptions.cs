using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Bannerlord.LauncherManager.Models;

public record LauncherOptions
{
    public static readonly LauncherOptions Empty = new()
    {
        Language = "English",
        UnblockFiles = true,
        FixCommonIssues = false,
        BetaSorting = false
    };

    public required string Language { get; set; }
    public required bool UnblockFiles { get; set; }
    public required bool FixCommonIssues { get; set; }
    public required bool BetaSorting { get; set; }

    public LauncherOptions() { }

    [SetsRequiredMembers, JsonConstructor]
    public LauncherOptions(bool betaSorting, bool fixCommonIssues, bool unblockFiles, string language)
    {
        BetaSorting = betaSorting;
        FixCommonIssues = fixCommonIssues;
        UnblockFiles = unblockFiles;
        Language = language;
    }
}