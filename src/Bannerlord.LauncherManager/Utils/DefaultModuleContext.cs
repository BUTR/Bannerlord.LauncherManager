﻿using Bannerlord.LauncherManager.Models;
using Bannerlord.ModuleManager;

using System.Collections.Generic;
using System.Linq;

namespace Bannerlord.LauncherManager.Utils;

public class DefaultModuleContext<TModuleViewModel> where TModuleViewModel : class, IModuleViewModel
{
    private readonly IDictionary<string, TModuleViewModel> _lookup;
    public DefaultModuleContext(IEnumerable<TModuleViewModel> moduleVMs)
    {
        _lookup = moduleVMs.ToDictionary(x => x.ModuleInfoExtended.Id, x => x);
    }
    public DefaultModuleContext(IDictionary<string, TModuleViewModel> lookup)
    {
        _lookup = lookup;
    }

    public bool GetIsValid(ModuleInfoExtended module)
    {
        if (FeatureIds.LauncherFeatures.Contains(module.Id))
            return true;

        return _lookup[module.Id].IsValid;
    }
    public bool GetIsSelected(ModuleInfoExtended module)
    {
        if (FeatureIds.LauncherFeatures.Contains(module.Id))
            return false;

        return _lookup[module.Id].IsSelected;
    }
    public void SetIsSelected(ModuleInfoExtended module, bool value)
    {
        if (FeatureIds.LauncherFeatures.Contains(module.Id))
            return;
        _lookup[module.Id].IsSelected = value;
    }
    public bool GetIsDisabled(ModuleInfoExtended module)
    {
        if (FeatureIds.LauncherFeatures.Contains(module.Id))
            return false;

        return _lookup[module.Id].IsDisabled;
    }
    public void SetIsDisabled(ModuleInfoExtended module, bool value)
    {
        if (FeatureIds.LauncherFeatures.Contains(module.Id))
            return;
        _lookup[module.Id].IsDisabled = value;
    }
}