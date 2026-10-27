using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.Wasm;

/// <summary>
/// The runtime resolves the promise of a returned Task from the thread pool, a setTimeout(0) hop per call: 4 ms in
/// Chromium, 15 ms in Node on Windows. The async exports settle the TS wrapper's promise themselves instead, from the
/// continuation, which runs in a microtask, see <see cref="MicrotaskSynchronizationContext"/>.
/// The promise is the wrapper's, by a call id, settled through imports: resolve and reject passed as functions became
/// proxies per call the runtime tracks, and that tracking broke under some GC timing, see asyncCall in runtime.ts
/// </summary>
internal static partial class AsyncExport
{
    private const string Module = "asyncresults";

    [JSImport("resolveVoid", Module)]
    private static partial void ResolveVoid(int call);

    [JSImport("resolveString", Module)]
    private static partial void ResolveString(int call, string? value);

    [JSImport("resolveBoolean", Module)]
    private static partial void ResolveBoolean(int call, bool value);

    [JSImport("reject", Module)]
    private static partial void Reject(int call, string error);

    public static async void Run(int call, Func<Task> action)
    {
        MicrotaskSynchronizationContext.Install();
        try
        {
            await action();
        }
        catch (Exception e)
        {
            Reject(call, e);
            return;
        }
        ResolveVoid(call);
    }

    public static async void RunString(int call, Func<Task<string?>> action)
    {
        MicrotaskSynchronizationContext.Install();
        string? result;
        try
        {
            result = await action();
        }
        catch (Exception e)
        {
            Reject(call, e);
            return;
        }
        ResolveString(call, result);
    }

    public static async void RunBoolean(int call, Func<Task<bool>> action)
    {
        MicrotaskSynchronizationContext.Install();
        bool result;
        try
        {
            result = await action();
        }
        catch (Exception e)
        {
            Reject(call, e);
            return;
        }
        ResolveBoolean(call, result);
    }

    private static void Reject(int call, Exception e)
    {
        var exception = e is AggregateException { InnerExceptions.Count: 1 } aggregate ? aggregate.InnerExceptions[0] : e;
        Reject(call, exception.ToString());
    }
}
