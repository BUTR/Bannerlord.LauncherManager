using System.Collections;
using System.Collections.Generic;

namespace Bannerlord.LauncherManager.Utils;

/// <summary>
/// Shared<br/>
/// </summary>
public sealed class AlphanumComparatorFast : IComparer<string?>, IComparer
{
    private readonly ModuleManager.AlphanumComparatorFast _alphanumComparatorFast = new();

    public int Compare(string? x, string? y) => _alphanumComparatorFast.Compare(x, y);
    public int Compare(object x, object y) => _alphanumComparatorFast.Compare(x, y);
}