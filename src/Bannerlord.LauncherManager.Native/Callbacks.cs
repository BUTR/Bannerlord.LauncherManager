using BUTR.NativeAOT.Shared;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: InternalsVisibleTo("Bannerlord.LauncherManager.Native.Tests")]

namespace Bannerlord.LauncherManager.Native;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_void* N_SetGameParametersDelegate(param_ptr* p_owner, param_string* p_executable, param_json* p_game_parameters, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void> p_callback);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_void* N_SendNotificationDelegate(param_ptr* p_owner, param_string* p_id, param_string* p_type, param_string* p_message, param_uint displayMs, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void> p_callback);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_void* N_SendDialogDelegate(param_ptr* p_owner, param_string* p_type, param_string* p_title, param_string* p_message, param_json* p_filters, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_string*, void> p_callback);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_void* N_GetInstallPathDelegate(param_ptr* p_owner, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_string*, void> p_callback);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_void* N_ReadFileContentDelegate(param_ptr* p_owner, param_string* p_file_path, param_int offset, param_int length, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_data*, void> p_callback);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_void* N_WriteFileContentDelegate(param_ptr* p_owner, param_string* p_file_path, param_data* p_data, param_int length, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void> p_callback);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_void* N_ReadDirectoryFileList(param_ptr* p_owner, param_string* p_directory_path, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void> p_callback);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_void* N_ReadDirectoryList(param_ptr* p_owner, param_string* p_directory_path, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void> p_callback);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_void* N_GetAllModuleViewModels(param_ptr* p_owner, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void> p_callback);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_void* N_GetModuleViewModels(param_ptr* p_owner, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void> p_callback);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_void* N_SetModuleViewModels(param_ptr* p_owner, param_json* p_module_view_models, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void> p_callback);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_void* N_GetOptions(param_ptr* p_owner, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void> p_callback);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate return_value_void* N_GetState(param_ptr* p_owner, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void> p_callback);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate param_int N_Log(param_ptr* p_owner,
    param_int level,
    param_string* p_messsage);