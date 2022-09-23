using System.Collections.Generic;

namespace Bannerlord.VortexExtension.Models
{
    //public sealed class LoadOrder : List<LoadOrderEntry>
    public sealed class LoadOrder : Dictionary<string, LoadOrderEntry>
    {
        public LoadOrder() { }
        public LoadOrder(IDictionary<string, LoadOrderEntry> dict): base(dict) { }

        public override string ToString() => string.Join(", ", this);
    }
}