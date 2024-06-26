﻿using Bannerlord.LauncherManager.Localization;
using Bannerlord.ModuleManager;

using System;

namespace Bannerlord.LauncherManager.Utils;

public static class ModuleIssueRenderer
{
    public static string Render(ModuleIssue issue) => RenderTextObject(issue).ToString();

    private static string Version(ApplicationVersionRange version) => version == ApplicationVersionRange.Empty
        ? version.ToString()
        : version.Min == version.Max
            ? version.Min.ToString()
            : "";

    private static BUTRTextObject RenderTextObject(ModuleIssue issue) => issue.Type switch
    {
        ModuleIssueType.Missing => new BUTRTextObject("{=J3Uh6MV4}Missing '{ID}' {VERSION} in modules list")
            .SetTextVariable("ID", issue.SourceId)
            .SetTextVariable("VERSION", issue.SourceVersion.Min.ToString()),

        ModuleIssueType.MissingDependencies => new BUTRTextObject("{=3eQSr6wt}Missing '{ID}' {VERSION}")
            .SetTextVariable("ID", issue.SourceId)
            .SetTextVariable("VERSION", Version(issue.SourceVersion)),
        ModuleIssueType.DependencyMissingDependencies => new BUTRTextObject("{=U858vdQX}'{ID}' is missing it's dependencies!")
            .SetTextVariable("ID", issue.SourceId),

        ModuleIssueType.DependencyValidationError => new BUTRTextObject("{=1LS8Z5DU}'{ID}' has unresolved issues!")
            .SetTextVariable("ID", issue.SourceId),

        ModuleIssueType.VersionMismatchLessThanOrEqual => new BUTRTextObject("{=Vjz9HQ41}'{ID}' wrong version <= {VERSION}")
            .SetTextVariable("ID", issue.SourceId)
            .SetTextVariable("VERSION", Version(issue.SourceVersion)),
        ModuleIssueType.VersionMismatchLessThan => new BUTRTextObject("{=ZvnlL7VE}'{ID}' wrong version < [{VERSION}]")
            .SetTextVariable("ID", issue.SourceId)
            .SetTextVariable("VERSION", Version(issue.SourceVersion)),
        ModuleIssueType.VersionMismatchGreaterThan => new BUTRTextObject("{=EfNuH2bG}'{ID}' wrong version > [{VERSION}]")
            .SetTextVariable("ID", issue.SourceId)
            .SetTextVariable("VERSION", Version(issue.SourceVersion)),

        ModuleIssueType.Incompatible => new BUTRTextObject("{=zXDidmpQ}'{ID}' is incompatible with this module")
            .SetTextVariable("ID", issue.SourceId),

        ModuleIssueType.DependencyConflictDependentAndIncompatible => new BUTRTextObject("{=4KFwqKgG}Module '{ID}' is both depended upon and marked as incompatible")
            .SetTextVariable("ID", issue.SourceId),
        ModuleIssueType.DependencyConflictDependentLoadBeforeAndAfter => new BUTRTextObject("{=9DRB6yXv}Module '{ID}' is both depended upon as LoadBefore and LoadAfter")
            .SetTextVariable("ID", issue.SourceId),
        ModuleIssueType.DependencyConflictCircular => new BUTRTextObject("{=RC1V9BbP}Circular dependencies. '{TARGETID}' and '{SOURCEID}' depend on each other")
            .SetTextVariable("TARGETID", issue.Target.Id)
            .SetTextVariable("SOURCEID", issue.SourceId),

        ModuleIssueType.DependencyNotLoadedBeforeThis => new BUTRTextObject("{=s3xbuejE}'{SOURCEID}' should be loaded before '{TARGETID}'")
            .SetTextVariable("TARGETID", issue.Target.Id)
            .SetTextVariable("SOURCEID", issue.SourceId),

        ModuleIssueType.DependencyNotLoadedAfterThis => new BUTRTextObject("{=2ALJB7z2}'{SOURCEID}' should be loaded after '{TARGETID}'")
            .SetTextVariable("ID", issue.SourceId),

        // TODO:
        ModuleIssueType.MissingModuleId => new BUTRTextObject($"Module Id is missing for '{issue.Target.Name}'"),
        ModuleIssueType.MissingModuleName => new BUTRTextObject($"Module Name is missing in '{issue.Target.Id}'"),

        ModuleIssueType.DependencyIsNull => new BUTRTextObject($"Found a null dependency in '{issue.Target.Id}'"),
        ModuleIssueType.DependencyMissingModuleId => new BUTRTextObject($"Module Id is missing for one if the dependencies of '{issue.Target.Id}'"),

        _ => throw new ArgumentOutOfRangeException()
    };
}