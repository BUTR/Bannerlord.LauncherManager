using System;

namespace Bannerlord.LauncherManager;

public sealed class LauncherManagerInitializedTwiceException : Exception
{
    public LauncherManagerInitializedTwiceException() : base("Second call to Initialize!") { }
}