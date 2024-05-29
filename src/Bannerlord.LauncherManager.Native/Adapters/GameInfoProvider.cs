using Bannerlord.LauncherManager.External;

using BUTR.NativeAOT.Shared;

using System;

namespace Bannerlord.LauncherManager.Native.Adapters;

internal sealed unsafe class GameInfoProvider : IGameInfoProvider
{
    private readonly param_ptr* _pOwner;
    private readonly N_GetInstallPathDelegate _getInstallPath;

    public GameInfoProvider(param_ptr* pOwner, N_GetInstallPathDelegate getInstallPath)
    {
        _pOwner = pOwner;
        _getInstallPath = getInstallPath;
    }

    public string GetInstallPath() => new(GetInstallPathNative());

    private ReadOnlySpan<char> GetInstallPathNative()
    {
        Logger.LogInput();

        using var result = SafeStructMallocHandle.Create(_getInstallPath(_pOwner), true);
        using var installPath = result.ValueAsString();

        var returnResult = installPath.ToSpan();
        Logger.LogOutput(returnResult, nameof(GetInstallPath));
        return returnResult;
    }
}