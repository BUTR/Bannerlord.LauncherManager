using Bannerlord.LauncherManager.Models;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Bannerlord.LauncherManager.Native.Models;

public sealed record ModuleViewModel : IModuleViewModel
{
    public required ModuleInfoExtendedWithPath ModuleInfoExtended { get; init; }
    public required bool IsValid { get; init; }
    public bool IsSelected { get; set; }
    public bool IsDisabled { get; set; }
    public int Index { get; set; }

    public ModuleViewModel() { }

    [SetsRequiredMembers, JsonConstructor]
    public ModuleViewModel(ModuleInfoExtendedWithPath moduleInfoExtended, bool isValid)
    {
        ModuleInfoExtended = moduleInfoExtended;
        IsValid = isValid;
    }
}