#ifndef VE_LAUNCHERMANAGER_CB_GUARD_HPP_
#define VE_LAUNCHERMANAGER_CB_GUARD_HPP_

#include "Bannerlord.LauncherManager.Native.h"
#include "Logger.hpp"
#include "Utils.Callbacks.hpp"
#include "Utils.Converters.hpp"
#include "Bindings.LauncherManager.hpp"

using namespace Napi;
using namespace Utils;
using namespace Bannerlord::LauncherManager::Native;

namespace Bindings::LauncherManager
{
    // Helper to get manager from owner pointer
    inline auto GetManager(param_ptr *p_owner)
    {
        return const_cast<Bindings::LauncherManager::LauncherManager *>(
            static_cast<const Bindings::LauncherManager::LauncherManager *>(p_owner));
    }

    // Non-blocking void callback helper with error handling
    template <typename TCallable>
    static return_value_void *NonBlockingVoidCallback(
        const char *functionName,
        param_ptr *p_owner,
        Napi::ThreadSafeFunction &tsfn,
        param_ptr *p_callback_handler,
        void (*p_callback)(param_ptr *, return_value_void *),
        TCallable &&jsCallLogic) noexcept
    {
        LoggerScope logger(functionName);
        return CallbackWithExceptionHandling<return_value_void>(logger, [&]()
                                                                {
            auto manager = GetManager(p_owner);

            if (std::this_thread::get_id() == manager->MainThreadId)
            {
                jsCallLogic(manager);
                return Create(return_value_void{nullptr});
            }
            else
            {
                const auto callback = [functionName, &jsCallLogic, manager, p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback)
                {
                    LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));
                    try
                    {
                        jsCallLogic(manager);
                    }
                    catch (const Napi::Error &e)
                    {
                        callbackLogger.LogError(e);
                        p_callback(p_callback_handler, Create(return_value_void{Copy(GetErrorMessage(e))}));
                    }
                };

                const auto status = tsfn.NonBlockingCall(callback);
                if (status != napi_ok)
                {
                    logger.Log("NonBlockingCall failed with status: " + std::to_string(status));
                    return Create(return_value_void{Copy(u"Failed to queue async call")});
                }
                return Create(return_value_void{nullptr});
            } });
    }

    // Blocking callback helper with synchronization
    // TReturnValue: the return value type for the callback
    // TMainThreadCall: callable for main thread execution (receives manager)
    // TBackgroundCall: callable for background execution (receives env, jsCallback, synchronization params)
    template <typename TReturnValue, typename TThreadSafeFunction, typename TMainThreadCall, typename TBackgroundCall>
    static return_value_void *BlockingCallback(
        const char *functionName,
        param_ptr *p_owner,
        TThreadSafeFunction &tsfn,
        param_ptr *p_callback_handler,
        void (*p_callback)(param_ptr *, TReturnValue *),
        TMainThreadCall &&mainThreadCall,
        TBackgroundCall &&backgroundCall) noexcept
    {
        LoggerScope logger(functionName);
        return CallbackWithExceptionHandling<return_value_void>(logger, [&]()
                                                                {
            auto manager = GetManager(p_owner);

            if (std::this_thread::get_id() == manager->MainThreadId)
            {
                mainThreadCall(manager);
                return Create(return_value_void{nullptr});
            }
            else
            {
                BlockingCallData blockingData;

                const auto callback = [functionName, p_callback_handler, p_callback, &backgroundCall, &blockingData](Napi::Env env, Napi::Function jsCallback)
                {
                    LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));
                    try
                    {
                        backgroundCall(env, jsCallback, &blockingData.mtx, &blockingData.cv, &blockingData.completed, &blockingData.result);
                    }
                    catch (const Napi::Error &e)
                    {
                        callbackLogger.LogError(e);
                        p_callback(p_callback_handler, CreateErrorResult<TReturnValue>(GetErrorMessage(e)));
                        SignalBlockingCallComplete(blockingData);
                    }
                };

                const auto status = tsfn.BlockingCall(callback);
                if (status != napi_ok)
                {
                    logger.Log("BlockingCall failed with status: " + std::to_string(status));
                    return Create(return_value_void{Copy(u"Failed to queue async call")});
                }

                return WaitForBlockingCall(logger, blockingData);
            } });
    }

    static return_value_void *setGameParameters(param_ptr *p_owner, param_string *p_executable, param_json *p_game_parameters, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_void *)) noexcept
    {
        auto manager = GetManager(p_owner);
        return NonBlockingVoidCallback(__FUNCTION__, p_owner, manager->TSFN, p_callback_handler, p_callback,
                                       [p_executable, p_game_parameters, p_callback_handler, p_callback](auto *mgr)
                                       {
                                           const auto env = mgr->FSetGameParameters.Env();
                                           const auto executable = p_executable == nullptr ? env.Null() : String::New(env, p_executable);
                                           const auto gameParameters = p_game_parameters == nullptr ? env.Null() : JSONParse(String::New(env, p_game_parameters));
                                           const auto jsResult = mgr->FSetGameParameters.Call({executable, gameParameters});
                                           HandleVoidPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                                       });
    }

    static return_value_void *sendNotification(param_ptr *p_owner, param_string *p_id, param_string *p_type, param_string *p_message, param_uint displayMs, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_void *)) noexcept
    {
        auto manager = GetManager(p_owner);
        return NonBlockingVoidCallback(__FUNCTION__, p_owner, manager->TSFN, p_callback_handler, p_callback,
                                       [p_id, p_type, p_message, displayMs, p_callback_handler, p_callback](auto *mgr)
                                       {
                                           const auto env = mgr->FSendNotification.Env();
                                           const auto id = p_id == nullptr ? env.Null() : String::New(env, p_id);
                                           const auto type = p_type == nullptr ? env.Null() : String::New(env, p_type);
                                           const auto message = p_message == nullptr ? env.Null() : String::New(env, p_message);
                                           const auto displayMs_ = Number::New(env, displayMs);
                                           const auto jsResult = mgr->FSendNotification.Call({id, type, message, displayMs_});
                                           HandleVoidPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                                       });
    }
    static return_value_void *sendDialog(param_ptr *p_owner, param_string *p_type, param_string *p_title, param_string *p_message, param_json *p_filters, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_string *)) noexcept
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);
        return CallbackWithExceptionHandling<return_value_void>(logger, [&]()
                                                                {
            auto manager = GetManager(p_owner);

            // Create resolve/reject handlers that will invoke the callback
            const auto onResolve = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                const auto result = info[0];
                if (result.IsNull() || result.IsUndefined())
                {
                    p_callback(p_callback_handler, Create(return_value_string{nullptr, nullptr}));
                }
                else
                {
                    const auto resultStr = result.As<Napi::String>();
                    p_callback(p_callback_handler, Create(return_value_string{nullptr, Copy(resultStr.Utf16Value())}));
                }
            };
            const auto onReject = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                if (info.Length() == 0)
                {
                    p_callback(p_callback_handler, Create(return_value_string{Copy(u"Unknown error"), nullptr}));
                }
                else
                {
                    const auto error = info[0].As<Napi::Error>();
                    p_callback(p_callback_handler, Create(return_value_string{Copy(GetErrorMessage(error)), nullptr}));
                }
            };

            // Helper to create arguments for FSendDialog
            auto createDialogArgs = [p_type, p_title, p_message, p_filters](const Napi::Env &env)
            {
                return std::vector<napi_value>{
                    p_type == nullptr ? env.Null() : String::New(env, p_type),
                    p_title == nullptr ? env.Null() : String::New(env, p_title),
                    p_message == nullptr ? env.Null() : String::New(env, p_message),
                    p_filters == nullptr ? env.Null() : JSONParse(String::New(env, p_filters))};
            };

            if (std::this_thread::get_id() == manager->MainThreadId)
            {
                // On main thread, call directly without blocking
                const auto env = manager->FSendDialog.Env();
                const auto args = createDialogArgs(env);
                const auto promise = manager->FSendDialog.Call(args).As<Napi::Promise>();

                // Attach .then() handlers directly
                const auto then = promise.Get("then").As<Napi::Function>();
                then.Call(promise, {Napi::Function::New(env, onResolve), Napi::Function::New(env, onReject)});
            }
            else
            {
                // Use the TSFN trampoline
                const auto callback = [functionName, manager, onResolve, onReject, createDialogArgs](Napi::Env env, Napi::Function jsCallback)
                {
                    LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));
                    const auto args = createDialogArgs(env);
                    const auto promise = manager->FSendDialog.Call(args);
                    jsCallback.Call({promise, Napi::Function::New(env, onResolve), Napi::Function::New(env, onReject)});
                };

                const auto status = manager->TSFN.NonBlockingCall(callback);
                if (status != napi_ok)
                {
                    logger.Log("NonBlockingCall failed with status: " + std::to_string(status));
                    return Create(return_value_void{Copy(u"Failed to queue async call")});
                }
            }

            return Create(return_value_void{nullptr}); });
    }
    static return_value_void *getInstallPath(param_ptr *p_owner, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_string *)) noexcept
    {
        auto manager = GetManager(p_owner);
        return BlockingCallback<return_value_string>(__FUNCTION__, p_owner, manager->TSFNGetInstallPath, p_callback_handler, p_callback,
                                                     [p_callback_handler, p_callback](auto *mgr)
                                                     {
                                                         const auto env = mgr->FGetInstallPath.Env();
                                                         const auto jsResult = mgr->FGetInstallPath.Call({});
                                                         HandleStringPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                                                     },
                                                     [p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback, std::mutex *mtx, std::condition_variable *cv, bool *completed, return_value_void **result)
                                                     {
                                                         const auto jsResult = jsCallback.Call({});
                                                         HandleStringPromiseOrValue(env, jsResult, p_callback_handler, p_callback, mtx, cv, completed, result);
                                                     });
    }
    static return_value_void *readFileContent(param_ptr *p_owner, param_string *p_file_path, param_int offset, param_int length, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_data *)) noexcept
    {
        auto manager = GetManager(p_owner);
        return BlockingCallback<return_value_data>(__FUNCTION__, p_owner, manager->TSFNReadFileContent, p_callback_handler, p_callback,
                                                   [p_file_path, offset, length, p_callback_handler, p_callback](auto *mgr)
                                                   {
                                                       const auto env = mgr->FReadFileContent.Env();
                                                       const auto filePath = p_file_path == nullptr ? env.Null() : String::New(env, p_file_path);
                                                       const auto offset_ = Number::New(env, offset);
                                                       const auto length_ = Number::New(env, length);
                                                       const auto jsResult = mgr->FReadFileContent.Call({filePath, offset_, length_});
                                                       HandleDataPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                                                   },
                                                   [p_file_path, offset, length, p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback, std::mutex *mtx, std::condition_variable *cv, bool *completed, return_value_void **result)
                                                   {
                                                       const auto filePath = p_file_path == nullptr ? env.Null() : String::New(env, p_file_path);
                                                       const auto offset_ = Number::New(env, offset);
                                                       const auto length_ = Number::New(env, length);
                                                       const auto jsResult = jsCallback.Call({filePath, offset_, length_});
                                                       HandleDataPromiseOrValue(env, jsResult, p_callback_handler, p_callback, mtx, cv, completed, result);
                                                   });
    }

    static return_value_void *writeFileContent(param_ptr *p_owner, param_string *p_file_path, param_data *p_data, param_int length, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_void *)) noexcept
    {
        auto manager = GetManager(p_owner);
        return BlockingCallback<return_value_void>(__FUNCTION__, p_owner, manager->TSFNWriteFileContent, p_callback_handler, p_callback,
                                                   [p_file_path, p_data, length, p_callback_handler, p_callback](auto *mgr)
                                                   {
                                                       const auto env = mgr->FWriteFileContent.Env();
                                                       const auto filePath = p_file_path == nullptr ? env.Null() : String::New(env, p_file_path);
                                                       const auto data = Buffer<uint8_t>::NewOrCopy(env, p_data, static_cast<size_t>(length));
                                                       const auto jsResult = mgr->FWriteFileContent.Call({filePath, data});
                                                       HandleVoidPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                                                   },
                                                   [p_file_path, p_data, length, p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback, std::mutex *mtx, std::condition_variable *cv, bool *completed, return_value_void **result)
                                                   {
                                                       const auto filePath = p_file_path == nullptr ? env.Null() : String::New(env, p_file_path);
                                                       const auto data = Buffer<uint8_t>::NewOrCopy(env, p_data, static_cast<size_t>(length));
                                                       const auto jsResult = jsCallback.Call({filePath, data});
                                                       HandleVoidPromiseOrValue(env, jsResult, p_callback_handler, p_callback, mtx, cv, completed, result);
                                                   });
    }

    static return_value_void *readDirectoryFileList(param_ptr *p_owner, param_string *p_directory_path, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_json *)) noexcept
    {
        auto manager = GetManager(p_owner);
        return BlockingCallback<return_value_json>(__FUNCTION__, p_owner, manager->TSFNReadDirectoryFileList, p_callback_handler, p_callback,
                                                   [p_directory_path, p_callback_handler, p_callback](auto *mgr)
                                                   {
                                                       const auto env = mgr->FReadDirectoryFileList.Env();
                                                       const auto directoryPath = p_directory_path == nullptr ? env.Null() : String::New(env, p_directory_path);
                                                       const auto jsResult = mgr->FReadDirectoryFileList.Call({directoryPath});
                                                       HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                                                   },
                                                   [p_directory_path, p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback, std::mutex *mtx, std::condition_variable *cv, bool *completed, return_value_void **result)
                                                   {
                                                       const auto directoryPath = p_directory_path == nullptr ? env.Null() : String::New(env, p_directory_path);
                                                       const auto jsResult = jsCallback.Call({directoryPath});
                                                       HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback, mtx, cv, completed, result);
                                                   });
    }

    static return_value_void *readDirectoryList(param_ptr *p_owner, param_string *p_directory_path, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_json *)) noexcept
    {
        auto manager = GetManager(p_owner);
        return BlockingCallback<return_value_json>(__FUNCTION__, p_owner, manager->TSFNReadDirectoryList, p_callback_handler, p_callback,
                                                   [p_directory_path, p_callback_handler, p_callback](auto *mgr)
                                                   {
                                                       const auto env = mgr->FReadDirectoryList.Env();
                                                       const auto directoryPath = p_directory_path == nullptr ? env.Null() : String::New(env, p_directory_path);
                                                       const auto jsResult = mgr->FReadDirectoryList.Call({directoryPath});
                                                       HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                                                   },
                                                   [p_directory_path, p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback, std::mutex *mtx, std::condition_variable *cv, bool *completed, return_value_void **result)
                                                   {
                                                       const auto directoryPath = p_directory_path == nullptr ? env.Null() : String::New(env, p_directory_path);
                                                       const auto jsResult = jsCallback.Call({directoryPath});
                                                       HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback, mtx, cv, completed, result);
                                                   });
    }

    static return_value_void *getAllModuleViewModels(param_ptr *p_owner, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_json *)) noexcept
    {
        auto manager = GetManager(p_owner);
        return BlockingCallback<return_value_json>(__FUNCTION__, p_owner, manager->TSFNGetAllModuleViewModels, p_callback_handler, p_callback,
                                                   [p_callback_handler, p_callback](auto *mgr)
                                                   {
                                                       const auto env = mgr->FGetAllModuleViewModels.Env();
                                                       const auto jsResult = mgr->FGetAllModuleViewModels.Call({});
                                                       HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                                                   },
                                                   [p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback, std::mutex *mtx, std::condition_variable *cv, bool *completed, return_value_void **result)
                                                   {
                                                       const auto jsResult = jsCallback.Call({});
                                                       HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback, mtx, cv, completed, result);
                                                   });
    }

    static return_value_void *getModuleViewModels(param_ptr *p_owner, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_json *)) noexcept
    {
        auto manager = GetManager(p_owner);
        return BlockingCallback<return_value_json>(__FUNCTION__, p_owner, manager->TSFNGetModuleViewModels, p_callback_handler, p_callback,
                                                   [p_callback_handler, p_callback](auto *mgr)
                                                   {
                                                       const auto env = mgr->FGetModuleViewModels.Env();
                                                       const auto jsResult = mgr->FGetModuleViewModels.Call({});
                                                       HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                                                   },
                                                   [p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback, std::mutex *mtx, std::condition_variable *cv, bool *completed, return_value_void **result)
                                                   {
                                                       const auto jsResult = jsCallback.Call({});
                                                       HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback, mtx, cv, completed, result);
                                                   });
    }
    static return_value_void *setModuleViewModels(param_ptr *p_owner, param_json *p_module_view_models, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_void *)) noexcept
    {
        auto manager = GetManager(p_owner);
        return NonBlockingVoidCallback(__FUNCTION__, p_owner, manager->TSFN, p_callback_handler, p_callback,
                                       [p_module_view_models, p_callback_handler, p_callback](auto *mgr)
                                       {
                                           const auto env = mgr->FSetModuleViewModels.Env();
                                           const auto moduleViewModels = p_module_view_models == nullptr ? env.Null() : JSONParse(Napi::String::New(env, p_module_view_models));
                                           const auto jsResult = mgr->FSetModuleViewModels.Call({moduleViewModels});
                                           HandleVoidPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                                       });
    }
    static return_value_void *getOptions(param_ptr *p_owner, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_json *)) noexcept
    {
        auto manager = GetManager(p_owner);
        return BlockingCallback<return_value_json>(__FUNCTION__, p_owner, manager->TSFNGetOptions, p_callback_handler, p_callback,
                                                   [p_callback_handler, p_callback](auto *mgr)
                                                   {
                                                       const auto env = mgr->FGetOptions.Env();
                                                       const auto jsResult = mgr->FGetOptions.Call({});
                                                       HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                                                   },
                                                   [p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback, std::mutex *mtx, std::condition_variable *cv, bool *completed, return_value_void **result)
                                                   {
                                                       const auto jsResult = jsCallback.Call({});
                                                       HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback, mtx, cv, completed, result);
                                                   });
    }

    static return_value_void *getState(param_ptr *p_owner, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_json *)) noexcept
    {
        auto manager = GetManager(p_owner);
        return BlockingCallback<return_value_json>(__FUNCTION__, p_owner, manager->TSFNGetState, p_callback_handler, p_callback,
                                                   [p_callback_handler, p_callback](auto *mgr)
                                                   {
                                                       const auto env = mgr->FGetState.Env();
                                                       const auto jsResult = mgr->FGetState.Call({});
                                                       HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                                                   },
                                                   [p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback, std::mutex *mtx, std::condition_variable *cv, bool *completed, return_value_void **result)
                                                   {
                                                       const auto jsResult = jsCallback.Call({});
                                                       HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback, mtx, cv, completed, result);
                                                   });
    }
}
#endif
