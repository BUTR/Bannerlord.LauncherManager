using BUTR.NativeAOT.Shared;

using System.Runtime.InteropServices;

namespace Bannerlord.VortexExtension.Native
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate return_value_json* N_GetActiveProfileDelegate(param_ptr* p_owner);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate return_value_json* N_GetProfileByIdDelegate(param_ptr* p_owner, param_string* p_profile_id);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate return_value_string* N_GetActiveGameIdDelegate(param_ptr* p_owner);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate return_value_void* N_SetGameParametersDelegate(param_ptr* p_owner, param_string* p_game_id, param_string* p_executable, param_json* p_game_parameters);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate return_value_json* N_GetLoadOrderDelegate(param_ptr* p_owner);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate return_value_void* N_SetLoadOrderDelegate(param_ptr* p_owner, param_json* p_load_order);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate return_value_string* N_TranslateStringDelegate(param_ptr* p_owner, param_string* p_text, param_string* p_ns);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate return_value_void* N_SendNotificationDelegate(param_ptr* p_owner, param_string* p_id, param_string* p_type, param_string* p_message, param_uint displayMs);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate return_value_string* N_GetInstallPathDelegate(param_ptr* p_owner);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate return_value_string* N_ReadFileContentDelegate(param_ptr* p_owner, param_string* p_file_path);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate return_value_json* N_ReadDirectoryFileList(param_ptr* p_owner, param_string* p_directory_path);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate return_value_json* N_ReadDirectoryList(param_ptr* p_owner, param_string* p_directory_path);
}