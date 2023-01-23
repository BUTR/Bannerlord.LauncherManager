using Bannerlord.ModuleManager;
using Bannerlord.VortexExtension.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Bannerlord.VortexExtension
{
    public partial class VortexExtensionHandler
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