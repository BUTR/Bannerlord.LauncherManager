using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Threading;

namespace Bannerlord.LauncherManager.Wasm;

/// <summary>
/// Runs await continuations in a JS microtask. Without a context they go to the thread pool, which the single-threaded
/// runtime runs from a setTimeout(0): ~15 ms each in Node on Windows, 1-4 ms in Chromium. A LauncherManager call awaits
/// its JS callbacks hundreds of times, every module list read is several awaits per module
/// </summary>
internal sealed partial class MicrotaskSynchronizationContext : SynchronizationContext
{
    private static readonly MicrotaskSynchronizationContext Instance = new();

    /// <summary>
    /// The thread pool resets the context after each of its work items, and they run on the one thread too.
    /// Every async export calls it before its first await
    /// </summary>
    public static void Install()
    {
        if (Current != Instance)
            SetSynchronizationContext(Instance);
    }

    private readonly Queue<(SendOrPostCallback Callback, object? State)> _queue = new();
    private readonly Action _drain;
    private bool _scheduled;

    public MicrotaskSynchronizationContext() => _drain = Drain;

    public override void Post(SendOrPostCallback d, object? state)
    {
        _queue.Enqueue((d, state));
        if (_scheduled) return;
        _scheduled = true;
        QueueMicrotask(_drain);
    }

    public override void Send(SendOrPostCallback d, object? state) => d(state);

    public override SynchronizationContext CreateCopy() => this;

    // Continuations posted while draining run in the same microtask
    private void Drain()
    {
        try
        {
            while (_queue.TryDequeue(out var item))
            {
                try
                {
                    Install();
                    item.Callback(item.State);
                }
                catch (Exception e)
                {
                    Native.Logger.ExternalLog(LogLevel.Error, $"[C# ] Continuation failed{Environment.NewLine}{e}");
                }
            }
        }
        finally
        {
            _scheduled = false;
        }
    }

    [JSImport("globalThis.queueMicrotask")]
    private static partial void QueueMicrotask([JSMarshalAs<JSType.Function>] Action callback);
}

public static partial class RuntimeExports
{
    /// <summary>
    /// The TS wrapper calls it once after the runtime starts
    /// </summary>
    [JSExport]
    public static void Initialize() => MicrotaskSynchronizationContext.Install();
}
