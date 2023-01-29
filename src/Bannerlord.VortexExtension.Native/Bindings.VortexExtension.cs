using Bannerlord.VortexExtension.Models;

using BUTR.NativeAOT.Shared;

using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Bannerlord.VortexExtension.Native
{
    public static unsafe partial class Bindings
    {
        [UnmanagedCallersOnly(EntryPoint = "ve_create_handler")]
        public static return_value_ptr* CreateVortexExtensionHandler(void* p_owner)
        {
            Logger.LogInput();
            try
            {
                Logger.LogOutput();
                return return_value_ptr.AsValue(new VortexExtensionHandlerNative(p_owner).HandlePtr);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_ptr.AsError(Utils.Copy(e.ToString()));
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "ve_dispose_handler")]
        public static return_value_void* DisposeVortexExtensionHandler(void* p_handle)
        {
            Logger.LogInput();
            try
            {
                if (p_handle is null || VortexExtensionHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_void.AsError(Utils.Copy("Handler is null or wrong!"));

                handler.Dispose();

                Logger.LogOutput();
                return return_value_void.AsValue();
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_void.AsError(Utils.Copy(e.ToString()));
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "ve_get_game_version")]
        public static return_value_string* GetGameVersion(void* p_handle)
        {
            Logger.LogInput();
            try
            {
                if (p_handle is null || VortexExtensionHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_string.AsError(Utils.Copy("Handler is null or wrong!"));

                var result = handler.GetGameVersion();

                Logger.LogOutput(result);
                return return_value_string.AsValue(Utils.Copy(result));
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_string.AsError(Utils.Copy(e.ToString()));
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "ve_test_module")]
        public static return_value_json* TestModule(void* p_handle, param_json* p_files, param_string* p_game_id)
        {
            Logger.LogInput(p_files, p_game_id);
            try
            {
                if (p_handle is null || VortexExtensionHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_json.AsError(Utils.Copy("Handler is null or wrong!"));

                var files = Utils.DeserializeJson(p_files, CustomSourceGenerationContext.StringArray);
                var gameId = new string(param_string.ToSpan(p_game_id));

                var result = handler.TestModuleContent(files, gameId);

                Logger.LogOutputManaged(result);
                return return_value_json.AsValue(Utils.SerializeJsonCopy(result, CustomSourceGenerationContext.SupportedResult));
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "ve_install_module")]
        public static return_value_json* InstallModule(void* p_handle, param_json* p_files, param_string* p_destination_path)
        {
            Logger.LogInput(p_files, p_destination_path);
            try
            {
                if (p_handle is null || VortexExtensionHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_json.AsError(Utils.Copy("Handler is null or wrong!"));

                var files = Utils.DeserializeJson(p_files, CustomSourceGenerationContext.StringArray);
                var destinationPath = new string(param_string.ToSpan(p_destination_path));

                var result = handler.InstallModuleContent(files, destinationPath);

                Logger.LogOutputManaged(result);
                return return_value_json.AsValue(Utils.SerializeJsonCopy(result, CustomSourceGenerationContext.InstallResult));
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "ve_is_sorting")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static return_value_bool* IsSorting(void* p_handle)
        {
            Logger.LogInput();
            try
            {
                if (p_handle is null || VortexExtensionHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_bool.AsError(Utils.Copy("Handler is null or wrong!"));

                var result = handler.IsSorting;

                Logger.LogOutputPrimitive(result);
                return return_value_bool.AsValue(result);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_bool.AsError(Utils.Copy(e.ToString()));
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "ve_sort")]
        public static return_value_void* SortVortexExtension(void* p_handle)
        {
            Logger.LogInput();
            try
            {
                if (p_handle is null || VortexExtensionHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_void.AsError(Utils.Copy("Handler is null or wrong!"));

                handler.Sort();

                Logger.LogOutput();
                return return_value_void.AsValue();
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_void.AsError(Utils.Copy(e.ToString()));
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "ve_get_load_order")]
        public static return_value_json* GetLoadOrder(void* p_handle)
        {
            Logger.LogInput();
            try
            {
                if (p_handle is null || VortexExtensionHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_json.AsError(Utils.Copy("Handler is null or wrong!"));

                var result = handler.GetLoadOrder();

                Logger.LogOutputManaged(result);
                return return_value_json.AsValue(Utils.SerializeJsonCopy(result, CustomSourceGenerationContext.LoadOrder));
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "ve_set_load_order")]
        public static return_value_void* SetLoadOrder(void* p_handle, param_json* p_load_order)
        {
            Logger.LogInput(p_load_order);
            try
            {
                if (p_handle is null || VortexExtensionHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_void.AsError(Utils.Copy("Handler is null or wrong!"));

                var loadOrder = Utils.DeserializeJson(p_load_order, CustomSourceGenerationContext.LoadOrder);
                handler.SetLoadOrder(loadOrder);

                Logger.LogOutput();
                return return_value_void.AsValue();
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_void.AsError(Utils.Copy(e.ToString()));
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "ve_get_modules")]
        public static return_value_json* GetModules(void* p_handle)
        {
            Logger.LogInput();
            try
            {
                if (p_handle is null || VortexExtensionHandlerNative.FromPointer(p_handle) is not { } handler)
                    return return_value_json.AsError(Utils.Copy("Handler is null or wrong!"));

                var result = handler.GetModules().ToArray();

                Logger.LogOutputManaged(result);
                return return_value_json.AsValue(Utils.SerializeJsonCopy(result, CustomSourceGenerationContext.ModuleInfoExtendedArray));
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }

        /*
        [UnmanagedCallersOnly(EntryPoint = "ve_get_module_paths")]
        public static return_value_void* GetModulePaths(void* p_handle, param_json* p_load_order)
        {
            Logger.LogInput();

            try
            {

            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_void.AsError(Utils.Copy(e.ToString()));
            }
        }
        */


        private static Profile GetActiveProfile(VortexExtensionHandlerNative handler)
        {
            Logger.LogInput();

            using var result = SafeStructMallocHandle.Create(handler.D_GetActiveProfile(handler.OwnerPtr));
            using var profile = result.ValueAsJson();

            var returnResult = Utils.DeserializeJson(profile, CustomSourceGenerationContext.Profile);
            Logger.LogOutputManaged(returnResult);
            return returnResult;
        }

        private static Profile GetProfileById(VortexExtensionHandlerNative handler, ReadOnlySpan<char> profileId)
        {
            Logger.LogInput();

            fixed (char* pProfileId = profileId)
            {
                Logger.LogInputChar(pProfileId);

                using var result = SafeStructMallocHandle.Create(handler.D_GetProfileById(handler.OwnerPtr, (param_string*) pProfileId));
                using var profile = result.ValueAsJson();

                var returnResult = Utils.DeserializeJson(profile, CustomSourceGenerationContext.Profile);
                Logger.LogOutputManaged(returnResult);
                return returnResult;
            }
        }

        private static ReadOnlySpan<char> GetActiveGameId(VortexExtensionHandlerNative handler)
        {
            Logger.LogInput();

            using var result = SafeStructMallocHandle.Create(handler.D_GetActiveGameId(handler.OwnerPtr));
            using var gameId = result.ValueAsString();

            var returnResult = new string(gameId);
            Logger.LogOutput(returnResult);
            return returnResult;
        }

        private static void SetGameParameters(VortexExtensionHandlerNative handler, ReadOnlySpan<char> gameId, ReadOnlySpan<char> executable, string[] gameParameters)
        {
            Logger.LogInput();

            fixed (char* pGameId = gameId)
            fixed (char* pExecutable = executable)
            fixed (char* pGameParameters = Utils.SerializeJson(gameParameters, CustomSourceGenerationContext.StringArray))
            {
                Logger.LogInputChar(pGameId, pExecutable, pGameParameters);

                using var result = SafeStructMallocHandle.Create(handler.D_SetGameParameters(handler.OwnerPtr, (param_string*) pGameId, (param_string*) pExecutable, (param_json*) pGameParameters));
                result.ValueAsVoid();
            }

            Logger.LogOutput();
        }

        private static LoadOrder GetLoadOrder(VortexExtensionHandlerNative handler)
        {
            Logger.LogInput();

            using var result = SafeStructMallocHandle.Create(handler.D_GetLoadOrder(handler.OwnerPtr));
            using var loadOrder = result.ValueAsJson();

            var returnResult = Utils.DeserializeJson(loadOrder, CustomSourceGenerationContext.LoadOrder);
            Logger.LogOutputManaged(returnResult);
            return returnResult;
        }

        private static void SetLoadOrder(VortexExtensionHandlerNative handler, LoadOrder loadOrder)
        {
            Logger.LogInput();

            fixed (char* pLoadOrder = Utils.SerializeJson(loadOrder, CustomSourceGenerationContext.LoadOrder))
            {
                Logger.LogInputChar(pLoadOrder);

                using var result = SafeStructMallocHandle.Create(handler.D_SetLoadOrder(handler.OwnerPtr, (param_json*) pLoadOrder));
                result.ValueAsVoid();
            }

            Logger.LogOutput();
        }

        private static ReadOnlySpan<char> TranslateString(VortexExtensionHandlerNative handler, ReadOnlySpan<char> text, ReadOnlySpan<char> ns)
        {
            Logger.LogInput();

            fixed (char* pText = text)
            fixed (char* pNs = ns)
            {
                Logger.LogInputChar(pText, pNs);

                using var result = SafeStructMallocHandle.Create(handler.D_TranslateString(handler.OwnerPtr, (param_string*) pText, (param_string*) pNs));
                using var localized = result.ValueAsString();

                var returnResult = new string(localized);
                Logger.LogOutput(returnResult);
                return returnResult;
            }
        }

        private static void SendNotification(VortexExtensionHandlerNative handler, ReadOnlySpan<char> id, ReadOnlySpan<char> type, ReadOnlySpan<char> message, uint displayMs)
        {
            Logger.LogInput();

            fixed (char* pId = id)
            fixed (char* pType = type)
            fixed (char* pMessage = message)
            {
                Logger.LogInputChar(pId, pType, pMessage);

                using var result = SafeStructMallocHandle.Create(handler.D_SendNotification(handler.OwnerPtr, (param_string*) pId, (param_string*) pType, (param_string*) pMessage, displayMs));
                result.ValueAsVoid();
            }

            Logger.LogOutput();
        }

        private static ReadOnlySpan<char> GetInstallPath(VortexExtensionHandlerNative handler)
        {
            Logger.LogInput();

            using var result = SafeStructMallocHandle.Create(handler.D_GetInstallPath(handler.OwnerPtr));
            using var installPath = result.ValueAsString();

            var returnResult = new string(installPath);
            Logger.LogOutput(returnResult);
            return returnResult;
        }

        private static string? ReadFileContent(VortexExtensionHandlerNative handler, ReadOnlySpan<char> filePath)
        {
            Logger.LogInput();

            fixed (char* pFilePath = filePath)
            {
                Logger.LogInputChar(pFilePath);

                using var result = SafeStructMallocHandle.Create(handler.D_ReadFileContent(handler.OwnerPtr, (param_string*) pFilePath));
                if (result.IsNull) return null;
                using var content = result.ValueAsString();

                var returnResult = new string(content);
                Logger.LogOutput(returnResult);
                return returnResult;
            }
        }

        private static string[]? ReadDirectoryFileList(VortexExtensionHandlerNative handler, ReadOnlySpan<char> directoryPath)
        {
            Logger.LogInput();

            fixed (char* pDirectoryPath = directoryPath)
            {
                Logger.LogInputChar(pDirectoryPath);

                using var result = SafeStructMallocHandle.Create(handler.D_ReadDirectoryFileList(handler.OwnerPtr, (param_string*) pDirectoryPath));
                if (result.IsNull) return Array.Empty<string>();
                using var fileList = result.ValueAsJson();

                var returnResult = fileList.IsInvalid ? null : Utils.DeserializeJson(fileList, CustomSourceGenerationContext.StringArray);
                Logger.LogOutputManaged(returnResult!);
                return returnResult;
            }
        }

        private static string[]? ReadDirectoryList(VortexExtensionHandlerNative handler, ReadOnlySpan<char> directoryPath)
        {
            Logger.LogInput();

            fixed (char* pDirectoryPath = directoryPath)
            {
                Logger.LogInputChar(pDirectoryPath);

                using var result = SafeStructMallocHandle.Create(handler.D_ReadDirectoryList(handler.OwnerPtr, (param_string*) pDirectoryPath));
                if (result.IsNull) return Array.Empty<string>();
                using var directoryList = result.ValueAsJson();

                var returnResult = directoryList.IsInvalid ? null : Utils.DeserializeJson(directoryList, CustomSourceGenerationContext.StringArray);
                Logger.LogOutputManaged(returnResult!);
                return returnResult;
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "ve_register_callbacks")]
        public static return_value_void* RegisterCallbacks(void* p_handle
            , delegate* unmanaged[Cdecl]<return_value_json*, void*> p_get_active_profile
            , delegate* unmanaged[Cdecl]<return_value_json*, void*, param_string*> p_get_profile_by_id
            , delegate* unmanaged[Cdecl]<return_value_string*, void*> p_get_active_game_id
            , delegate* unmanaged[Cdecl]<return_value_void*, void*, param_string*, param_string*, param_json*> p_set_game_parameters
            , delegate* unmanaged[Cdecl]<return_value_json*, void*> p_get_load_order
            , delegate* unmanaged[Cdecl]<return_value_void*, void*, param_json*> p_set_load_order
            , delegate* unmanaged[Cdecl]<return_value_string*, void*, param_string*, param_string*, param_string*, uint> p_translate_string
            , delegate* unmanaged[Cdecl]<return_value_void*, void*> p_send_notification
            , delegate* unmanaged[Cdecl]<return_value_string*, void*> p_get_install_path
            , delegate* unmanaged[Cdecl]<return_value_string*, void*, param_string*> p_read_file_content
            , delegate* unmanaged[Cdecl]<return_value_json*, void*, param_string*> p_read_directory_file_list
            , delegate* unmanaged[Cdecl]<return_value_json*, void*, param_string*> p_read_directory_list
            )
        {
            Logger.LogInput();

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
                return return_value_void.AsValue();
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return return_value_void.AsError(Utils.Copy(e.ToString()));
            }
        }
    }
}