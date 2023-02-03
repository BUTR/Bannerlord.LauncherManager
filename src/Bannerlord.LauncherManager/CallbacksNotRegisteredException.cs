using System;

namespace Bannerlord.VortexExtension
{
    public sealed class CallbacksNotRegisteredException : Exception
    {
        public CallbacksNotRegisteredException() : base("You need to call register callbacks first!") { }
    }
}