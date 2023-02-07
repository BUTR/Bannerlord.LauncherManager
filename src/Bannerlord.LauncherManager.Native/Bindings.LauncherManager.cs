using Bannerlord.LauncherManager.Models;

using BUTR.NativeAOT.Shared;

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bannerlord.LauncherManager.Native
{
    public static unsafe partial class Bindings
    {
        [UnmanagedCallersOnly(EntryPoint = "ve_create_handler", CallConvs = new[] { typeof(CallConvCdecl) }), IsNotConst<IsPtrConst>]
        public static return_value_ptr* CreateVortexExtensionHandler([IsConst<IsPtrConst>] param_ptr* p_owner)
        {
            Logger.LogInput();
            try
            {
                Logger.LogOutput();
                return return_value_ptr.AsValue(new LauncherManagerHandlerNative(p_owner).HandlePtr, false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_ptr.AsException(e, false);
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "ve_dispose_handler", CallConvs = new[] { typeof(CallConvCdecl) }), IsNotConst<IsPtrConst>]
        public static return_value_void* DisposeVortexExtensionHandler([IsConst<IsPtrConst>] param_ptr* p_handle)
        {
            Logger.LogInput();
            try
            {
                if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_void.AsError(Utils.Copy("Handler is null or wrong!", false), false);

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


        [UnmanagedCallersOnly(EntryPoint = "ve_get_game_version", CallConvs = new[] { typeof(CallConvCdecl) }), IsNotConst<IsPtrConst>]
        public static return_value_string* GetGameVersion([IsConst<IsPtrConst>] param_ptr* p_handle)
        {
            Logger.LogInput();
            try
            {
                if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_string.AsError(Utils.Copy("Handler is null or wrong!", false), false);

                var result = handler.GetGameVersion();

                Logger.LogOutput(result, nameof(GetGameVersion));
                return return_value_string.AsValue(Utils.Copy(result, false), false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_string.AsException(e, false);
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "ve_test_module", CallConvs = new[] { typeof(CallConvCdecl) }), IsNotConst<IsPtrConst>]
        public static return_value_json* TestModule([IsConst<IsPtrConst>] param_ptr* p_handle, [IsConst<IsPtrConst>] param_json* p_files, [IsConst<IsPtrConst>] param_string* p_game_id)
        {
            Logger.LogInput(p_files, p_game_id);
            try
            {
                if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_json.AsError(Utils.Copy("Handler is null or wrong!", false), false);

                var files = Utils.DeserializeJson(p_files, CustomSourceGenerationContext.StringArray);
                var gameId = new string(param_string.ToSpan(p_game_id));

                var result = handler.TestModuleContent(files, gameId);

                Logger.LogOutput(result);
                return return_value_json.AsValue(result, CustomSourceGenerationContext.SupportedResult, false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_json.AsException(e, false);
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "ve_install_module", CallConvs = new[] { typeof(CallConvCdecl) }), IsNotConst<IsPtrConst>]
        public static return_value_json* InstallModule([IsConst<IsPtrConst>] param_ptr* p_handle, [IsConst<IsPtrConst>] param_json* p_files, [IsConst<IsPtrConst>] param_string* p_destination_path)
        {
            Logger.LogInput(p_files, p_destination_path);
            try
            {
                if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_json.AsError(Utils.Copy("Handler is null or wrong!", false), false);

                var files = Utils.DeserializeJson(p_files, CustomSourceGenerationContext.StringArray);
                var destinationPath = new string(param_string.ToSpan(p_destination_path));

                var result = handler.InstallModuleContent(files, destinationPath);

                Logger.LogOutput(result);
                return return_value_json.AsValue(result, CustomSourceGenerationContext.InstallResult, false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_json.AsException(e, false);
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "ve_is_sorting", CallConvs = new[] { typeof(CallConvCdecl) }), IsNotConst<IsPtrConst>]
        public static return_value_bool* IsSorting([IsConst<IsPtrConst>] param_ptr* p_handle)
        {
            Logger.LogInput();
            try
            {
                if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_bool.AsError(Utils.Copy("Handler is null or wrong!", false), false);

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

        [UnmanagedCallersOnly(EntryPoint = "ve_sort", CallConvs = new[] { typeof(CallConvCdecl) }), IsNotConst<IsPtrConst>]
        public static return_value_void* SortVortexExtension([IsConst<IsPtrConst>] param_ptr* p_handle)
        {
            Logger.LogInput();
            try
            {
                if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_void.AsError(Utils.Copy("Handler is null or wrong!", false), false);

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


        [UnmanagedCallersOnly(EntryPoint = "ve_get_load_order", CallConvs = new[] { typeof(CallConvCdecl) }), IsNotConst<IsPtrConst>]
        public static return_value_json* GetLoadOrder([IsConst<IsPtrConst>] param_ptr* p_handle)
        {
            Logger.LogInput();
            try
            {
                if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_json.AsError(Utils.Copy("Handler is null or wrong!", false), false);

                var result = handler.GetLoadOrder();

                Logger.LogOutput(result);
                return return_value_json.AsValue(result, CustomSourceGenerationContext.LoadOrder, false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_json.AsException(e, false);
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "ve_set_load_order", CallConvs = new[] { typeof(CallConvCdecl) }), IsNotConst<IsPtrConst>]
        public static return_value_void* SetLoadOrder([IsConst<IsPtrConst>] param_ptr* p_handle, [IsConst<IsPtrConst>] param_json* p_load_order)
        {
            Logger.LogInput(p_load_order);
            try
            {
                if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_void.AsError(Utils.Copy("Handler is null or wrong!", false), false);

                var loadOrder = Utils.DeserializeJson(p_load_order, CustomSourceGenerationContext.LoadOrder);
                handler.SetLoadOrder(loadOrder);

                Logger.LogOutput();
                return return_value_void.AsValue(false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_void.AsException(e, false);
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "ve_get_modules", CallConvs = new[] { typeof(CallConvCdecl) }), IsNotConst<IsPtrConst>]
        public static return_value_json* GetModules([IsConst<IsPtrConst>] param_ptr* p_handle)
        {
            Logger.LogInput();
            try
            {
                if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_json.AsError(Utils.Copy("Handler is null or wrong!", false), false);

                var result = handler.GetModules().ToArray();

                Logger.LogOutput(result);
                return return_value_json.AsValue(result, CustomSourceGenerationContext.ModuleInfoExtendedArray, false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_json.AsException(e, false);
            }
        }
        

        [UnmanagedCallersOnly(EntryPoint = "ve_refresh_game_parameters", CallConvs = new[] { typeof(CallConvCdecl) }), IsNotConst<IsPtrConst>]
        public static return_value_void* RefreshGameParameters([IsConst<IsPtrConst>] param_ptr* p_handle, [IsConst<IsPtrConst>] param_json* p_load_order)
        {
            Logger.LogInput(p_load_order);
            try
            {
                if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_void.AsError(Utils.Copy("Handler is null or wrong!", false), false);

                var loadOrder = Utils.DeserializeJson(p_load_order, CustomSourceGenerationContext.LoadOrder);
                handler.RefreshGameParams(loadOrder);

                Logger.LogOutput();
                return return_value_void.AsValue(false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_void.AsException(e, false);
            }
        }

        
        /*
        [UnmanagedCallersOnly(EntryPoint = "ve_get_module_paths", CallConvs = new [] { typeof(CallConvCdecl) }), Const]
        public static return_value_void* GetModulePaths([Const(PointsToConstant = true)] param_ptr* p_handle, [Const(PointsToConstant = true)] param_json* p_load_order)
        {
            Logger.LogInput();

            try
            {

            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_void.AsException(e, false);
            }
        }
        */


        private static Profile GetActiveProfile(LauncherManagerHandlerNative handler)
        {
            Logger.LogInput();

            using var result = SafeStructMallocHandle.Create(handler.D_GetActiveProfile(handler.OwnerPtr), true);

            var returnResult = result.ValueAsJson(CustomSourceGenerationContext.Profile)!;
            Logger.LogOutput(returnResult);
            return returnResult;
        }

        private static Profile GetProfileById(LauncherManagerHandlerNative handler, ReadOnlySpan<char> profileId)
        {
            Logger.LogInput();

            fixed (char* pProfileId = profileId)
            {
                Logger.LogPinned(pProfileId);

                using var result = SafeStructMallocHandle.Create(handler.D_GetProfileById(handler.OwnerPtr, (param_string*) pProfileId), true);

                var returnResult = result.ValueAsJson(CustomSourceGenerationContext.Profile)!;
                Logger.LogOutput(returnResult);
                return returnResult;
            }
        }

        private static ReadOnlySpan<char> GetActiveGameId(LauncherManagerHandlerNative handler)
        {
            Logger.LogInput();

            using var result = SafeStructMallocHandle.Create(handler.D_GetActiveGameId(handler.OwnerPtr), true);
            using var gameId = result.ValueAsString();

            var returnResult = new string(gameId);
            Logger.LogOutput(returnResult, nameof(GetActiveGameId));
            return returnResult;
        }

        private static void SetGameParameters(LauncherManagerHandlerNative handler, ReadOnlySpan<char> gameId, ReadOnlySpan<char> executable, string[] gameParameters)
        {
            Logger.LogInput();

            fixed (char* pGameId = gameId)
            fixed (char* pExecutable = executable)
            fixed (char* pGameParameters = Utils.SerializeJson(gameParameters, CustomSourceGenerationContext.StringArray))
            {
                Logger.LogPinned(pGameId, pExecutable, pGameParameters);

                using var result = SafeStructMallocHandle.Create(handler.D_SetGameParameters(handler.OwnerPtr, (param_string*) pGameId, (param_string*) pExecutable, (param_json*) pGameParameters), true);
                result.ValueAsVoid();
            }

            Logger.LogOutput();
        }

        private static LoadOrder GetLoadOrder(LauncherManagerHandlerNative handler)
        {
            Logger.LogInput();

            using var result = SafeStructMallocHandle.Create(handler.D_GetLoadOrder(handler.OwnerPtr), true);

            var returnResult = result.ValueAsJson(CustomSourceGenerationContext.LoadOrder)!;
            Logger.LogOutput(returnResult);
            return returnResult;
        }

        private static void SetLoadOrder(LauncherManagerHandlerNative handler, LoadOrder loadOrder)
        {
            Logger.LogInput();

            fixed (char* pLoadOrder = Utils.SerializeJson(loadOrder, CustomSourceGenerationContext.LoadOrder))
            {
                Logger.LogPinned(pLoadOrder);

                using var result = SafeStructMallocHandle.Create(handler.D_SetLoadOrder(handler.OwnerPtr, (param_json*) pLoadOrder), true);
                result.ValueAsVoid();
            }

            Logger.LogOutput();
        }

        private static ReadOnlySpan<char> TranslateString(LauncherManagerHandlerNative handler, ReadOnlySpan<char> text, ReadOnlySpan<char> ns)
        {
            Logger.LogInput();

            fixed (char* pText = text)
            fixed (char* pNs = ns)
            {
                Logger.LogPinned(pText, pNs);

                using var result = SafeStructMallocHandle.Create(handler.D_TranslateString(handler.OwnerPtr, (param_string*) pText, (param_string*) pNs), true);
                using var localized = result.ValueAsString();

                var returnResult = new string(localized);
                Logger.LogOutput(returnResult, nameof(TranslateString));
                return returnResult;
            }
        }

        private static void SendNotification(LauncherManagerHandlerNative handler, ReadOnlySpan<char> id, ReadOnlySpan<char> type, ReadOnlySpan<char> message, uint displayMs)
        {
            Logger.LogInput();

            fixed (char* pId = id)
            fixed (char* pType = type)
            fixed (char* pMessage = message)
            {
                Logger.LogPinned(pId, pType, pMessage);

                using var result = SafeStructMallocHandle.Create(handler.D_SendNotification(handler.OwnerPtr, (param_string*) pId, (param_string*) pType, (param_string*) pMessage, displayMs), true);
                result.ValueAsVoid();
            }

            Logger.LogOutput();
        }

        private static ReadOnlySpan<char> GetInstallPath(LauncherManagerHandlerNative handler)
        {
            Logger.LogInput();

            using var result = SafeStructMallocHandle.Create(handler.D_GetInstallPath(handler.OwnerPtr), true);
            using var installPath = result.ValueAsString();

            var returnResult = new string(installPath);
            Logger.LogOutput(returnResult, nameof(GetInstallPath));
            return returnResult;
        }

        private static string? ReadFileContent(LauncherManagerHandlerNative handler, ReadOnlySpan<char> filePath)
        {
            Logger.LogInput();

            fixed (char* pFilePath = filePath)
            {
                Logger.LogPinned(pFilePath);

                using var result = SafeStructMallocHandle.Create(handler.D_ReadFileContent(handler.OwnerPtr, (param_string*) pFilePath), true);
                if (result.IsNull) return null;
                using var content = result.ValueAsString();

                var returnResult = new string(content);
                Logger.LogOutput(returnResult, nameof(GetInstallPath));
                return returnResult;
            }
        }

        private static string[]? ReadDirectoryFileList(LauncherManagerHandlerNative handler, ReadOnlySpan<char> directoryPath)
        {
            Logger.LogInput();

            fixed (char* pDirectoryPath = directoryPath)
            {
                Logger.LogPinned(pDirectoryPath);

                using var result = SafeStructMallocHandle.Create(handler.D_ReadDirectoryFileList(handler.OwnerPtr, (param_string*) pDirectoryPath), true);
                if (result.IsNull) return Array.Empty<string>();

                // TODO: Check null handle
                var returnResult = result.ValueAsJson(CustomSourceGenerationContext.StringArray)!;
                Logger.LogOutput(returnResult);
                return returnResult;
            }
        }

        private static string[]? ReadDirectoryList(LauncherManagerHandlerNative handler, ReadOnlySpan<char> directoryPath)
        {
            Logger.LogInput();

            fixed (char* pDirectoryPath = directoryPath)
            {
                Logger.LogPinned(pDirectoryPath);

                using var result = SafeStructMallocHandle.Create(handler.D_ReadDirectoryList(handler.OwnerPtr, (param_string*) pDirectoryPath), true);
                if (result.IsNull) return Array.Empty<string>();

                // TODO: Check null handle
                var returnResult = result.ValueAsJson(CustomSourceGenerationContext.StringArray)!;
                Logger.LogOutput(returnResult);
                return returnResult;
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "ve_register_callbacks", CallConvs = new[] { typeof(CallConvCdecl) }), IsNotConst<IsPtrConst>]
        public static return_value_void* RegisterCallbacks([IsConst<IsPtrConst>] param_ptr* p_handle,
            [ConstMeta<IsConst<IsPtrConst>, IsNotConst<IsPtrConst>>]
            delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*> p_get_active_profile,
            [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst<IsPtrConst>>]
            delegate* unmanaged[Cdecl]<param_ptr*, param_string*, return_value_json*> p_get_profile_by_id,
            [ConstMeta<IsConst<IsPtrConst>, IsNotConst<IsPtrConst>>]
            delegate* unmanaged[Cdecl]<param_ptr*, return_value_string*> p_get_active_game_id,
            [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst<IsPtrConst>>]
            delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_string*, param_json*, return_value_void*> p_set_game_parameters,
            [ConstMeta<IsConst<IsPtrConst>, IsNotConst<IsPtrConst>>]
            delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*> p_get_load_order,
            [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst<IsPtrConst>>]
            delegate* unmanaged[Cdecl]<param_ptr*, param_json*, return_value_void*> p_set_load_order,
            [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst<IsPtrConst>>]
            delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_string*, return_value_string*> p_translate_string,
            [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst, IsNotConst<IsPtrConst>>]
            delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_string*, param_string*, param_uint, return_value_void*> p_send_notification,
            [ConstMeta<IsConst<IsPtrConst>, IsNotConst<IsPtrConst>>]
            delegate* unmanaged[Cdecl]<param_ptr*, return_value_string*> p_get_install_path,
            [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst<IsPtrConst>>]
            delegate* unmanaged[Cdecl]<param_ptr*, param_string*, return_value_string*> p_read_file_content,
            [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst<IsPtrConst>>]
            delegate* unmanaged[Cdecl]<param_ptr*, param_string*, return_value_json*> p_read_directory_file_list,
            [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst<IsPtrConst>>]
            delegate* unmanaged[Cdecl]<param_ptr*, param_string*, return_value_json*> p_read_directory_list)
        {
            Logger.LogInput();

            try
            {
                if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_void.AsError(Utils.Copy("Handler is null or wrong!", false), false);

                handler.D_GetActiveProfile = Marshal.GetDelegateForFunctionPointer<N_GetActiveProfileDelegate>(new IntPtr(p_get_active_profile));
                handler.D_GetProfileById = Marshal.GetDelegateForFunctionPointer<N_GetProfileByIdDelegate>(new IntPtr(p_get_profile_by_id));
                handler.D_GetActiveGameId = Marshal.GetDelegateForFunctionPointer<N_GetActiveGameIdDelegate>(new IntPtr(p_get_active_game_id));
                handler.D_SetGameParameters = Marshal.GetDelegateForFunctionPointer<N_SetGameParametersDelegate>(new IntPtr(p_set_game_parameters));
                handler.D_GetLoadOrder = Marshal.GetDelegateForFunctionPointer<N_GetLoadOrderDelegate>(new IntPtr(p_get_load_order));
                handler.D_SetLoadOrder = Marshal.GetDelegateForFunctionPointer<N_SetLoadOrderDelegate>(new IntPtr(p_set_load_order));
                handler.D_TranslateString = Marshal.GetDelegateForFunctionPointer<N_TranslateStringDelegate>(new IntPtr(p_translate_string));
                handler.D_SendNotification = Marshal.GetDelegateForFunctionPointer<N_SendNotificationDelegate>(new IntPtr(p_send_notification));
                handler.D_GetInstallPath = Marshal.GetDelegateForFunctionPointer<N_GetInstallPathDelegate>(new IntPtr(p_get_install_path));
                handler.D_ReadFileContent = Marshal.GetDelegateForFunctionPointer<N_ReadFileContentDelegate>(new IntPtr(p_read_file_content));
                handler.D_ReadDirectoryFileList = Marshal.GetDelegateForFunctionPointer<N_ReadDirectoryFileList>(new IntPtr(p_read_directory_file_list));
                handler.D_ReadDirectoryList = Marshal.GetDelegateForFunctionPointer<N_ReadDirectoryList>(new IntPtr(p_read_directory_list));

                handler.RegisterCallbacks(
                    getActiveProfile: () => GetActiveProfile(handler),
                    getProfileById: profileId => (GetProfileById(handler, profileId)),
                    getActiveGameId: () => GetActiveGameId(handler),
                    setGameParameters: (executable, gameParameters) => SetGameParameters(handler, Constants.GameID, executable, gameParameters),
                    getLoadOrder: () => GetLoadOrder(handler),
                    setLoadOrder: (loadOrder) => SetLoadOrder(handler, loadOrder),
                    translateString: (text) => TranslateString(handler, text, Constants.I18Namespace),
                    sendNotification: (id, type, message, displayMs) => SendNotification(handler, id, type, message, displayMs),
                    getInstallPath: () => GetInstallPath(handler),
                    readFileContent: (filePath) => ReadFileContent(handler, filePath),
                    readDirectoryFileList: (directoryPath) => ReadDirectoryFileList(handler, directoryPath),
                    readDirectoryList: (directoryPath) => ReadDirectoryList(handler, directoryPath)
                );

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
}