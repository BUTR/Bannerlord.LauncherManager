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
#if DEBUG
        using var logger = LogMethod();
#else
        using var logger = LogMethod();
#endif
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

            return return_value_ptr.AsValue(launcherManager.HandlePtr, false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_ptr.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_dispose_handler", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* DisposeHandler(param_ptr* p_handle)
    {
#if DEBUG
        using var logger = LogMethod();
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.Dispose();

            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_get_game_version_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* GetGameVersionAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_string*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod();
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.GetGameVersionAsync().ContinueWith(result =>
            {
#if DEBUG
                using var logger = LogCallbackMethod(nameof(GetGameVersionAsync));
#else
                using var logger = LogCallbackMethod(nameof(GetGameVersionAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_string.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_string.AsValue((string?) null, false));
                        logger.Log("Cancelled");
                    }
                    else
                    {
                        p_callback(p_callback_handler, return_value_string.AsValue(BUTR.NativeAOT.Shared.Utils.Copy(result.Result, false), false));
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_string.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_test_module", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_json* TestModule(param_ptr* p_handle, param_json* p_files)
    {
#if DEBUG
        using var logger = LogMethod(p_files);
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_json.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_files is null)
            //    return return_value_json.AsValue(SupportedResult.AsNotSupported, CustomSourceGenerationContext.SupportedResult, false);

            var files = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_files, CustomSourceGenerationContext.StringArray);

            var result = handler.TestModuleContent(files);

            return return_value_json.AsValue(result, CustomSourceGenerationContext.SupportedResult, false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_install_module", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_json* InstallModule(param_ptr* p_handle, param_json* p_files, param_json* p_module_infos)
    {
#if DEBUG
        using var logger = LogMethod(p_files, p_module_infos);
#else
        using var logger = LogMethod();
#endif
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

            return return_value_json.AsValue(result, CustomSourceGenerationContext.InstallResult, false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_is_sorting", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_bool* IsSorting(param_ptr* p_handle)
    {
#if DEBUG
        using var logger = LogMethod();
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_bool.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var result = handler.IsSorting;

            return return_value_bool.AsValue(result, false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_bool.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_sort_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* SortAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod();
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.SortAsync().ContinueWith(result =>
            {
#if DEBUG
                using var logger = LogCallbackMethod(nameof(SortAsync));
#else
                using var logger = LogCallbackMethod(nameof(SortAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_void.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_void.AsValue(false));
                        logger.Log("Cancelled");
                    }
                    else
                    {
                        p_callback(p_callback_handler, return_value_void.AsValue(false));
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_void.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_refresh_modules_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* RefreshModulesAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod();
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.RefreshModulesAsync().ContinueWith(result =>
            {
#if DEBUG
                using var logger = LogCallbackMethod(nameof(RefreshModulesAsync));
#else
                using var logger = LogCallbackMethod(nameof(RefreshModulesAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_void.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_void.AsValue(false));
                        logger.Log("Cancelled");
                    }
                    else
                    {
                        p_callback(p_callback_handler, return_value_void.AsValue(false));
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_void.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_refresh_game_parameters_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* RefreshGameParametersAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod();
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.RefreshGameParametersAsync().ContinueWith(result =>
            {
#if DEBUG
                using var logger = LogCallbackMethod(nameof(RefreshGameParametersAsync));
#else
                using var logger = LogCallbackMethod(nameof(RefreshGameParametersAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_void.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_void.AsValue(false));
                        logger.Log("Cancelled");
                    }
                    else
                    {
                        p_callback(p_callback_handler, return_value_void.AsValue(false));
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_void.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_get_modules_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* GetModulesAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod();
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.GetModulesAsync().ContinueWith(result =>
            {
#if DEBUG
                using var logger = LogCallbackMethod(nameof(GetModulesAsync));
#else
                using var logger = LogCallbackMethod(nameof(GetModulesAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_json.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_json.AsValue(null, CustomSourceGenerationContext.IReadOnlyListModuleInfoExtendedWithMetadata, false));
                        logger.Log("Cancelled");
                    }
                    else
                    {
                        p_callback(p_callback_handler, return_value_json.AsValue(result.Result, CustomSourceGenerationContext.IReadOnlyListModuleInfoExtendedWithMetadata, false));
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_json.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_get_all_modules_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* GetAllModulesAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod();
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.GetAllModulesAsync().ContinueWith(result =>
            {
#if DEBUG
                using var logger = LogCallbackMethod(nameof(GetAllModulesAsync));
#else
                using var logger = LogCallbackMethod(nameof(GetAllModulesAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_json.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_json.AsValue(null, CustomSourceGenerationContext.IReadOnlyListModuleInfoExtendedWithMetadata, false));
                        logger.Log("Cancelled");
                    }
                    else
                    {
                        p_callback(p_callback_handler, return_value_json.AsValue(result.Result, CustomSourceGenerationContext.IReadOnlyListModuleInfoExtendedWithMetadata, false));
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_json.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_check_for_root_harmony_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* CheckForRootHarmonyAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod();
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.CheckForRootHarmonyAsync().ContinueWith(result =>
            {
#if DEBUG
                using var logger = LogCallbackMethod(nameof(CheckForRootHarmonyAsync));
#else
                using var logger = LogCallbackMethod(nameof(CheckForRootHarmonyAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_void.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_void.AsValue(false));
                        logger.Log("Cancelled");
                    }
                    else
                    {
                        p_callback(p_callback_handler, return_value_void.AsValue(false));
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_void.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_module_list_handler_export_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* ModuleListHandlerExportAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod();
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var moduleListHandler = new ModuleListHandler(handler);
            moduleListHandler.ExportAsync().ContinueWith(result =>
            {
#if DEBUG
                using var logger = LogCallbackMethod(nameof(ModuleListHandlerExportAsync));
#else
                using var logger = LogCallbackMethod(nameof(ModuleListHandlerExportAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_void.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_void.AsValue(false));
                        logger.Log("Cancelled");
                    }
                    else
                    {
                        p_callback(p_callback_handler, return_value_void.AsValue(false));
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_void.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_module_list_handler_export_save_file_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* ModuleListHandlerExportSaveFileAsync(param_ptr* p_handle, param_string* p_save_file, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod(p_save_file);
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var saveFile = new string(param_string.ToSpan(p_save_file));

            var moduleListHandler = new ModuleListHandler(handler);
            moduleListHandler.ExportSaveFileAsync(saveFile).ContinueWith(result =>
            {
#if DEBUG
                using var logger = LogCallbackMethod(nameof(ModuleListHandlerExportSaveFileAsync));
#else
                using var logger = LogCallbackMethod(nameof(ModuleListHandlerExportSaveFileAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_void.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_void.AsValue(false));
                        logger.Log("Cancelled");
                    }
                    else
                    {
                        p_callback(p_callback_handler, return_value_void.AsValue(false));
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_void.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_module_list_handler_import_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* ModuleListHandlerImportAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_bool*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod();
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_callback is null)
            //    return return_value_void.AsValue(false);

            var moduleListHandler = new ModuleListHandler(handler);
            moduleListHandler.ImportAsync().ContinueWith(result =>
            {
#if DEBUG
                using var logger = LogCallbackMethod(nameof(ModuleListHandlerImportAsync));
#else
                using var logger = LogCallbackMethod(nameof(ModuleListHandlerImportAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_bool.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_bool.AsValue(false, false));
                        logger.Log("Cancelled");
                    }
                    else
                    {
                        p_callback(p_callback_handler, return_value_bool.AsValue(result.Result, false));
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_bool.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_module_list_handler_import_save_file_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* ModuleListHandlerImportSaveFileAsync(param_ptr* p_handle, param_string* p_save_file, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_bool*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod(p_save_file);
#else
        using var logger = LogMethod();
#endif
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
#if DEBUG
                using var logger = LogCallbackMethod(nameof(ModuleListHandlerImportSaveFileAsync));
#else
                using var logger = LogCallbackMethod(nameof(ModuleListHandlerImportSaveFileAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_bool.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_bool.AsValue(false, false));
                        logger.Log("Cancelled");
                    }
                    else
                    {
                        p_callback(p_callback_handler, return_value_bool.AsValue(result.Result, false));
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_bool.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_sort_helper_validate_module_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* SortHelperValidateModuleAsync(param_ptr* p_handle, param_json* p_module_view_model, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod(p_module_view_model);
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_module_view_model is null)
            //    return return_value_json.AsValue([], CustomSourceGenerationContext.StringArray, false);

            var moduleViewModel = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module_view_model, CustomSourceGenerationContext.ModuleViewModel);

            handler.GetModuleViewModelsAsync().ContinueWith(result =>
            {
#if DEBUG
                using var logger = LogCallbackMethod(nameof(SortHelperValidateModuleAsync));
#else
                using var logger = LogCallbackMethod(nameof(SortHelperValidateModuleAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_json.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_json.AsValue(null, CustomSourceGenerationContext.StringArray, false));
                        logger.Log("Cancelled");
                    }
                    else
                    {
                        var modules = result.Result?.ToArray() ?? [];
                        var lookup = modules.ToDictionary(x => x.ModuleInfoExtended.Id, x => x);
                        var validateResult = SortHelper.ValidateModule(modules, lookup, moduleViewModel).ToArray();
                        p_callback(p_callback_handler, return_value_json.AsValue(validateResult, CustomSourceGenerationContext.StringArray, false));
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_json.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_sort_helper_toggle_module_selection_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* SortHelperToggleModuleSelectionAsync(param_ptr* p_handle, param_json* p_module_view_model, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod(p_module_view_model);
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_module_view_model is null)
            //    return return_value_json.AsValue([], CustomSourceGenerationContext.StringArray, false);

            var moduleViewModel = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module_view_model, CustomSourceGenerationContext.ModuleViewModel);

            handler.GetModuleViewModelsAsync().ContinueWith(result =>
            {
#if DEBUG
                using var logger = LogCallbackMethod(nameof(SortHelperToggleModuleSelectionAsync));
#else
                using var logger = LogCallbackMethod(nameof(SortHelperToggleModuleSelectionAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_json.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_json.AsValue(null, CustomSourceGenerationContext.ModuleViewModel, false));
                        logger.Log("Cancelled");
                    }
                    else
                    {
                        var modules = result.Result?.ToArray() ?? [];
                        var lookup = modules.ToDictionary(x => x.ModuleInfoExtended.Id, x => x);
                        SortHelper.ToggleModuleSelection(modules, lookup, moduleViewModel);
                        p_callback(p_callback_handler, return_value_json.AsValue(moduleViewModel, CustomSourceGenerationContext.ModuleViewModel, false));
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_json.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_sort_helper_change_module_position_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* SortHelperChangeModulePositionAsync(param_ptr* p_handle, param_json* p_module_view_model, param_int insertIndex, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_bool*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod(p_module_view_model, &insertIndex);
#else
        using var logger = LogMethod();
#endif

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
#if DEBUG
                using var logger = LogCallbackMethod(nameof(SortHelperChangeModulePositionAsync));
#else
                using var logger = LogCallbackMethod(nameof(SortHelperChangeModulePositionAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_bool.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                        return result;
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_bool.AsValue(false, false));
                        logger.Log("Cancelled");
                        return result;
                    }
                    else
                    {
                        return AfterGetModuleViewModelsAsync(handler, moduleViewModel, insertIndexInt, positionResult =>
                        {
                            p_callback(p_callback_handler, return_value_bool.AsValue(positionResult, false));
                        }, e =>
                        {
                            using var logger = LogCallbackMethod($"{nameof(SortHelperChangeModulePositionAsync)}{nameof(AfterGetModuleViewModelsAsync)}");

                            p_callback(p_callback_handler, return_value_bool.AsException(e, false));
                            logger.LogException(e);
                        }, result);
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_bool.AsException(e, false));
                    logger.LogException(e);
                    return Task.FromException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }
    private static async Task AfterGetModuleViewModelsAsync(LauncherManagerHandlerNative handler, ModuleViewModel moduleViewModel, param_int insertIndex, Action<bool> onResult, Action<Exception> onException, Task<IEnumerable<IModuleViewModel>?> result)
    {
        if (result.Exception is not null)
        {
            onException(result.Exception);
        }
        else
        {
            var modules = result.Result?.ToArray() ?? [];
            var lookup = modules.ToDictionary(x => x.ModuleInfoExtended.Id, x => x);
            var (positionResult, issues) = SortHelper.ChangeModulePosition(modules, lookup, moduleViewModel, insertIndex);
            if (issues is { Count: > 0 })
                await handler.ShowHintAsync(new BUTRTextObject("{=sP1a61KE}Failed to place the module to the desired position! Placing to the nearest available!{NL}Reason:{NL}{REASONS}").SetTextVariable("REASONS", string.Join("\n", issues)).ToString());
            onResult(positionResult);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_get_save_files_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* GetSaveFilesAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod();
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.GetSaveFilesAsync().ContinueWith(result =>
            {
#if DEBUG
                using var logger = LogCallbackMethod(nameof(GetSaveFilesAsync));
#else
                using var logger = LogCallbackMethod(nameof(GetSaveFilesAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_json.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_json.AsValue(null, CustomSourceGenerationContext.IReadOnlyListSaveMetadata, false));
                        logger.Log("Cancelled");
                    }
                    else
                    {
                        p_callback(p_callback_handler, return_value_json.AsValue(result.Result, CustomSourceGenerationContext.IReadOnlyListSaveMetadata, false));
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_json.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_get_save_metadata_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* GetSaveMetadataAsync(param_ptr* p_handle, param_string* p_save_file, param_data* p_data, param_int data_len, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod(p_save_file, p_data, &data_len);
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var saveFile = new string(param_string.ToSpan(p_save_file));
            var data = param_data.ToSpan(p_data, data_len).ToArray(); // TODO: 

            handler.GetSaveMetadataAsync(saveFile, data).ContinueWith(result =>
            {
#if DEBUG
                using var logger = LogCallbackMethod(nameof(GetSaveMetadataAsync));
#else
                using var logger = LogCallbackMethod(nameof(GetSaveMetadataAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_json.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_json.AsValue(null, CustomSourceGenerationContext.SaveMetadata, false));
                        logger.Log("Cancelled");
                    }
                    else
                    {
                        p_callback(p_callback_handler, return_value_json.AsValue(result.Result, CustomSourceGenerationContext.SaveMetadata, false));
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_json.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_get_save_file_path_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* GetSaveFilePathAsync(param_ptr* p_handle, param_string* p_save_file, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_string*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod(p_save_file);
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var saveFile = new string(param_string.ToSpan(p_save_file));

            handler.GetSaveFilePathAsync(saveFile).ContinueWith(result =>
            {
#if DEBUG
                using var logger = LogCallbackMethod(nameof(GetSaveFilePathAsync));
#else
                using var logger = LogCallbackMethod(nameof(GetSaveFilePathAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_string.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_string.AsValue((string?) null, false));
                        logger.Log("Cancelled");
                    }
                    else
                    {
                        p_callback(p_callback_handler, return_value_string.AsValue(result.Result, false));
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_string.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }


    public record OrderByLoadOrderResultNative(bool Result, IReadOnlyList<string>? Issues, IReadOnlyList<ModuleViewModel>? OrderedModuleViewModels);

    [UnmanagedCallersOnly(EntryPoint = "ve_order_by_load_order_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* OrderByLoadOrderAsync(param_ptr* p_handle, param_json* p_load_order, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod(p_load_order);
#else
        using var logger = LogMethod();
#endif
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
#if DEBUG
                using var logger = LogCallbackMethod(nameof(OrderByLoadOrderAsync));
#else
                using var logger = LogCallbackMethod(nameof(OrderByLoadOrderAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_json.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_json.AsValue(null, CustomSourceGenerationContext.OrderByLoadOrderResultNative, false));
                        logger.Log("Cancelled");
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
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_json.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_set_game_parameter_executable_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* SetGameParameterExecutableAsync(param_ptr* p_handle, param_string* p_executable, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod(p_executable);
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var executable = new string(param_string.ToSpan(p_executable));

            handler.SetGameParameterExecutableAsync(executable).ContinueWith(result =>
            {
#if DEBUG
                using var logger = LogCallbackMethod(nameof(SetGameParameterExecutableAsync));
#else
                using var logger = LogCallbackMethod(nameof(SetGameParameterExecutableAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_void.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_void.AsValue(false));
                        logger.Log("Cancelled");
                    }
                    else
                    {
                        p_callback(p_callback_handler, return_value_void.AsValue(false));
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_void.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_set_game_parameter_load_order_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* SetGameParameterLoadOrderAsync(param_ptr* p_handle, param_json* p_load_order, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod(p_load_order);
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_load_order is null)
            //    return return_value_void.AsValue(false);

            var loadOrder = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_load_order, CustomSourceGenerationContext.LoadOrder);

            handler.SetGameParameterLoadOrderAsync(loadOrder).ContinueWith(result =>
            {
#if DEBUG
                using var logger = LogCallbackMethod(nameof(SetGameParameterLoadOrderAsync));
#else
                using var logger = LogCallbackMethod(nameof(SetGameParameterLoadOrderAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_void.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_void.AsValue(false));
                        logger.Log("Cancelled");
                    }
                    else
                    {
                        p_callback(p_callback_handler, return_value_void.AsValue(false));
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_void.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_set_game_parameter_save_file_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* SetGameParameterSaveFileAsync(param_ptr* p_handle, param_string* p_save_file, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod(p_save_file);
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var saveFile = new string(param_string.ToSpan(p_save_file));

            handler.SetGameParameterSaveFileAsync(saveFile).ContinueWith(result =>
            {
#if DEBUG
                using var logger = LogCallbackMethod(nameof(SetGameParameterSaveFileAsync));
#else
                using var logger = LogCallbackMethod(nameof(SetGameParameterSaveFileAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_void.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_void.AsValue(false));
                        logger.Log("Cancelled");
                    }
                    else
                    {
                        p_callback(p_callback_handler, return_value_void.AsValue(false));
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_void.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_set_game_parameter_continue_last_save_file_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* SetGameParameterContinueLastSaveFileAsync(param_ptr* p_handle, param_bool p_value, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_void*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod(&p_value);
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.SetGameParameterContinueLastSaveFileAsync(p_value).ContinueWith(result =>
            {
#if DEBUG
                using var logger = LogCallbackMethod(nameof(SetGameParameterContinueLastSaveFileAsync));
#else
                using var logger = LogCallbackMethod(nameof(SetGameParameterContinueLastSaveFileAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_void.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_void.AsValue(false));
                        logger.Log("Cancelled");
                    }
                    else
                    {
                        p_callback(p_callback_handler, return_value_void.AsValue(false));
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_void.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_set_game_store", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_void* SetGameStore(param_ptr* p_handle, param_string* p_game_store)
    {
#if DEBUG
        using var logger = LogMethod(p_game_store);
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            var gameStoreStr = new string(param_string.ToSpan(p_game_store));
            var gameStore = Enum.Parse<GameStore>(gameStoreStr);

            handler.SetGameStore(gameStore);

            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_get_game_platform_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* GetGamePlatformAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_string*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod();
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.GetPlatformAsync().ContinueWith(result =>
            {
#if DEBUG
                using var logger = LogCallbackMethod(nameof(GetGamePlatformAsync));
#else
                using var logger = LogCallbackMethod(nameof(GetGamePlatformAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_string.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_string.AsValue((string?) null, false));
                        logger.Log("Cancelled");
                    }
                    else
                    {
                        var platform = result.Result.ToStringFast();
                        p_callback(p_callback_handler, return_value_string.AsValue(platform, false));
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_string.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_is_obfuscated_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* IsObfuscatedAsync(param_ptr* p_handle, param_json* p_module, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_bool*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod(p_module);
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);


            var module = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module, CustomSourceGenerationContext.ModuleInfoExtendedWithMetadata);

            handler.IsObfuscatedAsync(module).ContinueWith(result =>
            {
#if DEBUG
                using var logger = LogCallbackMethod(nameof(IsObfuscatedAsync));
#else
                using var logger = LogCallbackMethod(nameof(IsObfuscatedAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_bool.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_bool.AsValue(false, false));
                        logger.Log("Cancelled");
                    }
                    else
                    {
                        p_callback(p_callback_handler, return_value_bool.AsValue(result.Result, false));
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_bool.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "ve_dialog_test_warning_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* DialogTestWarningAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_string*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod();
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_callback is null)
            //    return return_value_void.AsValue(false);

            handler.SendDialogAsync(DialogType.Warning, "Test Title", "Test Message", Array.Empty<DialogFileFilter>()).ContinueWith(result =>
            {
#if DEBUG
                using var logger = LogCallbackMethod(nameof(DialogTestWarningAsync));
#else
                using var logger = LogCallbackMethod(nameof(DialogTestWarningAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_string.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_string.AsValue((string?) null, false));
                        logger.Log("Cancelled");
                    }
                    else
                    {
                        p_callback(p_callback_handler, return_value_string.AsValue(result.Result, false));
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_string.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_dialog_test_file_open_async", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe return_value_async* DialogTestFileOpenAsync(param_ptr* p_handle, param_ptr* p_callback_handler, delegate* unmanaged[Cdecl]<param_ptr*, return_value_string*, void> p_callback)
    {
#if DEBUG
        using var logger = LogMethod();
#else
        using var logger = LogMethod();
#endif
        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_async.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            //if (p_callback is null)
            //    return return_value_void.AsValue(false);

            handler.SendDialogAsync(DialogType.FileOpen, "Test Title", "Test Message", [new DialogFileFilter("Test Filter", ["*.test"])]).ContinueWith(result =>
            {
#if DEBUG
                using var logger = LogCallbackMethod(nameof(DialogTestFileOpenAsync));
#else
                using var logger = LogCallbackMethod(nameof(DialogTestFileOpenAsync));
#endif

                try
                {
                    if (result.Exception is not null)
                    {
                        p_callback(p_callback_handler, return_value_string.AsException(result.Exception, false));
                        logger.LogException(result.Exception);
                    }
                    else if (result.IsCanceled)
                    {
                        p_callback(p_callback_handler, return_value_string.AsValue((string?) null, false));
                        logger.Log("Cancelled");
                    }
                    else
                    {
                        p_callback(p_callback_handler, return_value_string.AsValue(result.Result, false));
                    }
                }
                catch (Exception e)
                {
                    p_callback(p_callback_handler, return_value_string.AsException(e, false));
                    logger.LogException(e);
                }
            });

            return return_value_async.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_async.AsException(e, false);
        }
    }
}