using Microsoft.Win32.SafeHandles;

using System;
using System.Runtime.InteropServices;

namespace Bannerlord.VortexExtension.Native
{
    internal unsafe class SafeStringMallocHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public static implicit operator ReadOnlySpan<char>(SafeStringMallocHandle handle) =>
            MemoryMarshal.CreateReadOnlySpanFromNullTerminated((char*) handle.handle.ToPointer());

        public SafeStringMallocHandle(): base(true) { }
        public SafeStringMallocHandle(char* ptr): base(true)
        {
            handle = new IntPtr(ptr);
            var b = false;
            DangerousAddRef(ref b);
        }

        protected override bool ReleaseHandle()
        {
            if (handle != IntPtr.Zero)
                NativeMemory.Free(handle.ToPointer());
            return true;
        }
    }
}