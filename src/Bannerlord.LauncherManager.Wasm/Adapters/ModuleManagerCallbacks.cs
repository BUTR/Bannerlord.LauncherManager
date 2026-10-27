using System.Runtime.InteropServices.JavaScript;

namespace Bannerlord.LauncherManager.Wasm.Adapters;

/// <summary>
/// The validation and enable/disable managers of the ModuleManager call in progress, the TS wrapper answers with the
/// call's own. Imports and not function parameters: a JS function passed per call became a proxy the runtime tracks,
/// and that tracking broke under some GC timing ("ReleaseCSOwnedObject expected to find registration"), see
/// ModuleManagerImports in runtime.ts
/// </summary>
internal static partial class ModuleManagerCallbacks
{
    private const string Module = "modulemanager";

    [JSImport("isSelected", Module)]
    public static partial bool IsSelected(string moduleId);

    [JSImport("getSelected", Module)]
    public static partial bool GetSelected(string moduleId);

    [JSImport("setSelected", Module)]
    public static partial void SetSelected(string moduleId, bool value);

    [JSImport("getDisabled", Module)]
    public static partial bool GetDisabled(string moduleId);

    [JSImport("setDisabled", Module)]
    public static partial void SetDisabled(string moduleId, bool value);
}
