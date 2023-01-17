using System;

namespace Bannerlord.VortexExtension.Native
{
    public class NativeCallException : Exception
    {
        public NativeCallException(string message) : base(message) { }
    }
}