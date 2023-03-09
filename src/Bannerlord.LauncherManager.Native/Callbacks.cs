using BUTR.NativeAOT.Shared;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: InternalsVisibleTo("Bannerlord.LauncherManager.Native.Tests")]

namespace Bannerlord.LauncherManager.Native;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_void* N_SetGameParametersDelegate(param_ptr* p_owner, param_string* p_executable, param_json* p_game_parameters);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_json* N_GetLoadOrderDelegate(param_ptr* p_owner);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_void* N_SetLoadOrderDelegate(param_ptr* p_owner, param_json* p_load_order);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_void* N_SendNotificationDelegate(param_ptr* p_owner, param_string* p_id, param_string* p_type, param_string* p_message, param_uint displayMs);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_void* N_SendDialogDelegate(param_ptr* p_owner, param_string* p_type, param_string* p_title, param_string* p_message, param_json* p_filters, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, param_string*, void> p_callback);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_string* N_GetInstallPathDelegate(param_ptr* p_owner);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_data* N_ReadFileContentDelegate(param_ptr* p_owner, param_string* p_file_path, param_int offset, param_int length);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_void* N_WriteFileContentDelegate(param_ptr* p_owner, param_string* p_file_path, param_data* data, param_int length);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_json* N_ReadDirectoryFileList(param_ptr* p_owner, param_string* p_directory_path);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_json* N_ReadDirectoryList(param_ptr* p_owner, param_string* p_directory_path);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_json* N_GetAllModuleViewModels(param_ptr* p_owner);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_json* N_GetModuleViewModels(param_ptr* p_owner);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_void* N_SetModuleViewModels(param_ptr* p_owner, param_json* p_module_view_model);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_json* N_GetOptions(param_ptr* p_owner);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_json* N_GetState(param_ptr* p_owner);