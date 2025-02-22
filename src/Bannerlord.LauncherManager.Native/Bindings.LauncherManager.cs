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
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_json*, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, void>, return_value_void*> p_set_game_parameters,
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_string*, param_string*, param_uint, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, void>, return_value_void*> p_send_notification,
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_string*, param_string*, param_json*, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, param_string*, void>, return_value_void*> p_send_dialog,
        delegate* unmanaged[Cdecl]<param_ptr*, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, param_string*, void>, return_value_void*> p_get_install_path,
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_int, param_int, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, param_data*, param_int, void>, return_value_void*> p_read_file_content,
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_data*, param_int, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, void>, return_value_void*> p_write_file_content,
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, param_json*, void>, return_value_void*> p_read_directory_file_list,
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, param_json*, void>, return_value_void*> p_read_directory_list,
        delegate* unmanaged[Cdecl]<param_ptr*, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, param_json*, void>, return_value_void*> p_get_all_module_view_models,
        delegate* unmanaged[Cdecl]<param_ptr*, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, param_json*, void>, return_value_void*> p_get_module_view_models,
        delegate* unmanaged[Cdecl]<param_ptr*, param_json*, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, void>, return_value_void*> p_set_module_view_models,
        delegate* unmanaged[Cdecl]<param_ptr*, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, param_json*, void>, return_value_void*> p_get_options,
        delegate* unmanaged[Cdecl]<param_ptr*, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, param_json*, void>, return_value_void*> p_get_state)
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


    [UnmanagedCallersOnly(EntryPoint = "ve_get_game_version", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* GetGameVersion(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, param_string*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.GetGameVersionAsync().ContinueWith(result =>
            {
                Logger.LogInput($"{nameof(GetGameVersion)}_{nameof(handler.GetGameVersionAsync)}");
               
                if (result.IsCompleted)
                {
                    fixed (char* pGameVersion = result.Result)
                    {
                        Logger.LogPinned(pGameVersion, $"{nameof(GetGameVersion)}_{nameof(handler.GetGameVersionAsync)}");
                        
                        p_callback(p_callback_handler, (param_string*) pGameVersion);
                        Logger.LogOutput(result, $"{nameof(GetGameVersion)}_{nameof(handler.GetGameVersionAsync)}");
                    }
                }
                if (result.IsFaulted)
                    Logger.LogException(result.Exception);
                if (result.IsCanceled)
                    Logger.LogException(new TaskCanceledException());
            });

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
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
            Logger.LogOutput(result);
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
                .Where(x => x is not null).ToArray();
            var moduleInfos = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module_infos, CustomSourceGenerationContext.ModuleInfoExtendedWithMetadataArray)
                .Where(x => x is not null).ToArray();

            var result = handler.InstallModuleContent(files, moduleInfos);
            Logger.LogOutput(result);
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

            Logger.LogOutput(result);
            return return_value_bool.AsValue(result, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_bool.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_sort", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* Sort(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.SortAsync().ContinueWith(result =>
            {
                Logger.LogInput($"{nameof(Sort)}_{nameof(handler.SortAsync)}");
               
                if (result.IsCompleted)
                {
                    p_callback(p_callback_handler);
                    
                    Logger.LogOutput($"{nameof(Sort)}_{nameof(handler.SortAsync)}");
                }
                if (result.IsFaulted)
                    Logger.LogException(result.Exception);
                if (result.IsCanceled)
                    Logger.LogException(new TaskCanceledException());
            });

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_refresh_modules", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* RefreshModules(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.RefreshModulesAsync().ContinueWith(result =>
            {
                Logger.LogInput($"{nameof(RefreshModules)}_{nameof(handler.RefreshModulesAsync)}");
                
                if (result.IsCompleted)
                {
                    p_callback(p_callback_handler);
                    
                    Logger.LogOutput($"{nameof(RefreshModules)}_{nameof(handler.RefreshModulesAsync)}");
                }
                if (result.IsFaulted)
                    Logger.LogException(result.Exception);
                if (result.IsCanceled)
                    Logger.LogException(new TaskCanceledException());
            });

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_refresh_game_parameters", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* RefreshGameParameters(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.RefreshGameParametersAsync().ContinueWith(result =>
            {
                Logger.LogInput($"{nameof(RefreshGameParameters)}_{nameof(handler.RefreshGameParametersAsync)}");
               
                if (result.IsCompleted)
                {
                    p_callback(p_callback_handler);
                   
                    Logger.LogOutput($"{nameof(RefreshGameParameters)}_{nameof(handler.RefreshGameParametersAsync)}");
                }
                if (result.IsFaulted)
                    Logger.LogException(result.Exception);
                if (result.IsCanceled)
                    Logger.LogException(new TaskCanceledException());
            });

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_get_modules", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* GetModules(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, param_json*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.GetModulesAsync().ContinueWith(result =>
            {
                Logger.LogInput($"{nameof(GetModules)}_{nameof(handler.GetModulesAsync)}");
             
                if (result.IsCompleted)
                {
                    var modulesJson = BUTR.NativeAOT.Shared.Utils.SerializeJson(result.Result, CustomSourceGenerationContext.IReadOnlyListModuleInfoExtendedWithMetadata);
                    fixed (char* pModulesJson = modulesJson ?? string.Empty)
                    {
                        Logger.LogPinned(pModulesJson, $"{nameof(GetModules)}_{nameof(handler.GetModulesAsync)}");
                        
                        p_callback(p_callback_handler, (param_json*) pModulesJson);
                  
                        Logger.LogOutput(result, $"{nameof(GetModules)}_{nameof(handler.GetModulesAsync)}");
                    }
                }
                if (result.IsFaulted)
                    Logger.LogException(result.Exception);
                if (result.IsCanceled)
                    Logger.LogException(new TaskCanceledException());
            });

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_get_all_modules", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* GetAllModules(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, param_json*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.GetAllModulesAsync().ContinueWith(result =>
            {
                Logger.LogInput($"{nameof(GetAllModules)}_{nameof(handler.GetAllModulesAsync)}");
                
                if (result.IsCompleted)
                {
                    var allModulesJson = BUTR.NativeAOT.Shared.Utils.SerializeJson(result.Result, CustomSourceGenerationContext.IReadOnlyListModuleInfoExtendedWithMetadata);
                    fixed (char* pAllModulesJson = allModulesJson ?? string.Empty)
                    {
                        Logger.LogPinned(pAllModulesJson, $"{nameof(GetAllModules)}_{nameof(handler.GetAllModulesAsync)}");
                        
                        p_callback(p_callback_handler, (param_json*) pAllModulesJson);
                    
                        Logger.LogOutput(result, $"{nameof(GetAllModules)}_{nameof(handler.GetAllModulesAsync)}");
                    }
                }
                if (result.IsFaulted)
                    Logger.LogException(result.Exception);
                if (result.IsCanceled)
                    Logger.LogException(new TaskCanceledException());
            });

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_check_for_root_harmony", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* CheckForRootHarmony(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.CheckForRootHarmonyAsync().ContinueWith(result =>
            {
                Logger.LogInput($"{nameof(CheckForRootHarmony)}_{nameof(IssuesChecker.CheckForRootHarmonyAsync)}");
                
                if (result.IsCompleted)
                {
                    p_callback(p_callback_handler);
                  
                    Logger.LogOutput($"{nameof(CheckForRootHarmony)}_{nameof(IssuesChecker.CheckForRootHarmonyAsync)}");
                }
                if (result.IsFaulted)
                    Logger.LogException(result.Exception);
                if (result.IsCanceled)
                    Logger.LogException(new TaskCanceledException());
            });

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_module_list_handler_export", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* ModuleListHandlerExport(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var moduleListHandler = new ModuleListHandler(handler);
            moduleListHandler.ExportAsync().ContinueWith(result =>
            {
                Logger.LogInput($"{nameof(ModuleListHandlerExport)}_{nameof(moduleListHandler.ExportAsync)}");
                
                if (result.IsCompleted)
                {
                    p_callback(p_callback_handler);
                    
                    Logger.LogOutput($"{nameof(ModuleListHandlerExport)}_{nameof(moduleListHandler.ExportAsync)}");
                }
                if (result.IsFaulted)
                    Logger.LogException(result.Exception);
                if (result.IsCanceled)
                    Logger.LogException(new TaskCanceledException());
            });

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_module_list_handler_export_save_file", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* ModuleListHandlerExportSaveFile(param_ptr* p_handle, param_string* p_save_file, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, void> p_callback)
    {
        Logger.LogInput(p_save_file);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var saveFile = new string(param_string.ToSpan(p_save_file));

            var moduleListHandler = new ModuleListHandler(handler);
            moduleListHandler.ExportSaveFileAsync(saveFile).ContinueWith(result =>
            {
                Logger.LogInput($"{nameof(ModuleListHandlerExportSaveFile)}_{nameof(moduleListHandler.ExportSaveFileAsync)}");
                
                if (result.IsCompleted)
                {
                    p_callback(p_callback_handler);
                    
                    Logger.LogOutput($"{nameof(ModuleListHandlerExportSaveFile)}_{nameof(moduleListHandler.ExportSaveFileAsync)}");
                }
                if (result.IsFaulted)
                    Logger.LogException(result.Exception);
                if (result.IsCanceled)
                    Logger.LogException(new TaskCanceledException());
            });

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_module_list_handler_import", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* ModuleListHandlerImport(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, param_bool, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_callback is null)
            //    return return_value_void.AsValue(false);

            var moduleListHandler = new ModuleListHandler(handler);
            moduleListHandler.ImportAsync().ContinueWith(result =>
            {
                Logger.LogInput($"{nameof(ModuleListHandlerImport)}_{nameof(moduleListHandler.ImportAsync)}");
                
                if (result.IsCompleted)
                {
                    p_callback(p_callback_handler, result.Result);
                   
                    Logger.LogOutput(result, $"{nameof(ModuleListHandlerImport)}_{nameof(moduleListHandler.ImportAsync)}");
                }
                if (result.IsFaulted)
                    Logger.LogException(result.Exception);
                if (result.IsCanceled)
                    Logger.LogException(new TaskCanceledException());
            });

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_module_list_handler_import_save_file", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* ModuleListHandlerImportSaveFile(param_ptr* p_handle, param_string* p_save_file, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, param_bool, void> p_callback)
    {
        Logger.LogInput(p_save_file);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_callback is null)
            //    return return_value_void.AsValue(false);

            var saveFile = new string(param_string.ToSpan(p_save_file));

            var moduleListHandler = new ModuleListHandler(handler);
            moduleListHandler.ImportSaveFileAsync(saveFile).ContinueWith(result =>
            {
                Logger.LogInput($"{nameof(ModuleListHandlerImportSaveFile)}_{nameof(moduleListHandler.ImportSaveFileAsync)}");
              
                if (result.IsCompleted)
                {
                    p_callback(p_callback_handler, result.Result);
                  
                    Logger.LogOutput(result, $"{nameof(ModuleListHandlerImportSaveFile)}_{nameof(moduleListHandler.ImportSaveFileAsync)}");
                }
                if (result.IsFaulted)
                    Logger.LogException(result.Exception);
                if (result.IsCanceled)
                    Logger.LogException(new TaskCanceledException());
            });

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_sort_helper_validate_module", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* SortHelperValidateModule(param_ptr* p_handle, param_json* p_module_view_model, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, param_json*, void> p_callback)
    {
        Logger.LogInput(p_module_view_model);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_module_view_model is null)
            //    return return_value_json.AsValue([], CustomSourceGenerationContext.StringArray, false);

            var moduleViewModel = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module_view_model, CustomSourceGenerationContext.ModuleViewModel);

            handler.GetModuleViewModelsAsync().ContinueWith(result =>
            {
                Logger.LogInput($"{nameof(SortHelperValidateModule)}_{nameof(handler.GetModuleViewModelsAsync)}");
              
                if (result.IsCompleted)
                {
                    var modules = result.Result?.ToArray() ?? [];
                    var lookup = modules.ToDictionary(x => x.ModuleInfoExtended.Id, x => x);
                    var validateResult = SortHelper.ValidateModule(modules, lookup, moduleViewModel).ToArray();
                    var validateResultJson = BUTR.NativeAOT.Shared.Utils.SerializeJson(validateResult, CustomSourceGenerationContext.StringArray);
                    fixed (char* pValidateResult = validateResultJson ?? string.Empty)
                    {
                        Logger.LogPinned(pValidateResult, $"{nameof(SortHelperValidateModule)}_{nameof(handler.GetModuleViewModelsAsync)}");
                        
                        p_callback(p_callback_handler, (param_json*) pValidateResult);
                       
                        Logger.LogOutput(validateResult, $"{nameof(SortHelperValidateModule)}_{nameof(handler.GetModuleViewModelsAsync)}");
                    }
                }
                if (result.IsFaulted)
                    Logger.LogException(result.Exception);
                if (result.IsCanceled)
                    Logger.LogException(new TaskCanceledException());
            });

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_sort_helper_toggle_module_selection", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* SortHelperToggleModuleSelection(param_ptr* p_handle, param_json* p_module_view_model, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, param_json*, void> p_callback)
    {
        Logger.LogInput(p_module_view_model);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_module_view_model is null)
            //    return return_value_json.AsValue([], CustomSourceGenerationContext.StringArray, false);

            var moduleViewModel = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module_view_model, CustomSourceGenerationContext.ModuleViewModel);

            handler.GetModuleViewModelsAsync().ContinueWith(result =>
            {
                Logger.LogInput($"{nameof(SortHelperToggleModuleSelection)}_{nameof(handler.GetModuleViewModelsAsync)}");
              
                if (result.IsCompleted)
                {
                    var modules = result.Result?.ToArray() ?? [];
                    var lookup = modules.ToDictionary(x => x.ModuleInfoExtended.Id, x => x);
                    SortHelper.ToggleModuleSelection(modules, lookup, moduleViewModel);
                    var moduleViewModelJson = BUTR.NativeAOT.Shared.Utils.SerializeJson(moduleViewModel, CustomSourceGenerationContext.ModuleViewModel);
                    fixed (char* pModuleViewModelJson = moduleViewModelJson ?? string.Empty)
                    {
                        Logger.LogPinned(pModuleViewModelJson, $"{nameof(SortHelperToggleModuleSelection)}_{nameof(handler.GetModuleViewModelsAsync)}");
                        
                        p_callback(p_callback_handler, (param_json*) pModuleViewModelJson);
                       
                        Logger.LogOutput(moduleViewModel, $"{nameof(SortHelperToggleModuleSelection)}_{nameof(handler.GetModuleViewModelsAsync)}");
                    }
                }
                if (result.IsFaulted)
                    Logger.LogException(result.Exception);
                if (result.IsCanceled)
                    Logger.LogException(new TaskCanceledException());
            });

            Logger.LogOutput(moduleViewModel);
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_sort_helper_change_module_position", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* SortHelperChangeModulePosition(param_ptr* p_handle, param_json* p_module_view_model, param_int insertIndex, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, param_bool, void> p_callback)
    {
        Logger.LogInput(p_module_view_model, &insertIndex);

        var insertIndexInt = (int) insertIndex;

        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_module_view_model is null)
            //    return return_value_bool.AsValue(false, false);

            var moduleViewModel = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module_view_model, CustomSourceGenerationContext.ModuleViewModel);

            handler.GetModuleViewModelsAsync().ContinueWith(x => SortHelperChangeModulePositionTask(handler, moduleViewModel, insertIndexInt, x));

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }
    private static async Task SortHelperChangeModulePositionTask(LauncherManagerHandlerNative handler, ModuleViewModel moduleViewModel, param_int insertIndex, Task<IEnumerable<IModuleViewModel>> result)
    {
        Logger.LogInput();
        if (result.IsCompleted)
        {
            var modules = result.Result?.ToArray() ?? [];
            var lookup = modules.ToDictionary(x => x.ModuleInfoExtended.Id, x => x);
            var (positionResult, issues) = SortHelper.ChangeModulePosition(modules, lookup, moduleViewModel, insertIndex);
            if (issues is { Count: > 0})
                await handler.ShowHintAsync(new BUTRTextObject("{=sP1a61KE}Failed to place the module to the desired position! Placing to the nearest available!{NL}Reason:{NL}{REASONS}").SetTextVariable("REASONS", string.Join("\n", issues)).ToString());
            Logger.LogOutput(positionResult);
        }
        if (result.IsFaulted)
            Logger.LogException(result.Exception);
        if (result.IsCanceled)
            Logger.LogException(new TaskCanceledException());
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_get_save_files", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* GetSaveFiles(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, param_json*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.GetSaveFilesAsync().ContinueWith(result =>
            {
                Logger.LogInput($"{nameof(GetSaveFiles)}_{nameof(handler.GetSaveFilesAsync)}");
                
                if (result.IsCompleted)
                {
                    var saveFilesJson = BUTR.NativeAOT.Shared.Utils.SerializeJson(result.Result, CustomSourceGenerationContext.IReadOnlyListSaveMetadata);
                    fixed (char* pSaveFilesJson = saveFilesJson ?? string.Empty)
                    {
                        Logger.LogPinned(pSaveFilesJson, $"{nameof(GetSaveFiles)}_{nameof(handler.GetSaveFilesAsync)}");
                        
                        p_callback(p_callback_handler, (param_json*) pSaveFilesJson);
                       
                        Logger.LogOutput(result, $"{nameof(GetSaveFiles)}_{nameof(handler.GetSaveFilesAsync)}");
                    }
                }
                if (result.IsFaulted)
                    Logger.LogException(result.Exception);
                if (result.IsCanceled)
                    Logger.LogException(new TaskCanceledException());
            });

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_get_save_metadata", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* GetSaveMetadata(param_ptr* p_handle, param_string* p_save_file, param_data* p_data, param_int data_len, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, param_json*, void> p_callback)
    {
        Logger.LogInput(p_save_file, p_data, &data_len);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var saveFile = new string(param_string.ToSpan(p_save_file));
            var data = param_data.ToSpan(p_data, data_len).ToArray(); // TODO: 

            handler.GetSaveMetadataAsync(saveFile, data).ContinueWith(result =>
            {
                Logger.LogInput($"{nameof(GetSaveMetadata)}_{nameof(handler.GetSaveMetadataAsync)}");
               
                if (result.IsCompleted)
                {
                    fixed (char* pSaveMetadata = BUTR.NativeAOT.Shared.Utils.SerializeJson(result.Result, CustomSourceGenerationContext.SaveMetadata) ?? string.Empty)
                    {
                        Logger.LogPinned(pSaveMetadata, $"{nameof(GetSaveMetadata)}_{nameof(handler.GetSaveMetadataAsync)}");
                        
                        p_callback(p_callback_handler, (param_json*) pSaveMetadata);
                        
                        Logger.LogOutput(result, $"{nameof(GetSaveMetadata)}_{nameof(handler.GetSaveMetadataAsync)}");
                    }
                }
                if (result.IsFaulted)
                    Logger.LogException(result.Exception);
                if (result.IsCanceled)
                    Logger.LogException(new TaskCanceledException());
            });

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_get_save_file_path", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* GetSaveFilePath(param_ptr* p_handle, param_string* p_save_file, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, param_string*, void> p_callback)
    {
        Logger.LogInput(p_save_file);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var saveFile = new string(param_string.ToSpan(p_save_file));

            handler.GetSaveFilePathAsync(saveFile).ContinueWith(result =>
            {
                Logger.LogInput($"{nameof(GetSaveFilePath)}_{nameof(handler.GetSaveFilePathAsync)}");
                
                if (result.IsCompleted)
                {
                    fixed (char* pSaveFilePath = result.Result)
                    {
                        Logger.LogPinned(pSaveFilePath, $"{nameof(GetSaveFilePath)}_{nameof(handler.GetSaveFilePathAsync)}");
                        
                        p_callback(p_callback_handler, (param_string*) pSaveFilePath);

                        Logger.LogOutput(result, $"{nameof(GetSaveFilePath)}_{nameof(handler.GetSaveFilePathAsync)}");
                    }
                }
                if (result.IsFaulted)
                    Logger.LogException(result.Exception);
                if (result.IsCanceled)
                    Logger.LogException(new TaskCanceledException());
            });

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }


    public record OrderByLoadOrderResult(bool Result, IReadOnlyList<string>? Issues, IReadOnlyList<ModuleViewModel>? OrderedModuleViewModels);

    [UnmanagedCallersOnly(EntryPoint = "ve_order_by_load_order", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* OrderByLoadOrder(param_ptr* p_handle, param_json* p_load_order, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, param_json*, void> p_callback)
    {
        Logger.LogInput(p_load_order);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_load_order is null)
            //    return return_value_json.AsValue(new OrderByLoadOrderResult(false, null, null), CustomSourceGenerationContext.OrderByLoadOrderResult, false);

            var loadOrder = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_load_order, CustomSourceGenerationContext.LoadOrder);

            var loadOrderDict = loadOrder.ToDictionary(x => x.Key, x => x.Value.IsSelected);
            handler.TryOrderByLoadOrderAsync(loadOrder.OrderBy(x => x.Value.Index).Select(x => x.Key), x => loadOrderDict.TryGetValue(x, out var isSelected) && isSelected).ContinueWith(result =>
            {
                Logger.LogInput($"{nameof(OrderByLoadOrder)}_{nameof(handler.TryOrderByLoadOrderAsync)}");
                
                var (res, issues, orderedModuleViewModels) = result.Result;
                if (result.IsCompleted)
                {
                    var orderByLoadOrderResult = new OrderByLoadOrderResult(res, issues, orderedModuleViewModels.Select(x => new ModuleViewModel(x.ModuleInfoExtended, x.IsValid)
                    {
                        IsSelected = x.IsSelected,
                        IsDisabled = x.IsDisabled,
                        Index = x.Index,
                    }).ToArray());
                    fixed (char* pOrderByLoadOrderResult = BUTR.NativeAOT.Shared.Utils.SerializeJson(orderByLoadOrderResult, CustomSourceGenerationContext.OrderByLoadOrderResult) ?? string.Empty)
                    {
                        Logger.LogPinned(pOrderByLoadOrderResult, $"{nameof(OrderByLoadOrder)}_{nameof(handler.TryOrderByLoadOrderAsync)}");
                        
                        p_callback(p_callback_handler, (param_json*) pOrderByLoadOrderResult);
                        
                        Logger.LogOutput(orderByLoadOrderResult, $"{nameof(OrderByLoadOrder)}_{nameof(handler.TryOrderByLoadOrderAsync)}");
                    }
                }
                if (result.IsFaulted)
                    Logger.LogException(result.Exception);
                if (result.IsCanceled)
                    Logger.LogException(new TaskCanceledException());
            });

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_set_game_parameter_executable", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* SetGameParameterExecutable(param_ptr* p_handle, param_string* p_executable, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, void> p_callback)
    {
        Logger.LogInput(p_executable);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var executable = new string(param_string.ToSpan(p_executable));

            handler.SetGameParameterExecutableAsync(executable).ContinueWith(result =>
            {
                Logger.LogInput($"{nameof(SetGameParameterExecutable)}_{nameof(handler.SetGameParameterExecutableAsync)}");
                
                if (result.IsCompleted)
                {
                    p_callback(p_callback_handler);
                    
                    Logger.LogOutput($"{nameof(SetGameParameterExecutable)}_{nameof(handler.SetGameParameterExecutableAsync)}");
                }
                if (result.IsFaulted)
                    Logger.LogException(result.Exception);
                if (result.IsCanceled)
                    Logger.LogException(new TaskCanceledException());
            });

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_set_game_parameter_load_order", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* SetGameParameterLoadOrder(param_ptr* p_handle, param_json* p_load_order, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, void> p_callback)
    {
        Logger.LogInput(p_load_order);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_load_order is null)
            //    return return_value_void.AsValue(false);

            var loadOrder = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_load_order, CustomSourceGenerationContext.LoadOrder);

            handler.SetGameParameterLoadOrderAsync(loadOrder).ContinueWith(result =>
            {
                Logger.LogInput($"{nameof(SetGameParameterLoadOrder)}_{nameof(handler.SetGameParameterLoadOrderAsync)}");
                
                if (result.IsCompleted)
                {
                    p_callback(p_callback_handler);
                    
                    Logger.LogOutput($"{nameof(SetGameParameterLoadOrder)}_{nameof(handler.SetGameParameterLoadOrderAsync)}");
                }
                if (result.IsFaulted)
                    Logger.LogException(result.Exception);
                if (result.IsCanceled)
                    Logger.LogException(new TaskCanceledException());
            });

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_set_game_parameter_save_file", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* SetGameParameterSaveFile(param_ptr* p_handle, param_string* p_save_file, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, void> p_callback)
    {
        Logger.LogInput(p_save_file);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var saveFile = new string(param_string.ToSpan(p_save_file));

            handler.SetGameParameterSaveFileAsync(saveFile).ContinueWith(result =>
            {
                Logger.LogInput($"{nameof(SetGameParameterSaveFile)}_{nameof(handler.SetGameParameterSaveFileAsync)}");
                
                if (result.IsCompleted)
                {
                    p_callback(p_callback_handler);
                    
                    Logger.LogOutput($"{nameof(SetGameParameterSaveFile)}_{nameof(handler.SetGameParameterSaveFileAsync)}");
                }
                if (result.IsFaulted)
                    Logger.LogException(result.Exception);
                if (result.IsCanceled)
                    Logger.LogException(new TaskCanceledException());
            });

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_set_game_parameter_continue_last_save_file", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* SetGameParameterContinueLastSaveFile(param_ptr* p_handle, param_bool p_value, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, void> p_callback)
    {
        Logger.LogInput(p_value);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.SetGameParameterContinueLastSaveFileAsync(p_value).ContinueWith(result =>
            {
                Logger.LogInput($"{nameof(SetGameParameterContinueLastSaveFile)}_{nameof(handler.SetGameParameterContinueLastSaveFileAsync)}");
                
                if (result.IsCompleted)
                {
                    p_callback(p_callback_handler);
                    
                    Logger.LogOutput($"{nameof(SetGameParameterContinueLastSaveFile)}_{nameof(handler.SetGameParameterContinueLastSaveFileAsync)}");
                }
                if (result.IsFaulted)
                    Logger.LogException(result.Exception);
                if (result.IsCanceled)
                    Logger.LogException(new TaskCanceledException());
            });

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
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

    [UnmanagedCallersOnly(EntryPoint = "ve_get_game_platform", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* GetGamePlatform(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, param_string*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.GetPlatformAsync().ContinueWith(result =>
            {
                Logger.LogInput($"{nameof(GetGamePlatform)}_{nameof(handler.GetPlatformAsync)}");
                
                if (result.IsCompleted)
                {
                    var platform = result.Result.ToStringFast();
                    fixed (char* pPlatform = platform ?? string.Empty)
                    {
                        Logger.LogPinned(pPlatform, $"{nameof(GetGamePlatform)}_{nameof(handler.GetPlatformAsync)}");
                        
                        p_callback(p_callback_handler, (param_string*) pPlatform);
                        
                        Logger.LogOutput($"{nameof(GetGamePlatform)}_{nameof(handler.GetPlatformAsync)}");
                    }
                }
                if (result.IsFaulted)
                    Logger.LogException(result.Exception);
                if (result.IsCanceled)
                    Logger.LogException(new TaskCanceledException());
            });
            
            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_dialog_test_warning", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* DialogTestWarning(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, param_string*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_callback is null)
            //    return return_value_void.AsValue(false);

            handler.SendDialogAsync(DialogType.Warning, "Test Title", "Test Message", Array.Empty<DialogFileFilter>()).ContinueWith(result =>
            {
                Logger.LogInput($"{nameof(DialogTestWarning)}_{nameof(handler.SendDialogAsync)}");
                
                if (result.IsCompleted)
                {
                    fixed (char* pResult = result.Result ?? string.Empty)
                    {
                        Logger.LogPinned(pResult, $"{nameof(DialogTestWarning)}_{nameof(handler.SendDialogAsync)}");
                        
                        p_callback(p_callback_handler, (param_string*) pResult);
                        
                        Logger.LogOutput(result, $"{nameof(DialogTestWarning)}_{nameof(handler.SendDialogAsync)}");
                    }
                }
                if (result.IsFaulted)
                    Logger.LogException(result.Exception);
                if (result.IsCanceled)
                    Logger.LogException(new TaskCanceledException());
            });

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_dialog_test_file_open", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* DialogTestFileOpen(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, param_string*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_callback is null)
            //    return return_value_void.AsValue(false);

            handler.SendDialogAsync(DialogType.FileOpen, "Test Title", "Test Message", [new DialogFileFilter("Test Filter", ["*.test"])]).ContinueWith(result =>
            {
                Logger.LogInput($"{nameof(DialogTestFileOpen)}_{nameof(handler.SendDialogAsync)}");
              
                if (result.IsCompleted)
                {
                    fixed (char* pResult = result.Result ?? string.Empty)
                    {
                        Logger.LogPinned(pResult, $"{nameof(DialogTestFileOpen)}_{nameof(handler.SendDialogAsync)}");
                        
                        p_callback(p_callback_handler, (param_string*) pResult);
                        
                        Logger.LogOutput(result, $"{nameof(DialogTestFileOpen)}_{nameof(handler.SendDialogAsync)}");
                    }
                }
                if (result.IsFaulted)
                    Logger.LogException(result.Exception);
                if (result.IsCanceled)
                    Logger.LogException(new TaskCanceledException());
            });

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }
}