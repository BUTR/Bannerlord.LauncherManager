using System;

namespace Bannerlord.LauncherManager.Native;

internal sealed class CallbackStorage
{
    private readonly Action<object> _action;
    public CallbackStorage(Action<object> action) => _action = action;
    public void SetResult(object result) => _action(result);
}