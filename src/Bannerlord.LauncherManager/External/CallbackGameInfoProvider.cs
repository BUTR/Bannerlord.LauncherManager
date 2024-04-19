using System;

namespace Bannerlord.LauncherManager.External;

public sealed class CallbackGameInfoProvider : IGameInfoProvider
{
    private readonly Func<string> _getInstallPath;

    public CallbackGameInfoProvider(Func<string> getInstallPath)
    {
        _getInstallPath = getInstallPath;
    }

    public string GetInstallPath() => _getInstallPath();
}