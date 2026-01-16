#ifndef VE_LAUNCHERMANAGER_CB_GUARD_HPP_
#define VE_LAUNCHERMANAGER_CB_GUARD_HPP_

#include "Bannerlord.LauncherManager.Native.h"
#include "Bindings.LauncherManager.hpp"
#include "Logger.hpp"
#include "Utils.Callbacks.hpp"
#include "Utils.Converters.hpp"
#include <napi.h>

using namespace Napi;
using namespace Utils;
using namespace Bannerlord::LauncherManager::Native;

namespace Bindings::LauncherManager
{
// Helper to get manager from owner pointer
inline auto GetManager(param_ptr *p_owner)
{
    return const_cast<Bindings::LauncherManager::LauncherManager *>(static_cast<const Bindings::LauncherManager::LauncherManager *>(p_owner));
}

static return_value_void *setGameParameters(param_ptr *p_owner, param_string *p_executable, param_json *p_game_parameters, param_ptr *p_callback_handler,
                                            void (*p_callback)(param_ptr *, return_value_void *)) noexcept
{
    auto manager = GetManager(p_owner);
    auto mainThreadCall = [p_executable, p_game_parameters, p_callback_handler, p_callback](auto *mgr) {
        const auto env = mgr->FSetGameParameters.Env();
        const auto executable = p_executable == nullptr ? env.Null() : String::New(env, p_executable);
        const auto gameParameters = p_game_parameters == nullptr ? env.Null() : JSONParse(String::New(env, p_game_parameters));
        const auto jsResult = mgr->FSetGameParameters.Call({executable, gameParameters});
        HandleVoidPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
    };
    auto backgroundCall = [p_executable, p_game_parameters, p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback, std::mutex *mtx, std::condition_variable *cv,
                                                                                            bool *completed, return_value_void **result) {
        const auto executable = p_executable == nullptr ? env.Null() : String::New(env, p_executable);
        const auto gameParameters = p_game_parameters == nullptr ? env.Null() : JSONParse(String::New(env, p_game_parameters));
        const auto jsResult = jsCallback.Call({executable, gameParameters});
        HandleVoidPromiseOrValue(env, jsResult, p_callback_handler, p_callback, mtx, cv, completed, result);
    };
    return AwaitableCallback<return_value_void>(__FUNCTION__, manager, manager->TSFNSetGameParameters, p_callback_handler, p_callback, mainThreadCall, backgroundCall);
}

static return_value_void *sendNotification(param_ptr *p_owner, param_string *p_id, param_string *p_type, param_string *p_message, param_uint displayMs,
                                           param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_void *)) noexcept
{
    auto manager = GetManager(p_owner);
    auto mainThreadCall = [p_id, p_type, p_message, displayMs, p_callback_handler, p_callback](auto *mgr) {
        const auto env = mgr->FSendNotification.Env();
        const auto id = p_id == nullptr ? env.Null() : String::New(env, p_id);
        const auto type = p_type == nullptr ? env.Null() : String::New(env, p_type);
        const auto message = p_message == nullptr ? env.Null() : String::New(env, p_message);
        const auto displayMs_ = Number::New(env, displayMs);
        const auto jsResult = mgr->FSendNotification.Call({id, type, message, displayMs_});
        HandleVoidPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
    };
    auto backgroundCall = [p_id, p_type, p_message, displayMs, p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback, std::mutex *mtx,
                                                                                               std::condition_variable *cv, bool *completed, return_value_void **result) {
        const auto id = p_id == nullptr ? env.Null() : String::New(env, p_id);
        const auto type = p_type == nullptr ? env.Null() : String::New(env, p_type);
        const auto message = p_message == nullptr ? env.Null() : String::New(env, p_message);
        const auto displayMs_ = Number::New(env, displayMs);
        const auto jsResult = jsCallback.Call({id, type, message, displayMs_});
        HandleVoidPromiseOrValue(env, jsResult, p_callback_handler, p_callback, mtx, cv, completed, result);
    };
    return AwaitableCallback<return_value_void>(__FUNCTION__, manager, manager->TSFNSendNotification, p_callback_handler, p_callback, mainThreadCall, backgroundCall);
}
static return_value_void *sendDialog(param_ptr *p_owner, param_string *p_type, param_string *p_title, param_string *p_message, param_json *p_filters, param_ptr *p_callback_handler,
                                     void (*p_callback)(param_ptr *, return_value_string *)) noexcept
{
    auto manager = GetManager(p_owner);
    auto mainThreadCall = [p_type, p_title, p_message, p_filters, p_callback_handler, p_callback](auto *mgr) {
        const auto env = mgr->FSendDialog.Env();
        const auto jsResult =
            mgr->FSendDialog.Call({p_type == nullptr ? env.Null() : String::New(env, p_type), p_title == nullptr ? env.Null() : String::New(env, p_title),
                                   p_message == nullptr ? env.Null() : String::New(env, p_message), p_filters == nullptr ? env.Null() : JSONParse(String::New(env, p_filters))});
        HandleStringPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
    };
    auto backgroundCall = [p_type, p_title, p_message, p_filters, p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback, std::mutex *mtx,
                                                                                                  std::condition_variable *cv, bool *completed, return_value_void **result) {
        const auto jsResult =
            jsCallback.Call({p_type == nullptr ? env.Null() : String::New(env, p_type), p_title == nullptr ? env.Null() : String::New(env, p_title),
                             p_message == nullptr ? env.Null() : String::New(env, p_message), p_filters == nullptr ? env.Null() : JSONParse(String::New(env, p_filters))});
        HandleStringPromiseOrValue(env, jsResult, p_callback_handler, p_callback, mtx, cv, completed, result);
    };
    return AwaitableCallback<return_value_string>(__FUNCTION__, manager, manager->TSFNSendDialog, p_callback_handler, p_callback, mainThreadCall, backgroundCall);
}
static return_value_void *getInstallPath(param_ptr *p_owner, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_string *)) noexcept
{
    auto manager = GetManager(p_owner);
    auto mainThreadCall = [p_callback_handler, p_callback](auto *mgr) {
        const auto env = mgr->FGetInstallPath.Env();
        const auto jsResult = mgr->FGetInstallPath.Call({});
        HandleStringPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
    };
    auto backgroundCall = [p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback, std::mutex *mtx, std::condition_variable *cv, bool *completed,
                                                           return_value_void **result) {
        const auto jsResult = jsCallback.Call({});
        HandleStringPromiseOrValue(env, jsResult, p_callback_handler, p_callback, mtx, cv, completed, result);
    };
    return AwaitableCallback<return_value_string>(__FUNCTION__, manager, manager->TSFNGetInstallPath, p_callback_handler, p_callback, mainThreadCall, backgroundCall);
}
static return_value_void *readFileContent(param_ptr *p_owner, param_string *p_file_path, param_int offset, param_int length, param_ptr *p_callback_handler,
                                          void (*p_callback)(param_ptr *, return_value_data *)) noexcept
{
    auto manager = GetManager(p_owner);
    auto mainThreadCall = [p_file_path, offset, length, p_callback_handler, p_callback](auto *mgr) {
        const auto env = mgr->FReadFileContent.Env();
        const auto filePath = p_file_path == nullptr ? env.Null() : String::New(env, p_file_path);
        const auto offset_ = Number::New(env, offset);
        const auto length_ = Number::New(env, length);
        const auto jsResult = mgr->FReadFileContent.Call({filePath, offset_, length_});
        HandleDataPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
    };
    auto backgroundCall = [p_file_path, offset, length, p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback, std::mutex *mtx, std::condition_variable *cv,
                                                                                        bool *completed, return_value_void **result) {
        const auto filePath = p_file_path == nullptr ? env.Null() : String::New(env, p_file_path);
        const auto offset_ = Number::New(env, offset);
        const auto length_ = Number::New(env, length);
        const auto jsResult = jsCallback.Call({filePath, offset_, length_});
        HandleDataPromiseOrValue(env, jsResult, p_callback_handler, p_callback, mtx, cv, completed, result);
    };
    return AwaitableCallback<return_value_data>(__FUNCTION__, manager, manager->TSFNReadFileContent, p_callback_handler, p_callback, mainThreadCall, backgroundCall);
}

static return_value_void *writeFileContent(param_ptr *p_owner, param_string *p_file_path, param_data *p_data, param_int length, param_ptr *p_callback_handler,
                                           void (*p_callback)(param_ptr *, return_value_void *)) noexcept
{
    auto manager = GetManager(p_owner);
    auto mainThreadCall = [p_file_path, p_data, length, p_callback_handler, p_callback](auto *mgr) {
        const auto env = mgr->FWriteFileContent.Env();
        const auto filePath = p_file_path == nullptr ? env.Null() : String::New(env, p_file_path);
        const auto data = Buffer<uint8_t>::NewOrCopy(env, p_data, static_cast<size_t>(length));
        const auto jsResult = mgr->FWriteFileContent.Call({filePath, data});
        HandleVoidPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
    };
    auto backgroundCall = [p_file_path, p_data, length, p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback, std::mutex *mtx, std::condition_variable *cv,
                                                                                        bool *completed, return_value_void **result) {
        const auto filePath = p_file_path == nullptr ? env.Null() : String::New(env, p_file_path);
        const auto data = Buffer<uint8_t>::NewOrCopy(env, p_data, static_cast<size_t>(length));
        const auto jsResult = jsCallback.Call({filePath, data});
        HandleVoidPromiseOrValue(env, jsResult, p_callback_handler, p_callback, mtx, cv, completed, result);
    };
    return AwaitableCallback<return_value_void>(__FUNCTION__, manager, manager->TSFNWriteFileContent, p_callback_handler, p_callback, mainThreadCall, backgroundCall);
}

static return_value_void *readDirectoryFileList(param_ptr *p_owner, param_string *p_directory_path, param_ptr *p_callback_handler,
                                                void (*p_callback)(param_ptr *, return_value_json *)) noexcept
{
    auto manager = GetManager(p_owner);
    auto mainThreadCall = [p_directory_path, p_callback_handler, p_callback](auto *mgr) {
        const auto env = mgr->FReadDirectoryFileList.Env();
        const auto directoryPath = p_directory_path == nullptr ? env.Null() : String::New(env, p_directory_path);
        const auto jsResult = mgr->FReadDirectoryFileList.Call({directoryPath});
        HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
    };
    auto backgroundCall = [p_directory_path, p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback, std::mutex *mtx, std::condition_variable *cv,
                                                                             bool *completed, return_value_void **result) {
        const auto directoryPath = p_directory_path == nullptr ? env.Null() : String::New(env, p_directory_path);
        const auto jsResult = jsCallback.Call({directoryPath});
        HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback, mtx, cv, completed, result);
    };
    return AwaitableCallback<return_value_json>(__FUNCTION__, manager, manager->TSFNReadDirectoryFileList, p_callback_handler, p_callback, mainThreadCall, backgroundCall);
}

static return_value_void *readDirectoryList(param_ptr *p_owner, param_string *p_directory_path, param_ptr *p_callback_handler,
                                            void (*p_callback)(param_ptr *, return_value_json *)) noexcept
{
    auto manager = GetManager(p_owner);
    auto mainThreadCall = [p_directory_path, p_callback_handler, p_callback](auto *mgr) {
        const auto env = mgr->FReadDirectoryList.Env();
        const auto directoryPath = p_directory_path == nullptr ? env.Null() : String::New(env, p_directory_path);
        const auto jsResult = mgr->FReadDirectoryList.Call({directoryPath});
        HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
    };
    auto backgroundCall = [p_directory_path, p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback, std::mutex *mtx, std::condition_variable *cv,
                                                                             bool *completed, return_value_void **result) {
        const auto directoryPath = p_directory_path == nullptr ? env.Null() : String::New(env, p_directory_path);
        const auto jsResult = jsCallback.Call({directoryPath});
        HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback, mtx, cv, completed, result);
    };
    return AwaitableCallback<return_value_json>(__FUNCTION__, manager, manager->TSFNReadDirectoryList, p_callback_handler, p_callback, mainThreadCall, backgroundCall);
}

static return_value_void *getAllModuleViewModels(param_ptr *p_owner, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_json *)) noexcept
{
    auto manager = GetManager(p_owner);
    auto mainThreadCall = [p_callback_handler, p_callback](auto *mgr) {
        const auto env = mgr->FGetAllModuleViewModels.Env();
        const auto jsResult = mgr->FGetAllModuleViewModels.Call({});
        HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
    };
    auto backgroundCall = [p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback, std::mutex *mtx, std::condition_variable *cv, bool *completed,
                                                           return_value_void **result) {
        const auto jsResult = jsCallback.Call({});
        HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback, mtx, cv, completed, result);
    };
    return AwaitableCallback<return_value_json>(__FUNCTION__, manager, manager->TSFNGetAllModuleViewModels, p_callback_handler, p_callback, mainThreadCall, backgroundCall);
}

static return_value_void *getModuleViewModels(param_ptr *p_owner, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_json *)) noexcept
{
    auto manager = GetManager(p_owner);
    auto mainThreadCall = [p_callback_handler, p_callback](auto *mgr) {
        const auto env = mgr->FGetModuleViewModels.Env();
        const auto jsResult = mgr->FGetModuleViewModels.Call({});
        HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
    };
    auto backgroundCall = [p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback, std::mutex *mtx, std::condition_variable *cv, bool *completed,
                                                           return_value_void **result) {
        const auto jsResult = jsCallback.Call({});
        HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback, mtx, cv, completed, result);
    };
    return AwaitableCallback<return_value_json>(__FUNCTION__, manager, manager->TSFNGetModuleViewModels, p_callback_handler, p_callback, mainThreadCall, backgroundCall);
}
static return_value_void *setModuleViewModels(param_ptr *p_owner, param_json *p_module_view_models, param_ptr *p_callback_handler,
                                              void (*p_callback)(param_ptr *, return_value_void *)) noexcept
{
    auto manager = GetManager(p_owner);
    auto mainThreadCall = [p_module_view_models, p_callback_handler, p_callback](auto *mgr) {
        const auto env = mgr->FSetModuleViewModels.Env();
        const auto moduleViewModels = p_module_view_models == nullptr ? env.Null() : JSONParse(Napi::String::New(env, p_module_view_models));
        const auto jsResult = mgr->FSetModuleViewModels.Call({moduleViewModels});
        HandleVoidPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
    };
    auto backgroundCall = [p_module_view_models, p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback, std::mutex *mtx, std::condition_variable *cv,
                                                                                 bool *completed, return_value_void **result) {
        const auto moduleViewModels = p_module_view_models == nullptr ? env.Null() : JSONParse(Napi::String::New(env, p_module_view_models));
        const auto jsResult = jsCallback.Call({moduleViewModels});
        HandleVoidPromiseOrValue(env, jsResult, p_callback_handler, p_callback, mtx, cv, completed, result);
    };
    return AwaitableCallback<return_value_void>(__FUNCTION__, manager, manager->TSFNSetModuleViewModels, p_callback_handler, p_callback, mainThreadCall, backgroundCall);
}
static return_value_void *getOptions(param_ptr *p_owner, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_json *)) noexcept
{
    auto manager = GetManager(p_owner);
    auto mainThreadCall = [p_callback_handler, p_callback](auto *mgr) {
        const auto env = mgr->FGetOptions.Env();
        const auto jsResult = mgr->FGetOptions.Call({});
        HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
    };
    auto backgroundCall = [p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback, std::mutex *mtx, std::condition_variable *cv, bool *completed,
                                                           return_value_void **result) {
        const auto jsResult = jsCallback.Call({});
        HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback, mtx, cv, completed, result);
    };
    return AwaitableCallback<return_value_json>(__FUNCTION__, manager, manager->TSFNGetOptions, p_callback_handler, p_callback, mainThreadCall, backgroundCall);
}

static return_value_void *getState(param_ptr *p_owner, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_json *)) noexcept
{
    auto manager = GetManager(p_owner);
    auto mainThreadCall = [p_callback_handler, p_callback](auto *mgr) {
        const auto env = mgr->FGetState.Env();
        const auto jsResult = mgr->FGetState.Call({});
        HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback);
    };
    auto backgroundCall = [p_callback_handler, p_callback](Napi::Env env, Napi::Function jsCallback, std::mutex *mtx, std::condition_variable *cv, bool *completed,
                                                           return_value_void **result) {
        const auto jsResult = jsCallback.Call({});
        HandleJsonPromiseOrValue(env, jsResult, p_callback_handler, p_callback, mtx, cv, completed, result);
    };
    return AwaitableCallback<return_value_json>(__FUNCTION__, manager, manager->TSFNGetState, p_callback_handler, p_callback, mainThreadCall, backgroundCall);
}
} // namespace Bindings::LauncherManager
#endif
