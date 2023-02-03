using BUTR.NativeAOT.Shared;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static Bannerlord.LauncherManager.Native.Tests.Utils2;

namespace Bannerlord.LauncherManager.Native.Tests
{
    public sealed partial class FetchVersionTests : BaseTests
    {
        [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static unsafe partial return_value_uint32* bfv_get_change_set(param_string* p_game_folder_path, param_string* p_lib_assembly);

        [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static unsafe partial return_value_string* bfv_get_version(param_string* p_game_folder_path, param_string* p_lib_assembly);

        [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static unsafe partial return_value_uint32* bfv_get_version_type(param_string* p_game_folder_path, param_string* p_lib_assembly);

        [Test]
        public unsafe void Test_Main()
        {
            Assert.DoesNotThrow(() =>
            {
                using var path = Utils.Copy(Path.GetFullPath("./Data"), true);
                using var dllName = Utils.Copy("TaleWorlds.Library.dll", true);

                var changeSet = GetResult(bfv_get_change_set(path, dllName));
                Assert.That(changeSet, Is.EqualTo(321460));

                var version = GetResult(bfv_get_version(path, dllName));
                Assert.That(version, Is.EqualTo("e1.8.0"));

                var versionType = GetResult(bfv_get_version_type(path, dllName));
                Assert.That(versionType, Is.EqualTo(4));
            });

#if DEBUG
            Assert.That(LibraryAliveCount(), Is.EqualTo(0));   
#endif
        }
    }
}