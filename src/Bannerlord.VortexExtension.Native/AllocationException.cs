using System;

namespace Bannerlord.VortexExtension.Native
{
    public class AllocationException : Exception
    {
        public AllocationException(string message) : base(message) { }
    }
}