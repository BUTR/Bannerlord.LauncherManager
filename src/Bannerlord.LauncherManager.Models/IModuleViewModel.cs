namespace Bannerlord.LauncherManager.Models;

public interface IModuleViewModel
{
    ModuleInfoExtendedWithPath ModuleInfoExtended { get; }
    bool IsValid { get; }
    bool IsSelected { get; set; }
    bool IsDisabled { get; set; }
    int Index { get; set; }
}