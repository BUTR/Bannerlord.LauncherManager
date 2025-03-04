#ifndef VE_LAUNCHERMANAGER_GUARD_HPP_
#define VE_LAUNCHERMANAGER_GUARD_HPP_

#include "utils.hpp"
#include "Bannerlord.LauncherManager.Native.h"
#include <sstream>
#include <string>

using namespace Napi;
using namespace Utils;
using namespace Bannerlord::LauncherManager::Native;

namespace Bannerlord::LauncherManager
{
    class LauncherManager : public Napi::ObjectWrap<LauncherManager>
    {
    public:
        Napi::ThreadSafeFunction TSFN = Napi::ThreadSafeFunction::New(Env(), Napi::Function::New(Env(), TSFNFunction), "TSFN", 0, 1);
        static void TSFNFunction(const Napi::CallbackInfo &info)
        {
            // LoggerScope logger(__FUNCTION__);
            const auto length = info.Length();
            // logger.Log("Length: " + std::to_string(length));
            if (length == 2)
            {
                const auto promise = info[0].As<Napi::Promise>();
                const auto onResolve = info[1].As<Napi::Function>();
                const auto then = promise.Get("then").As<Napi::Function>();
                then.Call(promise, {onResolve});
            }
            else if (length == 3)
            {
                const auto promise = info[0].As<Napi::Promise>();
                const auto onResolve = info[1].As<Napi::Function>();
                const auto onReject = info[2].As<Napi::Function>();
                const auto then = promise.Get("then").As<Napi::Function>();
                then.Call(promise, {onResolve, onReject});
            }
        }

        FunctionReference FGetActiveProfile;
        FunctionReference FGetProfileById;
        FunctionReference FGetActiveGameId;
        FunctionReference FSetGameParameters;
        FunctionReference FSendNotification;
        FunctionReference FSendDialog;
        FunctionReference FGetInstallPath;
        FunctionReference FReadFileContent;
        FunctionReference FWriteFileContent;
        FunctionReference FReadDirectoryFileList;
        FunctionReference FReadDirectoryList;
        FunctionReference FGetAllModuleViewModels;
        FunctionReference FGetModuleViewModels;
        FunctionReference FSetModuleViewModels;
        FunctionReference FGetOptions;
        FunctionReference FGetState;

        static Object Init(const Napi::Env env, const Object exports);

        LauncherManager(const CallbackInfo &info);
        ~LauncherManager();

        Napi::Value CheckForRootHarmonyAsync(const CallbackInfo &info);
        Napi::Value GetGamePlatformAsync(const CallbackInfo &info);
        Napi::Value GetGameVersionAsync(const CallbackInfo &info);
        Napi::Value GetModulesAsync(const CallbackInfo &info);
        Napi::Value GetAllModulesAsync(const CallbackInfo &info);
        Napi::Value GetSaveFilePathAsync(const CallbackInfo &info);
        Napi::Value GetSaveFilesAsync(const CallbackInfo &info);
        Napi::Value GetSaveMetadataAsync(const CallbackInfo &info);
        Napi::Value InstallModule(const CallbackInfo &info);
        Napi::Value IsObfuscatedAsync(const CallbackInfo &info);
        Napi::Value IsSorting(const CallbackInfo &info);
        Napi::Value ModuleListHandlerExportAsync(const CallbackInfo &info);
        Napi::Value ModuleListHandlerExportSaveFileAsync(const CallbackInfo &info);
        Napi::Value ModuleListHandlerImportAsync(const CallbackInfo &info);
        Napi::Value ModuleListHandlerImportSaveFileAsync(const CallbackInfo &info);
        Napi::Value OrderByLoadOrderAsync(const CallbackInfo &info);
        Napi::Value RefreshGameParametersAsync(const CallbackInfo &info);
        Napi::Value RefreshModulesAsync(const CallbackInfo &info);
        Napi::Value SetGameParameterExecutableAsync(const CallbackInfo &info);
        Napi::Value SetGameParameterSaveFileAsync(const CallbackInfo &info);
        Napi::Value SetGameParameterContinueLastSaveFileAsync(const CallbackInfo &info);
        void SetGameStore(const CallbackInfo &info);
        Napi::Value SortAsync(const CallbackInfo &info);
        Napi::Value SortHelperChangeModulePositionAsync(const CallbackInfo &info);
        Napi::Value SortHelperToggleModuleSelectionAsync(const CallbackInfo &info);
        Napi::Value SortHelperValidateModuleAsync(const CallbackInfo &info);
        Napi::Value TestModule(const CallbackInfo &info);
        Napi::Value DialogTestFileOpenAsync(const CallbackInfo &info);
        Napi::Value DialogTestWarningAsync(const CallbackInfo &info);
        Napi::Value SetGameParameterLoadOrderAsync(const CallbackInfo &info);

    private:
        void *_pInstance;
    };

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

    static return_value_void *setGameParameters(param_ptr *p_owner, param_string *p_executable, param_json *p_game_parameters, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_void *)) noexcept
    {
        // LoggerScope(__FUNCTION__, p_executable, p_game_parameters);
        try
        {
            const auto manager = static_cast<const Bannerlord::LauncherManager::LauncherManager *>(p_owner);
            const auto tsfn = manager->TSFN;

            const auto onResolve = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                p_callback(p_callback_handler, Create(return_value_void{}));
            };
            const auto onReject = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                const auto env = info.Env();
                if (info.Length() == 0)
                {
                    p_callback(p_callback_handler, Create(return_value_void{Copy(u"Unknown error")}));
                }
                else
                {
                    const auto error = info[0].As<Napi::Error>();
                    p_callback(p_callback_handler, Create(return_value_void{Copy(GetErrorMessage(error))}));
                }
            };

            const auto callback = [p_executable, p_game_parameters, onResolve, onReject](Napi::Env env, Napi::Function jsCallback, const Bannerlord::LauncherManager::LauncherManager *manager)
            {
                const auto executable = p_executable == nullptr ? env.Null() : String::New(env, p_executable);
                const auto gameParameters = p_game_parameters == nullptr ? env.Null() : JSONParse(String::New(env, p_game_parameters));
                const auto promise = manager->FSetGameParameters({executable, gameParameters}).As<Napi::Promise>();

                const auto onResolveCallback = Napi::Function::New(env, onResolve);
                const auto onRejectCallback = Napi::Function::New(env, onReject);
                jsCallback.Call({promise, onResolveCallback, onRejectCallback});
            };
            tsfn.BlockingCall(manager, callback);

            return Create(return_value_void{});
        }
        catch (const Napi::Error &e)
        {
            // logger.Log("Error: " + std::string(e.Message()));
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
    }
    static return_value_void *sendNotification(param_ptr *p_owner, param_string *p_id, param_string *p_type, param_string *p_message, param_uint displayMs, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_void *)) noexcept
    {
        // LoggerScope(__FUNCTION__, p_id, p_type, p_message, displayMs);
        try
        {
            const auto manager = static_cast<const Bannerlord::LauncherManager::LauncherManager *>(p_owner);
            const auto tsfn = manager->TSFN;

            const auto onResolve = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                p_callback(p_callback_handler, Create(return_value_void{}));
            };
            const auto onReject = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                const auto env = info.Env();
                if (info.Length() == 0)
                {
                    p_callback(p_callback_handler, Create(return_value_void{Copy(u"Unknown error")}));
                }
                else
                {
                    const auto error = info[0].As<Napi::Error>();
                    p_callback(p_callback_handler, Create(return_value_void{Copy(GetErrorMessage(error))}));
                }
            };

            const auto callback = [p_id, p_type, p_message, displayMs, onResolve, onReject](Napi::Env env, Napi::Function jsCallback, const Bannerlord::LauncherManager::LauncherManager *manager)
            {
                const auto id = p_id == nullptr ? env.Null() : String::New(env, p_id);
                const auto type = p_type == nullptr ? env.Null() : String::New(env, p_type);
                const auto message = p_message == nullptr ? env.Null() : String::New(env, p_message);
                const auto displayMs_ = Number::New(env, displayMs);
                const auto promise = manager->FSendNotification({id, type, message, displayMs_}).As<Napi::Promise>();

                const auto onResolveCallback = Napi::Function::New(env, onResolve);
                const auto onRejectCallback = Napi::Function::New(env, onReject);
                jsCallback.Call({promise, onResolveCallback, onRejectCallback});
            };
            tsfn.BlockingCall(manager, callback);

            return Create(return_value_void{});
        }
        catch (const Napi::Error &e)
        {
            // logger.Log("Error: " + std::string(e.Message()));
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
    }
    static return_value_void *sendDialog(param_ptr *p_owner, param_string *p_type, param_string *p_title, param_string *p_message, param_json *p_filters, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_string *)) noexcept
    {
        // LoggerScope(__FUNCTION__, p_type, p_title, p_message, p_filters);
        try
        {
            const auto manager = static_cast<const Bannerlord::LauncherManager::LauncherManager *>(p_owner);
            const auto tsfn = manager->TSFN;

            const auto onResolve = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                const auto env = info.Env();
                const auto result = info[0].As<Napi::String>();
                if (result.IsNull())
                {
                    p_callback(p_callback_handler, Create(return_value_string{nullptr, Copy(result.Utf16Value())}));
                }
                else
                {
                    p_callback(p_callback_handler, Create(return_value_string{nullptr, Copy(result.Utf16Value())}));
                }
            };
            const auto onReject = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                const auto env = info.Env();
                if (info.Length() == 0)
                {
                    p_callback(p_callback_handler, Create(return_value_string{Copy(u"Unknown error")}));
                }
                else
                {
                    const auto error = info[0].As<Napi::Error>();
                    p_callback(p_callback_handler, Create(return_value_string{Copy(GetErrorMessage(error))}));
                }
            };

            const auto callback = [p_type, p_title, p_message, p_filters, onResolve, onReject](Napi::Env env, Napi::Function jsCallback, const Bannerlord::LauncherManager::LauncherManager *manager)
            {
                const auto type = p_type == nullptr ? env.Null() : String::New(env, p_type);
                const auto title = p_title == nullptr ? env.Null() : String::New(env, p_title);
                const auto message = p_message == nullptr ? env.Null() : String::New(env, p_message);
                const auto filters = p_filters == nullptr ? env.Null() : JSONParse(String::New(env, p_filters));
                const auto promise = manager->FSendDialog({type, title, message, filters}).As<Napi::Promise>();

                const auto onResolveCallback = Napi::Function::New(env, onResolve);
                const auto onRejectCallback = Napi::Function::New(env, onReject);
                jsCallback.Call({promise, onResolveCallback, onRejectCallback});
            };
            tsfn.BlockingCall(manager, callback);

            return Create(return_value_void{});
        }
        catch (const Napi::Error &e)
        {
            // logger.Log("Error: " + std::string(e.Message()));
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
    }
    static return_value_void *getInstallPath(param_ptr *p_owner, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_string *)) noexcept
    {
        // LoggerScope(__FUNCTION__);
        try
        {
            const auto manager = static_cast<const Bannerlord::LauncherManager::LauncherManager *>(p_owner);
            const auto tsfn = manager->TSFN;

            const auto onResolve = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                const auto env = info.Env();
                const auto result = info[0].As<Napi::String>();
                if (result.IsNull())
                {
                    p_callback(p_callback_handler, Create(return_value_string{}));
                }
                else
                {
                    p_callback(p_callback_handler, Create(return_value_string{nullptr, Copy(result.Utf16Value())}));
                }
            };
            const auto onReject = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                const auto env = info.Env();
                if (info.Length() == 0)
                {
                    p_callback(p_callback_handler, Create(return_value_string{Copy(u"Unknown error")}));
                }
                else
                {
                    const auto error = info[0].As<Napi::Error>();
                    p_callback(p_callback_handler, Create(return_value_string{Copy(GetErrorMessage(error))}));
                }
            };

            const auto callback = [onResolve, onReject](Napi::Env env, Napi::Function jsCallback, const Bannerlord::LauncherManager::LauncherManager *manager)
            {
                const auto promise = manager->FGetInstallPath({}).As<Napi::Promise>();

                const auto onResolveCallback = Napi::Function::New(env, onResolve);
                const auto onRejectCallback = Napi::Function::New(env, onReject);
                jsCallback.Call({promise, onResolveCallback, onRejectCallback});
            };
            tsfn.BlockingCall(manager, callback);

            return Create(return_value_void{});
        }
        catch (const Napi::Error &e)
        {
            // logger.Log("Error: " + std::string(e.Message()));
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
    }
    static return_value_void *readFileContent(param_ptr *p_owner, param_string *p_file_path, param_int offset, param_int length, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_data *)) noexcept
    {
        // LoggerScope(__FUNCTION__, p_file_path, offset, length);
        try
        {
            const auto manager = static_cast<const Bannerlord::LauncherManager::LauncherManager *>(p_owner);
            const auto tsfn = manager->TSFN;

            const auto onResolve = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                const auto env = info.Env();
                const auto result = info[0].As<Napi::Object>();

                if (result.IsNull())
                {
                    p_callback(p_callback_handler, Create(return_value_data{}));
                }
                else if (!result.IsBuffer())
                {
                    std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
                    p_callback(p_callback_handler, Create(return_value_data{Copy(conv.from_bytes("Not an Buffer<uint8_t>"))}));
                }
                else
                {
                    auto buffer = result.As<Uint8Array>();

#ifndef NODE_API_NO_EXTERNAL_BUFFERS_ALLOWED
                    napi_value copyDataValue;
                    napi_status copyDataStatus = napi_get_named_property(env, buffer, "BannerlordSkipCopy", &copyDataValue);
                    const auto copyAsData = Value(env, copyDataValue);
                    if (copyDataStatus == napi_ok && copyAsData.IsBoolean() && copyAsData.As<Boolean>().Value())
                    {
                        p_callback(p_callback_handler, Create(return_value_data{nullptr, buffer.Data(), static_cast<int>(buffer.ByteLength())}));
                    }
                    else
                    {
                        p_callback(p_callback_handler, Create(return_value_data{nullptr, Copy(buffer.Data(), buffer.ByteLength()), static_cast<int>(buffer.ByteLength())}));
                    }
#else
                    p_callback(p_callback_handler, Create(return_value_data{nullptr, Copy(buffer.Data(), buffer.ByteLength()), static_cast<int>(buffer.ByteLength())}));
#endif
                }
            };
            const auto onReject = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                const auto env = info.Env();
                if (info.Length() == 0)
                {
                    p_callback(p_callback_handler, Create(return_value_data{Copy(u"Unknown error")}));
                }
                else
                {
                    const auto error = info[0].As<Napi::Error>();
                    p_callback(p_callback_handler, Create(return_value_data{Copy(GetErrorMessage(error))}));
                }
            };

            const auto callback = [p_file_path, offset, length, onResolve, onReject](Napi::Env env, Napi::Function jsCallback, const Bannerlord::LauncherManager::LauncherManager *manager)
            {
                const auto filePath = p_file_path == nullptr ? env.Null() : String::New(env, p_file_path);
                const auto offset_ = Number::New(env, offset);
                const auto length_ = Number::New(env, length);
                const auto promise = manager->FReadFileContent({filePath, offset_, length_}).As<Napi::Promise>();

                const auto onResolveCallback = Napi::Function::New(env, onResolve);
                const auto onRejectCallback = Napi::Function::New(env, onReject);
                jsCallback.Call({promise, onResolveCallback, onRejectCallback});
            };
            tsfn.BlockingCall(manager, callback);

            return Create(return_value_void{});
        }
        catch (const Napi::Error &e)
        {
            // logger.Log("Error: " + std::string(e.Message()));
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
    }
    static return_value_void *writeFileContent(param_ptr *p_owner, param_string *p_file_path, param_data *p_data, param_int length, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_void *)) noexcept
    {
        // LoggerScope(__FUNCTION__, p_file_path, p_data, length);
        try
        {
            const auto manager = static_cast<const Bannerlord::LauncherManager::LauncherManager *>(p_owner);
            const auto tsfn = manager->TSFN;

            const auto onResolve = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                p_callback(p_callback_handler, Create(return_value_void{}));
            };
            const auto onReject = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                const auto env = info.Env();
                if (info.Length() == 0)
                {
                    p_callback(p_callback_handler, Create(return_value_void{Copy(u"Unknown error")}));
                }
                else
                {
                    const auto error = info[0].As<Napi::Error>();
                    p_callback(p_callback_handler, Create(return_value_void{Copy(GetErrorMessage(error))}));
                }
            };

            const auto callback = [p_file_path, p_data, length, onResolve, onReject](Napi::Env env, Napi::Function jsCallback, const Bannerlord::LauncherManager::LauncherManager *manager)
            {
                const auto filePath = p_file_path == nullptr ? env.Null() : String::New(env, p_file_path);
                const auto data = Buffer<uint8_t>::NewOrCopy(env, p_data, static_cast<size_t>(length));
                const auto promise = manager->FWriteFileContent({filePath, data}).As<Napi::Promise>();

                const auto onResolveCallback = Napi::Function::New(env, onResolve);
                const auto onRejectCallback = Napi::Function::New(env, onReject);
                jsCallback.Call({promise, onResolveCallback, onRejectCallback});
            };
            tsfn.BlockingCall(manager, callback);

            return Create(return_value_void{});
        }
        catch (const Napi::Error &e)
        {
            // logger.Log("Error: " + std::string(e.Message()));
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
    }
    static return_value_void *readDirectoryFileList(param_ptr *p_owner, param_string *p_directory_path, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_json *)) noexcept
    {
        // LoggerScope(__FUNCTION__, p_directory_path);
        try
        {
            const auto manager = static_cast<const Bannerlord::LauncherManager::LauncherManager *>(p_owner);
            const auto tsfn = manager->TSFN;

            const auto onResolve = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                const auto env = info.Env();
                const auto result = info[0].As<Napi::Object>();
                if (result.IsNull())
                {
                    p_callback(p_callback_handler, Create(return_value_json{}));
                }
                else
                {
                    // Logger::Log(NAMEOF(thenCallback), "Value: " + JSONStringify(result).Utf8Value());
                    p_callback(p_callback_handler, Create(return_value_json{nullptr, Copy(JSONStringify(result).Utf16Value())}));
                }
            };
            const auto onReject = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                const auto env = info.Env();
                if (info.Length() == 0)
                {
                    p_callback(p_callback_handler, Create(return_value_json{Copy(u"Unknown error")}));
                }
                else
                {
                    const auto error = info[0].As<Napi::Error>();
                    p_callback(p_callback_handler, Create(return_value_json{Copy(GetErrorMessage(error))}));
                }
            };

            const auto callback = [p_directory_path, onResolve, onReject](Napi::Env env, Napi::Function jsCallback, const Bannerlord::LauncherManager::LauncherManager *manager)
            {
                const auto directoryPath = p_directory_path == nullptr ? env.Null() : String::New(env, p_directory_path);
                const auto promise = manager->FReadDirectoryFileList({directoryPath}).As<Napi::Promise>();

                const auto onResolveCallback = Napi::Function::New(env, onResolve);
                const auto onRejectCallback = Napi::Function::New(env, onReject);
                jsCallback.Call({promise, onResolveCallback, onRejectCallback});
            };
            tsfn.BlockingCall(manager, callback);

            return Create(return_value_void{});
        }
        catch (const Napi::Error &e)
        {
            // logger.Log("Error: " + std::string(e.Message()));
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
    }
    static return_value_void *readDirectoryList(param_ptr *p_owner, param_string *p_directory_path, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_json *)) noexcept
    {
        // LoggerScope(__FUNCTION__, p_directory_path);
        try
        {
            const auto manager = static_cast<const Bannerlord::LauncherManager::LauncherManager *>(p_owner);
            const auto tsfn = manager->TSFN;

            const auto onResolve = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                const auto env = info.Env();
                const auto result = info[0].As<Napi::Object>();
                if (result.IsNull())
                {
                    p_callback(p_callback_handler, Create(return_value_json{}));
                }
                else
                {
                    // Logger::Log(NAMEOF(thenCallback), "Value: " + JSONStringify(result).Utf8Value());
                    p_callback(p_callback_handler, Create(return_value_json{nullptr, Copy(JSONStringify(result).Utf16Value())}));
                }
            };
            const auto onReject = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                const auto env = info.Env();
                if (info.Length() == 0)
                {
                    p_callback(p_callback_handler, Create(return_value_json{Copy(u"Unknown error")}));
                }
                else
                {
                    const auto error = info[0].As<Napi::Error>();
                    p_callback(p_callback_handler, Create(return_value_json{Copy(GetErrorMessage(error))}));
                }
            };

            const auto callback = [p_directory_path, onResolve, onReject](Napi::Env env, Napi::Function jsCallback, const Bannerlord::LauncherManager::LauncherManager *manager)
            {
                const auto directoryPath = p_directory_path == nullptr ? env.Null() : String::New(env, p_directory_path);
                const auto promise = manager->FReadDirectoryList({directoryPath}).As<Napi::Promise>();

                const auto onResolveCallback = Napi::Function::New(env, onResolve);
                const auto onRejectCallback = Napi::Function::New(env, onReject);
                jsCallback.Call({promise, onResolveCallback, onRejectCallback});
            };
            tsfn.BlockingCall(manager, callback);

            return Create(return_value_void{});
        }
        catch (const Napi::Error &e)
        {
            // logger.Log("Error: " + std::string(e.Message()));
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
    }
    static return_value_void *getAllModuleViewModels(param_ptr *p_owner, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_json *)) noexcept
    {
        // LoggerScope(__FUNCTION__);
        try
        {
            const auto manager = static_cast<const Bannerlord::LauncherManager::LauncherManager *>(p_owner);
            const auto tsfn = manager->TSFN;

            const auto onResolve = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                const auto env = info.Env();
                const auto result = info[0].As<Napi::Object>();
                if (result.IsNull())
                {
                    p_callback(p_callback_handler, Create(return_value_json{}));
                }
                else
                {
                    p_callback(p_callback_handler, Create(return_value_json{nullptr, Copy(JSONStringify(result).Utf16Value())}));
                }
            };
            const auto onReject = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                const auto env = info.Env();
                if (info.Length() == 0)
                {
                    p_callback(p_callback_handler, Create(return_value_json{Copy(u"Unknown error")}));
                }
                else
                {
                    const auto error = info[0].As<Napi::Error>();
                    p_callback(p_callback_handler, Create(return_value_json{Copy(GetErrorMessage(error))}));
                }
            };

            const auto callback = [onResolve, onReject](Napi::Env env, Napi::Function jsCallback, const Bannerlord::LauncherManager::LauncherManager *manager)
            {
                const auto promise = manager->FGetAllModuleViewModels({}).As<Napi::Promise>();

                const auto onResolveCallback = Napi::Function::New(env, onResolve);
                const auto onRejectCallback = Napi::Function::New(env, onReject);
                jsCallback.Call({promise, onResolveCallback, onRejectCallback});
            };
            tsfn.BlockingCall(manager, callback);

            return Create(return_value_void{});
        }
        catch (const Napi::Error &e)
        {
            // logger.Log("Error: " + std::string(e.Message()));
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
    }
    static return_value_void *getModuleViewModels(param_ptr *p_owner, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_json *)) noexcept
    {
        // LoggerScope(__FUNCTION__);
        try
        {
            const auto manager = static_cast<const Bannerlord::LauncherManager::LauncherManager *>(p_owner);
            const auto tsfn = manager->TSFN;

            const auto onResolve = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                const auto env = info.Env();
                const auto result = info[0].As<Napi::Object>();
                if (result.IsNull())
                {
                    p_callback(p_callback_handler, Create(return_value_json{}));
                }
                else
                {
                    p_callback(p_callback_handler, Create(return_value_json{nullptr, Copy(JSONStringify(result).Utf16Value())}));
                }
            };
            const auto onReject = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                const auto env = info.Env();
                if (info.Length() == 0)
                {
                    p_callback(p_callback_handler, Create(return_value_json{Copy(u"Unknown error")}));
                }
                else
                {
                    const auto error = info[0].As<Napi::Error>();
                    p_callback(p_callback_handler, Create(return_value_json{Copy(GetErrorMessage(error))}));
                }
            };

            const auto callback = [onResolve, onReject](Napi::Env env, Napi::Function jsCallback, const Bannerlord::LauncherManager::LauncherManager *manager)
            {
                const auto promise = manager->FGetModuleViewModels({}).As<Napi::Promise>();

                const auto onResolveCallback = Napi::Function::New(env, onResolve);
                const auto onRejectCallback = Napi::Function::New(env, onReject);
                jsCallback.Call({promise, onResolveCallback, onRejectCallback});
            };
            tsfn.BlockingCall(manager, callback);

            return Create(return_value_void{});
        }
        catch (const Napi::Error &e)
        {
            // logger.Log("Error: " + std::string(e.Message()));
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
    }
    static return_value_void *setModuleViewModels(param_ptr *p_owner, param_json *p_module_view_models, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_void *)) noexcept
    {
        // LoggerScope(__FUNCTION__, p_module_view_models);
        try
        {
            const auto manager = static_cast<const Bannerlord::LauncherManager::LauncherManager *>(p_owner);
            const auto tsfn = manager->TSFN;

            const auto onResolve = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                p_callback(p_callback_handler, Create(return_value_void{}));
            };
            const auto onReject = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                const auto env = info.Env();
                if (info.Length() == 0)
                {
                    p_callback(p_callback_handler, Create(return_value_void{Copy(u"Unknown error")}));
                }
                else
                {
                    const auto error = info[0].As<Napi::Error>();
                    p_callback(p_callback_handler, Create(return_value_void{Copy(GetErrorMessage(error))}));
                }
            };

            const auto callback = [p_module_view_models, onResolve, onReject](Napi::Env env, Napi::Function jsCallback, const Bannerlord::LauncherManager::LauncherManager *manager)
            {
                const auto moduleViewModels = p_module_view_models == nullptr ? env.Null() : JSONParse(Napi::String::New(env, p_module_view_models));
                const auto promise = manager->FSetModuleViewModels({moduleViewModels}).As<Napi::Promise>();

                const auto onResolveCallback = Napi::Function::New(env, onResolve);
                const auto onRejectCallback = Napi::Function::New(env, onReject);
                jsCallback.Call({promise, onResolveCallback, onRejectCallback});
            };
            tsfn.BlockingCall(manager, callback);

            return Create(return_value_void{});
        }
        catch (const Napi::Error &e)
        {
            // logger.Log("Error: " + std::string(e.Message()));
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
    }
    static return_value_void *getOptions(param_ptr *p_owner, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_json *)) noexcept
    {
        // LoggerScope(__FUNCTION__);
        try
        {
            const auto manager = static_cast<const Bannerlord::LauncherManager::LauncherManager *>(p_owner);
            const auto tsfn = manager->TSFN;

            const auto onResolve = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                const auto env = info.Env();
                const auto result = info[0].As<Napi::Object>();
                if (result.IsNull())
                {
                    p_callback(p_callback_handler, Create(return_value_json{}));
                }
                else
                {
                    p_callback(p_callback_handler, Create(return_value_json{nullptr, Copy(JSONStringify(result).Utf16Value())}));
                }
            };
            const auto onReject = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                const auto env = info.Env();
                if (info.Length() == 0)
                {
                    p_callback(p_callback_handler, Create(return_value_json{Copy(u"Unknown error")}));
                }
                else
                {
                    const auto error = info[0].As<Napi::Error>();
                    p_callback(p_callback_handler, Create(return_value_json{Copy(GetErrorMessage(error))}));
                }
            };

            const auto callback = [onResolve, onReject](Napi::Env env, Napi::Function jsCallback, const Bannerlord::LauncherManager::LauncherManager *manager)
            {
                const auto promise = manager->FGetOptions({}).As<Napi::Promise>();

                const auto onResolveCallback = Napi::Function::New(env, onResolve);
                const auto onRejectCallback = Napi::Function::New(env, onReject);
                jsCallback.Call({promise, onResolveCallback, onRejectCallback});
            };
            tsfn.BlockingCall(manager, callback);

            return Create(return_value_void{});
        }
        catch (const Napi::Error &e)
        {
            // logger.Log("Error: " + std::string(e.Message()));
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
    }
    static return_value_void *getState(param_ptr *p_owner, param_ptr *p_callback_handler, void (*p_callback)(param_ptr *, return_value_json *)) noexcept
    {
        // LoggerScope(__FUNCTION__);
        try
        {
            const auto manager = static_cast<const Bannerlord::LauncherManager::LauncherManager *>(p_owner);
            const auto tsfn = manager->TSFN;

            const auto onResolve = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                const auto env = info.Env();
                const auto result = info[0].As<Napi::Object>();
                if (result.IsNull())
                {
                    p_callback(p_callback_handler, Create(return_value_json{}));
                }
                else
                {
                    p_callback(p_callback_handler, Create(return_value_json{nullptr, Copy(JSONStringify(result).Utf16Value())}));
                }
            };
            const auto onReject = [p_callback_handler, p_callback](const Napi::CallbackInfo &info)
            {
                const auto env = info.Env();
                if (info.Length() == 0)
                {
                    p_callback(p_callback_handler, Create(return_value_json{Copy(u"Unknown error")}));
                }
                else
                {
                    const auto error = info[0].As<Napi::Error>();
                    p_callback(p_callback_handler, Create(return_value_json{Copy(GetErrorMessage(error))}));
                }
            };

            const auto callback = [onResolve, onReject](Napi::Env env, Napi::Function jsCallback, const Bannerlord::LauncherManager::LauncherManager *manager)
            {
                const auto promise = manager->FGetState({}).As<Napi::Promise>();

                const auto onResolveCallback = Napi::Function::New(env, onResolve);
                const auto onRejectCallback = Napi::Function::New(env, onReject);
                jsCallback.Call({promise, onResolveCallback, onRejectCallback});
            };
            tsfn.BlockingCall(manager, callback);

            return Create(return_value_void{});
        }
        catch (const Napi::Error &e)
        {
            // logger.Log("Error: " + std::string(e.Message()));
            return Create(return_value_void{Copy(GetErrorMessage(e))});
        }
    }

    LauncherManager::LauncherManager(const CallbackInfo &info) : ObjectWrap<LauncherManager>(info)
    {
        const auto env = Env();
        this->TSFN.Ref(env);
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
    }

    LauncherManager::~LauncherManager()
    {
        const auto env = Env();
        this->TSFN.Unref(env);
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
        // LOGINPUT();
        const auto env = info.Env();

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleStringResultCallback));
        const auto result = ve_get_game_version_async(this->_pInstance, cbData, HandleStringResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }

    Napi::Value LauncherManager::TestModule(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();
        const auto files = JSONStringify(info[0].As<Object>());

        const auto filesCopy = CopyWithFree(files.Utf16Value());

        const auto result = ve_test_module(this->_pInstance, filesCopy.get());
        // LOGOUTPUT();
        return ThrowOrReturnJson(env, result);
    }

    Napi::Value LauncherManager::InstallModule(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();
        const auto files = JSONStringify(info[0].As<Object>());
        const auto destinationPath = JSONStringify(info[1].As<Object>());

        const auto filesCopy = CopyWithFree(files.Utf16Value());
        const auto destinationPathCopy = CopyWithFree(destinationPath.Utf16Value());

        const auto result = ve_install_module(this->_pInstance, filesCopy.get(), destinationPathCopy.get());
        // LOGOUTPUT();
        return ThrowOrReturnJson(env, result);
    }

    Napi::Value LauncherManager::IsObfuscatedAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();
        const auto module = JSONStringify(info[0].As<Object>());

        const auto moduleCopy = CopyWithFree(module.Utf16Value());

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleVoidResultCallback));
        const auto result = ve_is_obfuscated_async(this->_pInstance, moduleCopy.get(), cbData, HandleBooleanResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }

    Napi::Value LauncherManager::IsSorting(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();

        const auto result = ve_is_sorting(this->_pInstance);
        // LOGOUTPUT();
        return ThrowOrReturnBoolean(env, result);
    }

    Napi::Value LauncherManager::SortAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleVoidResultCallback));
        const auto result = ve_sort_async(this->_pInstance, cbData, HandleVoidResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }

    Napi::Value LauncherManager::GetModulesAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleJsonResultCallback));
        const auto result = ve_get_modules_async(this->_pInstance, cbData, HandleJsonResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }

    Napi::Value LauncherManager::GetAllModulesAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleJsonResultCallback));
        const auto result = ve_get_all_modules_async(this->_pInstance, cbData, HandleJsonResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }

    Napi::Value LauncherManager::RefreshGameParametersAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleVoidResultCallback));
        const auto result = ve_refresh_game_parameters_async(this->_pInstance, cbData, HandleVoidResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }

    Napi::Value LauncherManager::CheckForRootHarmonyAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleVoidResultCallback));
        auto result = ve_check_for_root_harmony_async(this->_pInstance, cbData, HandleVoidResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }

    Napi::Value LauncherManager::ModuleListHandlerExportAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleVoidResultCallback));
        auto result = ve_module_list_handler_export_async(this->_pInstance, cbData, HandleVoidResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }

    Napi::Value LauncherManager::ModuleListHandlerExportSaveFileAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();
        const auto saveFile = info[0].As<String>();

        const auto saveFileCopy = CopyWithFree(saveFile.Utf16Value());

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleVoidResultCallback));
        auto result = ve_module_list_handler_export_save_file_async(this->_pInstance, saveFileCopy.get(), cbData, HandleVoidResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }

    Napi::Value LauncherManager::ModuleListHandlerImportAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleBooleanResultCallback));
        auto result = ve_module_list_handler_import_async(this->_pInstance, cbData, HandleBooleanResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }

    Napi::Value LauncherManager::ModuleListHandlerImportSaveFileAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();
        const auto saveFile = info[0].As<String>();

        const auto saveFileCopy = CopyWithFree(saveFile.Utf16Value());

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleBooleanResultCallback));
        auto result = ve_module_list_handler_import_save_file_async(this->_pInstance, saveFileCopy.get(), cbData, HandleBooleanResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }

    Napi::Value LauncherManager::RefreshModulesAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleVoidResultCallback));
        auto result = ve_refresh_modules_async(this->_pInstance, cbData, HandleVoidResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }

    Napi::Value LauncherManager::SortHelperChangeModulePositionAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();
        const auto moduleViewModel = JSONStringify(info[0].As<Object>());
        const auto insertIndex = info[1].As<Number>();

        const auto moduleViewModelCopy = CopyWithFree(moduleViewModel.Utf16Value());

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleBooleanResultCallback));
        const auto result = ve_sort_helper_change_module_position_async(this->_pInstance, moduleViewModelCopy.get(), insertIndex.Int32Value(), cbData, HandleBooleanResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }

    Napi::Value LauncherManager::SortHelperToggleModuleSelectionAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();
        const auto moduleViewModel = JSONStringify(info[0].As<Object>());

        const auto moduleViewModelCopy = CopyWithFree(moduleViewModel.Utf16Value());

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleJsonResultCallback));
        const auto result = ve_sort_helper_toggle_module_selection_async(this->_pInstance, moduleViewModelCopy.get(), cbData, HandleJsonResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }

    Napi::Value LauncherManager::SortHelperValidateModuleAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();
        const auto moduleViewModel = JSONStringify(info[0].As<Object>());

        const auto moduleViewModelCopy = CopyWithFree(moduleViewModel.Utf16Value());

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleJsonResultCallback));
        const auto result = ve_sort_helper_validate_module_async(this->_pInstance, moduleViewModelCopy.get(), cbData, HandleJsonResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }

    Napi::Value LauncherManager::GetSaveFilesAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleJsonResultCallback));
        const auto result = ve_get_save_files_async(this->_pInstance, cbData, HandleJsonResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }

    Napi::Value LauncherManager::GetSaveMetadataAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();
        const auto saveFile = info[0].As<String>();
        auto data = info[1].As<Uint8Array>();

        const auto saveFileCopy = CopyWithFree(saveFile.Utf16Value());
        const auto dataCopy = CopyWithFree(data.Data(), data.ByteLength());

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleJsonResultCallback));
        const auto result = ve_get_save_metadata_async(this->_pInstance, saveFileCopy.get(), dataCopy.get(), static_cast<int32_t>(data.ByteLength()), cbData, HandleJsonResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }

    Napi::Value LauncherManager::GetSaveFilePathAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();
        const auto saveFile = info[0].As<String>();

        const auto saveFileCopy = CopyWithFree(saveFile.Utf16Value());

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleStringResultCallback));
        const auto result = ve_get_save_file_path_async(this->_pInstance, saveFileCopy.get(), cbData, HandleStringResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }

    Napi::Value LauncherManager::OrderByLoadOrderAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();
        const auto loadOrder = JSONStringify(info[0].As<Object>());

        const auto loadOrderCopy = CopyWithFree(loadOrder.Utf16Value());

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleJsonResultCallback));
        const auto result = ve_order_by_load_order_async(this->_pInstance, loadOrderCopy.get(), cbData, HandleJsonResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }

    Napi::Value LauncherManager::SetGameParameterExecutableAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();
        const auto executable = info[0].As<String>();

        const auto executableCopy = CopyWithFree(executable.Utf16Value());

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleVoidResultCallback));
        const auto result = ve_set_game_parameter_executable_async(this->_pInstance, executableCopy.get(), cbData, HandleVoidResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }

    Napi::Value LauncherManager::SetGameParameterSaveFileAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();
        const auto saveName = info[0].As<String>();

        const auto saveNameCopy = CopyWithFree(saveName.Utf16Value());

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleVoidResultCallback));
        const auto result = ve_set_game_parameter_save_file_async(this->_pInstance, saveNameCopy.get(), cbData, HandleVoidResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }

    Napi::Value LauncherManager::DialogTestWarningAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleStringResultCallback));
        const auto result = ve_dialog_test_warning_async(this->_pInstance, cbData, HandleStringResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }
    Napi::Value LauncherManager::DialogTestFileOpenAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleStringResultCallback));
        const auto result = ve_dialog_test_file_open_async(this->_pInstance, cbData, HandleStringResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }

    void LauncherManager::SetGameStore(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();
        const auto gameStore = info[0].As<String>();

        const auto gameStoreCopy = CopyWithFree(gameStore.Utf16Value());

        const auto result = ve_set_game_store(this->_pInstance, gameStoreCopy.get());
        ThrowOrReturn(env, result);
        // LOGOUTPUT();
    }

    Napi::Value LauncherManager::GetGamePlatformAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleStringResultCallback));
        const auto result = ve_get_game_platform_async(this->_pInstance, cbData, HandleStringResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }

    Napi::Value LauncherManager::SetGameParameterContinueLastSaveFileAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();
        const auto value = info[0].As<Boolean>();

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleVoidResultCallback));
        const auto result = ve_set_game_parameter_continue_last_save_file_async(this->_pInstance, value == true ? 1 : 0, cbData, HandleVoidResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }

    Napi::Value LauncherManager::SetGameParameterLoadOrderAsync(const CallbackInfo &info)
    {
        // LOGINPUT();
        const auto env = info.Env();
        const auto loadOrder = JSONStringify(info[0].As<Object>());

        const auto loadOrderCopy = CopyWithFree(loadOrder.Utf16Value());

        auto cbData = CreateResultCallbackData(env, NAMEOF(HandleVoidResultCallback));
        const auto result = ve_set_game_parameter_load_order_async(this->_pInstance, loadOrderCopy.get(), cbData, HandleVoidResultCallback);
        // LOGOUTPUT();
        return ReturnAndHandleReject(env, result, cbData);
    }
}
#endif
