using Bannerlord.LauncherManager.Localization;
using Bannerlord.LauncherManager.Models;
using Bannerlord.LauncherManager.Native.Adapters;
using Bannerlord.LauncherManager.Native.Models;
using Bannerlord.LauncherManager.Utils;

using BUTR.NativeAOT.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.Native;

public static partial class Bindings
{
    [UnmanagedCallersOnly(EntryPoint = "ve_create_handler", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_ptr* CreateHandler(param_ptr* p_owner,
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_json*, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void>, return_value_void*> p_set_game_parameters,
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_string*, param_string*, param_uint, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void>, return_value_void*> p_send_notification,
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_string*, param_string*, param_json*, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, return_value_string*, void>, return_value_void*> p_send_dialog,
        delegate* unmanaged[Cdecl]<param_ptr*, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, return_value_string*, void>, return_value_void*> p_get_install_path,
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_int, param_int, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, return_value_data*, void>, return_value_void*> p_read_file_content,
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_data*, param_int, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void>, return_value_void*> p_write_file_content,
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void>, return_value_void*> p_read_directory_file_list,
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void>, return_value_void*> p_read_directory_list,
        delegate* unmanaged[Cdecl]<param_ptr*, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void>, return_value_void*> p_get_all_module_view_models,
        delegate* unmanaged[Cdecl]<param_ptr*, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void>, return_value_void*> p_get_module_view_models,
        delegate* unmanaged[Cdecl]<param_ptr*, param_json*, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void>, return_value_void*> p_set_module_view_models,
        delegate* unmanaged[Cdecl]<param_ptr*, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void>, return_value_void*> p_get_options,
        delegate* unmanaged[Cdecl]<param_ptr*, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void>, return_value_void*> p_get_state)
    {
        Logger.LogInput();
        try
        {
            //if (p_set_game_parameters is null || p_load_load_order is null || p_save_load_order is null || p_send_notification is null || p_send_dialog is null ||
            //    p_get_install_path is null || p_read_file_content is null || p_write_file_content is null || p_read_directory_file_list is null || p_read_directory_list is null ||
            //    p_get_all_module_view_models is null || p_get_module_view_models is null || p_set_module_view_models is null || p_get_options is null || p_get_state is null)
            //    return return_value_ptr.AsValue(null, false);

            var launcherManager = new LauncherManagerHandlerNative(p_owner,
                dialogProvider: new DialogProvider(p_owner,
                    Marshal.GetDelegateForFunctionPointer<N_SendDialogDelegate>(new IntPtr(p_send_dialog))
                ),
                fileSystemProvider: new FileSystemProvider(p_owner,
                    Marshal.GetDelegateForFunctionPointer<N_ReadFileContentDelegate>(new IntPtr(p_read_file_content)),
                    Marshal.GetDelegateForFunctionPointer<N_WriteFileContentDelegate>(new IntPtr(p_write_file_content)),
                    Marshal.GetDelegateForFunctionPointer<N_ReadDirectoryFileList>(new IntPtr(p_read_directory_file_list)),
                    Marshal.GetDelegateForFunctionPointer<N_ReadDirectoryList>(new IntPtr(p_read_directory_list))
                ),
                gameInfoProvider: new GameInfoProvider(p_owner,
                    Marshal.GetDelegateForFunctionPointer<N_GetInstallPathDelegate>(new IntPtr(p_get_install_path))
                ),
                launcherStateUProvider: new LauncherStateProvider(p_owner,
                    Marshal.GetDelegateForFunctionPointer<N_SetGameParametersDelegate>(new IntPtr(p_set_game_parameters)),
                    Marshal.GetDelegateForFunctionPointer<N_GetOptions>(new IntPtr(p_get_options)),
                    Marshal.GetDelegateForFunctionPointer<N_GetState>(new IntPtr(p_get_state))
                ),
                notificationProvider: new NotificationProvider(p_owner,
                    Marshal.GetDelegateForFunctionPointer<N_SendNotificationDelegate>(new IntPtr(p_send_notification))
                ),
                loadOrderStateProvider: new LoadOrderStateProvider(p_owner,
                    Marshal.GetDelegateForFunctionPointer<N_GetAllModuleViewModels>(new IntPtr(p_get_all_module_view_models)),
                    Marshal.GetDelegateForFunctionPointer<N_GetModuleViewModels>(new IntPtr(p_get_module_view_models)),
                    Marshal.GetDelegateForFunctionPointer<N_SetModuleViewModels>(new IntPtr(p_set_module_view_models))
                )
            );

            Logger.LogOutput();
            return return_value_ptr.AsValue(launcherManager.HandlePtr, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_ptr.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_dispose_handler", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* DisposeHandler(param_ptr* p_handle)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.Dispose();

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_get_game_version_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* GetGameVersionAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_string*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.GetGameVersionAsync().ContinueWith(result =>
            {
                Logger.LogAsyncInput(
                    result.Result,
                    $"{nameof(GetGameVersionAsync)}_{nameof(handler.GetGameVersionAsync)}");

                if (result.Exception is not null)
                {
                    p_callback(p_callback_handler, return_value_string.AsException(result.Exception, false));
                    Logger.LogException(result.Exception, $"{nameof(GetGameVersionAsync)}_{nameof(handler.GetGameVersionAsync)}");
                }
                else
                {
                    p_callback(p_callback_handler, return_value_string.AsValue(BUTR.NativeAOT.Shared.Utils.Copy(result.Result, false), false));
                    Logger.LogOutput($"{nameof(GetGameVersionAsync)}_{nameof(handler.GetGameVersionAsync)}");
                }
            });

            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_test_module", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_json* TestModule(param_ptr* p_handle, param_json* p_files)
    {
        Logger.LogInput(p_files);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_json.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_files is null)
            //    return return_value_json.AsValue(SupportedResult.AsNotSupported, CustomSourceGenerationContext.SupportedResult, false);

            var files = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_files, CustomSourceGenerationContext.StringArray);

            var result = handler.TestModuleContent(files);
            Logger.LogOutput();
            return return_value_json.AsValue(result, CustomSourceGenerationContext.SupportedResult, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_install_module", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_json* InstallModule(param_ptr* p_handle, param_json* p_files, param_json* p_module_infos)
    {
        Logger.LogInput(p_files, p_module_infos);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_json.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_files is null || p_module_infos is null)
            //    return return_value_json.AsValue(InstallResult.AsInvalid, CustomSourceGenerationContext.InstallResult, false);

            var files = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_files, CustomSourceGenerationContext.StringArray)
                .Where(x => x != null!).ToArray();
            var moduleInfos = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module_infos, CustomSourceGenerationContext.ModuleInfoExtendedWithMetadataArray)
                .Where(x => x != null!).ToArray();

            var result = handler.InstallModuleContent(files, moduleInfos);
            Logger.LogOutput();
            return return_value_json.AsValue(result, CustomSourceGenerationContext.InstallResult, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_is_sorting", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_bool* IsSorting(param_ptr* p_handle)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_bool.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var result = handler.IsSorting;

            Logger.LogOutput();
            return return_value_bool.AsValue(result, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_bool.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_sort_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* SortAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.SortAsync().ContinueWith(result =>
            {
                Logger.LogAsyncInput($"{nameof(SortAsync)}_{nameof(handler.SortAsync)}");
               
                if (result.Exception is not null)
                {
                    p_callback(p_callback_handler, return_value_void.AsException(result.Exception, false));
                    Logger.LogException(result.Exception, $"{nameof(SortAsync)}_{nameof(handler.SortAsync)}");
                }
                else
                {
                    p_callback(p_callback_handler, return_value_void.AsValue(false));
                    Logger.LogOutput($"{nameof(SortAsync)}_{nameof(handler.SortAsync)}");
                }
            });

            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_refresh_modules_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* RefreshModulesAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.RefreshModulesAsync().ContinueWith(result =>
            {
                Logger.LogAsyncInput($"{nameof(RefreshModulesAsync)}_{nameof(handler.RefreshModulesAsync)}");
                
                if (result.Exception is not null)
                {
                    p_callback(p_callback_handler, return_value_void.AsException(result.Exception, false));
                    Logger.LogException(result.Exception, $"{nameof(RefreshModulesAsync)}_{nameof(handler.RefreshModulesAsync)}");
                }
                else
                {
                    p_callback(p_callback_handler, return_value_void.AsValue(false));
                    Logger.LogOutput($"{nameof(RefreshModulesAsync)}_{nameof(handler.RefreshModulesAsync)}");
                }
            });

            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_refresh_game_parameters_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* RefreshGameParametersAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.RefreshGameParametersAsync().ContinueWith(result =>
            {
                Logger.LogAsyncInput($"{nameof(RefreshGameParametersAsync)}_{nameof(handler.RefreshGameParametersAsync)}");
               
                if (result.Exception is not null)
                {
                    p_callback(p_callback_handler, return_value_void.AsException(result.Exception, false));
                    Logger.LogException(result.Exception, $"{nameof(RefreshGameParametersAsync)}_{nameof(handler.RefreshGameParametersAsync)}");
                }
                else
                {
                    p_callback(p_callback_handler, return_value_void.AsValue(false));
                    Logger.LogOutput($"{nameof(RefreshGameParametersAsync)}_{nameof(handler.RefreshGameParametersAsync)}");
                }
            });

            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_get_modules_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* GetModulesAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.GetModulesAsync().ContinueWith(result =>
            {
                Logger.LogAsyncInput(
                    BUTR.NativeAOT.Shared.Utils.SerializeJson(result.Result, CustomSourceGenerationContext.IReadOnlyListModuleInfoExtendedWithMetadata),
                    $"{nameof(GetModulesAsync)}_{nameof(handler.GetModulesAsync)}");
             
                if (result.Exception is not null)
                {
                    p_callback(p_callback_handler, return_value_json.AsException(result.Exception, false));
                    Logger.LogException(result.Exception, $"{nameof(GetModulesAsync)}_{nameof(handler.GetModulesAsync)}");
                }
                else
                {
                    p_callback(p_callback_handler, return_value_json.AsValue(result.Result, CustomSourceGenerationContext.IReadOnlyListModuleInfoExtendedWithMetadata, false));
                    Logger.LogOutput($"{nameof(GetModulesAsync)}_{nameof(handler.GetModulesAsync)}");
                }
            });

            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_get_all_modules_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* GetAllModulesAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.GetAllModulesAsync().ContinueWith(result =>
            {
                Logger.LogAsyncInput(
                    BUTR.NativeAOT.Shared.Utils.SerializeJson(result.Result, CustomSourceGenerationContext.IReadOnlyListModuleInfoExtendedWithMetadata),
                    $"{nameof(GetAllModulesAsync)}_{nameof(handler.GetAllModulesAsync)}");
                
                if (result.Exception is not null)
                {
                    p_callback(p_callback_handler, return_value_json.AsException(result.Exception, false));
                    Logger.LogException(result.Exception, $"{nameof(GetAllModulesAsync)}_{nameof(handler.GetAllModulesAsync)}");
                }
                else
                {
                    p_callback(p_callback_handler, return_value_json.AsValue(result.Result, CustomSourceGenerationContext.IReadOnlyListModuleInfoExtendedWithMetadata, false));
                    Logger.LogOutput($"{nameof(GetAllModulesAsync)}_{nameof(handler.GetAllModulesAsync)}");
                }
            });

            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_check_for_root_harmony_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* CheckForRootHarmonyAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.CheckForRootHarmonyAsync().ContinueWith(result =>
            {
                Logger.LogAsyncInput($"{nameof(CheckForRootHarmonyAsync)}_{nameof(IssuesChecker.CheckForRootHarmonyAsync)}");
                
                if (result.Exception is not null)
                {
                    p_callback(p_callback_handler, return_value_void.AsException(result.Exception, false));
                    Logger.LogException(result.Exception, $"{nameof(CheckForRootHarmonyAsync)}_{nameof(IssuesChecker.CheckForRootHarmonyAsync)}");
                }
                else
                {
                    p_callback(p_callback_handler, return_value_void.AsValue(false));
                    Logger.LogOutput($"{nameof(CheckForRootHarmonyAsync)}_{nameof(IssuesChecker.CheckForRootHarmonyAsync)}");
                }
            });

            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_module_list_handler_export_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* ModuleListHandlerExportAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var moduleListHandler = new ModuleListHandler(handler);
            moduleListHandler.ExportAsync().ContinueWith(result =>
            {
                Logger.LogAsyncInput($"{nameof(ModuleListHandlerExportAsync)}_{nameof(moduleListHandler.ExportAsync)}");
                
                if (result.Exception is not null)
                {
                    p_callback(p_callback_handler, return_value_void.AsException(result.Exception, false));
                    Logger.LogException(result.Exception, $"{nameof(ModuleListHandlerExportAsync)}_{nameof(moduleListHandler.ExportAsync)}");
                }
                else
                {
                    p_callback(p_callback_handler, return_value_void.AsValue(false));
                    Logger.LogOutput($"{nameof(ModuleListHandlerExportAsync)}_{nameof(moduleListHandler.ExportAsync)}");
                }
            });

            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_module_list_handler_export_save_file_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* ModuleListHandlerExportSaveFileAsync(param_ptr* p_handle, param_string* p_save_file, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void> p_callback)
    {
        Logger.LogInput(p_save_file);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var saveFile = new string(param_string.ToSpan(p_save_file));

            var moduleListHandler = new ModuleListHandler(handler);
            moduleListHandler.ExportSaveFileAsync(saveFile).ContinueWith(result =>
            {
                Logger.LogAsyncInput($"{nameof(ModuleListHandlerExportSaveFileAsync)}_{nameof(moduleListHandler.ExportSaveFileAsync)}");
                
                if (result.Exception is not null)
                {
                    p_callback(p_callback_handler, return_value_void.AsException(result.Exception, false));
                    Logger.LogException(result.Exception, $"{nameof(ModuleListHandlerExportSaveFileAsync)}_{nameof(moduleListHandler.ExportSaveFileAsync)}");
                }
                else
                {
                    p_callback(p_callback_handler, return_value_void.AsValue(false));
                    Logger.LogOutput($"{nameof(ModuleListHandlerExportSaveFileAsync)}_{nameof(moduleListHandler.ExportSaveFileAsync)}");
                }
            });

            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_module_list_handler_import_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* ModuleListHandlerImportAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_bool*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_callback is null)
            //    return return_value_void.AsValue(false);

            var moduleListHandler = new ModuleListHandler(handler);
            moduleListHandler.ImportAsync().ContinueWith(result =>
            {
                Logger.LogAsyncInput(
                    result.Result.ToString(),
                    $"{nameof(ModuleListHandlerImportAsync)}_{nameof(moduleListHandler.ImportAsync)}");
                
                if (result.Exception is not null)
                {
                    p_callback(p_callback_handler, return_value_bool.AsException(result.Exception, false));
                    Logger.LogException(result.Exception, $"{nameof(ModuleListHandlerImportAsync)}_{nameof(moduleListHandler.ImportAsync)}");
                }
                else
                {
                    p_callback(p_callback_handler, return_value_bool.AsValue(result.Result, false));
                    Logger.LogOutput($"{nameof(ModuleListHandlerImportAsync)}_{nameof(moduleListHandler.ImportAsync)}");
                }
            });

            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_module_list_handler_import_save_file_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* ModuleListHandlerImportSaveFileAsync(param_ptr* p_handle, param_string* p_save_file, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_bool*, void> p_callback)
    {
        Logger.LogInput(p_save_file);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_callback is null)
            //    return return_value_void.AsValue(false);

            var saveFile = new string(param_string.ToSpan(p_save_file));

            var moduleListHandler = new ModuleListHandler(handler);
            moduleListHandler.ImportSaveFileAsync(saveFile).ContinueWith(result =>
            {
                Logger.LogAsyncInput(
                    result.Result.ToString(),
                    $"{nameof(ModuleListHandlerImportSaveFileAsync)}_{nameof(moduleListHandler.ImportSaveFileAsync)}");
              
                if (result.Exception is not null)
                {
                    p_callback(p_callback_handler, return_value_bool.AsException(result.Exception, false));
                    Logger.LogException(result.Exception, $"{nameof(ModuleListHandlerImportSaveFileAsync)}_{nameof(moduleListHandler.ImportSaveFileAsync)}");
                }
                else
                {
                    p_callback(p_callback_handler, return_value_bool.AsValue(result.Result, false));
                    Logger.LogOutput($"{nameof(ModuleListHandlerImportSaveFileAsync)}_{nameof(moduleListHandler.ImportSaveFileAsync)}");
                }
            });

            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_sort_helper_validate_module_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* SortHelperValidateModuleAsync(param_ptr* p_handle, param_json* p_module_view_model, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void> p_callback)
    {
        Logger.LogInput(p_module_view_model);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_module_view_model is null)
            //    return return_value_json.AsValue([], CustomSourceGenerationContext.StringArray, false);

            var moduleViewModel = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module_view_model, CustomSourceGenerationContext.ModuleViewModel);

            handler.GetModuleViewModelsAsync().ContinueWith(result =>
            {
                Logger.LogAsyncInput(
                    BUTR.NativeAOT.Shared.Utils.SerializeJson(result.Result?.OfType<ModuleViewModel>().ToArray(), CustomSourceGenerationContext.ModuleViewModelArray),
                    $"{nameof(SortHelperValidateModuleAsync)}_{nameof(handler.GetModuleViewModelsAsync)}");
              
                if (result.Exception is not null)
                {
                    p_callback(p_callback_handler, return_value_json.AsException(result.Exception, false));
                    Logger.LogException(result.Exception, $"{nameof(SortHelperValidateModuleAsync)}_{nameof(handler.GetModuleViewModelsAsync)}");
                }
                else
                {
                    var modules = result.Result?.ToArray() ?? [];
                    var lookup = modules.ToDictionary(x => x.ModuleInfoExtended.Id, x => x);
                    var validateResult = SortHelper.ValidateModule(modules, lookup, moduleViewModel).ToArray();
                    p_callback(p_callback_handler, return_value_json.AsValue(validateResult, CustomSourceGenerationContext.StringArray, false));
                    Logger.LogOutput($"{nameof(SortHelperValidateModuleAsync)}_{nameof(handler.GetModuleViewModelsAsync)}");
                }
            });

            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_sort_helper_toggle_module_selection_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* SortHelperToggleModuleSelectionAsync(param_ptr* p_handle, param_json* p_module_view_model, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void> p_callback)
    {
        Logger.LogInput(p_module_view_model);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_module_view_model is null)
            //    return return_value_json.AsValue([], CustomSourceGenerationContext.StringArray, false);

            var moduleViewModel = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module_view_model, CustomSourceGenerationContext.ModuleViewModel);

            handler.GetModuleViewModelsAsync().ContinueWith(result =>
            {
                Logger.LogAsyncInput(
                    BUTR.NativeAOT.Shared.Utils.SerializeJson(result.Result?.OfType<ModuleViewModel>().ToArray(), CustomSourceGenerationContext.ModuleViewModelArray),
                    $"{nameof(SortHelperToggleModuleSelectionAsync)}_{nameof(handler.GetModuleViewModelsAsync)}");
              
                if (result.Exception is not null)
                {
                    p_callback(p_callback_handler, return_value_json.AsException(result.Exception, false));
                    Logger.LogException(result.Exception, $"{nameof(SortHelperToggleModuleSelectionAsync)}_{nameof(handler.GetModuleViewModelsAsync)}");
                }
                else
                {
                    var modules = result.Result?.ToArray() ?? [];
                    var lookup = modules.ToDictionary(x => x.ModuleInfoExtended.Id, x => x);
                    SortHelper.ToggleModuleSelection(modules, lookup, moduleViewModel);
                    p_callback(p_callback_handler, return_value_json.AsValue(moduleViewModel, CustomSourceGenerationContext.ModuleViewModel, false));
                    Logger.LogOutput($"{nameof(SortHelperToggleModuleSelectionAsync)}_{nameof(handler.GetModuleViewModelsAsync)}");
                }
            });

            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_sort_helper_change_module_position_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* SortHelperChangeModulePositionAsync(param_ptr* p_handle, param_json* p_module_view_model, param_int insertIndex, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_bool*, void> p_callback)
    {
        Logger.LogInput(p_module_view_model, &insertIndex);

        var insertIndexInt = (int) insertIndex;

        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_module_view_model is null)
            //    return return_value_bool.AsValue(false, false);

            var moduleViewModel = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module_view_model, CustomSourceGenerationContext.ModuleViewModel);

            handler.GetModuleViewModelsAsync().ContinueWith(result =>
            {
                Logger.LogAsyncInput(
                    BUTR.NativeAOT.Shared.Utils.SerializeJson(result.Result?.OfType<ModuleViewModel>().ToArray(), CustomSourceGenerationContext.ModuleViewModelArray),
                    $"{nameof(SortHelperChangeModulePositionAsync)}_{nameof(handler.GetModuleViewModelsAsync)}");

                return AfterGetModuleViewModelsAsync(handler, moduleViewModel, insertIndexInt, positionResult =>
                {
                    p_callback(p_callback_handler, return_value_bool.AsValue(positionResult, false));
                    Logger.LogOutput($"{nameof(SortHelperChangeModulePositionAsync)}_{nameof(handler.GetModuleViewModelsAsync)}");
                }, e =>
                {
                    p_callback(p_callback_handler, return_value_bool.AsException(e, false));
                    Logger.LogException(e, $"{nameof(SortHelperChangeModulePositionAsync)}_{nameof(handler.GetModuleViewModelsAsync)}");
                }, result);
            });

            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }
    private static async Task AfterGetModuleViewModelsAsync(LauncherManagerHandlerNative handler, ModuleViewModel moduleViewModel, param_int insertIndex, Action<bool> onResult, Action<Exception> onException, Task<IEnumerable<IModuleViewModel>?> result)
    {
        Logger.LogInput();
        
        if (result.Exception is not null)
        {
            onException(result.Exception);
        }
        else
        {
            var modules = result.Result?.ToArray() ?? [];
            var lookup = modules.ToDictionary(x => x.ModuleInfoExtended.Id, x => x);
            var (positionResult, issues) = SortHelper.ChangeModulePosition(modules, lookup, moduleViewModel, insertIndex);
            if (issues is { Count: > 0})
                await handler.ShowHintAsync(new BUTRTextObject("{=sP1a61KE}Failed to place the module to the desired position! Placing to the nearest available!{NL}Reason:{NL}{REASONS}").SetTextVariable("REASONS", string.Join("\n", issues)).ToString());
            onResult(positionResult);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_get_save_files_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* GetSaveFilesAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.GetSaveFilesAsync().ContinueWith(result =>
            {
                Logger.LogAsyncInput(
                    BUTR.NativeAOT.Shared.Utils.SerializeJson(result.Result, CustomSourceGenerationContext.IReadOnlyListSaveMetadata),
                    $"{nameof(GetSaveFilesAsync)}_{nameof(handler.GetSaveFilesAsync)}");

                if (result.Exception is not null)
                {
                    p_callback(p_callback_handler, return_value_json.AsException(result.Exception, false));
                    Logger.LogException(result.Exception, $"{nameof(GetSaveFilesAsync)}_{nameof(handler.GetSaveFilesAsync)}");
                }
                else
                {
                    p_callback(p_callback_handler, return_value_json.AsValue(result.Result, CustomSourceGenerationContext.IReadOnlyListSaveMetadata, false));
                    Logger.LogOutput($"{nameof(GetSaveFilesAsync)}_{nameof(handler.GetSaveFilesAsync)}");
                }
            });

            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_get_save_metadata_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* GetSaveMetadataAsync(param_ptr* p_handle, param_string* p_save_file, param_data* p_data, param_int data_len, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void> p_callback)
    {
        Logger.LogInput(p_save_file, p_data, &data_len);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var saveFile = new string(param_string.ToSpan(p_save_file));
            var data = param_data.ToSpan(p_data, data_len).ToArray(); // TODO: 

            handler.GetSaveMetadataAsync(saveFile, data).ContinueWith(result =>
            {
                Logger.LogAsyncInput(
                    BUTR.NativeAOT.Shared.Utils.SerializeJson(result.Result, CustomSourceGenerationContext.SaveMetadata),
                    $"{nameof(GetSaveMetadataAsync)}_{nameof(handler.GetSaveMetadataAsync)}");
               
                if (result.Exception is not null)
                {
                    p_callback(p_callback_handler, return_value_json.AsException(result.Exception, false));
                    Logger.LogException(result.Exception, $"{nameof(GetSaveMetadataAsync)}_{nameof(handler.GetSaveMetadataAsync)}");
                }
                else
                {
                    p_callback(p_callback_handler, return_value_json.AsValue(result.Result, CustomSourceGenerationContext.SaveMetadata, false));
                    Logger.LogOutput($"{nameof(GetSaveMetadataAsync)}_{nameof(handler.GetSaveMetadataAsync)}");
                }
            });

            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_get_save_file_path_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* GetSaveFilePathAsync(param_ptr* p_handle, param_string* p_save_file, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_string*, void> p_callback)
    {
        Logger.LogInput(p_save_file);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var saveFile = new string(param_string.ToSpan(p_save_file));

            handler.GetSaveFilePathAsync(saveFile).ContinueWith(result =>
            {
                Logger.LogAsyncInput(
                    result.Result ?? "null",
                    $"{nameof(GetSaveFilePathAsync)}_{nameof(handler.GetSaveFilePathAsync)}");
                
                if (result.Exception is not null)
                {
                    p_callback(p_callback_handler, return_value_string.AsException(result.Exception, false));
                    Logger.LogException(result.Exception, $"{nameof(GetSaveFilePathAsync)}_{nameof(handler.GetSaveFilePathAsync)}");
                }
                else
                {
                    p_callback(p_callback_handler, return_value_string.AsValue(result.Result, false));
                    Logger.LogOutput($"{nameof(GetSaveFilePathAsync)}_{nameof(handler.GetSaveFilePathAsync)}");
                }
            });

            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }


    public record OrderByLoadOrderResultNative(bool Result, IReadOnlyList<string>? Issues, IReadOnlyList<ModuleViewModel>? OrderedModuleViewModels);

    [UnmanagedCallersOnly(EntryPoint = "ve_order_by_load_order_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* OrderByLoadOrderAsync(param_ptr* p_handle, param_json* p_load_order, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void> p_callback)
    {
        Logger.LogInput(p_load_order);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_load_order is null)
            //    return return_value_json.AsValue(new OrderByLoadOrderResult(false, null, null), CustomSourceGenerationContext.OrderByLoadOrderResult, false);

            var loadOrder = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_load_order, CustomSourceGenerationContext.LoadOrder);

            var loadOrderDict = loadOrder.ToDictionary(x => x.Key, x => x.Value.IsSelected);
            handler.TryOrderByLoadOrderAsync(loadOrder.OrderBy(x => x.Value.Index).Select(x => x.Key), x => loadOrderDict.TryGetValue(x, out var isSelected) && isSelected).ContinueWith(result =>
            {
                Logger.LogAsyncInput(
                    BUTR.NativeAOT.Shared.Utils.SerializeJson(result.Result, CustomSourceGenerationContext.OrderByLoadOrderResult),
                    $"{nameof(OrderByLoadOrderAsync)}_{nameof(handler.TryOrderByLoadOrderAsync)}");
                
                if (result.Exception is not null)
                {
                    p_callback(p_callback_handler, return_value_json.AsException(result.Exception, false));
                    Logger.LogException(result.Exception, $"{nameof(OrderByLoadOrderAsync)}_{nameof(handler.TryOrderByLoadOrderAsync)}");
                }
                else
                {
                    var (res, issues, orderedModuleViewModels) = result.Result;
                    var orderByLoadOrderResult = new OrderByLoadOrderResultNative(res, issues, orderedModuleViewModels.Select(x => new ModuleViewModel(x.ModuleInfoExtended, x.IsValid)
                    {
                        IsSelected = x.IsSelected,
                        IsDisabled = x.IsDisabled,
                        Index = x.Index,
                    }).ToArray());
                    p_callback(p_callback_handler, return_value_json.AsValue(orderByLoadOrderResult, CustomSourceGenerationContext.OrderByLoadOrderResultNative, false));
                    Logger.LogOutput($"{nameof(OrderByLoadOrderAsync)}_{nameof(handler.TryOrderByLoadOrderAsync)}");
                }
            });

            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_set_game_parameter_executable_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* SetGameParameterExecutableAsync(param_ptr* p_handle, param_string* p_executable, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void> p_callback)
    {
        Logger.LogInput(p_executable);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var executable = new string(param_string.ToSpan(p_executable));

            handler.SetGameParameterExecutableAsync(executable).ContinueWith(result =>
            {
                Logger.LogAsyncInput($"{nameof(SetGameParameterExecutableAsync)}_{nameof(handler.SetGameParameterExecutableAsync)}");
                
                
                if (result.Exception is not null)
                {
                    p_callback(p_callback_handler, return_value_void.AsException(result.Exception, false));
                    Logger.LogException(result.Exception, $"{nameof(SetGameParameterExecutableAsync)}_{nameof(handler.SetGameParameterExecutableAsync)}");
                }
                else
                {
                    p_callback(p_callback_handler, return_value_void.AsValue(false));
                    Logger.LogOutput($"{nameof(SetGameParameterExecutableAsync)}_{nameof(handler.SetGameParameterExecutableAsync)}");
                }
            });

            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_set_game_parameter_load_order_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* SetGameParameterLoadOrderAsync(param_ptr* p_handle, param_json* p_load_order, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void> p_callback)
    {
        Logger.LogInput(p_load_order);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_load_order is null)
            //    return return_value_void.AsValue(false);

            var loadOrder = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_load_order, CustomSourceGenerationContext.LoadOrder);

            handler.SetGameParameterLoadOrderAsync(loadOrder).ContinueWith(result =>
            {
                Logger.LogAsyncInput($"{nameof(SetGameParameterLoadOrderAsync)}_{nameof(handler.SetGameParameterLoadOrderAsync)}");
                
                if (result.Exception is not null)
                {
                    p_callback(p_callback_handler, return_value_void.AsException(result.Exception, false));
                    Logger.LogException(result.Exception, $"{nameof(SetGameParameterLoadOrderAsync)}_{nameof(handler.SetGameParameterLoadOrderAsync)}");
                }
                else
                {
                    p_callback(p_callback_handler, return_value_void.AsValue(false));
                    Logger.LogOutput($"{nameof(SetGameParameterLoadOrderAsync)}_{nameof(handler.SetGameParameterLoadOrderAsync)}");
                }
            });

            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_set_game_parameter_save_file_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* SetGameParameterSaveFileAsync(param_ptr* p_handle, param_string* p_save_file, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void> p_callback)
    {
        Logger.LogInput(p_save_file);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var saveFile = new string(param_string.ToSpan(p_save_file));

            handler.SetGameParameterSaveFileAsync(saveFile).ContinueWith(result =>
            {
                Logger.LogAsyncInput($"{nameof(SetGameParameterSaveFileAsync)}_{nameof(handler.SetGameParameterSaveFileAsync)}");
                
                if (result.Exception is not null)
                {
                    p_callback(p_callback_handler, return_value_void.AsException(result.Exception, false));
                    Logger.LogException(result.Exception, $"{nameof(SetGameParameterSaveFileAsync)}_{nameof(handler.SetGameParameterSaveFileAsync)}");
                }
                else
                {
                    p_callback(p_callback_handler, return_value_void.AsValue(false));
                    Logger.LogOutput($"{nameof(SetGameParameterSaveFileAsync)}_{nameof(handler.SetGameParameterSaveFileAsync)}");
                }
            });

            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_set_game_parameter_continue_last_save_file_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* SetGameParameterContinueLastSaveFileAsync(param_ptr* p_handle, param_bool p_value, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void> p_callback)
    {
        Logger.LogInput(&p_value);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.SetGameParameterContinueLastSaveFileAsync(p_value).ContinueWith(result =>
            {
                Logger.LogAsyncInput($"{nameof(SetGameParameterContinueLastSaveFileAsync)}_{nameof(handler.SetGameParameterContinueLastSaveFileAsync)}");
                
                if (result.Exception is not null)
                {
                    p_callback(p_callback_handler, return_value_void.AsException(result.Exception, false));
                    Logger.LogException(result.Exception, $"{nameof(SetGameParameterContinueLastSaveFileAsync)}_{nameof(handler.SetGameParameterContinueLastSaveFileAsync)}");
                }
                else
                {
                    p_callback(p_callback_handler, return_value_void.AsValue(false));
                    Logger.LogOutput($"{nameof(SetGameParameterContinueLastSaveFileAsync)}_{nameof(handler.SetGameParameterContinueLastSaveFileAsync)}");
                }
            });

            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_set_game_store", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* SetGameStore(param_ptr* p_handle, param_string* p_game_store)
    {
        Logger.LogInput(p_game_store);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var gameStoreStr = new string(param_string.ToSpan(p_game_store));
            var gameStore = Enum.Parse<GameStore>(gameStoreStr);

            handler.SetGameStore(gameStore);

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_get_game_platform_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* GetGamePlatformAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_string*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.GetPlatformAsync().ContinueWith(result =>
            {
                Logger.LogAsyncInput(
                    result.Result.ToStringFast(),
                    $"{nameof(GetGamePlatformAsync)}_{nameof(handler.GetPlatformAsync)}");
                
                if (result.Exception is not null)
                {
                    p_callback(p_callback_handler, return_value_string.AsException(result.Exception, false));
                    Logger.LogException(result.Exception, $"{nameof(GetGamePlatformAsync)}_{nameof(handler.GetPlatformAsync)}");
                }
                else
                {
                    var platform = result.Result.ToStringFast();
                    p_callback(p_callback_handler, return_value_string.AsValue(platform, false));
                    Logger.LogOutput($"{nameof(GetGamePlatformAsync)}_{nameof(handler.GetPlatformAsync)}");
                }
            });
            
            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_dialog_test_warning_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* DialogTestWarningAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_string*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_callback is null)
            //    return return_value_void.AsValue(false);

            handler.SendDialogAsync(DialogType.Warning, "Test Title", "Test Message", Array.Empty<DialogFileFilter>()).ContinueWith(result =>
            {
                Logger.LogAsyncInput(
                    result.Result,
                    $"{nameof(DialogTestWarningAsync)}_{nameof(handler.SendDialogAsync)}");
                
                if (result.Exception is not null)
                {
                    p_callback(p_callback_handler, return_value_string.AsException(result.Exception, false));
                    Logger.LogException(result.Exception, $"{nameof(DialogTestWarningAsync)}_{nameof(handler.SendDialogAsync)}");
                }
                else
                {
                    p_callback(p_callback_handler, return_value_string.AsValue(result.Result, false));
                    Logger.LogOutput($"{nameof(DialogTestWarningAsync)}_{nameof(handler.SendDialogAsync)}");
                }
            });

            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_dialog_test_file_open_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* DialogTestFileOpenAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_string*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_callback is null)
            //    return return_value_void.AsValue(false);

            handler.SendDialogAsync(DialogType.FileOpen, "Test Title", "Test Message", [new DialogFileFilter("Test Filter", ["*.test"])]).ContinueWith(result =>
            {
                Logger.LogAsyncInput(
                    result.Result,
                    $"{nameof(DialogTestFileOpenAsync)}_{nameof(handler.SendDialogAsync)}");
              
                if (result.Exception is not null)
                {
                    p_callback(p_callback_handler, return_value_string.AsException(result.Exception, false));
                    Logger.LogException(result.Exception, $"{nameof(DialogTestFileOpenAsync)}_{nameof(handler.SendDialogAsync)}");
                }
                else
                {
                    p_callback(p_callback_handler, return_value_string.AsValue(result.Result, false));
                    Logger.LogOutput($"{nameof(DialogTestFileOpenAsync)}_{nameof(handler.SendDialogAsync)}");
                }
            });

            Logger.LogOutput();
            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }
}