using BUTR.NativeAOT.Shared;

using System.Runtime.CompilerServices;

using static Bannerlord.VortexExtension.Native.Tests.Utils2;

namespace Bannerlord.VortexExtension.Native.Tests;

internal class ValidationManager
{
    public static unsafe return_value_bool* IsSelected(void* owner, param_string* moduleId)
    {
        var manager = Unsafe.AsRef<ValidationManager>(owner);
        return return_value_bool.AsValue(manager.IsSelected(ToSpan(moduleId).ToString()));
    }

    public bool IsSelected(string moduleId)
    {
        return true;
    }
}