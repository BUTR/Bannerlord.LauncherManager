using BUTR.NativeAOT.Shared;

using FetchBannerlordVersion;

using System;
using System.Runtime.InteropServices;

namespace Bannerlord.VortexExtension.Native
{
    public static unsafe partial class Bindings
    {
        [UnmanagedCallersOnly(EntryPoint = "bfv_get_change_set")]
        public static return_value_uint32* GetChangeSet(param_string* p_game_folder_path, param_string* p_lib_assembly)
        {
            Logger.LogInput(p_game_folder_path, p_lib_assembly);
            try
            {
                var gameFolderPath = new string(param_string.ToSpan(p_game_folder_path));
                var libAssembly = new string(param_string.ToSpan(p_lib_assembly));

                var result = (uint) Fetcher.GetChangeSet(gameFolderPath, libAssembly);

                Logger.LogOutputPrimitive(result);
                return return_value_uint32.AsValue(result);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_uint32.AsError(BUTR.NativeAOT.Shared.Utils.Copy(e.ToString()));
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "bfv_get_version")]
        public static return_value_string* GetVersion(param_string* p_game_folder_path, param_string* p_lib_assembly)
        {
            Logger.LogInput(p_game_folder_path, p_lib_assembly);
            try
            {
                var gameFolderPath = new string(param_string.ToSpan(p_game_folder_path));
                var libAssembly = new string(param_string.ToSpan(p_lib_assembly));

                var result = Fetcher.GetVersion(gameFolderPath, libAssembly);

                Logger.LogOutput(result);
                return return_value_string.AsValue(BUTR.NativeAOT.Shared.Utils.Copy(result));
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_string.AsError(BUTR.NativeAOT.Shared.Utils.Copy(e.ToString()));
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "bfv_get_version_type")]
        public static return_value_uint32* GetVersionType(param_string* p_game_folder_path, param_string* p_lib_assembly)
        {
            Logger.LogInput(p_game_folder_path, p_lib_assembly);
            try
            {
                var gameFolderPath = new string(param_string.ToSpan(p_game_folder_path));
                var libAssembly = new string(param_string.ToSpan(p_lib_assembly));

                var result = (uint) Fetcher.GetVersionType(gameFolderPath, libAssembly);

                Logger.LogOutputPrimitive(result);
                return return_value_uint32.AsValue(result);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_uint32.AsError(BUTR.NativeAOT.Shared.Utils.Copy(e.ToString()));
            }
        }
    }
}