using BUTR.NativeAOT.Shared;

using System;
using System.Globalization;

namespace Bannerlord.LauncherManager.Native;

static partial class Logger
{
    // Ref means that it will never leave the stack
    internal readonly ref struct LoggerScope
    {
        private readonly string? _caller;

        public LoggerScope(string? caller)
        {
            _caller = caller;
            
#if DEBUG
            NativeInstance?.LogStarted(_caller);
#endif
        }

        public LoggerScope WithInput(string p1)
        {
#if DEBUG
            NativeInstance?.LogParameters1(_caller, p1);
#endif
            return this;
        }
        public LoggerScope WithInput(string p1, string p2)
        {
#if DEBUG
            NativeInstance?.LogParameters2(_caller, p1, p2);
#endif
            return this;
        }
        public LoggerScope WithInput(string p1, string p2, string p3)
        {
#if DEBUG
            NativeInstance?.LogParameters3(_caller, p1, p2, p3);
#endif
            return this;
        }
        public LoggerScope WithInput(string p1, string p2, string p3, string p4)
        {
#if DEBUG
            NativeInstance?.LogParameters4(_caller, p1, p2, p3, p4);
#endif  
            return this;
        }
        public LoggerScope WithInput(string p1, string p2, string p3, string p4, string p5)
        {
#if DEBUG
            NativeInstance?.LogParameters5(_caller, p1, p2, p3, p4, p5);
#endif
            return this;
        }
        public LoggerScope WithInput(string p1, string p2, string p3, string p4, string p5, string p6)
        {
#if DEBUG
            NativeInstance?.LogParameters6(_caller, p1, p2, p3, p4, p5, p6);
#endif
            return this;
        }
        public unsafe LoggerScope WithResult<TStruct>(TStruct* result)
            where TStruct : unmanaged, IReturnValueSpanFormattable<TStruct>
        {
#if DEBUG
            NativeInstance?.LogResult1(_caller, TStruct.ToSpan(result).ToString());
#endif
            return this;
        }

        public void LogException(Exception exception)
        {
            NativeInstance?.LogException(_caller, exception);
        }

        public void Log(string message)
        {
            NativeInstance?.LogMessage(_caller, message);
        }

        public unsafe void LogResult<TStruct>(SafeStructMallocHandle<TStruct> result)
            where TStruct : unmanaged, IReturnValueSpanFormattable<TStruct>
        {
            
#if DEBUG
            NativeInstance?.LogResult1(_caller, TStruct.ToSpan(result.Value).ToString());
#endif
        }

        public void LogResult<TResult>(TResult result, string? format = null)
            where TResult : IFormattable
        {
#if DEBUG
            NativeInstance?.LogResult1(_caller, result.ToString(format, CultureInfo.InvariantCulture));
#endif
        }

        public void Dispose()
        {
#if DEBUG
            NativeInstance?.LogFinished(_caller);
#endif
        }
    }
}