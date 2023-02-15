using Bannerlord.LauncherManager.Models;
using Bannerlord.LauncherManager.Native.Models;

using BUTR.NativeAOT.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bannerlord.LauncherManager.Native;

public static unsafe partial class Bindings
{
    private static void SetGameParameters(LauncherManagerHandlerNative handler, ReadOnlySpan<char> executable, IReadOnlyList<string> gameParameters)
    {
        Logger.LogInput();

        fixed (char* pExecutable = executable)
        fixed (char* pGameParameters = BUTR.NativeAOT.Shared.Utils.SerializeJson(gameParameters, CustomSourceGenerationContext.IReadOnlyListString))
        {
            Logger.LogPinned(pExecutable, pGameParameters);

            using var result = SafeStructMallocHandle.Create(handler.D_SetGameParameters(handler.OwnerPtr, (param_string*) pExecutable, (param_json*) pGameParameters), true);
            result.ValueAsVoid();
        }

        Logger.LogOutput();
    }

    private static LoadOrder LoadLoadOrder(LauncherManagerHandlerNative handler)
    {
        Logger.LogInput();

        using var result = SafeStructMallocHandle.Create(handler.D_GetLoadOrder(handler.OwnerPtr), true);

        var returnResult = result.ValueAsJson(CustomSourceGenerationContext.LoadOrder)!;
        Logger.LogOutput(returnResult);
        return returnResult;
    }

    private static void SaveLoadOrder(LauncherManagerHandlerNative handler, LoadOrder loadOrder)
    {
        Logger.LogInput();

        fixed (char* pLoadOrder = BUTR.NativeAOT.Shared.Utils.SerializeJson(loadOrder, CustomSourceGenerationContext.LoadOrder))
        {
            Logger.LogPinned(pLoadOrder);

            using var result = SafeStructMallocHandle.Create(handler.D_SetLoadOrder(handler.OwnerPtr, (param_json*) pLoadOrder), true);
            result.ValueAsVoid();
        }

        Logger.LogOutput();
    }

    private static void SendNotification(LauncherManagerHandlerNative handler, ReadOnlySpan<char> id, ReadOnlySpan<char> type, ReadOnlySpan<char> message, uint displayMs)
    {
        Logger.LogInput(displayMs);

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

    private static void SendDialog(LauncherManagerHandlerNative handler, ReadOnlySpan<char> type, ReadOnlySpan<char> title, ReadOnlySpan<char> message, IReadOnlyList<DialogFileFilter> filters, Action<string> onResult)
    {
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        static void Callback(param_ptr* p_ptr, param_string* p_result)
        {
            Logger.LogInput($"{nameof(SendDialog)}_{nameof(Callback)}");
            var result = new string(param_string.ToSpan(p_result));
            Logger.LogOutput(result, $"{nameof(SendDialog)}_{nameof(Callback)}");

            var handle = GCHandle.FromIntPtr(new IntPtr(p_ptr));
            try
            {
                var callbackStorage = handle.Target as CallbackStorage;
                callbackStorage?.SetResult(result);
            }
            catch (Exception e)
            {
                Logger.LogException(e, $"{nameof(SendDialog)}_{nameof(Callback)}");
            }
            finally
            {
                handle.Free();
            }
        }

        Logger.LogInput();

        var pCallbackStorage = GCHandle.ToIntPtr(GCHandle.Alloc(new CallbackStorage(result => onResult((string) result)), GCHandleType.Normal)).ToPointer();
        fixed (char* pType = type)
        fixed (char* pTitle = title)
        fixed (char* pFilters = BUTR.NativeAOT.Shared.Utils.SerializeJson(filters, CustomSourceGenerationContext.IReadOnlyListDialogFileFilter))
        fixed (char* pMessage = message)
        {
            Logger.LogPinned(pType, pTitle, pFilters, pMessage);

            using var result = SafeStructMallocHandle.Create(handler.D_SendDialog(handler.OwnerPtr, (param_string*) pType, (param_string*) pTitle, (param_string*) pMessage, (param_json*) pFilters, (param_ptr*) pCallbackStorage, &Callback), true);
            result.ValueAsVoid();

            Logger.LogOutput();
        }
    }

    private static string GetInstallPath(LauncherManagerHandlerNative handler)
    {
        Logger.LogInput();

        using var result = SafeStructMallocHandle.Create(handler.D_GetInstallPath(handler.OwnerPtr), true);
        using var installPath = result.ValueAsString();

        var returnResult = new string(installPath);
        Logger.LogOutput(returnResult, nameof(GetInstallPath));
        return returnResult;
    }

    private static byte[]? ReadFileContent(LauncherManagerHandlerNative handler, ReadOnlySpan<char> filePath, int offset, int length)
    {
        Logger.LogInput(offset, length);

        fixed (char* pFilePath = filePath)
        {
            Logger.LogPinned(pFilePath);

            using var result = SafeStructMallocHandle.Create(handler.D_ReadFileContent(handler.OwnerPtr, (param_string*) pFilePath, offset, length), true);
            using var content = result.ValueAsData();
            if (content.IsInvalid) return null;

            var returnResult = content.ToSpan().ToArray();
            Logger.LogOutput(returnResult, nameof(GetInstallPath));
            return returnResult;
        }
    }

    private static void WriteFileContent(LauncherManagerHandlerNative handler, ReadOnlySpan<char> filePath, ReadOnlySpan<byte> data, int length)
    {
        Logger.LogInput(length);

        fixed (char* pFilePath = filePath)
        fixed (byte* pData = data)
        {
            Logger.LogPinned(pFilePath);

            using var result = SafeStructMallocHandle.Create(handler.D_WriteFileContent(handler.OwnerPtr, (param_string*) pFilePath, (param_data*) pData, length), true);
            result.ValueAsVoid();
        }
        Logger.LogOutput();
    }

    private static string[]? ReadDirectoryFileList(LauncherManagerHandlerNative handler, ReadOnlySpan<char> directoryPath)
    {
        Logger.LogInput();

        fixed (char* pDirectoryPath = directoryPath)
        {
            Logger.LogPinned(pDirectoryPath);

            using var result = SafeStructMallocHandle.Create(handler.D_ReadDirectoryFileList(handler.OwnerPtr, (param_string*) pDirectoryPath), true);

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
            if (result.IsNull) return null;

            // TODO: Check null handle
            var returnResult = result.ValueAsJson(CustomSourceGenerationContext.StringArray)!;
            Logger.LogOutput(returnResult);
            return returnResult;
        }
    }

    private static IModuleViewModel[]? GetModuleViewModels(LauncherManagerHandlerNative handler)
    {
        Logger.LogInput();

        using var result = SafeStructMallocHandle.Create(handler.D_GetModuleViewModels(handler.OwnerPtr), true);
        if (result.IsNull) return null;

        var returnResult = result.ValueAsJson(CustomSourceGenerationContext.IReadOnlyListModuleViewModel)?.OrderBy(x => x.Index).ToArray<IModuleViewModel>();
        Logger.LogOutput(returnResult);
        return returnResult;
    }

    private static void SetModuleViewModels(LauncherManagerHandlerNative handler, IReadOnlyList<IModuleViewModel> moduleViewModels)
    {
        Logger.LogInput();

        fixed (char* pModuleViewModels = BUTR.NativeAOT.Shared.Utils.SerializeJson((IReadOnlyList<ModuleViewModel>) moduleViewModels, CustomSourceGenerationContext.IReadOnlyListModuleViewModel))
        {
            Logger.LogPinned(pModuleViewModels);

            using var result = SafeStructMallocHandle.Create(handler.D_SetModuleViewModels(handler.OwnerPtr, (param_json*) pModuleViewModels), true);

        }
        Logger.LogOutput();
    }

    private static LauncherOptions GetOptions(LauncherManagerHandlerNative handler)
    {
        Logger.LogInput();

        using var result = SafeStructMallocHandle.Create(handler.D_GetOptions(handler.OwnerPtr), true);
        if (result.IsNull) return LauncherOptions.Empty;

        var returnResult = result.ValueAsJson(CustomSourceGenerationContext.LauncherOptions)!;
        Logger.LogOutput(returnResult);
        return returnResult;
    }

    private static LauncherState GetState(LauncherManagerHandlerNative handler)
    {
        Logger.LogInput();

        using var result = SafeStructMallocHandle.Create(handler.D_GetState(handler.OwnerPtr), true);
        if (result.IsNull) return LauncherState.Empty;

        var returnResult = result.ValueAsJson(CustomSourceGenerationContext.LauncherState)!;
        Logger.LogOutput(returnResult);
        return returnResult;
    }

    [UnmanagedCallersOnly(EntryPoint = "ve_register_callbacks", CallConvs = new[] { typeof(CallConvCdecl) }), IsNotConst<IsPtrConst>]
    public static return_value_void* RegisterCallbacks(param_ptr* p_handle,
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
        delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*> p_get_module_view_models,
        delegate* unmanaged[Cdecl]<param_ptr*, param_json*, return_value_void*> p_set_module_view_models,
        delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*> p_get_options,
        delegate* unmanaged[Cdecl]<param_ptr*, return_value_json*> p_get_state)
    {
        Logger.LogInput();

        try
        {
            if (p_handle is null || LauncherManagerHandlerNative.FromPointer(p_handle) is not { } handler)
                return return_value_void.AsError(BUTR.NativeAOT.Shared.Utils.Copy("Handler is null or wrong!", false), false);

            handler.D_SetGameParameters = Marshal.GetDelegateForFunctionPointer<N_SetGameParametersDelegate>(new IntPtr(p_set_game_parameters));
            handler.D_GetLoadOrder = Marshal.GetDelegateForFunctionPointer<N_GetLoadOrderDelegate>(new IntPtr(p_load_load_order));
            handler.D_SetLoadOrder = Marshal.GetDelegateForFunctionPointer<N_SetLoadOrderDelegate>(new IntPtr(p_save_load_order));
            handler.D_SendNotification = Marshal.GetDelegateForFunctionPointer<N_SendNotificationDelegate>(new IntPtr(p_send_notification));
            handler.D_SendDialog = Marshal.GetDelegateForFunctionPointer<N_SendDialogDelegate>(new IntPtr(p_send_dialog));
            handler.D_GetInstallPath = Marshal.GetDelegateForFunctionPointer<N_GetInstallPathDelegate>(new IntPtr(p_get_install_path));
            handler.D_ReadFileContent = Marshal.GetDelegateForFunctionPointer<N_ReadFileContentDelegate>(new IntPtr(p_read_file_content));
            handler.D_WriteFileContent = Marshal.GetDelegateForFunctionPointer<N_WriteFileContentDelegate>(new IntPtr(p_write_file_content));
            handler.D_ReadDirectoryFileList = Marshal.GetDelegateForFunctionPointer<N_ReadDirectoryFileList>(new IntPtr(p_read_directory_file_list));
            handler.D_ReadDirectoryList = Marshal.GetDelegateForFunctionPointer<N_ReadDirectoryList>(new IntPtr(p_read_directory_list));
            handler.D_GetModuleViewModels = Marshal.GetDelegateForFunctionPointer<N_GetModuleViewModels>(new IntPtr(p_get_module_view_models));
            handler.D_SetModuleViewModels = Marshal.GetDelegateForFunctionPointer<N_SetModuleViewModels>(new IntPtr(p_set_module_view_models));
            handler.D_GetOptions = Marshal.GetDelegateForFunctionPointer<N_GetOptions>(new IntPtr(p_get_options));
            handler.D_GetState = Marshal.GetDelegateForFunctionPointer<N_GetState>(new IntPtr(p_get_state));

            handler.RegisterCallbacks(
                setGameParameters: (executable, gameParameters) => SetGameParameters(handler, executable, gameParameters),
                loadLoadOrder: () => LoadLoadOrder(handler),
                saveLoadOrder: (loadOrder) => SaveLoadOrder(handler, loadOrder),
                sendNotification: (id, type, message, displayMs) => SendNotification(handler, id, type.ToString().ToLowerInvariant(), message, displayMs),
                sendDialog: (type, title, message, filters, onResult) => SendDialog(handler, type.ToString().ToLowerInvariant(), title, message, filters, onResult),
                getInstallPath: () => GetInstallPath(handler),
                readFileContent: (filePath, offset, length) => ReadFileContent(handler, filePath, offset, length),
                writeFileContent: (filePath, data) => WriteFileContent(handler, filePath, data, data?.Length ?? 0),
                readDirectoryFileList: (directoryPath) => ReadDirectoryFileList(handler, directoryPath),
                readDirectoryList: (directoryPath) => ReadDirectoryList(handler, directoryPath),
                getModuleViewModels: () => GetModuleViewModels(handler),
                setModuleViewModels: (moduleViewModels) => SetModuleViewModels(handler, moduleViewModels),
                getOptions: () => GetOptions(handler),
                getState: () => GetState(handler)
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