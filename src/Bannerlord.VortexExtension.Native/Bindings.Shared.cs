using BUTR.NativeAOT.Shared;

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bannerlord.VortexExtension.Native
{
    public static unsafe partial class Bindings
    {
        [UnmanagedCallersOnly(EntryPoint = "alloc", CallConvs = new[] { typeof(CallConvCdecl) }), IsNotConst<IsPtrConst>]
        public static void* Alloc([IsConst<IsPtrConst>] nuint size)
        {
            Logger.LogInput(size);
            try
            {
                var result = Allocator.Alloc(size);

                Logger.LogOutput(new IntPtr(result).ToString("x16"), nameof(Alloc));
                return result;
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return null;
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "dealloc", CallConvs = new[] { typeof(CallConvCdecl) }), IsNotConst<IsPtrConst>]
        public static void Dealloc([IsConst<IsPtrConst>] param_ptr* ptr)
        {
            Logger.LogInput(new IntPtr(ptr).ToString("x16"), nameof(Dealloc));
            try
            {
                Allocator.Free(ptr);

                Logger.LogOutput();
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "alloc_alive_count", CallConvs = new[] { typeof(CallConvCdecl) }), IsNotConst<IsPtrConst>]
        public static int AllocAliveCount()
        {
            Logger.LogInput();
            try
            {
#if TRACK_ALLOCATIONS
                var result = Allocator.GetCurrentAllocations();
#else
                var result = 0;
#endif

                Logger.LogOutput(result);
                return result;
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return -1;
            }
        }
    }
}