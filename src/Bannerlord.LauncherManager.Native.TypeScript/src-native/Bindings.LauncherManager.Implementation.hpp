#ifndef VE_LAUNCHERMANAGER_IMPL_GUARD_HPP_
#define VE_LAUNCHERMANAGER_IMPL_GUARD_HPP_

#include <thread>
#include "Bannerlord.LauncherManager.Native.h"
#include "Logger.hpp"
#include "Utils.Return.hpp"
#include "Bindings.LauncherManager.hpp"
#include "Bindings.LauncherManager.Callbacks.hpp"

using namespace Napi;
using namespace Utils;
using namespace Bannerlord::LauncherManager::Native;

namespace Bindings::LauncherManager
{
    // Promise-handling trampoline function - attaches .then() handlers to promises
    static void TSFNFunction(const Napi::CallbackInfo &info)
    {
        const auto length = info.Length();
        if (length == 2)
        {
            // Promise with resolve handler only
            const auto promise = info[0].As<Napi::Promise>();
            const auto onResolve = info[1].As<Napi::Function>();
            const auto then = promise.Get("then").As<Napi::Function>();
            then.Call(promise, {onResolve});
        }
        else if (length == 3)
        {
            // Promise with resolve and reject handlers
            const auto promise = info[0].As<Napi::Promise>();
            const auto onResolve = info[1].As<Napi::Function>();
            const auto onReject = info[2].As<Napi::Function>();
            const auto then = promise.Get("then").As<Napi::Function>();
            then.Call(promise, {onResolve, onReject});
        }
    }

    // Initialize native add-on
    Napi::Object Init(const Napi::Env env, const Napi::Object exports)
    {
        return LauncherManager::Init(env, exports);
    }

    Object LauncherManager::Init(const Napi::Env env, const Object exports)
    {
        // This method is used to hook the accessor and method callbacks
        const auto func = DefineClass(env, "LauncherManager",
                                      {
                                          InstanceMethod<&LauncherManager::CheckForRootHarmonyAsync>("checkForRootHarmonyAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::GetGameVersionAsync>("getGameVersionAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::GetModulesAsync>("getModulesAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::GetAllModulesAsync>("getAllModulesAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::GetSaveFilePathAsync>("getSaveFilePathAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::GetSaveFilesAsync>("getSaveFilesAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::GetSaveMetadataAsync>("getSaveMetadataAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::InstallModule>("installModule", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::IsObfuscatedAsync>("isObfuscatedAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::IsSorting>("isSorting", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::ModuleListHandlerExportAsync>("moduleListHandlerExportAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::ModuleListHandlerExportSaveFileAsync>("moduleListHandlerExportSaveFileAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::ModuleListHandlerImportAsync>("moduleListHandlerImportAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::ModuleListHandlerImportSaveFileAsync>("moduleListHandlerImportSaveFileAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::OrderByLoadOrderAsync>("orderByLoadOrderAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::RefreshGameParametersAsync>("refreshGameParametersAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::RefreshModulesAsync>("refreshModulesAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::SetGameParameterExecutableAsync>("setGameParameterExecutableAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::SetGameParameterSaveFileAsync>("setGameParameterSaveFileAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::SortAsync>("sortAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::SortHelperChangeModulePositionAsync>("sortHelperChangeModulePositionAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::SortHelperToggleModuleSelectionAsync>("sortHelperToggleModuleSelectionAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::SortHelperValidateModuleAsync>("sortHelperValidateModuleAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::TestModule>("testModule", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::DialogTestFileOpenAsync>("dialogTestFileOpenAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::DialogTestWarningAsync>("dialogTestWarningAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::SetGameStore>("setGameStore", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::GetGamePlatformAsync>("getGamePlatformAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::SetGameParameterContinueLastSaveFileAsync>("setGameParameterContinueLastSaveFileAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::SetGameParameterLoadOrderAsync>("setGameParameterLoadOrderAsync", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                      });

        auto *const constructor = new FunctionReference();

        // Create a persistent reference to the class constructor. This will allow
        // a function called on a class prototype and a function
        // called on instance of a class to be distinguished from each other.
        *constructor = Persistent(func);
        exports.Set("LauncherManager", func);

        // Store the constructor as the add-on instance data. This will allow this
        // add-on to support multiple instances of itself running on multiple worker
        // threads, as well as multiple instances of itself running in different
        // contexts on the same thread.
        //
        // By default, the value set on the environment here will be destroyed when
        // the add-on is unloaded using the `delete` operator, but it is also
        // possible to supply a custom deleter.
        env.SetInstanceData<FunctionReference>(constructor);

        return exports;
    }

    LauncherManager::LauncherManager(const CallbackInfo &info) : ObjectWrap<LauncherManager>(info)
    {
        const auto env = Env();
        this->FSetGameParameters = Persistent(info[0].As<Function>());
        this->FSendNotification = Persistent(info[1].As<Function>());
        this->FSendDialog = Persistent(info[2].As<Function>());
        this->FGetInstallPath = Persistent(info[3].As<Function>());
        this->FReadFileContent = Persistent(info[4].As<Function>());
        this->FWriteFileContent = Persistent(info[5].As<Function>());
        this->FReadDirectoryFileList = Persistent(info[6].As<Function>());
        this->FReadDirectoryList = Persistent(info[7].As<Function>());
        this->FGetAllModuleViewModels = Persistent(info[8].As<Function>());
        this->FGetModuleViewModels = Persistent(info[9].As<Function>());
        this->FSetModuleViewModels = Persistent(info[10].As<Function>());
        this->FGetOptions = Persistent(info[11].As<Function>());
        this->FGetState = Persistent(info[12].As<Function>());

        this->TSFNSetGameParameters = Napi::ThreadSafeFunction::New(env, this->FSetGameParameters.Value(), "SetGameParameters", 0, 1);
        this->TSFNSendNotification = Napi::ThreadSafeFunction::New(env, this->FSendNotification.Value(), "SendNotification", 0, 1);
        this->TSFNSendDialog = Napi::ThreadSafeFunction::New(env, this->FSendDialog.Value(), "SendDialog", 0, 1);
        this->TSFNGetInstallPath = Napi::ThreadSafeFunction::New(env, this->FGetInstallPath.Value(), "GetInstallPath", 0, 1);
        this->TSFNReadFileContent = Napi::ThreadSafeFunction::New(env, this->FReadFileContent.Value(), "ReadFileContent", 0, 1);
        this->TSFNWriteFileContent = Napi::ThreadSafeFunction::New(env, this->FWriteFileContent.Value(), "WriteFileContent", 0, 1);
        this->TSFNReadDirectoryFileList = Napi::ThreadSafeFunction::New(env, this->FReadDirectoryFileList.Value(), "ReadDirectoryFileList", 0, 1);
        this->TSFNReadDirectoryList = Napi::ThreadSafeFunction::New(env, this->FReadDirectoryList.Value(), "ReadDirectoryList", 0, 1);
        this->TSFNGetAllModuleViewModels = Napi::ThreadSafeFunction::New(env, this->FGetAllModuleViewModels.Value(), "GetAllModuleViewModels", 0, 1);
        this->TSFNGetModuleViewModels = Napi::ThreadSafeFunction::New(env, this->FGetModuleViewModels.Value(), "GetModuleViewModels", 0, 1);
        this->TSFNSetModuleViewModels = Napi::ThreadSafeFunction::New(env, this->FSetModuleViewModels.Value(), "SetModuleViewModels", 0, 1);
        this->TSFNGetOptions = Napi::ThreadSafeFunction::New(env, this->FGetOptions.Value(), "GetOptions", 0, 1);
        this->TSFNGetState = Napi::ThreadSafeFunction::New(env, this->FGetState.Value(), "GetState", 0, 1);

        // Create TSFN for promise-handling trampoline
        const auto tsfnFunc = Napi::Function::New(env, TSFNFunction, "TSFNFunction");
        this->TSFN = Napi::ThreadSafeFunction::New(env, tsfnFunc, "TSFN", 0, 1);

        const auto result = ve_create_handler(this,
                                              setGameParameters,
                                              sendNotification,
                                              sendDialog,
                                              getInstallPath,
                                              readFileContent,
                                              writeFileContent,
                                              readDirectoryFileList,
                                              readDirectoryList,
                                              getAllModuleViewModels,
                                              getModuleViewModels,
                                              setModuleViewModels,
                                              getOptions,
                                              getState);
        this->_pInstance = ThrowOrReturnPtr(env, result);

        this->MainThreadId = std::this_thread::get_id();
    }

    LauncherManager::~LauncherManager()
    {
        const auto env = Env();
        this->TSFNSetGameParameters.Release();
        this->TSFNSendNotification.Release();
        this->TSFNSendDialog.Release();
        this->TSFNGetInstallPath.Release();
        this->TSFNReadFileContent.Release();
        this->TSFNWriteFileContent.Release();
        this->TSFNReadDirectoryFileList.Release();
        this->TSFNReadDirectoryList.Release();
        this->TSFNGetAllModuleViewModels.Release();
        this->TSFNGetModuleViewModels.Release();
        this->TSFNSetModuleViewModels.Release();
        this->TSFNGetOptions.Release();
        this->TSFNGetState.Release();
        this->TSFN.Release();

        this->FSetGameParameters.Unref();
        this->FSendNotification.Unref();
        this->FSendDialog.Unref();
        this->FGetInstallPath.Unref();
        this->FReadFileContent.Unref();
        this->FWriteFileContent.Unref();
        this->FReadDirectoryFileList.Unref();
        this->FReadDirectoryList.Unref();
        this->FGetModuleViewModels.Unref();
        this->FSetModuleViewModels.Unref();
        this->FGetOptions.Unref();
        this->FGetState.Unref();

        ve_dispose_handler(this->_pInstance);
    }

    Napi::Value LauncherManager::GetGameVersionAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_get_game_version_async(this->_pInstance, cbData, HandleStringResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::TestModule(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);

        try
        {
            const auto env = info.Env();
            const auto files = JSONStringify(info[0].As<Object>());

            const auto filesCopy = CopyWithFree(files.Utf16Value());

            const auto result = ve_test_module(this->_pInstance, filesCopy.get());
            return ThrowOrReturnJson(env, result);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::InstallModule(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);

        try
        {
            const auto env = info.Env();
            const auto files = JSONStringify(info[0].As<Object>());
            const auto destinationPath = JSONStringify(info[1].As<Object>());

            const auto filesCopy = CopyWithFree(files.Utf16Value());
            const auto destinationPathCopy = CopyWithFree(destinationPath.Utf16Value());

            const auto result = ve_install_module(this->_pInstance, filesCopy.get(), destinationPathCopy.get());
            return ThrowOrReturnJson(env, result);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::IsObfuscatedAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();
            const auto module = JSONStringify(info[0].As<Object>());

            const auto moduleCopy = CopyWithFree(module.Utf16Value());

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_is_obfuscated_async(this->_pInstance, moduleCopy.get(), cbData, HandleBooleanResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::IsSorting(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);

        try
        {
            const auto env = info.Env();

            const auto result = ve_is_sorting(this->_pInstance);
            return ThrowOrReturnBoolean(env, result);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::SortAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);
        std::cout << "[SortAsync] invoked" << std::endl;

        try
        {
            const auto env = info.Env();

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            std::cout << "[SortAsync] Calling ve_sort_async" << std::endl;
            const auto result = ve_sort_async(this->_pInstance, cbData, HandleVoidResultCallback);
            std::cout << "[SortAsync] ve_sort_async returned" << std::endl;
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::GetModulesAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_get_modules_async(this->_pInstance, cbData, HandleJsonResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::GetAllModulesAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_get_all_modules_async(this->_pInstance, cbData, HandleJsonResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::RefreshGameParametersAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_refresh_game_parameters_async(this->_pInstance, cbData, HandleVoidResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::CheckForRootHarmonyAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_check_for_root_harmony_async(this->_pInstance, cbData, HandleVoidResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::ModuleListHandlerExportAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_module_list_handler_export_async(this->_pInstance, cbData, HandleVoidResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::ModuleListHandlerExportSaveFileAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();
            const auto saveFile = info[0].As<String>();

            const auto saveFileCopy = CopyWithFree(saveFile.Utf16Value());

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_module_list_handler_export_save_file_async(this->_pInstance, saveFileCopy.get(), cbData, HandleVoidResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::ModuleListHandlerImportAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_module_list_handler_import_async(this->_pInstance, cbData, HandleBooleanResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::ModuleListHandlerImportSaveFileAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();
            const auto saveFile = info[0].As<String>();

            const auto saveFileCopy = CopyWithFree(saveFile.Utf16Value());

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_module_list_handler_import_save_file_async(this->_pInstance, saveFileCopy.get(), cbData, HandleBooleanResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::RefreshModulesAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_refresh_modules_async(this->_pInstance, cbData, HandleVoidResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::SortHelperChangeModulePositionAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();
            const auto moduleViewModel = JSONStringify(info[0].As<Object>());
            const auto insertIndex = info[1].As<Number>();

            const auto moduleViewModelCopy = CopyWithFree(moduleViewModel.Utf16Value());

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_sort_helper_change_module_position_async(this->_pInstance, moduleViewModelCopy.get(), insertIndex.Int32Value(), cbData, HandleBooleanResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::SortHelperToggleModuleSelectionAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();
            const auto moduleViewModel = JSONStringify(info[0].As<Object>());

            const auto moduleViewModelCopy = CopyWithFree(moduleViewModel.Utf16Value());

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_sort_helper_toggle_module_selection_async(this->_pInstance, moduleViewModelCopy.get(), cbData, HandleJsonResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::SortHelperValidateModuleAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();
            const auto moduleViewModel = JSONStringify(info[0].As<Object>());

            const auto moduleViewModelCopy = CopyWithFree(moduleViewModel.Utf16Value());

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_sort_helper_validate_module_async(this->_pInstance, moduleViewModelCopy.get(), cbData, HandleJsonResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::GetSaveFilesAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_get_save_files_async(this->_pInstance, cbData, HandleJsonResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::GetSaveMetadataAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();
            const auto saveFile = info[0].As<String>();
            auto data = info[1].As<Uint8Array>();

            const auto saveFileCopy = CopyWithFree(saveFile.Utf16Value());
            const auto dataCopy = CopyWithFree(data.Data(), data.ByteLength());

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_get_save_metadata_async(this->_pInstance, saveFileCopy.get(), dataCopy.get(), static_cast<int32_t>(data.ByteLength()), cbData, HandleJsonResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::GetSaveFilePathAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();
            const auto saveFile = info[0].As<String>();

            const auto saveFileCopy = CopyWithFree(saveFile.Utf16Value());

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_get_save_file_path_async(this->_pInstance, saveFileCopy.get(), cbData, HandleStringResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::OrderByLoadOrderAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();
            const auto loadOrder = JSONStringify(info[0].As<Object>());

            const auto loadOrderCopy = CopyWithFree(loadOrder.Utf16Value());

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_order_by_load_order_async(this->_pInstance, loadOrderCopy.get(), cbData, HandleJsonResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::SetGameParameterExecutableAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();
            const auto executable = info[0].As<String>();

            const auto executableCopy = CopyWithFree(executable.Utf16Value());

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_set_game_parameter_executable_async(this->_pInstance, executableCopy.get(), cbData, HandleVoidResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::SetGameParameterSaveFileAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();
            const auto saveName = info[0].As<String>();

            const auto saveNameCopy = CopyWithFree(saveName.Utf16Value());

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_set_game_parameter_save_file_async(this->_pInstance, saveNameCopy.get(), cbData, HandleVoidResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::DialogTestWarningAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_dialog_test_warning_async(this->_pInstance, cbData, HandleStringResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::DialogTestFileOpenAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_dialog_test_file_open_async(this->_pInstance, cbData, HandleStringResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    void LauncherManager::SetGameStore(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);

        try
        {
            const auto env = info.Env();
            const auto gameStore = info[0].As<String>();

            const auto gameStoreCopy = CopyWithFree(gameStore.Utf16Value());

            const auto result = ve_set_game_store(this->_pInstance, gameStoreCopy.get());
            ThrowOrReturn(env, result);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::GetGamePlatformAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_get_game_platform_async(this->_pInstance, cbData, HandleStringResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::SetGameParameterContinueLastSaveFileAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();
            const auto value = info[0].As<Boolean>();

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_set_game_parameter_continue_last_save_file_async(this->_pInstance, value == true ? 1 : 0, cbData, HandleVoidResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }

    Napi::Value LauncherManager::SetGameParameterLoadOrderAsync(const CallbackInfo &info)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);

        try
        {
            const auto env = info.Env();
            const auto loadOrder = JSONStringify(info[0].As<Object>());
            const auto loadOrderCopy = CopyWithFree(loadOrder.Utf16Value());

            auto cbData = CreateResultCallbackData(env, functionName);
            const auto deferred = cbData->deferred;
            const auto tsfn = cbData->tsfn;

            const auto result = ve_set_game_parameter_load_order_async(this->_pInstance, loadOrderCopy.get(), cbData, HandleVoidResultCallback);
            return ReturnAndHandleReject(env, result, deferred, tsfn);
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            throw;
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            throw;
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            throw;
        }
    }
}
#endif
