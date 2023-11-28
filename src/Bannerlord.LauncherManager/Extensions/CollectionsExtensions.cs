using System;
using System.Collections.Generic;

namespace Bannerlord.LauncherManager.Extensions;

public static class CollectionsExtensions
{
    public static int IndexOf<T>(this IReadOnlyList<T> self, Func<T, bool> preficate)
    {
        var i = 0;
        foreach (var element in self)
        {
            if (preficate(element))
                return i;
            i++;
        }
        return -1;
    }
}