using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

// The bindings read saves ahead too
[assembly: InternalsVisibleTo("Bannerlord.LauncherManager.Native")]
[assembly: InternalsVisibleTo("Bannerlord.LauncherManager.Wasm")]

namespace Bannerlord.LauncherManager.Utils;

/// <summary>
/// The host's file IO is asynchronous and each call waits on something, a disk seek or an IPC round trip. Starting the
/// next calls while earlier ones are in flight overlaps those waits instead of adding them up
/// </summary>
internal static class ReadAhead
{
    /// <summary>
    /// How many calls are in flight at once: enough to keep the host's IO busy, few enough that a slow disk serves the
    /// first results first
    /// </summary>
    public const int Window = 16;

    /// <summary>
    /// Starts the work for the next items while earlier ones complete, at most <see cref="Window"/> at once, and yields
    /// the results in the items' order
    /// </summary>
    public static async IAsyncEnumerable<TResult> SelectInOrderAsync<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, Task<TResult>> start)
    {
        var pending = new Queue<Task<TResult>>(Window);
        await foreach (var item in source)
        {
            if (pending.Count == Window)
                yield return await pending.Dequeue();
            pending.Enqueue(start(item));
        }
        while (pending.Count > 0)
            yield return await pending.Dequeue();
    }
}
