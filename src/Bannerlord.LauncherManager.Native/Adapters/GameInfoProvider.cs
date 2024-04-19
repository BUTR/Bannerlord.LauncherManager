using Bannerlord.LauncherManager.External;

using BUTR.NativeAOT.Shared;

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

    public string GetInstallPath() => GetInstallPathNative();

    private string GetInstallPathNative()
    {
        Logger.LogInput();

        using var result = SafeStructMallocHandle.Create(_getInstallPath(_pOwner), true);
        using var installPath = result.ValueAsString();

        var returnResult = new string(installPath);
        Logger.LogOutput(returnResult, nameof(GetInstallPath));
        return returnResult;
    }
}