using Bannerlord.LauncherManager.Models;

using BUTR.NativeAOT.Shared;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static Bannerlord.LauncherManager.Native.Tests.Utils2;

namespace Bannerlord.LauncherManager.Native.Tests
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct param_data : IParameter<param_data>, IParameterSpanFormattable<param_data>, IParameterRawPtr<param_data, byte>, IParameterIntPtr<param_data>
    {
        public static implicit operator param_data*(param_data value) => &value;
        public static implicit operator byte*(param_data ptr) => (byte*) &ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> ToSpan(param_data* ptr) => new IntPtr(ptr).ToString();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<byte> ToSpan(param_data* ptr, int length) => new(ptr, length);
        static IntPtr IParameterIntPtr<param_data>.ToPtr(param_data* ptr) => new(ptr);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte* ToRawPtr(param_data* ptr) => (byte*) ptr;

        public readonly byte* Value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct return_value_data : IReturnValueWithError<return_value_data>, IReturnValueWithException<return_value_data>
    {
        public static return_value_data* AsValue(byte* value, int length, bool isOwner) => BUTR.NativeAOT.Shared.Utils.Create(new return_value_data(value, length, null), isOwner);
        public static return_value_data* AsError(char* error, bool isOwner) => BUTR.NativeAOT.Shared.Utils.Create(new return_value_data(null, 0, error), isOwner);
        public static return_value_data* AsException(Exception e, bool isOwner) => BUTR.NativeAOT.Shared.Utils.AsException<return_value_data>(e, isOwner);

        public readonly char* Error;
        public readonly byte* Value;
        public readonly int Length;

        private return_value_data(byte* value, int length, char* error)
        {
            Value = value;
            Length = length;
            Error = error;
        }
    }

    public sealed unsafe class LauncherManagerWrapper
    {
        public static unsafe byte* Copy(in ReadOnlySpan<byte> data, bool isOwner)
        {
            var dst = (byte*) Allocator.Alloc(new UIntPtr((uint) data.Length));
            data.CopyTo(new Span<byte>(dst, data.Length));
            return dst;
        }

        public static return_value_void* SetGameParameters(param_ptr* handler, param_string* executable, param_json* gameParameters)
        {
            return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Unsupported", false), false);
        }

        public static return_value_json* LoadLoadOrder(param_ptr* handler)
        {
            return return_value_json.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Unsupported", false), false);
        }

        public static return_value_void* SaveLoadOrder(param_ptr* handler, param_json* loadOrder)
        {
            return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Unsupported", false), false);
        }

        public static return_value_void* SendNotification(param_ptr* handler, param_string* id, param_string* type, param_string* message, param_uint displayMs)
        {
            return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Unsupported", false), false);
        }

        public static return_value_void* SendDialog(param_ptr* handler, param_string* type, param_string* title, param_string* message, param_json* filters, param_ptr* callbackOwner, delegate*<param_ptr*, param_string*, void> onResult)
        {
            return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Unsupported", false), false);
        }

        public static return_value_string* GetInstallPath(param_ptr* handler)
        {
            return return_value_string.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Unsupported", false), false);
        }

        public static return_value_data* ReadFileContent(param_ptr* handler, param_string* pFilePath)
        {
            var filePath = new string(param_string.ToSpan(pFilePath));

            var handle = GCHandle.FromIntPtr(new IntPtr(handler)).Target as LauncherManagerWrapper;
            var data = File.Exists(filePath) ? File.ReadAllBytes(filePath) : null;
            return return_value_data.AsValue(Copy(data, false), data?.Length ?? 0, false);


            return return_value_data.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Unsupported", false), false);
        }

        public static return_value_void* WriteFileContent(param_ptr* handler, param_string* filePath, param_data* data, param_int length)
        {
            return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Unsupported", false), false);
        }

        public static return_value_json* ReadDirectoryFileList(param_ptr* handler, param_string* directoryPath)
        {
            return return_value_json.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Unsupported", false), false);
        }

        public static return_value_json* ReadDirectoryList(param_ptr* handler, param_string* directoryPath)
        {
            return return_value_json.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Unsupported", false), false);
        }

        public static return_value_json* GetModuleViewModels(param_ptr* handler)
        {
            return return_value_json.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Unsupported", false), false);
        }

        public static return_value_void* SetModuleViewModels(param_ptr* handler, param_json* moduleViewModels)
        {
            return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Unsupported", false), false);
        }

        public static return_value_json* GetOptions(param_ptr* handler)
        {
            return return_value_json.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Unsupported", false), false);
        }

        public static return_value_json* GetState(param_ptr* handler)
        {
            return return_value_json.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Unsupported", false), false);
        }
    }

    public sealed partial class LauncherManagerTests : BaseTests
    {
        [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static unsafe partial return_value_ptr* ve_create_handler(param_ptr* p_owner);

        [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static unsafe partial return_value_void* ve_register_callbacks(param_ptr* p_handle,
            delegate*<param_ptr*, param_string*, param_json*, return_value_void*> p_set_game_parameters,
            delegate*<param_ptr*, return_value_json*> p_load_load_order,
            delegate*<param_ptr*, param_json*, return_value_void*> p_save_load_order,
            delegate*<param_ptr*, param_string*, param_string*, param_string*, param_uint, return_value_void*> p_send_notification,
            delegate*<param_ptr*, param_string*, param_string*, param_string*, param_json*, param_ptr*, delegate*<param_ptr*, param_string*, void>, return_value_void*> p_send_dialog,
            delegate*<param_ptr*, return_value_string*> p_get_install_path,
            delegate*<param_ptr*, param_string*, return_value_data*> p_read_file_content,
            delegate*<param_ptr*, param_string*, param_data*, param_int, return_value_void*> p_write_file_content,
            delegate*<param_ptr*, param_string*, return_value_json*> p_read_directory_file_list,
            delegate*<param_ptr*, param_string*, return_value_json*> p_read_directory_list,
            delegate*<param_ptr*, return_value_json*> p_get_module_view_models,
            delegate*<param_ptr*, param_json*, return_value_void*> p_set_module_view_models,
            delegate*<param_ptr*, return_value_json*> p_get_options,
            delegate*<param_ptr*, return_value_json*> p_get_state);

        [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static unsafe partial return_value_json* ve_install_module(param_ptr* p_handle, param_json* p_files, param_string* p_destination_path);

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
                using var destinationPath = BUTR.NativeAOT.Shared.Utils.Copy("C:\\Users\\vitalii.mikhailov\\Downloads\\Harmony-2006-v2-2-2-140-1667140570", true);
                var launcherManagerWrapper = new LauncherManagerWrapper();
                var launcherManagerWrapperHandle = GCHandle.Alloc(launcherManagerWrapper);
                var handle = GCHandle.ToIntPtr(launcherManagerWrapperHandle);
                var launcherManagerPtr = GetResult(ve_create_handler((param_ptr*) handle.ToPointer()));
                GetResult(ve_register_callbacks((param_ptr*) launcherManagerPtr,
                    p_set_game_parameters: &LauncherManagerWrapper.SetGameParameters,
                    p_load_load_order: &LauncherManagerWrapper.LoadLoadOrder,
                    p_save_load_order: &LauncherManagerWrapper.SaveLoadOrder,
                    p_send_notification: &LauncherManagerWrapper.SendNotification,
                    p_send_dialog: &LauncherManagerWrapper.SendDialog,
                    p_get_install_path: &LauncherManagerWrapper.GetInstallPath,
                    p_read_file_content: &LauncherManagerWrapper.ReadFileContent,
                    p_write_file_content: &LauncherManagerWrapper.WriteFileContent,
                    p_read_directory_file_list: &LauncherManagerWrapper.ReadDirectoryFileList,
                    p_read_directory_list: &LauncherManagerWrapper.ReadDirectoryList,
                    p_get_module_view_models: &LauncherManagerWrapper.GetModuleViewModels,
                    p_set_module_view_models: &LauncherManagerWrapper.SetModuleViewModels,
                    p_get_options: &LauncherManagerWrapper.GetOptions,
                    p_get_state: &LauncherManagerWrapper.GetState));
                GetResult(ve_create_handler((param_ptr*) handle.ToPointer()));

                var result = GetResult<InstallResult>(ve_install_module((param_ptr*) launcherManagerPtr, (param_json*) files, (param_string*) destinationPath));

                ;
            });

#if DEBUG
            Assert.That(LibraryAliveCount(), Is.EqualTo(0));
#endif
        }
    }
}