using BUTR.NativeAOT.Shared;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static Bannerlord.VortexExtension.Native.Tests.Utils2;

namespace Bannerlord.VortexExtension.Native.Tests;

public partial class FetchVersionTests
{
    private const string DllPath = "../../../../../src/Bannerlord.VortexExtension.Native/bin/Release/net7.0/win-x64/native/Bannerlord.VortexExtension.Native.dll";

    [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe partial return_value_uint32* bfv_get_change_set(param_string* p_game_folder_path, param_string* p_lib_assembly);

    [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe partial return_value_string* bfv_get_version(param_string* p_game_folder_path, param_string* p_lib_assembly);

    [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe partial return_value_uint32* bfv_get_version_type(param_string* p_game_folder_path, param_string* p_lib_assembly);

    [Test]
    public unsafe void Test_Main()
    {
        var path = Path.GetFullPath("./Data");
        var dllName = "TaleWorlds.Library.dll";

        var (changeSetError, changeSet) = GetResult(bfv_get_change_set((param_string*) Copy(path), (param_string*) Copy(dllName)));
        Assert.That(changeSetError, Is.Empty);
        Assert.That(changeSet, Is.EqualTo(321460));

        var (versionError, version) = GetResult(bfv_get_version((param_string*) Copy(path), (param_string*) Copy(dllName)));
        Assert.That(versionError, Is.Empty);
        Assert.That(version, Is.EqualTo("e1.8.0"));

        var (versionTypeError, versionType) = GetResult(bfv_get_version_type((param_string*) Copy(path), (param_string*) Copy(dllName)));
        Assert.That(versionTypeError, Is.Empty);
        Assert.That(versionType, Is.EqualTo(4));
    }
}