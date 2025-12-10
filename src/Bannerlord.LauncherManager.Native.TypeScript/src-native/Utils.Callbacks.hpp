#ifndef VE_LIB_UTILS_CALLBACKS_GUARD_HPP_
#define VE_LIB_UTILS_CALLBACKS_GUARD_HPP_

#include <napi.h>
#include <codecvt>
#include <iostream>
#include "Logger.hpp"
#include "Utils.Generic.hpp"
#include "Utils.JS.hpp"

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
        std::cout << "[HandleVoidResultCallback] invoked" << std::endl;
        try
        {
            auto manager = const_cast<ResultCallbackData *>(static_cast<const ResultCallbackData *>(p_owner));
            del_rcbd del{manager};

            const auto callback = [functionName, manager, returnData](Napi::Env env, Napi::Function jsCallback)
            {
                std::cout << "[HandleVoidResultCallback] TSFN callback invoked" << std::endl;
                LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));

                del_void del{returnData};

                if (returnData == nullptr)
                {
                    std::cout << "[HandleVoidResultCallback] Null return data" << std::endl;
                    callbackLogger.Log("Null return data");
                    const auto isError = Napi::Boolean::New(env, true);
                    const auto error = Napi::Error::New(env, "Return value was null!").Value();
                    jsCallback.Call({isError, error});
                    return;
                }

                if (returnData->error != nullptr)
                {
                    std::cout << "[HandleVoidResultCallback] Error" << std::endl;
                    callbackLogger.Log("Error");
                    const auto isError = Napi::Boolean::New(env, true);
                    const auto errorStr = std::unique_ptr<char16_t[], common_deallocor<char16_t>>(returnData->error);
                    const auto error = Napi::Error::New(env, String::New(env, errorStr.get())).Value();
                    jsCallback.Call({isError, error});
                }
                else
                {
                    std::cout << "[HandleVoidResultCallback] Resolving" << std::endl;
                    callbackLogger.Log("Resolving");
                    jsCallback.Call({});
                }
                std::cout << "[HandleVoidResultCallback] TSFN callback completed" << std::endl;
            };

            std::cout << "[HandleVoidResultCallback] Calling NonBlockingCall" << std::endl;
            manager->tsfn.NonBlockingCall(callback);
            std::cout << "[HandleVoidResultCallback] Calling Release" << std::endl;
            manager->tsfn.Release();
            std::cout << "[HandleVoidResultCallback] Done" << std::endl;
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
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
        std::cout << "[HandleJsonResultCallback] invoked" << std::endl;
        try
        {
            auto manager = const_cast<ResultCallbackData *>(static_cast<const ResultCallbackData *>(p_owner));
            del_rcbd del{manager};

            const auto callback = [functionName, manager, returnData](Napi::Env env, Napi::Function jsCallback)
            {
                std::cout << "[HandleJsonResultCallback] TSFN callback invoked" << std::endl;
                LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));

                del_json del{returnData};

                if (returnData == nullptr)
                {
                    std::cout << "[HandleJsonResultCallback] Null return data" << std::endl;
                    callbackLogger.Log("Null return data");
                    const auto isError = Napi::Boolean::New(env, true);
                    const auto error = Napi::Error::New(env, "Return value was null!").Value();
                    jsCallback.Call({isError, error});
                    return;
                }

                if (returnData->error != nullptr)
                {
                    std::cout << "[HandleJsonResultCallback] Error" << std::endl;
                    callbackLogger.Log("Error");
                    const auto isError = Napi::Boolean::New(env, true);
                    const auto errorStr = std::unique_ptr<char16_t[], common_deallocor<char16_t>>(returnData->error);
                    const auto error = Napi::Error::New(env, String::New(env, errorStr.get())).Value();
                    jsCallback.Call({isError, error});
                }
                else
                {
                    std::cout << "[HandleJsonResultCallback] Resolving" << std::endl;
                    callbackLogger.Log("Resolving");
                    const auto isError = Napi::Boolean::New(env, false);
                    if (returnData->value == nullptr)
                    {
                        std::cout << "[HandleJsonResultCallback] Result is null" << std::endl;
                        callbackLogger.Log("Result is null");
                        const auto result = env.Null();
                        jsCallback.Call({isError, result});
                    }
                    else
                    {
                        std::cout << "[HandleJsonResultCallback] Result is not null" << std::endl;
                        callbackLogger.Log("Result is not null");
                        const auto resultStr = std::unique_ptr<char16_t[], common_deallocor<char16_t>>(returnData->value);
                        const auto result = JSONParse(Napi::String::New(env, resultStr.get()));
                        jsCallback.Call({isError, result});
                    }
                }
                std::cout << "[HandleJsonResultCallback] TSFN callback completed" << std::endl;
            };

            std::cout << "[HandleJsonResultCallback] Calling NonBlockingCall" << std::endl;
            manager->tsfn.NonBlockingCall(callback);
            std::cout << "[HandleJsonResultCallback] Calling Release" << std::endl;
            manager->tsfn.Release();
            std::cout << "[HandleJsonResultCallback] Done" << std::endl;
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
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
        std::cout << "[HandleStringResultCallback] invoked" << std::endl;
        try
        {
            auto manager = const_cast<ResultCallbackData *>(static_cast<const ResultCallbackData *>(p_owner));
            del_rcbd del{manager};

            const auto callback = [functionName, manager, returnData](Napi::Env env, Napi::Function jsCallback)
            {
                std::cout << "[HandleStringResultCallback] TSFN callback invoked" << std::endl;
                LoggerScope callbackLogger(NAMEOFWITHCALLBACK(functionName, callback));

                del_string del{returnData};

                if (returnData == nullptr)
                {
                    std::cout << "[HandleStringResultCallback] Null return data" << std::endl;
                    callbackLogger.Log("Null return data");
                    const auto isError = Napi::Boolean::New(env, true);
                    const auto error = Napi::Error::New(env, "Return value was null!").Value();
                    jsCallback.Call({isError, error});
                    return;
                }

                if (returnData->error != nullptr)
                {
                    std::cout << "[HandleStringResultCallback] Error" << std::endl;
                    callbackLogger.Log("Error");
                    const auto isError = Napi::Boolean::New(env, true);
                    const auto errorStr = std::unique_ptr<char16_t[], common_deallocor<char16_t>>(returnData->error);
                    const auto error = Napi::Error::New(env, String::New(env, errorStr.get())).Value();
                    jsCallback.Call({isError, error});
                }
                else
                {
                    std::cout << "[HandleStringResultCallback] Resolving" << std::endl;
                    callbackLogger.Log("Resolving");
                    const auto isError = Napi::Boolean::New(env, false);
                    if (returnData->value == nullptr)
                    {
                        std::cout << "[HandleStringResultCallback] Result is null" << std::endl;
                        callbackLogger.Log("Result is null");
                        const auto result = env.Null();
                        jsCallback.Call({isError, result});
                    }
                    else
                    {
                        std::cout << "[HandleStringResultCallback] Result is not null" << std::endl;
                        callbackLogger.Log("Result is not null");
                        const auto resultStr = std::unique_ptr<char16_t[], common_deallocor<char16_t>>(returnData->value);
                        const auto result = Napi::String::New(env, resultStr.get());
                        jsCallback.Call({isError, result});
                    }
                }
                std::cout << "[HandleStringResultCallback] TSFN callback completed" << std::endl;
            };

            std::cout << "[HandleStringResultCallback] Calling NonBlockingCall" << std::endl;
            manager->tsfn.NonBlockingCall(callback);
            std::cout << "[HandleStringResultCallback] Calling Release" << std::endl;
            manager->tsfn.Release();
            std::cout << "[HandleStringResultCallback] Done" << std::endl;
        }
        catch (const Napi::Error &e)
        {
            logger.LogError(e);
        }
        catch (const std::exception &e)
        {
            logger.LogException(e);
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
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
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
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

        std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
        return conv.from_bytes(e.Message());
    }

    // Helper to check if a Napi::Value is a Promise
    inline bool IsPromise(const Napi::Value &value)
    {
        if (!value.IsObject())
            return false;
        const auto obj = value.As<Napi::Object>();
        return obj.Has("then") && obj.Get("then").IsFunction();
    }

    // Helper to handle Promise results for string-returning callbacks
    // If jsResult is a Promise, attaches then/catch handlers that will call the callback
    // If jsResult is not a Promise, calls the callback immediately with the result
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
        std::cout << "[HandleStringPromiseOrValue] invoked, mtx=" << (mtx ? "yes" : "no") << std::endl;
        if (IsPromise(jsResult))
        {
            std::cout << "[HandleStringPromiseOrValue] jsResult is Promise" << std::endl;
            const auto promise = jsResult.As<Napi::Object>();
            const auto then = promise.Get("then").As<Napi::Function>();

            // Create resolve handler
            auto onResolve = Napi::Function::New(env, [p_callback_handler, p_callback, mtx, cv, completed, result](const Napi::CallbackInfo &info)
                                                 {
                std::cout << "[HandleStringPromiseOrValue onResolve] handler invoked" << std::endl;
                const auto resolvedValue = info[0];
                p_callback(p_callback_handler, ConvertToStringResult(resolvedValue));
                std::cout << "[HandleStringPromiseOrValue onResolve] p_callback done" << std::endl;
                if (mtx && cv && completed && result) {
                    std::cout << "[HandleStringPromiseOrValue onResolve] signaling cv" << std::endl;
                    std::lock_guard<std::mutex> lock(*mtx);
                    *result = Create(return_value_void{nullptr});
                    *completed = true;
                    cv->notify_one();
                    std::cout << "[HandleStringPromiseOrValue onResolve] cv signaled" << std::endl;
                } });

            // Create reject handler
            auto onReject = Napi::Function::New(env, [p_callback_handler, p_callback, mtx, cv, completed, result](const Napi::CallbackInfo &info)
                                                {
                std::cout << "[HandleStringPromiseOrValue onReject] handler invoked" << std::endl;
                const auto error = info[0];
                std::u16string errorMsg = u"Promise rejected";
                if (error.IsObject()) {
                    const auto errorObj = error.As<Napi::Object>();
                    if (errorObj.Has("message")) {
                        errorMsg = errorObj.Get("message").As<Napi::String>().Utf16Value();
                    }
                }
                p_callback(p_callback_handler, Create(return_value_string{Copy(errorMsg), nullptr}));
                if (mtx && cv && completed && result) {
                    std::cout << "[HandleStringPromiseOrValue onReject] signaling cv" << std::endl;
                    std::lock_guard<std::mutex> lock(*mtx);
                    *result = Create(return_value_void{nullptr});
                    *completed = true;
                    cv->notify_one();
                } });

            then.Call(promise, {onResolve, onReject});
            std::cout << "[HandleStringPromiseOrValue] .then() attached" << std::endl;
        }
        else
        {
            std::cout << "[HandleStringPromiseOrValue] jsResult is not Promise" << std::endl;
            p_callback(p_callback_handler, ConvertToStringResult(jsResult));
            if (mtx && cv && completed && result)
            {
                std::cout << "[HandleStringPromiseOrValue] signaling cv (non-Promise)" << std::endl;
                std::lock_guard<std::mutex> lock(*mtx);
                *result = Create(return_value_void{nullptr});
                *completed = true;
                cv->notify_one();
            }
        }
        std::cout << "[HandleStringPromiseOrValue] done" << std::endl;
    }

    // Helper to handle Promise results for JSON-returning callbacks
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
        std::cout << "[HandleJsonPromiseOrValue] invoked, mtx=" << (mtx ? "yes" : "no") << std::endl;
        if (IsPromise(jsResult))
        {
            std::cout << "[HandleJsonPromiseOrValue] jsResult is Promise" << std::endl;
            const auto promise = jsResult.As<Napi::Object>();
            const auto then = promise.Get("then").As<Napi::Function>();

            // Create resolve handler
            auto onResolve = Napi::Function::New(env, [p_callback_handler, p_callback, mtx, cv, completed, result](const Napi::CallbackInfo &info)
                                                 {
                std::cout << "[HandleJsonPromiseOrValue onResolve] handler invoked" << std::endl;
                const auto resolvedValue = info[0];
                p_callback(p_callback_handler, ConvertToJsonResult(resolvedValue));
                std::cout << "[HandleJsonPromiseOrValue onResolve] p_callback done" << std::endl;
                if (mtx && cv && completed && result) {
                    std::cout << "[HandleJsonPromiseOrValue onResolve] signaling cv" << std::endl;
                    std::lock_guard<std::mutex> lock(*mtx);
                    *result = Create(return_value_void{nullptr});
                    *completed = true;
                    cv->notify_one();
                    std::cout << "[HandleJsonPromiseOrValue onResolve] cv signaled" << std::endl;
                } });

            // Create reject handler
            auto onReject = Napi::Function::New(env, [p_callback_handler, p_callback, mtx, cv, completed, result](const Napi::CallbackInfo &info)
                                                {
                std::cout << "[HandleJsonPromiseOrValue onReject] handler invoked" << std::endl;
                const auto error = info[0];
                std::u16string errorMsg = u"Promise rejected";
                if (error.IsObject()) {
                    const auto errorObj = error.As<Napi::Object>();
                    if (errorObj.Has("message")) {
                        errorMsg = errorObj.Get("message").As<Napi::String>().Utf16Value();
                    }
                }
                p_callback(p_callback_handler, Create(return_value_json{Copy(errorMsg), nullptr}));
                if (mtx && cv && completed && result) {
                    std::cout << "[HandleJsonPromiseOrValue onReject] signaling cv" << std::endl;
                    std::lock_guard<std::mutex> lock(*mtx);
                    *result = Create(return_value_void{nullptr});
                    *completed = true;
                    cv->notify_one();
                } });

            then.Call(promise, {onResolve, onReject});
            std::cout << "[HandleJsonPromiseOrValue] .then() attached" << std::endl;
        }
        else
        {
            std::cout << "[HandleJsonPromiseOrValue] jsResult is not Promise" << std::endl;
            p_callback(p_callback_handler, ConvertToJsonResult(jsResult));
            if (mtx && cv && completed && result)
            {
                std::cout << "[HandleJsonPromiseOrValue] signaling cv (non-Promise)" << std::endl;
                std::lock_guard<std::mutex> lock(*mtx);
                *result = Create(return_value_void{nullptr});
                *completed = true;
                cv->notify_one();
            }
        }
        std::cout << "[HandleJsonPromiseOrValue] done" << std::endl;
    }

    // Helper to handle Promise results for data-returning callbacks
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
        if (IsPromise(jsResult))
        {
            const auto promise = jsResult.As<Napi::Object>();
            const auto then = promise.Get("then").As<Napi::Function>();

            // Create resolve handler
            auto onResolve = Napi::Function::New(env, [p_callback_handler, p_callback, mtx, cv, completed, result](const Napi::CallbackInfo &info)
                                                 {
                const auto resolvedValue = info[0];
                p_callback(p_callback_handler, ConvertToDataResult(resolvedValue));
                if (mtx && cv && completed && result) {
                    std::lock_guard<std::mutex> lock(*mtx);
                    *result = Create(return_value_void{nullptr});
                    *completed = true;
                    cv->notify_one();
                } });

            // Create reject handler
            auto onReject = Napi::Function::New(env, [p_callback_handler, p_callback, mtx, cv, completed, result](const Napi::CallbackInfo &info)
                                                {
                const auto error = info[0];
                std::u16string errorMsg = u"Promise rejected";
                if (error.IsObject()) {
                    const auto errorObj = error.As<Napi::Object>();
                    if (errorObj.Has("message")) {
                        errorMsg = errorObj.Get("message").As<Napi::String>().Utf16Value();
                    }
                }
                p_callback(p_callback_handler, Create(return_value_data{Copy(errorMsg), nullptr, 0}));
                if (mtx && cv && completed && result) {
                    std::lock_guard<std::mutex> lock(*mtx);
                    *result = Create(return_value_void{nullptr});
                    *completed = true;
                    cv->notify_one();
                } });

            then.Call(promise, {onResolve, onReject});
        }
        else
        {
            p_callback(p_callback_handler, ConvertToDataResult(jsResult));
            if (mtx && cv && completed && result)
            {
                std::lock_guard<std::mutex> lock(*mtx);
                *result = Create(return_value_void{nullptr});
                *completed = true;
                cv->notify_one();
            }
        }
    }

    // Helper to handle Promise results for void-returning callbacks
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
        std::cout << "[HandleVoidPromiseOrValue] invoked, mtx=" << (mtx ? "yes" : "no") << std::endl;
        if (IsPromise(jsResult))
        {
            std::cout << "[HandleVoidPromiseOrValue] jsResult is Promise" << std::endl;
            const auto promise = jsResult.As<Napi::Object>();
            const auto then = promise.Get("then").As<Napi::Function>();

            // Create resolve handler
            auto onResolve = Napi::Function::New(env, [p_callback_handler, p_callback, mtx, cv, completed, result](const Napi::CallbackInfo &info)
                                                 {
                std::cout << "[HandleVoidPromiseOrValue onResolve] handler invoked" << std::endl;
                p_callback(p_callback_handler, Create(return_value_void{nullptr}));
                std::cout << "[HandleVoidPromiseOrValue onResolve] p_callback done" << std::endl;
                if (mtx && cv && completed && result) {
                    std::cout << "[HandleVoidPromiseOrValue onResolve] signaling cv" << std::endl;
                    std::lock_guard<std::mutex> lock(*mtx);
                    *result = Create(return_value_void{nullptr});
                    *completed = true;
                    cv->notify_one();
                    std::cout << "[HandleVoidPromiseOrValue onResolve] cv signaled" << std::endl;
                } });

            // Create reject handler
            auto onReject = Napi::Function::New(env, [p_callback_handler, p_callback, mtx, cv, completed, result](const Napi::CallbackInfo &info)
                                                {
                std::cout << "[HandleVoidPromiseOrValue onReject] handler invoked" << std::endl;
                const auto error = info[0];
                std::u16string errorMsg = u"Promise rejected";
                if (error.IsObject()) {
                    const auto errorObj = error.As<Napi::Object>();
                    if (errorObj.Has("message")) {
                        errorMsg = errorObj.Get("message").As<Napi::String>().Utf16Value();
                    }
                }
                p_callback(p_callback_handler, Create(return_value_void{Copy(errorMsg)}));
                if (mtx && cv && completed && result) {
                    std::cout << "[HandleVoidPromiseOrValue onReject] signaling cv" << std::endl;
                    std::lock_guard<std::mutex> lock(*mtx);
                    *result = Create(return_value_void{nullptr});
                    *completed = true;
                    cv->notify_one();
                } });

            then.Call(promise, {onResolve, onReject});
            std::cout << "[HandleVoidPromiseOrValue] .then() attached" << std::endl;
        }
        else
        {
            std::cout << "[HandleVoidPromiseOrValue] jsResult is not Promise" << std::endl;
            p_callback(p_callback_handler, Create(return_value_void{nullptr}));
            if (mtx && cv && completed && result)
            {
                std::cout << "[HandleVoidPromiseOrValue] signaling cv (non-Promise)" << std::endl;
                std::lock_guard<std::mutex> lock(*mtx);
                *result = Create(return_value_void{nullptr});
                *completed = true;
                cv->notify_one();
            }
        }
        std::cout << "[HandleVoidPromiseOrValue] done" << std::endl;
    }
}

#endif
