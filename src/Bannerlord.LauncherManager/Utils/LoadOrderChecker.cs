using Bannerlord.ModuleManager;

using System.Collections.Generic;
using System.Linq;

namespace Bannerlord.LauncherManager.Utils;

public static class LoadOrderChecker
{
    /// <summary>
    /// External<br/>
    /// </summary>
    public static IEnumerable<string> IsLoadOrderCorrect(IReadOnlyList<ModuleInfoExtended> modules)
    {
        var loadOrder = FeatureIds.LauncherFeatures.Select(x => new ModuleInfoExtended { Id = x }).Concat(modules).ToList();
        foreach (var module in modules)
        {
            var issues = ModuleUtilities.ValidateLoadOrder(loadOrder, module).ToList();
            if (issues.Any())
                return issues.Select(ModuleIssueRenderer.Render);
        }
        return Enumerable.Empty<string>();
    }
}