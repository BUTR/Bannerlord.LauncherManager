#ifndef VE_LAUNCHERMANAGER_CB_GUARD_HPP_
#define VE_LAUNCHERMANAGER_CB_GUARD_HPP_

#include <mutex>
#include <condition_variable>
#include <thread>
#include <iostream>
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
    static return_value_void *setGameParameters(param_ptr *p_owner, param_string *p_executable, param_json *p_game_parameters, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_void *)) noexcept
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);
        std::cout << "[setGameParameters] invoked" << std::endl;
        try
        {
            auto manager = const_cast<Bindings::LauncherManager::LauncherManager *>(static_cast<const Bindings::LauncherManager::LauncherManager *>(p_owner));

            if (std::this_thread::get_id() == manager->MainThreadId)
            {
                std::cout << "[setGameParameters] On main thread" << std::endl;
                const auto env = manager->FSetGameParameters.Env();

                const auto executable = p_executable == nullptr ? env.Null() : String::New(env, p_executable);
                const auto gameParameters = p_game_parameters == nullptr ? env.Null() : JSONParse(String::New(env, p_game_parameters));
                const auto jsResult = manager->FSetGameParameters.Call({executable, gameParameters});

                HandleVoidPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                std::cout << "[setGameParameters] returning" << std::endl;
                return Create(return_value_void{nullptr});
            }
            else
            {
                std::cout << "[setGameParameters] Not on main thread, using TSFN" << std::endl;
                const auto callback = [functionName, manager, p_executable, p_game_parameters, p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback)
                {
                    std::cout << "[setGameParameters] TSFN callback invoked" << std::endl;
                    LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));
                    try
                    {
                        const auto executable = p_executable == nullptr ? env.Null() : String::New(env, p_executable);
                        const auto gameParameters = p_game_parameters == nullptr ? env.Null() : JSONParse(String::New(env, p_game_parameters));
                        const auto jsResult = manager->FSetGameParameters.Call({executable, gameParameters});

                        HandleVoidPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                    }
                    catch (const Napi::Error &e)
                    {
                        callbackLogger.LogError(e);
                        p_callback(p_callback_handler, Create(return_value_void{Copy(GetErrorMessage(e))}));
                    }
                    std::cout << "[setGameParameters] TSFN callback done" << std::endl;
                };

                const auto status = manager->TSFN.NonBlockingCall(callback);
                if (status != napi_ok)
                {
                    logger.Log("NonBlockingCall failed with status: " + std::to_string(status));
                    return Create(return_value_void{Copy(u"Failed to queue async call")});
                }
                std::cout << "[setGameParameters] NonBlockingCall queued, returning" << std::endl;
                return Create(return_value_void{nullptr});
            }
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_void{Copy(conv.from_bytes(e.what()))});
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            return Create(return_value_void{Copy(u"Unknown exception")});
        }
    }
    static return_value_void *sendNotification(param_ptr *p_owner, param_string *p_id, param_string *p_type, param_string *p_message, param_uint displayMs, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_void *)) noexcept
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);
        std::cout << "[sendNotification] invoked" << std::endl;
        try
        {
            auto manager = const_cast<Bindings::LauncherManager::LauncherManager *>(static_cast<const Bindings::LauncherManager::LauncherManager *>(p_owner));

            if (std::this_thread::get_id() == manager->MainThreadId)
            {
                std::cout << "[sendNotification] On main thread" << std::endl;
                const auto env = manager->FSendNotification.Env();

                const auto id = p_id == nullptr ? env.Null() : String::New(env, p_id);
                const auto type = p_type == nullptr ? env.Null() : String::New(env, p_type);
                const auto message = p_message == nullptr ? env.Null() : String::New(env, p_message);
                const auto displayMs_ = Number::New(env, displayMs);
                const auto jsResult = manager->FSendNotification.Call({id, type, message, displayMs_});

                HandleVoidPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                std::cout << "[sendNotification] returning" << std::endl;
                return Create(return_value_void{nullptr});
            }
            else
            {
                std::cout << "[sendNotification] Not on main thread, using TSFN" << std::endl;
                const auto callback = [functionName, manager, p_id, p_type, p_message, displayMs, p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback)
                {
                    std::cout << "[sendNotification] TSFN callback invoked" << std::endl;
                    LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));
                    try
                    {
                        const auto id = p_id == nullptr ? env.Null() : String::New(env, p_id);
                        const auto type = p_type == nullptr ? env.Null() : String::New(env, p_type);
                        const auto message = p_message == nullptr ? env.Null() : String::New(env, p_message);
                        const auto displayMs_ = Number::New(env, displayMs);
                        const auto jsResult = manager->FSendNotification.Call({id, type, message, displayMs_});

                        HandleVoidPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                    }
                    catch (const Napi::Error &e)
                    {
                        callbackLogger.LogError(e);
                        p_callback(p_callback_handler, Create(return_value_void{Copy(GetErrorMessage(e))}));
                    }
                    std::cout << "[sendNotification] TSFN callback done" << std::endl;
                };

                const auto status = manager->TSFN.NonBlockingCall(callback);
                if (status != napi_ok)
                {
                    logger.Log("NonBlockingCall failed with status: " + std::to_string(status));
                    return Create(return_value_void{Copy(u"Failed to queue async call")});
                }
                std::cout << "[sendNotification] NonBlockingCall queued, returning" << std::endl;
                return Create(return_value_void{nullptr});
            }
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_void{Copy(conv.from_bytes(e.what()))});
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            return Create(return_value_void{Copy(u"Unknown exception")});
        }
    }
    static return_value_void *sendDialog(param_ptr *p_owner, param_string *p_type, param_string *p_title, param_string *p_message, param_json *p_filters, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_string *)) noexcept
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);
        try
        {
            auto manager = const_cast<Bindings::LauncherManager::LauncherManager *>(static_cast<const Bindings::LauncherManager::LauncherManager *>(p_owner));

            // Create resolve/reject handlers that will invoke the callback
            const auto onResolve = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                std::cout << "[sendDialog onResolve] Handler invoked" << std::endl;
                const auto env = info.Env();
                const auto result = info[0];
                if (result.IsNull() || result.IsUndefined())
                {
                    std::cout << "[sendDialog onResolve] Result is null/undefined" << std::endl;
                    p_callback(p_callback_handler, Create(return_value_string{nullptr, nullptr}));
                }
                else
                {
                    const auto resultStr = result.As<Napi::String>();
                    std::cout << "[sendDialog onResolve] Result: " << resultStr.Utf8Value() << std::endl;
                    p_callback(p_callback_handler, Create(return_value_string{nullptr, Copy(resultStr.Utf16Value())}));
                }
                std::cout << "[sendDialog onResolve] Handler completed" << std::endl;
            };
            const auto onReject = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                std::cout << "[sendDialog onReject] Handler invoked" << std::endl;
                const auto env = info.Env();
                if (info.Length() == 0)
                {
                    p_callback(p_callback_handler, Create(return_value_string{Copy(u"Unknown error"), nullptr}));
                }
                else
                {
                    const auto error = info[0].As<Napi::Error>();
                    p_callback(p_callback_handler, Create(return_value_string{Copy(GetErrorMessage(error)), nullptr}));
                }
                std::cout << "[sendDialog onReject] Handler completed" << std::endl;
            };

            // Use the TSFN trampoline pattern - queue the work and let the promise resolve asynchronously
            const auto callback = [functionName, manager, p_type, p_title, p_message, p_filters, onResolve, onReject](Napi::Env env, Napi::Function jsCallback)
            {
                LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));
                const auto type = p_type == nullptr ? env.Null() : String::New(env, p_type);
                const auto title = p_title == nullptr ? env.Null() : String::New(env, p_title);
                const auto message = p_message == nullptr ? env.Null() : String::New(env, p_message);
                const auto filters = p_filters == nullptr ? env.Null() : JSONParse(String::New(env, p_filters));

                // Call the JS callback to get the promise
                const auto promise = manager->FSendDialog.Call({type, title, message, filters});

                // Pass the promise and handlers to the TSFN trampoline which will attach .then()
                const auto onResolveCallback = Napi::Function::New(env, onResolve);
                const auto onRejectCallback = Napi::Function::New(env, onReject);
                jsCallback.Call({promise, onResolveCallback, onRejectCallback});
            };

            if (std::this_thread::get_id() == manager->MainThreadId)
            {
                std::cout << "[sendDialog] On main thread, calling FSendDialog directly" << std::endl;
                // On main thread, call directly without blocking (since blocking would deadlock)
                const auto env = manager->FSendDialog.Env();
                const auto type = p_type == nullptr ? env.Null() : String::New(env, p_type);
                const auto title = p_title == nullptr ? env.Null() : String::New(env, p_title);
                const auto message = p_message == nullptr ? env.Null() : String::New(env, p_message);
                const auto filters = p_filters == nullptr ? env.Null() : JSONParse(String::New(env, p_filters));

                std::cout << "[sendDialog] Calling FSendDialog.Call()" << std::endl;
                const auto promise = manager->FSendDialog.Call({type, title, message, filters}).As<Napi::Promise>();
                std::cout << "[sendDialog] Got promise, attaching .then() handlers" << std::endl;

                // Attach .then() handlers directly
                const auto onResolveCallback = Napi::Function::New(env, onResolve);
                const auto onRejectCallback = Napi::Function::New(env, onReject);
                const auto then = promise.Get("then").As<Napi::Function>();
                then.Call(promise, {onResolveCallback, onRejectCallback});
                std::cout << "[sendDialog] .then() handlers attached, returning" << std::endl;
            }
            else
            {
                // Use the TSFN trampoline (not TSFNSendDialog) so the jsCallback is TSFNFunction
                const auto status = manager->TSFN.NonBlockingCall(callback);
                if (status != napi_ok)
                {
                    logger.Log("NonBlockingCall failed with status: " + std::to_string(status));
                    return Create(return_value_void{Copy(u"Failed to queue async call")});
                }
            }

            // Return immediately - the callback will be invoked when the promise resolves
            return Create(return_value_void{nullptr});
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_void{Copy(conv.from_bytes(e.what()))});
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            return Create(return_value_void{Copy(u"Unknown exception")});
        }
    }
    static return_value_void *getInstallPath(param_ptr *p_owner, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_string *)) noexcept
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);
        std::cout << "[getInstallPath] invoked" << std::endl;
        try
        {
            auto manager = const_cast<Bindings::LauncherManager::LauncherManager *>(static_cast<const Bindings::LauncherManager::LauncherManager *>(p_owner));

            if (std::this_thread::get_id() == manager->MainThreadId)
            {
                std::cout << "[getInstallPath] On main thread" << std::endl;
                const auto env = manager->FGetInstallPath.Env();

                const auto jsResult = manager->FGetInstallPath.Call({});
                std::cout << "[getInstallPath] FGetInstallPath.Call() returned" << std::endl;

                HandleStringPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                std::cout << "[getInstallPath] HandleStringPromiseOrValue done, returning" << std::endl;
                return Create(return_value_void{nullptr});
            }
            else
            {
                std::mutex mtx;
                std::condition_variable cv;
                bool completed = false;
                return_value_void *result = nullptr;

                const auto callback = [functionName, manager, p_callback_handler, p_callback, &result, &mtx, &cv, &completed](Napi::Env env, Napi::Function jsCallback)
                {
                    LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));
                    try
                    {
                        const auto jsResult = jsCallback.Call({});

                        HandleStringPromiseOrValue(env, jsResult, p_callback_handler, p_callback, &mtx, &cv, &completed, &result);
                    }
                    catch (const Napi::Error &e)
                    {
                        callbackLogger.LogError(e);
                        p_callback(p_callback_handler, Create(return_value_string{Copy(GetErrorMessage(e)), nullptr}));
                        std::lock_guard<std::mutex> lock(mtx);
                        result = Create(return_value_void{nullptr});
                        completed = true;
                        cv.notify_one();
                    }
                };

                const auto status = manager->TSFNGetInstallPath.BlockingCall(callback);
                if (status != napi_ok)
                {
                    logger.Log("BlockingCall failed with status: " + std::to_string(status));
                    return Create(return_value_void{Copy(u"Failed to queue async call")});
                }

                std::unique_lock<std::mutex> lock(mtx);
                cv.wait(lock, [&completed]
                        { return completed; });

                logger.Log("Blocking call completed");
                return result;
            }
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_void{Copy(conv.from_bytes(e.what()))});
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            return Create(return_value_void{Copy(u"Unknown exception")});
        }
    }
    static return_value_void *readFileContent(param_ptr *p_owner, param_string *p_file_path, param_int offset, param_int length, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_data *)) noexcept
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);
        try
        {
            auto manager = const_cast<Bindings::LauncherManager::LauncherManager *>(static_cast<const Bindings::LauncherManager::LauncherManager *>(p_owner));

            if (std::this_thread::get_id() == manager->MainThreadId)
            {
                const auto env = manager->FReadFileContent.Env();

                const auto filePath = p_file_path == nullptr ? env.Null() : String::New(env, p_file_path);
                const auto offset_ = Number::New(env, offset);
                const auto length_ = Number::New(env, length);
                const auto jsResult = manager->FReadFileContent.Call({filePath, offset_, length_});

                HandleDataPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                return Create(return_value_void{nullptr});
            }
            else
            {
                std::mutex mtx;
                std::condition_variable cv;
                bool completed = false;
                return_value_void *result = nullptr;

                const auto callback = [functionName, manager, p_file_path, offset, length, p_callback_handler, p_callback, &result, &mtx, &cv, &completed](Napi::Env env, Napi::Function jsCallback)
                {
                    LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));
                    try
                    {
                        const auto filePath = p_file_path == nullptr ? env.Null() : String::New(env, p_file_path);
                        const auto offset_ = Number::New(env, offset);
                        const auto length_ = Number::New(env, length);

                        const auto jsResult = jsCallback.Call({filePath, offset_, length_});

                        HandleDataPromiseOrValue(env, jsResult, p_callback_handler, p_callback, &mtx, &cv, &completed, &result);
                    }
                    catch (const Napi::Error &e)
                    {
                        callbackLogger.LogError(e);
                        p_callback(p_callback_handler, Create(return_value_data{Copy(GetErrorMessage(e)), nullptr, 0}));
                        std::lock_guard<std::mutex> lock(mtx);
                        result = Create(return_value_void{nullptr});
                        completed = true;
                        cv.notify_one();
                    }
                };

                const auto status = manager->TSFNReadFileContent.BlockingCall(callback);
                if (status != napi_ok)
                {
                    logger.Log("BlockingCall failed with status: " + std::to_string(status));
                    return Create(return_value_void{Copy(u"Failed to queue async call")});
                }

                std::unique_lock<std::mutex> lock(mtx);
                cv.wait(lock, [&completed]
                        { return completed; });

                logger.Log("Blocking call completed");
                return result;
            }
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_void{Copy(conv.from_bytes(e.what()))});
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            return Create(return_value_void{Copy(u"Unknown exception")});
        }
    }
    static return_value_void *writeFileContent(param_ptr *p_owner, param_string *p_file_path, param_data *p_data, param_int length, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_void *)) noexcept
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);
        try
        {
            auto manager = const_cast<Bindings::LauncherManager::LauncherManager *>(static_cast<const Bindings::LauncherManager::LauncherManager *>(p_owner));

            if (std::this_thread::get_id() == manager->MainThreadId)
            {
                const auto env = manager->FWriteFileContent.Env();

                const auto filePath = p_file_path == nullptr ? env.Null() : String::New(env, p_file_path);
                const auto data = Buffer<uint8_t>::NewOrCopy(env, p_data, static_cast<size_t>(length));
                const auto jsResult = manager->FWriteFileContent.Call({filePath, data});

                HandleVoidPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                return Create(return_value_void{nullptr});
            }
            else
            {
                std::mutex mtx;
                std::condition_variable cv;
                bool completed = false;
                return_value_void *result = nullptr;

                const auto callback = [functionName, manager, p_file_path, p_data, length, p_callback_handler, p_callback, &result, &mtx, &cv, &completed](Napi::Env env, Napi::Function jsCallback)
                {
                    LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));
                    try
                    {
                        const auto filePath = p_file_path == nullptr ? env.Null() : String::New(env, p_file_path);
                        const auto data = Buffer<uint8_t>::NewOrCopy(env, p_data, static_cast<size_t>(length));

                        const auto jsResult = jsCallback.Call({filePath, data});

                        HandleVoidPromiseOrValue(env, jsResult, p_callback_handler, p_callback, &mtx, &cv, &completed, &result);
                    }
                    catch (const Napi::Error &e)
                    {
                        callbackLogger.LogError(e);
                        p_callback(p_callback_handler, Create(return_value_void{Copy(GetErrorMessage(e))}));
                        std::lock_guard<std::mutex> lock(mtx);
                        result = Create(return_value_void{nullptr});
                        completed = true;
                        cv.notify_one();
                    }
                };

                const auto status = manager->TSFNWriteFileContent.BlockingCall(callback);
                if (status != napi_ok)
                {
                    logger.Log("BlockingCall failed with status: " + std::to_string(status));
                    return Create(return_value_void{Copy(u"Failed to queue async call")});
                }

                std::unique_lock<std::mutex> lock(mtx);
                cv.wait(lock, [&completed]
                        { return completed; });

                logger.Log("Blocking call completed");
                return result;
            }
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_void{Copy(conv.from_bytes(e.what()))});
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            return Create(return_value_void{Copy(u"Unknown exception")});
        }
    }
    static return_value_void *readDirectoryFileList(param_ptr *p_owner, param_string *p_directory_path, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_json *)) noexcept
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);
        try
        {
            auto manager = const_cast<Bindings::LauncherManager::LauncherManager *>(static_cast<const Bindings::LauncherManager::LauncherManager *>(p_owner));

            if (std::this_thread::get_id() == manager->MainThreadId)
            {
                const auto env = manager->FReadDirectoryFileList.Env();

                const auto directoryPath = p_directory_path == nullptr ? env.Null() : String::New(env, p_directory_path);
                const auto jsResult = manager->FReadDirectoryFileList.Call({directoryPath});

                HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                return Create(return_value_void{nullptr});
            }
            else
            {
                std::mutex mtx;
                std::condition_variable cv;
                bool completed = false;
                return_value_void *result = nullptr;

                const auto callback = [functionName, manager, p_directory_path, p_callback_handler, p_callback, &result, &mtx, &cv, &completed](Napi::Env env, Napi::Function jsCallback)
                {
                    LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));
                    try
                    {
                        const auto directoryPath = p_directory_path == nullptr ? env.Null() : String::New(env, p_directory_path);

                        const auto jsResult = jsCallback.Call({directoryPath});

                        HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback, &mtx, &cv, &completed, &result);
                    }
                    catch (const Napi::Error &e)
                    {
                        callbackLogger.LogError(e);
                        p_callback(p_callback_handler, Create(return_value_json{Copy(GetErrorMessage(e)), nullptr}));
                        std::lock_guard<std::mutex> lock(mtx);
                        result = Create(return_value_void{nullptr});
                        completed = true;
                        cv.notify_one();
                    }
                };

                const auto status = manager->TSFNReadDirectoryFileList.BlockingCall(callback);
                if (status != napi_ok)
                {
                    logger.Log("BlockingCall failed with status: " + std::to_string(status));
                    return Create(return_value_void{Copy(u"Failed to queue async call")});
                }

                std::unique_lock<std::mutex> lock(mtx);
                cv.wait(lock, [&completed]
                        { return completed; });

                logger.Log("Blocking call completed");
                return result;
            }
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_void{Copy(conv.from_bytes(e.what()))});
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            return Create(return_value_void{Copy(u"Unknown exception")});
        }
    }
    static return_value_void *readDirectoryList(param_ptr *p_owner, param_string *p_directory_path, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_json *)) noexcept
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);
        try
        {
            auto manager = const_cast<Bindings::LauncherManager::LauncherManager *>(static_cast<const Bindings::LauncherManager::LauncherManager *>(p_owner));

            if (std::this_thread::get_id() == manager->MainThreadId)
            {
                const auto env = manager->FReadDirectoryList.Env();

                const auto directoryPath = p_directory_path == nullptr ? env.Null() : String::New(env, p_directory_path);
                const auto jsResult = manager->FReadDirectoryList.Call({directoryPath});

                HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                return Create(return_value_void{nullptr});
            }
            else
            {
                std::mutex mtx;
                std::condition_variable cv;
                bool completed = false;
                return_value_void *result = nullptr;

                const auto callback = [functionName, manager, p_directory_path, p_callback_handler, p_callback, &result, &mtx, &cv, &completed](Napi::Env env, Napi::Function jsCallback)
                {
                    LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));
                    try
                    {
                        const auto directoryPath = p_directory_path == nullptr ? env.Null() : String::New(env, p_directory_path);

                        const auto jsResult = jsCallback.Call({directoryPath});

                        HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback, &mtx, &cv, &completed, &result);
                    }
                    catch (const Napi::Error &e)
                    {
                        callbackLogger.LogError(e);
                        p_callback(p_callback_handler, Create(return_value_json{Copy(GetErrorMessage(e)), nullptr}));
                        std::lock_guard<std::mutex> lock(mtx);
                        result = Create(return_value_void{nullptr});
                        completed = true;
                        cv.notify_one();
                    }
                };

                const auto status = manager->TSFNReadDirectoryList.BlockingCall(callback);
                if (status != napi_ok)
                {
                    logger.Log("BlockingCall failed with status: " + std::to_string(status));
                    return Create(return_value_void{Copy(u"Failed to queue async call")});
                }

                std::unique_lock<std::mutex> lock(mtx);
                cv.wait(lock, [&completed]
                        { return completed; });

                logger.Log("Blocking call completed");
                return result;
            }
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_void{Copy(conv.from_bytes(e.what()))});
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            return Create(return_value_void{Copy(u"Unknown exception")});
        }
    }
    static return_value_void *getAllModuleViewModels(param_ptr *p_owner, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_json *)) noexcept
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);
        std::cout << "[getAllModuleViewModels] invoked" << std::endl;
        try
        {
            auto manager = const_cast<Bindings::LauncherManager::LauncherManager *>(static_cast<const Bindings::LauncherManager::LauncherManager *>(p_owner));

            if (std::this_thread::get_id() == manager->MainThreadId)
            {
                std::cout << "[getAllModuleViewModels] On main thread" << std::endl;
                const auto env = manager->FGetAllModuleViewModels.Env();

                const auto jsResult = manager->FGetAllModuleViewModels.Call({});

                HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                std::cout << "[getAllModuleViewModels] returning" << std::endl;
                return Create(return_value_void{nullptr});
            }
            else
            {
                std::cout << "[getAllModuleViewModels] Not on main thread, using BlockingCall" << std::endl;
                std::mutex mtx;
                std::condition_variable cv;
                bool completed = false;
                return_value_void *result = nullptr;

                const auto callback = [functionName, manager, p_callback_handler, p_callback, &result, &mtx, &cv, &completed](Napi::Env env, Napi::Function jsCallback)
                {
                    std::cout << "[getAllModuleViewModels] TSFN callback invoked" << std::endl;
                    LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));
                    try
                    {
                        const auto jsResult = jsCallback.Call({});

                        HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback, &mtx, &cv, &completed, &result);
                    }
                    catch (const Napi::Error &e)
                    {
                        callbackLogger.LogError(e);
                        p_callback(p_callback_handler, Create(return_value_json{Copy(GetErrorMessage(e)), nullptr}));
                        std::lock_guard<std::mutex> lock(mtx);
                        result = Create(return_value_void{nullptr});
                        completed = true;
                        cv.notify_one();
                    }
                    std::cout << "[getAllModuleViewModels] TSFN callback done" << std::endl;
                };

                std::cout << "[getAllModuleViewModels] Calling TSFNGetAllModuleViewModels.BlockingCall" << std::endl;
                const auto status = manager->TSFNGetAllModuleViewModels.BlockingCall(callback);
                std::cout << "[getAllModuleViewModels] BlockingCall returned, status=" << status << std::endl;
                if (status != napi_ok)
                {
                    logger.Log("BlockingCall failed with status: " + std::to_string(status));
                    return Create(return_value_void{Copy(u"Failed to queue async call")});
                }

                std::cout << "[getAllModuleViewModels] Waiting on cv" << std::endl;
                std::unique_lock<std::mutex> lock(mtx);
                cv.wait(lock, [&completed]
                        { return completed; });

                std::cout << "[getAllModuleViewModels] cv.wait completed" << std::endl;
                logger.Log("Blocking call completed");
                return result;
            }
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_void{Copy(conv.from_bytes(e.what()))});
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            return Create(return_value_void{Copy(u"Unknown exception")});
        }
    }
    static return_value_void *getModuleViewModels(param_ptr *p_owner, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_json *)) noexcept
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);
        try
        {
            auto manager = const_cast<Bindings::LauncherManager::LauncherManager *>(static_cast<const Bindings::LauncherManager::LauncherManager *>(p_owner));

            if (std::this_thread::get_id() == manager->MainThreadId)
            {
                const auto env = manager->FGetModuleViewModels.Env();

                const auto jsResult = manager->FGetModuleViewModels.Call({});

                HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                return Create(return_value_void{nullptr});
            }
            else
            {
                std::mutex mtx;
                std::condition_variable cv;
                bool completed = false;
                return_value_void *result = nullptr;

                const auto callback = [functionName, manager, p_callback_handler, p_callback, &result, &mtx, &cv, &completed](Napi::Env env, Napi::Function jsCallback)
                {
                    LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));
                    try
                    {
                        const auto jsResult = jsCallback.Call({});

                        HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback, &mtx, &cv, &completed, &result);
                    }
                    catch (const Napi::Error &e)
                    {
                        callbackLogger.LogError(e);
                        p_callback(p_callback_handler, Create(return_value_json{Copy(GetErrorMessage(e)), nullptr}));
                        std::lock_guard<std::mutex> lock(mtx);
                        result = Create(return_value_void{nullptr});
                        completed = true;
                        cv.notify_one();
                    }
                };

                const auto status = manager->TSFNGetModuleViewModels.BlockingCall(callback);
                if (status != napi_ok)
                {
                    logger.Log("BlockingCall failed with status: " + std::to_string(status));
                    return Create(return_value_void{Copy(u"Failed to queue async call")});
                }

                std::unique_lock<std::mutex> lock(mtx);
                cv.wait(lock, [&completed]
                        { return completed; });

                logger.Log("Blocking call completed");
                return result;
            }
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_void{Copy(conv.from_bytes(e.what()))});
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            return Create(return_value_void{Copy(u"Unknown exception")});
        }
    }
    static return_value_void *setModuleViewModels(param_ptr *p_owner, param_json *p_module_view_models, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_void *)) noexcept
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);
        std::cout << "[setModuleViewModels] invoked" << std::endl;
        try
        {
            auto manager = const_cast<Bindings::LauncherManager::LauncherManager *>(static_cast<const Bindings::LauncherManager::LauncherManager *>(p_owner));

            if (std::this_thread::get_id() == manager->MainThreadId)
            {
                std::cout << "[setModuleViewModels] On main thread" << std::endl;
                const auto env = manager->FSetModuleViewModels.Env();

                const auto moduleViewModels = p_module_view_models == nullptr ? env.Null() : JSONParse(Napi::String::New(env, p_module_view_models));
                const auto jsResult = manager->FSetModuleViewModels.Call({moduleViewModels});

                HandleVoidPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                std::cout << "[setModuleViewModels] returning" << std::endl;
                return Create(return_value_void{nullptr});
            }
            else
            {
                std::cout << "[setModuleViewModels] Not on main thread, using TSFN" << std::endl;
                const auto callback = [functionName, manager, p_module_view_models, p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback)
                {
                    std::cout << "[setModuleViewModels] TSFN callback invoked" << std::endl;
                    LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));
                    try
                    {
                        const auto moduleViewModels = p_module_view_models == nullptr ? env.Null() : JSONParse(Napi::String::New(env, p_module_view_models));
                        const auto jsResult = manager->FSetModuleViewModels.Call({moduleViewModels});

                        HandleVoidPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                    }
                    catch (const Napi::Error &e)
                    {
                        callbackLogger.LogError(e);
                        p_callback(p_callback_handler, Create(return_value_void{Copy(GetErrorMessage(e))}));
                    }
                    std::cout << "[setModuleViewModels] TSFN callback done" << std::endl;
                };

                const auto status = manager->TSFN.NonBlockingCall(callback);
                if (status != napi_ok)
                {
                    logger.Log("NonBlockingCall failed with status: " + std::to_string(status));
                    return Create(return_value_void{Copy(u"Failed to queue async call")});
                }
                std::cout << "[setModuleViewModels] NonBlockingCall queued, returning" << std::endl;
                return Create(return_value_void{nullptr});
            }
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_void{Copy(conv.from_bytes(e.what()))});
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            return Create(return_value_void{Copy(u"Unknown exception")});
        }
    }
    static return_value_void *getOptions(param_ptr *p_owner, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_json *)) noexcept
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);
        std::cout << "[getOptions] invoked" << std::endl;
        try
        {
            auto manager = const_cast<Bindings::LauncherManager::LauncherManager *>(static_cast<const Bindings::LauncherManager::LauncherManager *>(p_owner));

            if (std::this_thread::get_id() == manager->MainThreadId)
            {
                std::cout << "[getOptions] On main thread" << std::endl;
                const auto env = manager->FGetOptions.Env();

                const auto jsResult = manager->FGetOptions.Call({});

                HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                return Create(return_value_void{nullptr});
            }
            else
            {
                std::cout << "[getOptions] Not on main thread, using BlockingCall" << std::endl;
                std::mutex mtx;
                std::condition_variable cv;
                bool completed = false;
                return_value_void *result = nullptr;

                const auto callback = [functionName, manager, p_callback_handler, p_callback, &result, &mtx, &cv, &completed](Napi::Env env, Napi::Function jsCallback)
                {
                    std::cout << "[getOptions] TSFN callback invoked" << std::endl;
                    LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));
                    try
                    {
                        const auto jsResult = jsCallback.Call({});

                        HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback, &mtx, &cv, &completed, &result);
                    }
                    catch (const Napi::Error &e)
                    {
                        callbackLogger.LogError(e);
                        p_callback(p_callback_handler, Create(return_value_json{Copy(GetErrorMessage(e)), nullptr}));
                        std::lock_guard<std::mutex> lock(mtx);
                        result = Create(return_value_void{nullptr});
                        completed = true;
                        cv.notify_one();
                    }
                    std::cout << "[getOptions] TSFN callback done" << std::endl;
                };

                std::cout << "[getOptions] Calling TSFNGetOptions.BlockingCall" << std::endl;
                const auto status = manager->TSFNGetOptions.BlockingCall(callback);
                if (status != napi_ok)
                {
                    logger.Log("BlockingCall failed with status: " + std::to_string(status));
                    return Create(return_value_void{Copy(u"Failed to queue async call")});
                }

                std::unique_lock<std::mutex> lock(mtx);
                cv.wait(lock, [&completed]
                        { return completed; });

                logger.Log("Blocking call completed");
                return result;
            }
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_void{Copy(conv.from_bytes(e.what()))});
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            return Create(return_value_void{Copy(u"Unknown exception")});
        }
    }
    static return_value_void *getState(param_ptr *p_owner, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_json *)) noexcept
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);
        try
        {
            auto manager = const_cast<Bindings::LauncherManager::LauncherManager *>(static_cast<const Bindings::LauncherManager::LauncherManager *>(p_owner));

            if (std::this_thread::get_id() == manager->MainThreadId)
            {
                const auto env = manager->FGetState.Env();

                const auto jsResult = manager->FGetState.Call({});

                HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
                return Create(return_value_void{nullptr});
            }
            else
            {
                std::mutex mtx;
                std::condition_variable cv;
                bool completed = false;
                return_value_void *result = nullptr;

                const auto callback = [functionName, manager, p_callback_handler, p_callback, &result, &mtx, &cv, &completed](Napi::Env env, Napi::Function jsCallback)
                {
                    LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));
                    try
                    {
                        const auto jsResult = jsCallback.Call({});

                        HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback, &mtx, &cv, &completed, &result);
                    }
                    catch (const Napi::Error &e)
                    {
                        callbackLogger.LogError(e);
                        p_callback(p_callback_handler, Create(return_value_json{Copy(GetErrorMessage(e)), nullptr}));
                        std::lock_guard<std::mutex> lock(mtx);
                        result = Create(return_value_void{nullptr});
                        completed = true;
                        cv.notify_one();
                    }
                };

                const auto status = manager->TSFNGetState.BlockingCall(callback);
                if (status != napi_ok)
                {
                    logger.Log("BlockingCall failed with status: " + std::to_string(status));
                    return Create(return_value_void{Copy(u"Failed to queue async call")});
                }

                std::unique_lock<std::mutex> lock(mtx);
                cv.wait(lock, [&completed]
                        { return completed; });

                logger.Log("Blocking call completed");
                return result;
            }
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_void{Copy(conv.from_bytes(e.what()))});
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            return Create(return_value_void{Copy(u"Unknown exception")});
        }
    }
}
#endif
