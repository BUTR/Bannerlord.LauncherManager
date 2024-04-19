namespace Bannerlord.LauncherManager.Models;

public interface IModuleViewModel
{
    ModuleInfoExtendedWithMetadata ModuleInfoExtended { get; }
    bool IsValid { get; }
    bool IsSelected { get; set; }
    bool IsDisabled { get; set; }
    int Index { get; set; }
}