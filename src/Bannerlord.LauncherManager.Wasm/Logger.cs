using Microsoft.Extensions.Logging;

using System;
using System.Runtime.CompilerServices;

namespace Bannerlord.LauncherManager.Native;

/// <summary>
/// The logging API the shared native sources use, forwarded to the log callback JS registers.
/// Only errors are logged, like the native Release build.
/// </summary>
internal static class Logger
{
    private static Action<int, string>? _log;

    public static void SetCallback(Action<int, string>? log) => _log = log;

    public static void ExternalLog(LogLevel level, string message)
    {
        if (_log is not { } log) return;
        try
        {
            log((int) level, message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public static LoggerScope LogMethod([CallerMemberName] string? caller = null) => new(caller);

    public readonly struct LoggerScope : IDisposable
    {
        private readonly string? _caller;

        public LoggerScope(string? caller) => _caller = caller;

        public void LogException(Exception e) => ExternalLog(LogLevel.Error, $"[C# ] {_caller} - Exception{Environment.NewLine}{e}");

        public void Dispose() { }
    }
}
