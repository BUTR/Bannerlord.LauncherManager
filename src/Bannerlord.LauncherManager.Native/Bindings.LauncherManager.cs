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

namespace Bannerlord.LauncherManager.Native;

public static unsafe partial class Bindings
{
    [UnmanagedCallersOnly(EntryPoint = "ve_create_handler", CallConvs = [typeof(CallConvCdecl)])]
    public static return_value_ptr* CreateHandler(param_ptr* p_owner,
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_json*, return_value_void*> p_set_game_parameters,
        delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*> p_load_load_order,
        delegate* unmanaged[Cdecl]<param_ptr*, param_json*, return_value_void*> p_save_load_order,
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_string*, param_string*, param_uint, return_value_void*> p_send_notification,
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_string*, param_string*, param_json*, param_ptr*, delegate* unmanaged[Cdecl]<param_ptr*, param_string*, void>, return_value_void*> p_send_dialog,
        delegate* unmanaged[Cdecl]<param_ptr*, return_value_string*> p_get_install_path,
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_int, param_int, return_value_data*> p_read_file_content,
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_data*, param_int, return_value_void*> p_write_file_content,
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, return_value_json*> p_read_directory_file_list,
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, return_value_json*> p_read_directory_list,
        delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*> p_get_all_module_view_models,
        delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*> p_get_module_view_models,
        delegate* unmanaged[Cdecl]<param_ptr*, param_json*, return_value_void*> p_set_module_view_models,
        delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*> p_get_options,
        delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*> p_get_state)
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
    public static return_value_void* DisposeHandler(param_ptr* p_handle)
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
    public static return_value_string* GetGameVersion(param_ptr* p_handle)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_string.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var result = handler.GetGameVersion();

            Logger.LogOutput(result, nameof(GetGameVersion));
            return return_value_string.AsValue(BUTR.NativeAOT.Shared.Utils.Copy(result, false), false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_string.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_test_module", CallConvs = [typeof(CallConvCdecl)])]
    public static return_value_json* TestModule(param_ptr* p_handle, param_json* p_files)
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
    public static return_value_json* InstallModule(param_ptr* p_handle, param_json* p_files, param_json* p_module_infos)
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
    public static return_value_bool* IsSorting(param_ptr* p_handle)
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
    public static return_value_void* Sort(param_ptr* p_handle)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.Sort();

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
    public static return_value_void* RefreshModules(param_ptr* p_handle)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.RefreshModules();

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
    public static return_value_void* RefreshGameParameters(param_ptr* p_handle)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.RefreshGameParameters();

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
    public static return_value_json* GetModules(param_ptr* p_handle)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_json.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var result = handler.GetModules().ToArray();

            Logger.LogOutput(result);
            return return_value_json.AsValue(result, CustomSourceGenerationContext.ModuleInfoExtendedWithMetadataArray, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_get_all_modules", CallConvs = [typeof(CallConvCdecl)])]
    public static return_value_json* GetAllModules(param_ptr* p_handle)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_json.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var result = handler.GetAllModules().ToArray();

            Logger.LogOutput(result);
            return return_value_json.AsValue(result, CustomSourceGenerationContext.ModuleInfoExtendedWithMetadataArray, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_check_for_root_harmony", CallConvs = [typeof(CallConvCdecl)])]
    public static return_value_void* CheckForRootHarmony(param_ptr* p_handle)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.CheckForRootHarmony();

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
    public static return_value_void* ModuleListHandlerExport(param_ptr* p_handle)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var moduleListHandler = new ModuleListHandler(handler);
            moduleListHandler.Export();

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
    public static return_value_void* ModuleListHandlerExportSaveFile(param_ptr* p_handle, param_string* p_save_file)
    {
        Logger.LogInput(p_save_file);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var saveFile = new string(param_string.ToSpan(p_save_file));

            var moduleListHandler = new ModuleListHandler(handler);
            moduleListHandler.ExportSaveFile(saveFile);

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
    public static return_value_void* ModuleListHandlerImport(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, param_bool, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_callback is null)
            //    return return_value_void.AsValue(false);

            var moduleListHandler = new ModuleListHandler(handler);
            moduleListHandler.Import(result =>
            {
                Logger.LogInput();
                p_callback(p_callback_handler, result);
                Logger.LogOutput(result);
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
    public static return_value_void* ModuleListHandlerImportSaveFile(param_ptr* p_handle, param_string* p_save_file, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, param_bool, void> p_callback)
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
            moduleListHandler.ImportSaveFile(saveFile, result =>
            {
                Logger.LogInput();
                p_callback(p_callback_handler, result);
                Logger.LogOutput(result);
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
    public static return_value_json* SortHelperValidateModule(param_ptr* p_handle, param_json* p_module_view_model)
    {
        Logger.LogInput(p_module_view_model);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_json.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_module_view_model is null)
            //    return return_value_json.AsValue([], CustomSourceGenerationContext.StringArray, false);

            var moduleViewModel = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module_view_model, CustomSourceGenerationContext.ModuleViewModel);

            var modules = handler.GetModuleViewModels()?.ToArray() ?? [];
            var lookup = modules.ToDictionary(x => x.ModuleInfoExtended.Id, x => x);
            var result = SortHelper.ValidateModule(modules, lookup, moduleViewModel).ToArray();

            Logger.LogOutput(result);
            return return_value_json.AsValue(result, CustomSourceGenerationContext.StringArray, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_sort_helper_toggle_module_selection", CallConvs = [typeof(CallConvCdecl)])]
    public static return_value_json* SortHelperToggleModuleSelection(param_ptr* p_handle, param_json* p_module_view_model)
    {
        Logger.LogInput(p_module_view_model);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_json.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_module_view_model is null)
            //    return return_value_json.AsValue([], CustomSourceGenerationContext.StringArray, false);

            var moduleViewModel = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module_view_model, CustomSourceGenerationContext.ModuleViewModel);

            var modules = handler.GetModuleViewModels()?.ToArray() ?? [];
            var lookup = modules.ToDictionary(x => x.ModuleInfoExtended.Id, x => x);
            SortHelper.ToggleModuleSelection(modules, lookup, moduleViewModel);

            Logger.LogOutput(moduleViewModel);
            return return_value_json.AsValue(moduleViewModel, CustomSourceGenerationContext.ModuleViewModel, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_sort_helper_change_module_position", CallConvs = [typeof(CallConvCdecl)])]
    public static return_value_bool* SortHelperChangeModulePosition(param_ptr* p_handle, param_json* p_module_view_model, param_int insertIndex)
    {
        Logger.LogInput(p_module_view_model, &insertIndex);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_bool.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_module_view_model is null)
            //    return return_value_bool.AsValue(false, false);

            var moduleViewModel = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module_view_model, CustomSourceGenerationContext.ModuleViewModel);

            var modules = handler.GetModuleViewModels()?.ToArray() ?? [];
            var lookup = modules.ToDictionary(x => x.ModuleInfoExtended.Id, x => x);
            var result = SortHelper.ChangeModulePosition(modules, lookup, moduleViewModel, insertIndex, (issues =>
            {
                handler.ShowHint(new BUTRTextObject("{=sP1a61KE}Failed to place the module to the desired position! Placing to the nearest available!{NL}Reason:{NL}{REASONS}").SetTextVariable("REASONS", string.Join("\n", issues)).ToString());
            }));

            Logger.LogOutput(result);
            return return_value_bool.AsValue(result, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_bool.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_get_save_files", CallConvs = [typeof(CallConvCdecl)])]
    public static return_value_json* GetSaveFiles(param_ptr* p_handle)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_json.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var result = handler.GetSaveFiles();

            Logger.LogOutput(result);
            return return_value_json.AsValue(result, CustomSourceGenerationContext.SaveMetadataArray, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_get_save_metadata", CallConvs = [typeof(CallConvCdecl)])]
    public static return_value_json* GetSaveMetadata(param_ptr* p_handle, param_string* p_save_file, param_data* p_data, param_int data_len)
    {
        Logger.LogInput(p_save_file, p_data, &data_len);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_json.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var saveFile = new string(param_string.ToSpan(p_save_file));
            var data = param_data.ToSpan(p_data, data_len);

            var result = handler.GetSaveMetadata(saveFile, data);
            if (result is null) return return_value_json.AsValue(null, false);

            Logger.LogOutput(result);
            return return_value_json.AsValue(result, CustomSourceGenerationContext.SaveMetadata, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_get_save_file_path", CallConvs = [typeof(CallConvCdecl)])]
    public static return_value_string* GetSaveFilePath(param_ptr* p_handle, param_string* p_save_file)
    {
        Logger.LogInput(p_save_file);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_string.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var saveFile = new string(param_string.ToSpan(p_save_file));

            var result = handler.GetSaveFilePath(saveFile);

            Logger.LogOutput(result);
            return return_value_string.AsValue(result, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_string.AsException(e, false);
        }
    }


    public record OrderByLoadOrderResult(bool Result, IReadOnlyList<string>? Issues, IReadOnlyList<ModuleViewModel>? OrderedModuleViewModels);

    [UnmanagedCallersOnly(EntryPoint = "ve_order_by_load_order", CallConvs = [typeof(CallConvCdecl)])]
    public static return_value_json* OrderByLoadOrder(param_ptr* p_handle, param_json* p_load_order)
    {
        Logger.LogInput(p_load_order);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_json.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_load_order is null)
            //    return return_value_json.AsValue(new OrderByLoadOrderResult(false, null, null), CustomSourceGenerationContext.OrderByLoadOrderResult, false);

            var loadOrder = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_load_order, CustomSourceGenerationContext.LoadOrder);

            var loadOrderDict = loadOrder.ToDictionary(x => x.Key, x => x.Value.IsSelected);
            var res = handler.TryOrderByLoadOrder(loadOrder.OrderBy(x => x.Value.Index).Select(x => x.Key), x => loadOrderDict.TryGetValue(x, out var isSelected) && isSelected, out var issues, out var orderedModuleViewModels);
            var result = new OrderByLoadOrderResult(res, issues, orderedModuleViewModels.Select(x => new ModuleViewModel(x.ModuleInfoExtended, x.IsValid)
            {
                IsSelected = x.IsSelected,
                IsDisabled = x.IsDisabled,
                Index = x.Index,
            }).ToArray());

            Logger.LogOutput(result);
            return return_value_json.AsValue(result, CustomSourceGenerationContext.OrderByLoadOrderResult, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_set_game_parameter_executable", CallConvs = [typeof(CallConvCdecl)])]
    public static return_value_void* SetGameParameterExecutable(param_ptr* p_handle, param_string* p_executable)
    {
        Logger.LogInput(p_executable);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var executable = new string(param_string.ToSpan(p_executable));

            handler.SetGameParameterExecutable(executable);

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
    public static return_value_void* SetGameParameterLoadOrder(param_ptr* p_handle, param_json* p_load_order)
    {
        Logger.LogInput(p_load_order);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_load_order is null)
            //    return return_value_void.AsValue(false);

            var loadOrder = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_load_order, CustomSourceGenerationContext.LoadOrder);

            handler.SetGameParameterLoadOrder(loadOrder);

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
    public static return_value_void* SetGameParameterSaveFile(param_ptr* p_handle, param_string* p_save_file)
    {
        Logger.LogInput(p_save_file);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var saveFile = new string(param_string.ToSpan(p_save_file));

            handler.SetGameParameterSaveFile(saveFile);

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
    public static return_value_void* SetGameParameterContinueLastSaveFile(param_ptr* p_handle, param_bool p_value)
    {
        Logger.LogInput(p_value);
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.SetGameParameterContinueLastSaveFile(p_value);

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
    public static return_value_void* SetGameStore(param_ptr* p_handle, param_string* p_game_store)
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
    public static return_value_string* GetGamePlatform(param_ptr* p_handle)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_string.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            Logger.LogOutput();
            return return_value_string.AsValue(handler.GetPlatform().ToStringFast(), false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_string.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_dialog_test_warning", CallConvs = [typeof(CallConvCdecl)])]
    public static return_value_void* DialogTestWarning(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, param_string*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_callback is null)
            //    return return_value_void.AsValue(false);

            handler.SendDialog(DialogType.Warning, "Test Title", "Test Message", Array.Empty<DialogFileFilter>(), result =>
            {
                Logger.LogInput($"{nameof(DialogTestWarning)}_{nameof(handler.SendDialog)}");
                fixed (char* pResult = result ?? string.Empty)
                {
                    p_callback(p_callback_handler, (param_string*) pResult);
                }
                Logger.LogOutput(result!, $"{nameof(DialogTestWarning)}_{nameof(handler.SendDialog)}");
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
    public static return_value_void* DialogTestFileOpen(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, param_string*, void> p_callback)
    {
        Logger.LogInput();
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_callback is null)
            //    return return_value_void.AsValue(false);

            handler.SendDialog(DialogType.FileOpen, "Test Title", "Test Message", new[] { new DialogFileFilter("Test Filter", new[] { "*.test" }) }, result =>
            {
                Logger.LogInput($"{nameof(DialogTestFileOpen)}_{nameof(handler.SendDialog)}");
                fixed (char* pResult = result ?? string.Empty)
                {
                    p_callback(p_callback_handler, (param_string*) pResult);
                }
                Logger.LogOutput(result!, $"{nameof(DialogTestFileOpen)}_{nameof(handler.SendDialog)}");
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