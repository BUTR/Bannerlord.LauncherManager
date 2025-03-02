using System;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.External;

public sealed class CallbackGameInfoProvider : IGameInfoProvider
{
    private readonly Action<Action<string>> _getInstallPath;

    public CallbackGameInfoProvider(Action<Action<string>> getInstallPath)
    {
        _getInstallPath = getInstallPath;
    }

    public async Task<string> GetInstallPathAsync()
    {
        var tcs = new TaskCompletionSource<string>();
        try
        {
            _getInstallPath(result => tcs.TrySetResult(result));
        }
        catch (Exception ex)
        {
            tcs.TrySetException(ex);
        }
        return await tcs.Task;
    }
}