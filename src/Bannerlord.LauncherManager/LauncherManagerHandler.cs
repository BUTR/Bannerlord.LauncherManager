﻿using Bannerlord.ModuleManager;
using Bannerlord.LauncherManager.Models;

using System.Collections.Generic;
using System.Linq;

namespace Bannerlord.LauncherManager
{
    public partial class LauncherManagerHandler
    {
        public IReadOnlyCollection<ModuleInfoExtended> GetFromLoadOrder(LoadOrder loadOrder)
        {
            var ids = loadOrder.Select(x => x.Key).ToHashSet();
            return GetModules().Where(x => ids.Contains(x.Id)).ToList();
        }

        public LoadOrder GetFromModules(IEnumerable<ModuleInfoExtended> modules)
        {
            return new LoadOrder(modules.Select((x, i) => new
            {
                Module = x,
                LoadOrder = new LoadOrderEntry
                {
                    Pos = i + 1,
                    Enabled = true,
                    Name = x.Name,
                    //Locked = LockedState.@false,
                }
            }).ToDictionary(x => x.Module.Id, x => x.LoadOrder));
        }
    }
}