using System;
using System.Runtime.InteropServices;
using FetchBannerlordVersion;

namespace Bannerlord.VortexExtension.Native
{
    public static unsafe partial class Bindings
    {
        [UnmanagedCallersOnly(EntryPoint = "bfv_get_change_set")]
        public static return_value_uint32* GetChangeSet(param_string* p_game_folder_path, param_string* p_lib_assembly)
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(GetChangeSet)}! p_game_folder_path: {param_string.ToSpan(p_game_folder_path)}; p_lib_assembly: {param_string.ToSpan(p_lib_assembly)}");
#endif

            try
            {
                var gameFolderPath = new string(param_string.ToSpan(p_game_folder_path));
                var libAssembly = new string(param_string.ToSpan(p_lib_assembly));

                var result = (uint) Fetcher.GetChangeSet(gameFolderPath, libAssembly);
#if LOGGING
                Logger.Log($"Result of {nameof(GetChangeSet)}: {result}");
#endif
                return return_value_uint32.AsValue(result);
            }
            catch (Exception e)
            {
#if LOGGING
                Logger.Log($"Exception of {nameof(GetChangeSet)}: {e}");
#endif
                return return_value_uint32.AsError(Utils.Copy(e.ToString()));
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "bfv_get_version")]
        public static return_value_string* GetVersion(param_string* p_game_folder_path, param_string* p_lib_assembly)
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(GetVersion)}! p_game_folder_path: {param_string.ToSpan(p_game_folder_path)}; p_lib_assembly: {param_string.ToSpan(p_lib_assembly)}");
#endif

            try
            {
                var gameFolderPath = new string(param_string.ToSpan(p_game_folder_path));
                var libAssembly = new string(param_string.ToSpan(p_lib_assembly));

                var result = Fetcher.GetVersion(gameFolderPath, libAssembly);
#if LOGGING
                Logger.Log($"Result of {nameof(GetVersion)}: {result}");
#endif
                return return_value_string.AsValue(Utils.Copy(result));
            }
            catch (Exception e)
            {
#if LOGGING
                Logger.Log($"Exception of {nameof(GetVersion)}: {e}");
#endif
                return return_value_string.AsError(Utils.Copy(e.ToString()));
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "bfv_get_version_type")]
        public static return_value_uint32* GetVersionType(param_string* p_game_folder_path, param_string* p_lib_assembly)
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(GetVersionType)}! p_game_folder_path: {param_string.ToSpan(p_game_folder_path)}; p_lib_assembly: {param_string.ToSpan(p_lib_assembly)}");
#endif

            try
            {
                var gameFolderPath = new string(param_string.ToSpan(p_game_folder_path));
                var libAssembly = new string(param_string.ToSpan(p_lib_assembly));

                var result = (uint) Fetcher.GetVersionType(gameFolderPath, libAssembly);
#if LOGGING
                Logger.Log($"Result of {nameof(GetVersionType)}: {result}");
#endif
                return return_value_uint32.AsValue(result);
            }
            catch (Exception e)
            {
#if LOGGING
                Logger.Log($"Exception of {nameof(GetVersionType)}: {e}");
#endif
                return return_value_uint32.AsError(Utils.Copy(e.ToString()));
            }
        }
    }
}