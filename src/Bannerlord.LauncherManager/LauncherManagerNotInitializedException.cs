using System;

namespace Bannerlord.LauncherManager;

public sealed class LauncherManagerNotInitializedException : Exception
{
    public LauncherManagerNotInitializedException() : base("You need to call Initialize first!") { }
}