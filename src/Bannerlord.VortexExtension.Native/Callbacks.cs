using BUTR.NativeAOT.Shared;

using System.Runtime.InteropServices;

namespace Bannerlord.VortexExtension.Native
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate return_value_json* N_GetActiveProfileDelegate(void* p_owner);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate return_value_json* N_GetProfileByIdDelegate(void* p_owner, param_string* p_profile_id);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate return_value_string* N_GetActiveGameIdDelegate(void* p_owner);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate return_value_void* N_SetGameParametersDelegate(void* p_owner, param_string* p_game_id, param_string* p_executable, param_json* p_game_parameters);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate return_value_json* N_GetLoadOrderDelegate(void* p_owner);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate return_value_void* N_SetLoadOrderDelegate(void* p_owner, param_json* p_load_order);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate return_value_string* N_TranslateStringDelegate(void* p_owner, param_string* p_text, param_string* p_ns);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate return_value_void* N_SendNotificationDelegate(void* p_owner, param_string* p_id, param_string* p_type, param_string* p_message, uint displayMs);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate return_value_string* N_GetInstallPathDelegate(void* p_owner);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate return_value_string* N_ReadFileContentDelegate(void* p_owner, param_string* p_file_path);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate return_value_json* N_ReadDirectoryFileList(void* p_owner, param_string* p_directory_path);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate return_value_json* N_ReadDirectoryList(void* p_owner, param_string* p_directory_path);
}