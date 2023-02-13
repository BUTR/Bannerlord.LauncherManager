using System;

namespace Bannerlord.LauncherManager;

public sealed class CallbacksNotRegisteredException : Exception
{
    public CallbacksNotRegisteredException() : base("You need to call register callbacks first!") { }
}