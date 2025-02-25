using Bannerlord.LauncherManager.Models;

using BUTR.NativeAOT.Shared;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static Bannerlord.LauncherManager.Native.Tests.Utils2;

namespace Bannerlord.LauncherManager.Native.Tests;

public sealed unsafe class LauncherManagerWrapper
{
    private static byte* Copy(in ReadOnlySpan<byte> data, bool isOwner)
    {
        var dst = (byte*) Allocator.Alloc(new UIntPtr((uint) data.Length));
        data.CopyTo(new Span<byte>(dst, data.Length));
        return dst;
    }
    
    public static return_value_void* SetGameParameters(param_ptr* handler, param_string* executable, param_json* gameParameters, param_ptr* p_callback_handler, delegate* <param_ptr*, return_value_void*, void> p_callback)
    {
        return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Unsupported", false), false);
    }

    public static return_value_void* SendNotification(param_ptr* handler, param_string* id, param_string* type, param_string* message, param_uint displayMs, param_ptr* p_callback_handler, delegate* <param_ptr*, return_value_void*, void> p_callback)
    {
        return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Unsupported", false), false);
    }

    public static return_value_void* SendDialog(param_ptr* handler, param_string* type, param_string* title, param_string* message, param_json* filters, param_ptr* p_callback_handler, delegate* <param_ptr*, return_value_string*, void> p_callback)
    {
        return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Unsupported", false), false);
    }

    public static return_value_void* GetInstallPath(param_ptr* handler, param_ptr* p_callback_handler, delegate* <param_ptr*, return_value_string*, void> p_callback)
    {
        var installPath = Path.GetFullPath("./Data");
        p_callback(p_callback_handler, return_value_string.AsValue(BUTR.NativeAOT.Shared.Utils.Copy(installPath, false), false));
        return return_value_void.AsValue(false);
    }

    public static return_value_void* ReadFileContent(param_ptr* handler, param_string* pFilePath, param_int offset, param_int length, param_ptr* p_callback_handler, delegate* <param_ptr*, return_value_data*, void> p_callback)
    {
        var path = new string(param_string.ToSpan(pFilePath));

        if (!File.Exists(path))
        {
            p_callback(p_callback_handler, null);
            return return_value_void.AsValue(false);
        }

        if (offset == 0 && length == -1)
        {
            var data = File.ReadAllBytes(path);
            p_callback(p_callback_handler, return_value_data.AsValue(Copy(data, false), data.Length, false));
            return return_value_void.AsValue(false);
        }
        
        if (offset >= 0 && length > 0)
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var data = new byte[length];
            fs.Seek(offset, SeekOrigin.Begin);
            fs.ReadExactly(data, 0, length);
            p_callback(p_callback_handler, return_value_data.AsValue(Copy(data, false), data.Length, false));
            return return_value_void.AsValue(false);
        }

        p_callback(p_callback_handler, return_value_data.AsValue(null, 0, false));
        return return_value_void.AsValue(false);
    }

    public static return_value_void* WriteFileContent(param_ptr* handler, param_string* filePath, param_data* data, param_int length, param_ptr* p_callback_handler, delegate* <param_ptr*, return_value_void*, void> p_callback)
    {
        return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Unsupported", false), false);
    }

    public static return_value_void* ReadDirectoryFileList(param_ptr* handler, param_string* pDirectoryPath, param_ptr* p_callback_handler, delegate* <param_ptr*, return_value_json*, void> p_callback)
    {
        var directoryPath = new string(param_string.ToSpan(pDirectoryPath));
        var data = Directory.Exists(directoryPath) ? Directory.GetFiles(directoryPath) : null;
        p_callback(p_callback_handler, data is null ? return_value_json.AsValue(null, false) : return_value_json.AsValue(data, CustomSourceGenerationContext.StringArray, false));
        return return_value_void.AsValue(false);
    }

    public static return_value_void* ReadDirectoryList(param_ptr* handler, param_string* pDirectoryPath, param_ptr* p_callback_handler, delegate* <param_ptr*, return_value_json*, void> p_callback)
    {
        var directoryPath = new string(param_string.ToSpan(pDirectoryPath));
        var data = Directory.Exists(directoryPath) ? Directory.GetDirectories(directoryPath) : null;
        p_callback(p_callback_handler, data is null ? return_value_json.AsValue(null, false) : return_value_json.AsValue(data, CustomSourceGenerationContext.StringArray, false));
        return return_value_void.AsValue(false);
    }

    public static return_value_void* GetAllModuleViewModels(param_ptr* handler, param_ptr* p_callback_handler, delegate* <param_ptr*, return_value_json*, void> p_callback)
    {
        return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Unsupported", false), false);
    }

    public static return_value_void* GetModuleViewModels(param_ptr* handler, param_ptr* p_callback_handler, delegate* <param_ptr*, return_value_json*, void> p_callback)
    {
        return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Unsupported", false), false);
    }

    public static return_value_void* SetModuleViewModels(param_ptr* handler, param_json* moduleViewModels, param_ptr* p_callback_handler, delegate* <param_ptr*, return_value_void*, void> p_callback)
    {
        return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Unsupported", false), false);
    }

    public static return_value_void* GetOptions(param_ptr* handler, param_ptr* p_callback_handler, delegate* <param_ptr*, return_value_json*, void> p_callback)
    {
        return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Unsupported", false), false);
    }

    public static return_value_void* GetState(param_ptr* handler, param_ptr* p_callback_handler, delegate* <param_ptr*, return_value_json*, void> p_callback)
    {
        return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Unsupported", false), false);
    }
}

public sealed partial class LauncherManagerTests : BaseTests
{
    [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    private static unsafe partial return_value_ptr* ve_create_handler(param_ptr* p_owner,
        delegate* <param_ptr*, param_string*, param_json*, param_ptr*, delegate* <param_ptr*, return_value_void*, void>, return_value_void*> p_set_game_parameters,
        delegate* <param_ptr*, param_string*, param_string*, param_string*, param_uint, param_ptr*, delegate* <param_ptr*, return_value_void*, void>, return_value_void*> p_send_notification,
        delegate* <param_ptr*, param_string*, param_string*, param_string*, param_json*, param_ptr*, delegate* <param_ptr*, return_value_string*, void>, return_value_void*> p_send_dialog,
        delegate* <param_ptr*, param_ptr*, delegate* <param_ptr*, return_value_string*, void>, return_value_void*> p_get_install_path,
        delegate* <param_ptr*, param_string*, param_int, param_int, param_ptr*, delegate* <param_ptr*, return_value_data*, void>, return_value_void*> p_read_file_content,
        delegate* <param_ptr*, param_string*, param_data*, param_int, param_ptr*, delegate* <param_ptr*, return_value_void*, void>, return_value_void*> p_write_file_content,
        delegate* <param_ptr*, param_string*, param_ptr*, delegate* <param_ptr*, return_value_json*, void>, return_value_void*> p_read_directory_file_list,
        delegate* <param_ptr*, param_string*, param_ptr*, delegate* <param_ptr*, return_value_json*, void>, return_value_void*> p_read_directory_list,
        delegate* <param_ptr*, param_ptr*, delegate* <param_ptr*, return_value_json*, void>, return_value_void*> p_get_all_module_view_models,
        delegate* <param_ptr*, param_ptr*, delegate* <param_ptr*, return_value_json*, void>, return_value_void*> p_get_module_view_models,
        delegate* <param_ptr*, param_json*, param_ptr*, delegate* <param_ptr*, return_value_void*, void>, return_value_void*> p_set_module_view_models,
        delegate* <param_ptr*, param_ptr*, delegate* <param_ptr*, return_value_json*, void>, return_value_void*> p_get_options,
        delegate* <param_ptr*, param_ptr*, delegate* <param_ptr*, return_value_json*, void>, return_value_void*> p_get_state);

    [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    private static unsafe partial return_value_json* ve_install_module(param_ptr* p_handle, param_json* p_files, param_json* p_module_infos);

    [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public static unsafe partial return_value_async* ve_get_save_files_async(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void> p_callback);

    [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public static unsafe partial return_value_async* ve_get_save_metadata_async(param_ptr* p_handle, param_string* p_save_file, param_data* p_data, param_int data_len, param_ptr* p_callback_handler, delegate* <param_ptr*, return_value_json*, void> p_callback);

    [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public static unsafe partial return_value_async* ve_get_save_file_path_async(param_ptr* p_handle, param_string* p_save_file, param_ptr* p_callback_handler, delegate* <param_ptr*, return_value_string*, void> p_callback);

    [Test]
    public unsafe void Test_Main()
    {
        Assert.DoesNotThrow(() =>
        {
            using var files = ToJson(new[]
            {
                "Modules\\",
                "Modules\\Bannerlord.Harmony\\",
                "Modules\\Bannerlord.Harmony\\ModuleData\\",
                "Modules\\Bannerlord.Harmony\\SubModule.xml",
                "Modules\\Bannerlord.Harmony\\bin\\",
                "Modules\\Bannerlord.Harmony\\ModuleData\\Languages\\",
                "Modules\\Bannerlord.Harmony\\ModuleData\\Languages\\EN\\",
                "Modules\\Bannerlord.Harmony\\ModuleData\\Languages\\RU\\",
                "Modules\\Bannerlord.Harmony\\ModuleData\\Languages\\EN\\language_data.xml",
                "Modules\\Bannerlord.Harmony\\ModuleData\\Languages\\EN\\sta_strings.xml",
                "Modules\\Bannerlord.Harmony\\ModuleData\\Languages\\RU\\language_data.xml",
                "Modules\\Bannerlord.Harmony\\ModuleData\\Languages\\RU\\sta_strings.xml",
                "Modules\\Bannerlord.Harmony\\bin\\Win64_Shipping_Client\\",
                "Modules\\Bannerlord.Harmony\\bin\\Win64_Shipping_Client\\0Harmony.dll",
                "Modules\\Bannerlord.Harmony\\bin\\Win64_Shipping_Client\\Bannerlord.Harmony.dll",
                "Modules\\Bannerlord.Harmony\\bin\\Win64_Shipping_Client\\Bannerlord.Harmony.pdb",
            });
            using var modules = ToJson(new ModuleInfoExtendedWithMetadata[]
            {
                new()
                {
                    Id = "Bannerlord.Harmony",
                    ModuleProviderType = ModuleProviderType.Default,
                    Path = "Modules\\Bannerlord.Harmony\\SubModule.xml"
                }
            });
            var launcherManagerWrapper = new LauncherManagerWrapper();
            var launcherManagerWrapperHandle = GCHandle.Alloc(launcherManagerWrapper);
            var handle = GCHandle.ToIntPtr(launcherManagerWrapperHandle);
            var launcherManagerPtr = GetResult(ve_create_handler((param_ptr*) handle.ToPointer(),
                p_set_game_parameters: &LauncherManagerWrapper.SetGameParameters,
                p_send_notification: &LauncherManagerWrapper.SendNotification,
                p_send_dialog: &LauncherManagerWrapper.SendDialog,
                p_get_install_path: &LauncherManagerWrapper.GetInstallPath,
                p_read_file_content: &LauncherManagerWrapper.ReadFileContent,
                p_write_file_content: &LauncherManagerWrapper.WriteFileContent,
                p_read_directory_file_list: &LauncherManagerWrapper.ReadDirectoryFileList,
                p_read_directory_list: &LauncherManagerWrapper.ReadDirectoryList,
                p_get_all_module_view_models: &LauncherManagerWrapper.GetAllModuleViewModels,
                p_get_module_view_models: &LauncherManagerWrapper.GetModuleViewModels,
                p_set_module_view_models: &LauncherManagerWrapper.SetModuleViewModels,
                p_get_options: &LauncherManagerWrapper.GetOptions,
                p_get_state: &LauncherManagerWrapper.GetState));

            var result = GetResult<InstallResult>(ve_install_module((param_ptr*) launcherManagerPtr, (param_json*) files, (param_json*) modules));

            var tcs = new TaskCompletionSource<SaveMetadata[]?>();
            GetResult(ve_get_save_files_async((param_ptr*) launcherManagerPtr, (param_ptr*) GCHandle.ToIntPtr(GCHandle.Alloc(tcs, GCHandleType.Normal)), &Test));
            tcs.Task.Wait(TimeSpan.FromSeconds(5));
            ;
        });

#if DEBUG
        Assert.That(LibraryAliveCount(), Is.EqualTo(0));
#endif
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void Test(param_ptr* p, return_value_json* j)
    {
        var tcs = (TaskCompletionSource<SaveMetadata[]?>) GCHandle.FromIntPtr((IntPtr) p).Target!;
        GetResult(j, tcs);
    }
}