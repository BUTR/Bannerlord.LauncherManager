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
        private readonly bool _silent;

        public LoggerScope(string? caller) : this(caller, false) { }
        
        public LoggerScope(string? caller, bool silent)
        {
            _caller = caller;
            _silent = silent;
            
#if DEBUG
            _silent = false;
#endif
            
            if (!_silent)
            {
                NativeInstance?.LogStarted(_caller);
            }
        }

        public LoggerScope WithInput(string p1)
        {
            if (!_silent)
            {
                NativeInstance?.LogParameters1(_caller, p1);
            }
            return this;
        }
        public LoggerScope WithInput(string p1, string p2)
        {
            if (!_silent)
            {
                NativeInstance?.LogParameters2(_caller, p1, p2);
            }
            return this;
        }
        public LoggerScope WithInput(string p1, string p2, string p3)
        {
            if (!_silent)
            {
                NativeInstance?.LogParameters3(_caller, p1, p2, p3);
            }
            return this;
        }
        public LoggerScope WithInput(string p1, string p2, string p3, string p4)
        {
            if (!_silent)
            {
                NativeInstance?.LogParameters4(_caller, p1, p2, p3, p4);
            }
            return this;
        }
        public LoggerScope WithInput(string p1, string p2, string p3, string p4, string p5)
        {
            if (!_silent)
            {
                NativeInstance?.LogParameters5(_caller, p1, p2, p3, p4, p5);
            }
            return this;
        }
        public LoggerScope WithInput(string p1, string p2, string p3, string p4, string p5, string p6)
        {
            if (!_silent)
            {
                NativeInstance?.LogParameters6(_caller, p1, p2, p3, p4, p5, p6);
            }
            return this;
        }
        public unsafe LoggerScope WithResult<TStruct>(TStruct* result)
            where TStruct : unmanaged, IReturnValueSpanFormattable<TStruct>
        {
            if (!_silent)
            {
                NativeInstance?.LogResult1(_caller, TStruct.ToSpan(result).ToString());
            }
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
            if (!_silent)
            {
                NativeInstance?.LogResult1(_caller, TStruct.ToSpan(result.Value).ToString());
            }
        }

        public void LogResult<TResult>(TResult result, string? format = null)
            where TResult : IFormattable
        {
            if (!_silent)
            {
                NativeInstance?.LogResult1(_caller, result.ToString(format, CultureInfo.InvariantCulture));
            }
        }

        public void Dispose()
        {
            if (!_silent)
            {
                NativeInstance?.LogFinished(_caller);
            }
        }
    }
}