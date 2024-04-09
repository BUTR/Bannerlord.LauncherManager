using BUTR.NativeAOT.Shared;

using FetchBannerlordVersion;

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bannerlord.LauncherManager.Native;

public static unsafe partial class Bindings
{
    [UnmanagedCallersOnly(EntryPoint = "bfv_get_change_set", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_uint32* GetChangeSet([IsConst<IsPtrConst>] param_string* p_game_folder_path, [IsConst<IsPtrConst>] param_string* p_lib_assembly)
    {
        Logger.LogInput(p_game_folder_path, p_lib_assembly);
        try
        {
            var gameFolderPath = new string(param_string.ToSpan(p_game_folder_path));
            var libAssembly = new string(param_string.ToSpan(p_lib_assembly));

            var result = (uint) Fetcher.GetChangeSet(Path.GetFullPath(gameFolderPath), libAssembly);

            Logger.LogOutput(result);
            return return_value_uint32.AsValue(result, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_uint32.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "bfv_get_version", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_string* GetVersion([IsConst<IsPtrConst>] param_string* p_game_folder_path, [IsConst<IsPtrConst>] param_string* p_lib_assembly)
    {
        Logger.LogInput(p_game_folder_path, p_lib_assembly);
        try
        {
            var gameFolderPath = new string(param_string.ToSpan(p_game_folder_path));
            var libAssembly = new string(param_string.ToSpan(p_lib_assembly));

            var result = Fetcher.GetVersion(Path.GetFullPath(gameFolderPath), libAssembly);

            Logger.LogOutput(result);
            return return_value_string.AsValue(BUTR.NativeAOT.Shared.Utils.Copy(result, false), false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_string.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "bfv_get_version_type", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_uint32* GetVersionType([IsConst<IsPtrConst>] param_string* p_game_folder_path, [IsConst<IsPtrConst>] param_string* p_lib_assembly)
    {
        Logger.LogInput(p_game_folder_path, p_lib_assembly);
        try
        {
            var gameFolderPath = new string(param_string.ToSpan(p_game_folder_path));
            var libAssembly = new string(param_string.ToSpan(p_lib_assembly));

            var result = (uint) Fetcher.GetVersionType(Path.GetFullPath(gameFolderPath), libAssembly);

            Logger.LogOutput(result);
            return return_value_uint32.AsValue(result, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_uint32.AsException(e, false);
        }
    }
}