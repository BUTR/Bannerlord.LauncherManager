using Bannerlord.ModuleManager;

using System;
using System.Runtime.InteropServices;
using Bannerlord.VortexExtension.Models;

using System.Linq;

namespace Bannerlord.VortexExtension.Native
{
    public static unsafe partial class Bindings
    {
        [UnmanagedCallersOnly(EntryPoint = "ve_create_handler")]
        public static return_value_ptr* CreateVortexExtensionHandler(void* p_owner)
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(CreateVortexExtensionHandler)}!");
#endif

            try
            {
                return return_value_ptr.AsValue(new VortexExtensionHandlerNative(p_owner).HandlePtr);
            }
            catch (Exception e)
            {
                return return_value_ptr.AsError(Utils.Copy(e.ToString()));
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "ve_dispose_handler")]
        public static return_value_void* DisposeVortexExtensionHandler(void* p_handle)
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(DisposeVortexExtensionHandler)}!");
#endif

            try
            {
                if (p_handle is null || VortexExtensionHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_void.AsError(Utils.Copy("Handler is null or wrong!"));

                handler.Dispose();
                return return_value_void.AsValue();
            }
            catch (Exception e)
            {
                return return_value_void.AsError(Utils.Copy(e.ToString()));
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "ve_get_game_version")]
        public static return_value_string* GetGameVersion(void* p_handle)
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(GetGameVersion)}!");
#endif

            try
            {
                if (p_handle is null || VortexExtensionHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_string.AsError(Utils.Copy("Handler is null or wrong!"));

                var result = handler.GetGameVersion();
#if LOGGING
                Logger.Log($"Result of {nameof(GetGameVersion)}: {result}");
#endif
                return return_value_string.AsValue(Utils.Copy(result));
            }
            catch (Exception e)
            {
#if LOGGING
                Logger.Log($"Exception of {nameof(GetGameVersion)}: {e}");
#endif
                return return_value_string.AsError(Utils.Copy(e.ToString()));
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "ve_test_module")]
        public static return_value_json* TestModule(void* p_handle, param_json* p_files, param_string* p_game_id)
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(TestModule)}! p_game_id: {param_string.ToSpan(p_game_id)}");
#endif

            try
            {
                if (p_handle is null || VortexExtensionHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_json.AsError(Utils.Copy("Handler is null or wrong!"));

                var files = Utils.DeserializeJson<string[]>(p_files, _customSourceGenerationContext.StringArray);
                var gameId = new string(param_string.ToSpan(p_game_id));

                var result = handler.TestModuleContent(files, gameId);
#if LOGGING
                Logger.Log($"Result of {nameof(TestModule)}: {result}");
#endif
                return return_value_json.AsValue(Utils.SerializeJsonCopy<SupportedResult>(result, _customSourceGenerationContext.SupportedResult));
            }
            catch (Exception e)
            {
#if LOGGING
                Logger.Log($"Exception of {nameof(TestModule)}: {e}");
#endif
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "ve_install_module")]
        public static return_value_json* InstallModule(void* p_handle, param_json* p_files, param_string* p_destination_path)
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(InstallModule)}! p_files: {param_json.ToSpan(p_files)}; p_destination_path: {param_string.ToSpan(p_destination_path)}");
#endif

            try
            {
                if (p_handle is null || VortexExtensionHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_json.AsError(Utils.Copy("Handler is null or wrong!"));

                var files = Utils.DeserializeJson<string[]>(p_files, _customSourceGenerationContext.StringArray);
                var destinationPath = new string(param_string.ToSpan(p_destination_path));

                var result = handler.InstallModuleContent(files, destinationPath);
#if LOGGING
                Logger.Log($"Result of {nameof(InstallModule)}: {result}");
#endif
                return return_value_json.AsValue(Utils.SerializeJsonCopy<InstallResult>(result, _customSourceGenerationContext.InstallResult));
            }
            catch (Exception e)
            {
#if LOGGING
                Logger.Log($"Exception of {nameof(InstallModule)}: {e}");
#endif
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "ve_is_sorting")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static return_value_bool* IsSorting(void* p_handle)
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(IsSorting)}!");
#endif

            try
            {
                if (p_handle is null || VortexExtensionHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_bool.AsError(Utils.Copy("Handler is null or wrong!"));

                var result = handler.IsSorting;
#if LOGGING
                Logger.Log($"Result of {nameof(IsSorting)}: {result}");
#endif
                return return_value_bool.AsValue(result);
            }
            catch (Exception e)
            {
#if LOGGING
                Logger.Log($"Exception of {nameof(IsSorting)}: {e}");
#endif
                return return_value_bool.AsError(Utils.Copy(e.ToString()));
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "ve_sort")]
        public static return_value_void* SortVortexExtension(void* p_handle)
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(SortVortexExtension)}!");
#endif

            try
            {
                if (p_handle is null || VortexExtensionHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_void.AsError(Utils.Copy("Handler is null or wrong!"));

                handler.Sort();
                return return_value_void.AsValue();
            }
            catch (Exception e)
            {
#if LOGGING
                Logger.Log($"Exception of {nameof(SortVortexExtension)}: {e}");
#endif
                return return_value_void.AsError(Utils.Copy(e.ToString()));
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "ve_get_load_order")]
        public static return_value_json* GetLoadOrder(void* p_handle)
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(GetLoadOrder)}!");
#endif

            try
            {
                if (p_handle is null || VortexExtensionHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_json.AsError(Utils.Copy("Handler is null or wrong!"));

                var result = handler.GetLoadOrder();
#if LOGGING
                Logger.Log($"Result of {nameof(GetLoadOrder)}: {result}");
#endif
                return return_value_json.AsValue(Utils.SerializeJsonCopy<LoadOrder>(result, _customSourceGenerationContext.LoadOrder));
            }
            catch (Exception e)
            {
#if LOGGING
                Logger.Log($"Exception of {nameof(GetLoadOrder)}: {e}");
#endif
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "ve_set_load_order")]
        public static return_value_void* SetLoadOrder(void* p_handle, param_json* p_load_order)
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(SetLoadOrder)}! p_load_order: {param_json.ToSpan(p_load_order)}");
#endif

            try
            {
                if (p_handle is null || VortexExtensionHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_void.AsError(Utils.Copy("Handler is null or wrong!"));

                var loadOrder = Utils.DeserializeJson<LoadOrder>(p_load_order, _customSourceGenerationContext.LoadOrder);
                handler.SetLoadOrder(loadOrder);
                return return_value_void.AsValue();
            }
            catch (Exception e)
            {
#if LOGGING
                Logger.Log($"Exception of {nameof(SetLoadOrder)}: {e}");
#endif
                return return_value_void.AsError(Utils.Copy(e.ToString()));
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "ve_get_modules")]
        public static return_value_json* GetModules(void* p_handle)
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(GetModules)}!");
#endif

            try
            {
                if (p_handle is null || VortexExtensionHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_json.AsError(Utils.Copy("Handler is null or wrong!"));

                var result = handler.GetModules().ToArray();
#if LOGGING
                Logger.Log($"Result of {nameof(GetModules)}: {result}");
#endif
                return return_value_json.AsValue(Utils.SerializeJsonCopy<ModuleInfoExtended[]>(result, _customSourceGenerationContext.ModuleInfoExtendedArray));
            }
            catch (Exception e)
            {
#if LOGGING
                Logger.Log($"Exception of {nameof(GetModules)}: {e}");
#endif
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }

        /*
        [UnmanagedCallersOnly(EntryPoint = "ve_get_module_paths")]
        public static return_value_void* GetModulePaths(void* p_handle, param_json* p_load_order)
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(GetModulePaths)}!");
#endif

            try
            {

            }
            catch (Exception e)
            {
                return return_value_void.AsError(Utils.Copy(e.ToString()));
            }
        }
        */


        private static Profile GetActiveProfile(VortexExtensionHandlerNative handler)
        {
#if LOGGING
            Logger.Log($"Received callback to {nameof(GetActiveProfile)}!");
#endif

            using var result = SafeStructMallocHandle.Create(handler.D_GetActiveProfile(handler.OwnerPtr));
            using var profile = result.ValueAsJson();
            var returnResult = Utils.DeserializeJson<Profile>(profile, _customSourceGenerationContext.Profile);
#if LOGGING
            Logger.Log($"Result of {nameof(GetActiveProfile)}: {returnResult}");
#endif
            return returnResult;
        }

        private static Profile GetProfileById(VortexExtensionHandlerNative handler, ReadOnlySpan<char> profileId)
        {
#if LOGGING
            Logger.Log($"Received callback to {nameof(GetProfileById)}! profileId: {profileId}");
#endif

            fixed (char* pProfileId = profileId)
            {
                using var result = SafeStructMallocHandle.Create(handler.D_GetProfileById(handler.OwnerPtr, (param_string*) pProfileId));
                using var profile = result.ValueAsJson();
                var returnResult = Utils.DeserializeJson<Profile>(profile, _customSourceGenerationContext.Profile);
#if LOGGING
                Logger.Log($"Result of {nameof(GetProfileById)}: {returnResult}");
#endif
                return returnResult;
            }
        }

        private static ReadOnlySpan<char> GetActiveGameId(VortexExtensionHandlerNative handler)
        {
#if LOGGING
            Logger.Log($"Received callback to {nameof(GetActiveGameId)}!");
#endif

            using var result = SafeStructMallocHandle.Create(handler.D_GetActiveGameId(handler.OwnerPtr));
            using var gameId = result.ValueAsString();
            var returnResult = new string(gameId);
#if LOGGING
            Logger.Log($"Result of {nameof(GetActiveGameId)}: {returnResult}");
#endif
            return returnResult;
        }

        private static void SetGameParameters(VortexExtensionHandlerNative handler, ReadOnlySpan<char> gameId, ReadOnlySpan<char> executable, string[] gameParameters)
        {
#if LOGGING
            Logger.Log($"Received callback to {nameof(SetGameParameters)}! gameId: {gameId}; executable: {executable}; gameParameters: {string.Join(", ", gameParameters)}");
#endif

            fixed (char* pGameId = gameId)
            fixed (char* pExecutable = executable)
            fixed (char* pGameParameters = Utils.SerializeJson<string[]>(gameParameters, _customSourceGenerationContext.StringArray))
            {
                using var result = SafeStructMallocHandle.Create(handler.D_SetGameParameters(handler.OwnerPtr, (param_string*) pGameId, (param_string*) pExecutable, (param_json*) pGameParameters));
                result.ValueAsVoid();
            }
        }

        private static LoadOrder GetLoadOrder(VortexExtensionHandlerNative handler)
        {
#if LOGGING
            Logger.Log($"Received callback to {nameof(GetLoadOrder)}!");
#endif

            using var result = SafeStructMallocHandle.Create(handler.D_GetLoadOrder(handler.OwnerPtr));
            using var loadOrder = result.ValueAsJson();
            var returnResult = Utils.DeserializeJson<LoadOrder>(loadOrder, _customSourceGenerationContext.LoadOrder);
#if LOGGING
            Logger.Log($"Result of {nameof(GetLoadOrder)}: {returnResult}");
#endif
            return returnResult;
        }

        private static void SetLoadOrder(VortexExtensionHandlerNative handler, LoadOrder loadOrder)
        {
#if LOGGING
            Logger.Log($"Received callback to {nameof(SetLoadOrder)}! loadOrder: {loadOrder}");
#endif

            fixed (char* pLoadOrder = Utils.SerializeJson<LoadOrder>(loadOrder, _customSourceGenerationContext.LoadOrder))
            {
                using var result = SafeStructMallocHandle.Create(handler.D_SetLoadOrder(handler.OwnerPtr, (param_json*) pLoadOrder));
                result.ValueAsVoid();
            }
        }

        private static ReadOnlySpan<char> TranslateString(VortexExtensionHandlerNative handler, ReadOnlySpan<char> text, ReadOnlySpan<char> ns)
        {
#if LOGGING
            Logger.Log($"Received callback to {nameof(TranslateString)}! text: {text} ns: {ns}");
#endif

            fixed (char* pText = text)
            fixed (char* pNs = ns)
            {
                using var result = SafeStructMallocHandle.Create(handler.D_TranslateString(handler.OwnerPtr, (param_string*) pText, (param_string*) pNs));
                using var localized = result.ValueAsString();
                var returnResult = new string(localized);
#if LOGGING
                Logger.Log($"Result of {nameof(TranslateString)}: {returnResult}");
#endif
                return returnResult;
            }
        }

        private static void SendNotification(VortexExtensionHandlerNative handler, ReadOnlySpan<char> id, ReadOnlySpan<char> type, ReadOnlySpan<char> message, uint displayMs)
        {
#if LOGGING
            Logger.Log($"Received callback to {nameof(SendNotification)}! id: {id}; type: {type}; message: {message}; displayMs: {displayMs}");
#endif

            fixed (char* pId = id)
            fixed (char* pType = type)
            fixed (char* pMessage = message)
            {
                using var result = SafeStructMallocHandle.Create(handler.D_SendNotification(handler.OwnerPtr, (param_string*) pId, (param_string*) pType, (param_string*) pMessage, displayMs));
                result.ValueAsVoid();
            }
        }

        private static ReadOnlySpan<char> GetInstallPath(VortexExtensionHandlerNative handler)
        {
#if LOGGING
            Logger.Log($"Received callback to {nameof(GetInstallPath)}!");
#endif

            using var result = SafeStructMallocHandle.Create(handler.D_GetInstallPath(handler.OwnerPtr));
            using var installPath = result.ValueAsString();
            var returnResult = new string(installPath);
#if LOGGING
            Logger.Log($"Result of {nameof(GetInstallPath)}: {returnResult}");
#endif
            return returnResult;
        }

        private static string? ReadFileContent(VortexExtensionHandlerNative handler, ReadOnlySpan<char> filePath)
        {
#if LOGGING
            Logger.Log($"Received callback to {nameof(ReadFileContent)}! filePath: {filePath}");
#endif

            fixed (char* pFilePath = filePath)
            {
                using var result = SafeStructMallocHandle.Create(handler.D_ReadFileContent(handler.OwnerPtr, (param_string*) pFilePath));
                if (result.IsNull) return null;
                using var content = result.ValueAsString();
                var returnResult = new string(content);
#if LOGGING
                Logger.Log($"Result of {nameof(ReadFileContent)}: {returnResult}");
#endif
                return returnResult;
            }
        }

        private static string[] ReadDirectoryFileList(VortexExtensionHandlerNative handler, ReadOnlySpan<char> directoryPath)
        {
#if LOGGING
            Logger.Log($"Received callback to {nameof(ReadDirectoryFileList)}!directoryPath: {directoryPath}");
#endif

            fixed (char* pDirectoryPath = directoryPath)
            {
                using var result = SafeStructMallocHandle.Create(handler.D_ReadDirectoryFileList(handler.OwnerPtr, (param_string*) pDirectoryPath));
                if (result.IsNull) return Array.Empty<string>();
                using var fileList = result.ValueAsJson();
                var returnResult = Utils.DeserializeJson<string[]>(fileList, _customSourceGenerationContext.StringArray);
#if LOGGING
                Logger.Log($"Result of {nameof(ReadDirectoryFileList)}: {returnResult}");
#endif
                return returnResult;
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "ve_register_callbacks")]
        public static return_value_void* RegisterCallbacks(void* p_handle
            , delegate* unmanaged[Cdecl]<return_value_json*, void*> p_get_active_profile
            , delegate* unmanaged[Cdecl]<return_value_json*, void*, char*> p_get_profile_by_id
            , delegate* unmanaged[Cdecl]<return_value_string*, void*> p_get_active_game_id
            , delegate* unmanaged[Cdecl]<return_value_void*, void*, char*, char*, char*> p_set_game_parameters
            , delegate* unmanaged[Cdecl]<return_value_json*, void*> p_get_load_order
            , delegate* unmanaged[Cdecl]<return_value_void*, void*, char*> p_set_load_order
            , delegate* unmanaged[Cdecl]<return_value_string*, void*, char*, char*, char*, uint> p_translate_string
            , delegate* unmanaged[Cdecl]<return_value_void*, void*> p_send_notification
            , delegate* unmanaged[Cdecl]<return_value_string*, void*> p_get_install_path
            , delegate* unmanaged[Cdecl]<return_value_string*, void*, char*> p_read_file_content
            , delegate* unmanaged[Cdecl]<return_value_json*, void*, char*> p_read_directory_file_list
            )
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(RegisterCallbacks)}!");
#endif

            try
            {
                if (p_handle is null || VortexExtensionHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_void.AsError(Utils.Copy("Handler is null or wrong!"));

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
                    readDirectoryFileList: (directoryPath) => ReadDirectoryFileList(handler, directoryPath)
                );
                return return_value_void.AsValue();
            }
            catch (Exception e)
            {
                return return_value_void.AsError(Utils.Copy(e.ToString()));
            }
        }
    }
}