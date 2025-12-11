#ifndef VE_LIB_UTILS_CALLBACKS_GUARD_HPP_
#define VE_LIB_UTILS_CALLBACKS_GUARD_HPP_

#include <napi.h>
#include <mutex>
#include <condition_variable>
#include <thread>
#include "Logger.hpp"
#include "Utils.Generic.hpp"
#include "Utils.JS.hpp"
#include "Utils.Utf.hpp"

using namespace Napi;

namespace Utils
{
    struct ResultCallbackData
    {
        Napi::Promise::Deferred deferred;
        Napi::ThreadSafeFunction tsfn;
    };

    using del_rcbd = std::unique_ptr<ResultCallbackData, deleter<ResultCallbackData>>;

    ResultCallbackData *CreateResultCallbackData(const Napi::Env env, const std::string callbackName)
    {
        const auto functionName = callbackName + std::string(__FUNCTION__) + "_";

        const auto deferred = Napi::Promise::Deferred::New(env);
        const auto callback = [functionName, deferred](const Napi::CallbackInfo &info)
        {
            LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));
            const auto env = info.Env();

            const auto length = info.Length();

            if (length == 0)
            {
                deferred.Resolve(env.Null());
            }
            else if (length == 2 && info[0].IsBoolean())
            {
                const auto isErr = info[0].As<Napi::Boolean>();
                const auto obj = info[1];
                if (isErr.Value())
                {
                    callbackLogger.Log("Rejecting");
                    deferred.Reject(obj);
                }
                else
                {
                    callbackLogger.Log("Resolving");
                    deferred.Resolve(obj);
                }
            }
            else
            {
                deferred.Reject(Napi::Error::New(env, "Too many arguments or incorrect arguments").Value());
            }
        };
        const auto tsfn = Napi::ThreadSafeFunction::New(env, Napi::Function::New(env, callback), "CreateResultCallbackData", 0, 1);
        return new ResultCallbackData{deferred, tsfn};
    }

    void HandleVoidResultCallback(param_ptr *p_owner, return_value_void *returnData)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);
        try
        {
            auto manager = const_cast<ResultCallbackData *>(static_cast<const ResultCallbackData *>(p_owner));
            del_rcbd del{manager};

            const auto callback = [functionName, manager, returnData](Napi::Env env, Napi::Function jsCallback)
            {
                LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));

                del_void del{returnData};

                if (returnData == nullptr)
                {
                    callbackLogger.Log("Null return data");
                    const auto isError = Napi::Boolean::New(env, true);
                    const auto error = Napi::Error::New(env, "Return value was null!").Value();
                    jsCallback.Call({isError, error});
                    return;
                }

                if (returnData->error != nullptr)
                {
                    callbackLogger.Log("Error");
                    const auto isError = Napi::Boolean::New(env, true);
                    const auto errorStr = std::unique_ptr<char16_t[], common_deallocor<char16_t>>(returnData->error);
                    const auto error = Napi::Error::New(env, String::New(env, errorStr.get())).Value();
                    jsCallback.Call({isError, error});
                }
                else
                {
                    callbackLogger.Log("Resolving");
                    jsCallback.Call({});
                }
            };

            manager->tsfn.NonBlockingCall(callback);
            manager->tsfn.Release();
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
        }
        catch (...)
        {
            logger.Log("Unknown exception");
        }
    }

    void HandleJsonResultCallback(param_ptr *p_owner, return_value_json *returnData)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);
        try
        {
            auto manager = const_cast<ResultCallbackData *>(static_cast<const ResultCallbackData *>(p_owner));
            del_rcbd del{manager};

            const auto callback = [functionName, manager, returnData](Napi::Env env, Napi::Function jsCallback)
            {
                LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));

                del_json del{returnData};

                if (returnData == nullptr)
                {
                    callbackLogger.Log("Null return data");
                    const auto isError = Napi::Boolean::New(env, true);
                    const auto error = Napi::Error::New(env, "Return value was null!").Value();
                    jsCallback.Call({isError, error});
                    return;
                }

                if (returnData->error != nullptr)
                {
                    callbackLogger.Log("Error");
                    const auto isError = Napi::Boolean::New(env, true);
                    const auto errorStr = std::unique_ptr<char16_t[], common_deallocor<char16_t>>(returnData->error);
                    const auto error = Napi::Error::New(env, String::New(env, errorStr.get())).Value();
                    jsCallback.Call({isError, error});
                }
                else
                {
                    callbackLogger.Log("Resolving");
                    const auto isError = Napi::Boolean::New(env, false);
                    if (returnData->value == nullptr)
                    {
                        callbackLogger.Log("Result is null");
                        const auto result = env.Null();
                        jsCallback.Call({isError, result});
                    }
                    else
                    {
                        callbackLogger.Log("Result is not null");
                        const auto resultStr = std::unique_ptr<char16_t[], common_deallocor<char16_t>>(returnData->value);
                        const auto result = JSONParse(Napi::String::New(env, resultStr.get()));
                        jsCallback.Call({isError, result});
                    }
                }
            };

            manager->tsfn.NonBlockingCall(callback);
            manager->tsfn.Release();
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
        }
        catch (...)
        {
            logger.Log("Unknown exception");
        }
    }

    void HandleStringResultCallback(param_ptr *p_owner, return_value_string *returnData)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);
        try
        {
            auto manager = const_cast<ResultCallbackData *>(static_cast<const ResultCallbackData *>(p_owner));
            del_rcbd del{manager};

            const auto callback = [functionName, manager, returnData](Napi::Env env, Napi::Function jsCallback)
            {
                LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));

                del_string del{returnData};

                if (returnData == nullptr)
                {
                    callbackLogger.Log("Null return data");
                    const auto isError = Napi::Boolean::New(env, true);
                    const auto error = Napi::Error::New(env, "Return value was null!").Value();
                    jsCallback.Call({isError, error});
                    return;
                }

                if (returnData->error != nullptr)
                {
                    callbackLogger.Log("Error");
                    const auto isError = Napi::Boolean::New(env, true);
                    const auto errorStr = std::unique_ptr<char16_t[], common_deallocor<char16_t>>(returnData->error);
                    const auto error = Napi::Error::New(env, String::New(env, errorStr.get())).Value();
                    jsCallback.Call({isError, error});
                }
                else
                {
                    callbackLogger.Log("Resolving");
                    const auto isError = Napi::Boolean::New(env, false);
                    if (returnData->value == nullptr)
                    {
                        callbackLogger.Log("Result is null");
                        const auto result = env.Null();
                        jsCallback.Call({isError, result});
                    }
                    else
                    {
                        callbackLogger.Log("Result is not null");
                        const auto resultStr = std::unique_ptr<char16_t[], common_deallocor<char16_t>>(returnData->value);
                        const auto result = Napi::String::New(env, resultStr.get());
                        jsCallback.Call({isError, result});
                    }
                }
            };

            manager->tsfn.NonBlockingCall(callback);
            manager->tsfn.Release();
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
        }
        catch (...)
        {
            logger.Log("Unknown exception");
        }
    }

    void HandleBooleanResultCallback(param_ptr *p_owner, return_value_bool *returnData)
    {
        const auto functionName = __FUNCTION__;
        LoggerScope logger(functionName);
        try
        {
            auto manager = const_cast<ResultCallbackData *>(static_cast<const ResultCallbackData *>(p_owner));
            del_rcbd del{manager};

            const auto callback = [functionName, manager, returnData](Napi::Env env, Napi::Function jsCallback)
            {
                LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));

                del_bool del{returnData};

                if (returnData == nullptr)
                {
                    callbackLogger.Log("Null return data");
                    const auto isError = Napi::Boolean::New(env, true);
                    const auto error = Napi::Error::New(env, "Return value was null!").Value();
                    jsCallback.Call({isError, error});
                    return;
                }

                if (returnData->error != nullptr)
                {
                    callbackLogger.Log("Error");
                    const auto isError = Napi::Boolean::New(env, true);
                    const auto errorStr = std::unique_ptr<char16_t[], common_deallocor<char16_t>>(returnData->error);
                    const auto error = Napi::Error::New(env, String::New(env, errorStr.get())).Value();
                    jsCallback.Call({isError, error});
                }
                else
                {
                    callbackLogger.Log("Resolving");
                    const auto isError = Napi::Boolean::New(env, false);
                    const auto result = Boolean::New(env, returnData->value == 1);
                    jsCallback.Call({isError, result});
                }
            };

            manager->tsfn.NonBlockingCall(callback);
            manager->tsfn.Release();
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
        }
        catch (...)
        {
            logger.Log("Unknown exception");
        }
    }

    std::u16string GetErrorMessage(const Napi::Error e)
    {
        const auto errorValue = e.Value();
        if (errorValue.Has("stack"))
        {
            const auto stack = e.Value().Get("stack").As<String>();
            return stack.Utf16Value();
        }

        return Utf8ToUtf16(e.Message());
    }

    // Template specializations to create error results for each return type
    template <typename T>
    T *CreateErrorResult(const std::u16string &errorMsg);

    template <>
    inline return_value_string *CreateErrorResult(const std::u16string &errorMsg)
    {
        return Create(return_value_string{Copy(errorMsg), nullptr});
    }

    template <>
    inline return_value_json *CreateErrorResult(const std::u16string &errorMsg)
    {
        return Create(return_value_json{Copy(errorMsg), nullptr});
    }

    template <>
    inline return_value_data *CreateErrorResult(const std::u16string &errorMsg)
    {
        return Create(return_value_data{Copy(errorMsg), nullptr, 0});
    }

    template <>
    inline return_value_void *CreateErrorResult(const std::u16string &errorMsg)
    {
        return Create(return_value_void{Copy(errorMsg)});
    }

    // Generic template for handling Promise or immediate value results
    // TReturnValue: the return value type (e.g., return_value_string, return_value_json)
    // TConverter: callable that converts Napi::Value to TReturnValue*
    template <typename TReturnValue, typename TConverter>
    inline void HandlePromiseOrValue(
        const Napi::Env &env,
        const Napi::Value &jsResult,
        param_ptr *p_callback_handler,
        void (*p_callback)(param_ptr *, TReturnValue *),
        TConverter converter,
        std::mutex *mtx = nullptr,
        std::condition_variable *cv = nullptr,
        bool *completed = nullptr,
        return_value_void **result = nullptr)
    {
        auto signalCompletion = [mtx, cv, completed, result]()
        {
            if (mtx && cv && completed && result)
            {
                std::lock_guard<std::mutex> lock(*mtx);
                *result = Create(return_value_void{nullptr});
                *completed = true;
                cv->notify_one();
            }
        };

        if (jsResult.IsPromise())
        {
            const auto promise = jsResult.As<Napi::Object>();
            const auto then = promise.Get("then").As<Napi::Function>();

            auto onResolve = Napi::Function::New(env, [p_callback_handler, p_callback, converter, signalCompletion](const Napi::CallbackInfo &info)
                                                 {
                const auto resolvedValue = info[0];
                p_callback(p_callback_handler, converter(resolvedValue));
                signalCompletion(); });

            auto onReject = Napi::Function::New(env, [p_callback_handler, p_callback, signalCompletion](const Napi::CallbackInfo &info)
                                                {
                const auto error = info[0];
                std::u16string errorMsg = u"Promise rejected";
                if (error.IsObject()) {
                    const auto errorObj = error.As<Napi::Object>();
                    if (errorObj.Has("message")) {
                        errorMsg = errorObj.Get("message").As<Napi::String>().Utf16Value();
                    }
                }
                p_callback(p_callback_handler, CreateErrorResult<TReturnValue>(errorMsg));
                signalCompletion(); });

            then.Call(promise, {onResolve, onReject});
        }
        else
        {
            p_callback(p_callback_handler, converter(jsResult));
            signalCompletion();
        }
    }

    // Convenience wrappers for backward compatibility
    inline void HandleStringPromiseOrValue(
        const Napi::Env &env,
        const Napi::Value &jsResult,
        param_ptr *p_callback_handler,
        void (*p_callback)(param_ptr *, return_value_string *),
        std::mutex *mtx = nullptr,
        std::condition_variable *cv = nullptr,
        bool *completed = nullptr,
        return_value_void **result = nullptr)
    {
        HandlePromiseOrValue(env, jsResult, p_callback_handler, p_callback, ConvertToStringResult, mtx, cv, completed, result);
    }

    inline void HandleJsonPromiseOrValue(
        const Napi::Env &env,
        const Napi::Value &jsResult,
        param_ptr *p_callback_handler,
        void (*p_callback)(param_ptr *, return_value_json *),
        std::mutex *mtx = nullptr,
        std::condition_variable *cv = nullptr,
        bool *completed = nullptr,
        return_value_void **result = nullptr)
    {
        HandlePromiseOrValue(env, jsResult, p_callback_handler, p_callback, ConvertToJsonResult, mtx, cv, completed, result);
    }

    inline void HandleDataPromiseOrValue(
        const Napi::Env &env,
        const Napi::Value &jsResult,
        param_ptr *p_callback_handler,
        void (*p_callback)(param_ptr *, return_value_data *),
        std::mutex *mtx = nullptr,
        std::condition_variable *cv = nullptr,
        bool *completed = nullptr,
        return_value_void **result = nullptr)
    {
        HandlePromiseOrValue(env, jsResult, p_callback_handler, p_callback, ConvertToDataResult, mtx, cv, completed, result);
    }

    inline void HandleVoidPromiseOrValue(
        const Napi::Env &env,
        const Napi::Value &jsResult,
        param_ptr *p_callback_handler,
        void (*p_callback)(param_ptr *, return_value_void *),
        std::mutex *mtx = nullptr,
        std::condition_variable *cv = nullptr,
        bool *completed = nullptr,
        return_value_void **result = nullptr)
    {
        HandlePromiseOrValue(env, jsResult, p_callback_handler, p_callback,
                             [](const Napi::Value &) -> return_value_void * { return Create(return_value_void{nullptr}); },
                             mtx, cv, completed, result);
    }

    // Helper for outer exception handling in callbacks - returns error result
    template <typename TResult, typename Func>
    inline TResult *CallbackWithExceptionHandling(LoggerScope &logger, Func &&func) noexcept
    {
        try
        {
            return func();
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
            return CreateErrorResult<TResult>(GetErrorMessage(e));
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            return CreateErrorResult<TResult>(Utf8ToUtf16(e.what()));
        }
        catch (...)
        {
            logger.Log("Unknown exception");
            return CreateErrorResult<TResult>(u"Unknown exception");
        }
    }

    // Blocking call synchronization data
    struct BlockingCallData
    {
        std::mutex mtx;
        std::condition_variable cv;
        bool completed = false;
        return_value_void *result = nullptr;
    };

    // Signal completion of a blocking call
    inline void SignalBlockingCallComplete(BlockingCallData &data)
    {
        std::lock_guard<std::mutex> lock(data.mtx);
        data.result = Create(return_value_void{nullptr});
        data.completed = true;
        data.cv.notify_one();
    }

    // Wait for a blocking call to complete and return the result
    inline return_value_void *WaitForBlockingCall(LoggerScope &logger, BlockingCallData &data)
    {
        std::unique_lock<std::mutex> lock(data.mtx);
        data.cv.wait(lock, [&data]
                     { return data.completed; });
        logger.Log("Blocking call completed");
        return data.result;
    }

    // Non-blocking callback pattern helper (void return type)
    // Used for callbacks that don't need to return a value synchronously
    template <typename TManager, typename TThreadSafeFunction, typename TMainThreadCall, typename TBackgroundCall>
    inline return_value_void *ExecuteNonBlockingCallback(
        const char *functionName,
        TManager *manager,
        TThreadSafeFunction &tsfn,
        TMainThreadCall &&mainThreadCall,
        TBackgroundCall &&backgroundCall) noexcept
    {
        LoggerScope logger(functionName);
        return CallbackWithExceptionHandling<return_value_void>(logger, [&]()
                                                                {
            if (std::this_thread::get_id() == manager->MainThreadId)
            {
                mainThreadCall();
                return Create(return_value_void{nullptr});
            }
            else
            {
                const auto callback = [functionName, &backgroundCall](Napi::Env env, Napi::Function jsCallback)
                {
                    LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));
                    try
                    {
                        backgroundCall(env);
                    }
                    catch (const Napi::Error &e)
                    {
                        callbackLogger.LogError(e);
                        // For non-blocking, we just log - no direct return value
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

    // Blocking callback pattern helper
    // Used for callbacks that need to return a value synchronously from another thread
    template <typename TManager, typename TThreadSafeFunction, typename TReturnValue, typename TMainThreadCall, typename TBackgroundCall, typename TErrorResultCreator>
    inline return_value_void *ExecuteBlockingCallback(
        const char *functionName,
        TManager *manager,
        TThreadSafeFunction &tsfn,
        param_ptr *p_callback_handler,
        void (*p_callback)(param_ptr *, TReturnValue *),
        TMainThreadCall &&mainThreadCall,
        TBackgroundCall &&backgroundCall,
        TErrorResultCreator &&createErrorResult) noexcept
    {
        LoggerScope logger(functionName);
        return CallbackWithExceptionHandling<return_value_void>(logger, [&]()
                                                                {
            if (std::this_thread::get_id() == manager->MainThreadId)
            {
                mainThreadCall();
                return Create(return_value_void{nullptr});
            }
            else
            {
                BlockingCallData blockingData;

                const auto callback = [functionName, p_callback_handler, p_callback, &blockingData, &backgroundCall, &createErrorResult](Napi::Env env, Napi::Function jsCallback)
                {
                    LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));
                    try
                    {
                        backgroundCall(env, jsCallback, &blockingData.mtx, &blockingData.cv, &blockingData.completed, &blockingData.result);
                    }
                    catch (const Napi::Error &e)
                    {
                        callbackLogger.LogError(e);
                        p_callback(p_callback_handler, createErrorResult(GetErrorMessage(e)));
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
}

#endif
