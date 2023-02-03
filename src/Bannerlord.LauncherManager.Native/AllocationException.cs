using System;

namespace Bannerlord.LauncherManager.Native
{
    public class AllocationException : Exception
    {
        public AllocationException(string message) : base(message) { }
    }
}