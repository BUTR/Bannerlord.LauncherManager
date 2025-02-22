#ifndef VE_LAUNCHERMANAGER_GUARD_HPP_
#define VE_LAUNCHERMANAGER_GUARD_HPP_

#define NAMEOF(x) #x

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
                                          InstanceMethod<&LauncherManager::SortAsync>("sort", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
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

    static Napi::Value CallbackString(const Napi::CallbackInfo &info)
    {
        const auto data = static_cast<param_callback *>(info.Data());
        const auto env = info.Env();

        try
        {
            const auto str = info[0].As<String>();

            const auto strCopy = CopyWithFree(str.Utf16Value());

            std::stringstream ss;
            ss << data;
            ss = std::stringstream();
            ss << data->p_callback_ptr;
            ss = std::stringstream();
            ss << data->p_callback;

            data->p_callback(data->p_callback_ptr, strCopy.get());
            delete data;
            return info.Env().Null();
        }
        catch (const Napi::Error &e)
        {
            return info.Env().Null();
        }
    }

    static return_value_void *setGameParameters(void *p_owner, char16_t *p_executable, char16_t *p_game_parameters, void *p_callback_handler, void(__cdecl *p_callback)(void *)) noexcept
    {
        return ExecuteAsync(p_owner, [p_executable, p_game_parameters](const LauncherManager *manager)
                            {
        const auto env = manager->Env();
        const auto executable = p_executable == nullptr ? String::New(env, "") : String::New(env, p_executable);
        const auto gameParameters = JSONParse(env, p_game_parameters == nullptr ? String::New(env, "") : String::New(env, p_game_parameters));
        return manager->FSetGameParameters({executable, gameParameters}); }, [p_callback_handler, p_callback](const Napi::Value &result)
                            { p_callback(p_callback_handler); });
    }
    static return_value_void *sendNotification(void *p_owner, char16_t *p_id, char16_t *p_type, char16_t *p_message, uint32_t displayMs, void *p_callback_handler, void(__cdecl *p_callback)(void *)) noexcept
    {
        return ExecuteAsync(p_owner, [p_id, p_type, p_message, displayMs](const LauncherManager *manager)
                            {
        const auto env = manager->Env();
        const auto id = p_id == nullptr ? String::New(env, "") : String::New(env, p_id);
        const auto type = p_type == nullptr ? String::New(env, "") : String::New(env, p_type);
        const auto message = p_message == nullptr ? String::New(env, "") : String::New(env, p_message);
        const auto displayMs_ = Number::New(env, displayMs);
        return manager->FSendNotification({id, type, message, displayMs_}); }, [p_callback_handler, p_callback](const Napi::Value &result)
                            { p_callback(p_callback_handler); });
    }
    static return_value_void *sendDialog(void *p_owner, char16_t *p_type, char16_t *p_title, char16_t *p_message, char16_t *p_filters, void *p_callback_handler, void(__cdecl *p_callback)(void *, char16_t *)) noexcept
    {
        return ExecuteAsync(p_owner, [p_type, p_title, p_message, p_filters](const LauncherManager *manager)
                            { 
            const auto env = manager->Env();
            const auto type = p_type == nullptr ? String::New(env, "") : String::New(env, p_type);
            const auto title = p_title == nullptr ? String::New(env, "") : String::New(env, p_title);
            const auto message = p_message == nullptr ? String::New(env, "") : String::New(env, p_message);
            const auto filters = JSONParse(env, p_filters == nullptr ? String::New(env, "") : String::New(env, p_filters));
            return manager->FSendDialog({type, title, message, filters}); }, [p_callback_handler, p_callback](const Napi::Value &result)
                            {
                if (result.IsNull()) {
                    p_callback(p_callback_handler, nullptr);
                } else {
                    const auto env = result.Env();
                    const auto path = CopyWithFree(result.As<String>().Utf16Value());
                    p_callback(p_callback_handler, path.get());
                } });
    }
    static return_value_void *getInstallPath(void *p_owner, void *p_callback_handler, void(__cdecl *p_callback)(void *, char16_t *)) noexcept
    {
        return ExecuteAsync(p_owner, [](const LauncherManager *manager)
                            { 
            const auto env = manager->Env();
            return manager->FGetInstallPath({}); }, [p_callback_handler, p_callback](const Napi::Value &result)
                            {
                if (result.IsNull()) {
                    p_callback(p_callback_handler, nullptr);
                } else {
                    const auto env = result.Env();
                    const auto installPath = CopyWithFree(result.As<String>().Utf16Value());
                    p_callback(p_callback_handler, installPath.get());
                } });
    }
    static return_value_void *readFileContent(void *p_owner, char16_t *p_file_path, int32_t p_offset, int32_t p_length, void *p_callback_handler, void(__cdecl *p_callback)(void *, uint8_t *, int32_t)) noexcept
    {
        return ExecuteAsync(p_owner, [p_file_path, p_offset, p_length](const LauncherManager *manager)
                            { 
            const auto env = manager->Env();
            const auto filePath = p_file_path == nullptr ? String::New(env, "") : String::New(env, p_file_path);
            const auto offset = Number::New(env, p_offset);
            const auto length = Number::New(env, p_length);
            return manager->FReadDirectoryFileList({filePath, offset, length}); }, [p_callback_handler, p_callback](const Napi::Value &result)
                            {
                if (result.IsNull()) {
                    p_callback(p_callback_handler, nullptr, 0);
                }

                /*
                if (!result.IsBuffer())
                {
                    std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
                    return Create(return_value_void{Copy(conv.from_bytes("Not an Buffer<uint8_t>"))});
                }
                */

                auto buffer = result.As<Buffer<uint8_t>>();
                p_callback(p_callback_handler, buffer.Data(), static_cast<int>(buffer.ByteLength())); });
    }
    static return_value_void *writeFileContent(void *p_owner, char16_t *p_file_path, uint8_t *p_data, int32_t length, void *p_callback_handler, void(__cdecl *p_callback)(void *)) noexcept
    {
        return ExecuteAsync(p_owner, [p_file_path, p_data, length](const LauncherManager *manager)
                            {
        const auto env = manager->Env();
        const auto filePath = p_file_path == nullptr ? String::New(env, "") : String::New(env, p_file_path);
        const auto data = Buffer<uint8_t>::New(env, p_data, static_cast<size_t>(length));
        return manager->FWriteFileContent({filePath, data}); }, [p_callback_handler, p_callback](const Napi::Value &result)
                            { p_callback(p_callback_handler); });
    }
    static return_value_void *readDirectoryFileList(void *p_owner, char16_t *p_directory_path, void *p_callback_handler, void(__cdecl *p_callback)(void *, char16_t *)) noexcept
    {
        return ExecuteAsync(p_owner, [p_directory_path](const LauncherManager *manager)
                            { 
            const auto env = manager->Env();
            const auto directoryPath = p_directory_path == nullptr ? String::New(env, "") : String::New(env, p_directory_path);
            return manager->FReadDirectoryFileList({directoryPath}); }, [p_callback_handler, p_callback](const Napi::Value &result)
                            {
                if (result.IsNull()) {
                    p_callback(p_callback_handler, nullptr);
                } else {
                    const auto env = result.Env();
                    const auto fileList = CopyWithFree(JSONStringify(env, result.As<Object>()).Utf16Value());
                    p_callback(p_callback_handler, fileList.get());
                } });
    }
    static return_value_void *readDirectoryList(void *p_owner, char16_t *p_directory_path, void *p_callback_handler, void(__cdecl *p_callback)(void *, char16_t *)) noexcept
    {
        return ExecuteAsync(p_owner, [p_directory_path](const LauncherManager *manager)
                            { 
            const auto env = manager->Env();
            const auto directoryPath = p_directory_path == nullptr ? String::New(env, "") : String::New(env, p_directory_path);
            return manager->FReadDirectoryList({directoryPath}); }, [p_callback_handler, p_callback](const Napi::Value &result)
                            {
                if (result.IsNull()) {
                    p_callback(p_callback_handler, nullptr);
                } else {
                    const auto env = result.Env();
                    const auto directoryList = CopyWithFree(JSONStringify(env, result.As<Object>()).Utf16Value());
                    p_callback(p_callback_handler, directoryList.get());
                } });
    }
    static return_value_void *getAllModuleViewModels(void *p_owner, void *p_callback_handler, void(__cdecl *p_callback)(void *, char16_t *)) noexcept
    {
        return ExecuteAsync(p_owner, [](const LauncherManager *manager)
                            { return manager->FGetAllModuleViewModels({}); }, [p_callback_handler, p_callback](const Napi::Value &result)
                            {
                if (result.IsNull()) {
                    p_callback(p_callback_handler, nullptr);
                } else {
                    const auto env = result.Env();
                    const auto allModuleViewModels = CopyWithFree(JSONStringify(env, result.As<Napi::Object>()).Utf16Value());
                    p_callback(p_callback_handler, allModuleViewModels.get());
                } });
    }
    static return_value_void *getModuleViewModels(void *p_owner, void *p_callback_handler, void(__cdecl *p_callback)(void *, char16_t *)) noexcept
    {
        return ExecuteAsync(p_owner, [](const LauncherManager *manager)
                            { return manager->FGetModuleViewModels({}); }, [p_callback_handler, p_callback](const Napi::Value &result)
                            {
                if (result.IsNull()) {
                    p_callback(p_callback_handler, nullptr);
                } else {
                    const auto env = result.Env();
                    const auto moduleViewModels = CopyWithFree(JSONStringify(env, result.As<Napi::Object>()).Utf16Value());
                    p_callback(p_callback_handler, moduleViewModels.get());
                } });
    }
    static return_value_void *setModuleViewModels(void *p_owner, char16_t *p_module_view_models, void *p_callback_handler, void(__cdecl *p_callback)(void *)) noexcept
    {
        return ExecuteAsync(p_owner, [p_module_view_models](const LauncherManager *manager)
                            {
            const auto env = manager->Env();
            const auto moduleViewModels = JSONParse(env, p_module_view_models ? 
                Napi::String::New(env, p_module_view_models) : 
                Napi::String::New(env, ""));
            return manager->FSetModuleViewModels({moduleViewModels}); }, [p_callback_handler, p_callback](const Napi::Value & /*result*/)
                            { p_callback(p_callback_handler); });
    }
    static return_value_void *getOptions(void *p_owner, void *p_callback_handler, void(__cdecl *p_callback)(void *, char16_t *)) noexcept
    {
        return ExecuteAsync(p_owner, [](const LauncherManager *manager)
                            { return manager->FGetOptions({}); }, [p_callback_handler, p_callback](const Napi::Value &result)
                            {
                if (result.IsNull()) {
                    p_callback(p_callback_handler, nullptr);
                } else {
                    const auto env = result.Env();
                    const auto options = CopyWithFree(JSONStringify(env, result.As<Napi::Object>()).Utf16Value());
                    p_callback(p_callback_handler, options.get());
                } });
    }
    static return_value_void *getState(void *p_owner, void *p_callback_handler, void(__cdecl *p_callback)(void *, char16_t *)) noexcept
    {
        return ExecuteAsync(p_owner, [](const LauncherManager *manager)
                            { return manager->FGetState({}); }, [p_callback_handler, p_callback](const Napi::Value &result)
                            {
                if (result.IsNull()) {
                    p_callback(p_callback_handler, nullptr);
                } else {
                    const auto env = result.Env();
                    const auto state = CopyWithFree(JSONStringify(env, result.As<Napi::Object>()).Utf16Value());
                    p_callback(p_callback_handler, state.get());
                } });
    }

    LauncherManager::LauncherManager(const CallbackInfo &info) : ObjectWrap<LauncherManager>(info)
    {
        const auto env = info.Env();
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
        const auto env = info.Env();

        const auto deferred = Napi::Promise::Deferred::New(env);
        auto *cbData = CreateCallbackData(env, [](const Napi::CallbackInfo &) {}, NAMEOF(StringCallback));
        const auto result = ve_get_game_version(this->_pInstance, cbData, StringCallback);
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }

    Napi::Value LauncherManager::TestModule(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto files = JSONStringify(env, info[0].As<Object>());

        const auto filesCopy = CopyWithFree(files.Utf16Value());

        const auto result = ve_test_module(this->_pInstance, filesCopy.get());
        return ThrowOrReturnJson(env, result);
    }

    Napi::Value LauncherManager::InstallModule(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto files = JSONStringify(env, info[0].As<Object>());
        const auto destinationPath = JSONStringify(env, info[1].As<Object>());

        const auto filesCopy = CopyWithFree(files.Utf16Value());
        const auto destinationPathCopy = CopyWithFree(destinationPath.Utf16Value());

        const auto result = ve_install_module(this->_pInstance, filesCopy.get(), destinationPathCopy.get());
        return ThrowOrReturnJson(env, result);
    }

    Napi::Value LauncherManager::IsSorting(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto result = ve_is_sorting(this->_pInstance);
        return ThrowOrReturnBoolean(env, result);
    }

    Napi::Value LauncherManager::SortAsync(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto deferred = Napi::Promise::Deferred::New(env);
        auto *cbData = CreateCallbackData(env, [](const Napi::CallbackInfo &) {}, NAMEOF(VoidCallback));
        const auto result = ve_sort(this->_pInstance, cbData, VoidCallback);
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }

    Napi::Value LauncherManager::GetModulesAsync(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto deferred = Napi::Promise::Deferred::New(env);
        auto *cbData = CreateCallbackData(env, [](const Napi::CallbackInfo &) {}, NAMEOF(JsonCallback));
        const auto result = ve_get_modules(this->_pInstance, cbData, JsonCallback);
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }

    Napi::Value LauncherManager::GetAllModulesAsync(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto deferred = Napi::Promise::Deferred::New(env);
        auto *cbData = CreateCallbackData(env, [](const Napi::CallbackInfo &) {}, NAMEOF(JsonCallback));
        const auto result = ve_get_all_modules(this->_pInstance, cbData, JsonCallback);
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }

    Napi::Value LauncherManager::RefreshGameParametersAsync(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto deferred = Napi::Promise::Deferred::New(env);
        auto *cbData = CreateCallbackData(env, [](const Napi::CallbackInfo &) {}, NAMEOF(VoidCallback));
        const auto result = ve_refresh_game_parameters(this->_pInstance, cbData, VoidCallback);
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }

    Napi::Value LauncherManager::CheckForRootHarmonyAsync(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto deferred = Napi::Promise::Deferred::New(env);
        auto *cbData = CreateCallbackData(env, [](const Napi::CallbackInfo &) {}, NAMEOF(VoidCallback));
        const auto result = ve_check_for_root_harmony(this->_pInstance, cbData, VoidCallback);
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }

    Napi::Value LauncherManager::ModuleListHandlerExportAsync(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto deferred = Napi::Promise::Deferred::New(env);
        auto *cbData = CreateCallbackData(env, [](const Napi::CallbackInfo &) {}, NAMEOF(VoidCallback));
        const auto result = ve_module_list_handler_export(this->_pInstance, cbData, VoidCallback);
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }

    Napi::Value LauncherManager::ModuleListHandlerExportSaveFileAsync(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto saveFile = info[0].As<String>();

        const auto saveFileCopy = CopyWithFree(saveFile.Utf16Value());

        const auto deferred = Napi::Promise::Deferred::New(env);
        auto *cbData = CreateCallbackData(env, [](const Napi::CallbackInfo &) {}, NAMEOF(VoidCallback));
        const auto result = ve_module_list_handler_export_save_file(this->_pInstance, saveFileCopy.get(), cbData, VoidCallback);
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }

    Napi::Value LauncherManager::ModuleListHandlerImportAsync(const CallbackInfo &info)
    {
        const auto env = info.Env();

        auto deferred = Promise::Deferred::New(env);
        auto callbackStorage = CallbackStorage<param_bool>{env, deferred};
        const auto result = ve_module_list_handler_import(this->_pInstance, static_cast<void *>(&callbackStorage), static_cast<void(__cdecl *)(param_ptr *, param_bool)>(&callbackStorage.Callback));
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }

    Napi::Value LauncherManager::ModuleListHandlerImportSaveFileAsync(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto saveFile = info[0].As<String>();

        const auto saveFileCopy = CopyWithFree(saveFile.Utf16Value());

        auto deferred = Promise::Deferred::New(env);
        auto callbackStorage = CallbackStorage<param_bool>{env, deferred};
        const auto result = ve_module_list_handler_import_save_file(this->_pInstance, saveFileCopy.get(), static_cast<void *>(&callbackStorage), static_cast<void(__cdecl *)(param_ptr *, param_bool)>(&callbackStorage.Callback));
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }

    Napi::Value LauncherManager::RefreshModulesAsync(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto deferred = Napi::Promise::Deferred::New(env);
        auto *cbData = CreateCallbackData(env, [](const Napi::CallbackInfo &) {}, NAMEOF(VoidCallback));
        const auto result = ve_refresh_modules(this->_pInstance, cbData, VoidCallback);
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }

    Napi::Value LauncherManager::SortHelperChangeModulePositionAsync(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto moduleViewModel = JSONStringify(env, info[0].As<Object>());
        const auto insertIndex = info[1].As<Number>();

        const auto moduleViewModelCopy = CopyWithFree(moduleViewModel.Utf16Value());

        const auto deferred = Napi::Promise::Deferred::New(env);
        auto *cbData = CreateCallbackData(env, [](const Napi::CallbackInfo &) {}, NAMEOF(BooleanCallback));
        const auto result = ve_sort_helper_change_module_position(this->_pInstance, moduleViewModelCopy.get(), insertIndex.Int32Value(), cbData, BooleanCallback);
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }

    Napi::Value LauncherManager::SortHelperToggleModuleSelectionAsync(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto moduleViewModel = JSONStringify(env, info[0].As<Object>());

        const auto moduleViewModelCopy = CopyWithFree(moduleViewModel.Utf16Value());

        const auto deferred = Napi::Promise::Deferred::New(env);
        auto *cbData = CreateCallbackData(env, [](const Napi::CallbackInfo &) {}, NAMEOF(StringCallback));
        const auto result = ve_sort_helper_toggle_module_selection(this->_pInstance, moduleViewModelCopy.get(), cbData, StringCallback);
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }

    Napi::Value LauncherManager::SortHelperValidateModuleAsync(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto moduleViewModel = JSONStringify(env, info[0].As<Object>());

        const auto moduleViewModelCopy = CopyWithFree(moduleViewModel.Utf16Value());

        const auto deferred = Napi::Promise::Deferred::New(env);
        auto *cbData = CreateCallbackData(env, [](const Napi::CallbackInfo &) {}, NAMEOF(StringCallback));
        const auto result = ve_sort_helper_validate_module(this->_pInstance, moduleViewModelCopy.get(), cbData, StringCallback);
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }

    Napi::Value LauncherManager::GetSaveFilesAsync(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto deferred = Napi::Promise::Deferred::New(env);
        auto *cbData = CreateCallbackData(env, [](const Napi::CallbackInfo &) {}, NAMEOF(StringCallback));
        const auto result = ve_get_save_files(this->_pInstance, cbData, StringCallback);
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }

    Napi::Value LauncherManager::GetSaveMetadataAsync(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto saveFile = info[0].As<String>();
        auto data = info[1].As<Buffer<uint8_t>>();

        const auto saveFileCopy = CopyWithFree(saveFile.Utf16Value());
        const auto dataCopy = CopyWithFree(data.Data(), data.ByteLength());

        const auto deferred = Napi::Promise::Deferred::New(env);
        auto *cbData = CreateCallbackData(env, [](const Napi::CallbackInfo &) {}, NAMEOF(JsonCallback));
        const auto result = ve_get_save_metadata(this->_pInstance, saveFileCopy.get(), dataCopy.get(), static_cast<int32_t>(data.ByteLength()), cbData, JsonCallback);
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }

    Napi::Value LauncherManager::GetSaveFilePathAsync(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto saveFile = info[0].As<String>();

        const auto saveFileCopy = CopyWithFree(saveFile.Utf16Value());

        const auto deferred = Napi::Promise::Deferred::New(env);
        auto *cbData = CreateCallbackData(env, [](const Napi::CallbackInfo &) {}, NAMEOF(StringCallback));
        const auto result = ve_get_save_file_path(this->_pInstance, saveFileCopy.get(), cbData, StringCallback);
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }

    Napi::Value LauncherManager::OrderByLoadOrderAsync(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto loadOrder = JSONStringify(env, info[0].As<Object>());

        const auto loadOrderCopy = CopyWithFree(loadOrder.Utf16Value());

        const auto deferred = Napi::Promise::Deferred::New(env);
        auto *cbData = CreateCallbackData(env, [](const Napi::CallbackInfo &) {}, NAMEOF(StringCallback));
        const auto result = ve_order_by_load_order(this->_pInstance, loadOrderCopy.get(), cbData, StringCallback);
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }

    Napi::Value LauncherManager::SetGameParameterExecutableAsync(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto executable = info[0].As<String>();

        const auto executableCopy = CopyWithFree(executable.Utf16Value());

        const auto deferred = Napi::Promise::Deferred::New(env);
        auto *cbData = CreateCallbackData(env, [](const Napi::CallbackInfo &) {}, NAMEOF(VoidCallback));
        const auto result = ve_set_game_parameter_executable(this->_pInstance, executableCopy.get(), cbData, VoidCallback);
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }

    Napi::Value LauncherManager::SetGameParameterSaveFileAsync(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto saveName = info[0].As<String>();

        const auto saveNameCopy = CopyWithFree(saveName.Utf16Value());

        const auto deferred = Napi::Promise::Deferred::New(env);
        auto *cbData = CreateCallbackData(env, [](const Napi::CallbackInfo &) {}, NAMEOF(VoidCallback));
        const auto result = ve_set_game_parameter_save_file(this->_pInstance, saveNameCopy.get(), cbData, VoidCallback);
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }

    Napi::Value LauncherManager::DialogTestWarningAsync(const CallbackInfo &info)
    {
        const auto env = info.Env();

        auto deferred = Promise::Deferred::New(env);
        auto callbackStorage = new CallbackStorage<param_string *>{env, deferred};
        const auto result = ve_dialog_test_warning(this->_pInstance, static_cast<void *>(callbackStorage), static_cast<void(__cdecl *)(param_ptr *, param_string *)>(callbackStorage->Callback));
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }
    Napi::Value LauncherManager::DialogTestFileOpenAsync(const CallbackInfo &info)
    {
        const auto env = info.Env();

        auto deferred = Promise::Deferred::New(env);
        auto callbackStorage = new CallbackStorage<param_string *>{env, deferred};
        const auto result = ve_dialog_test_file_open(this->_pInstance, static_cast<void *>(callbackStorage), static_cast<void(__cdecl *)(param_ptr *, param_string *)>(callbackStorage->Callback));
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }

    void LauncherManager::SetGameStore(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto gameStore = info[0].As<String>();

        const auto gameStoreCopy = CopyWithFree(gameStore.Utf16Value());

        const auto result = ve_set_game_store(this->_pInstance, gameStoreCopy.get());
        ThrowOrReturn(env, result);
    }

    Napi::Value LauncherManager::GetGamePlatformAsync(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto deferred = Napi::Promise::Deferred::New(env);
        auto *cbData = CreateCallbackData(env, [](const Napi::CallbackInfo &) {}, NAMEOF(StringCallback));
        const auto result = ve_get_game_platform(this->_pInstance, cbData, StringCallback);
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }

    Napi::Value LauncherManager::SetGameParameterContinueLastSaveFileAsync(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto value = info[0].As<Boolean>();

        const auto deferred = Napi::Promise::Deferred::New(env);
        auto *cbData = CreateCallbackData(env, [](const Napi::CallbackInfo &) {}, NAMEOF(VoidCallback));
        const auto result = ve_set_game_parameter_continue_last_save_file(this->_pInstance, value == true ? 1 : 0, cbData, VoidCallback);
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }

    Napi::Value LauncherManager::SetGameParameterLoadOrderAsync(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto loadOrder = JSONStringify(env, info[0].As<Object>());

        const auto loadOrderCopy = CopyWithFree(loadOrder.Utf16Value());

        const auto deferred = Napi::Promise::Deferred::New(env);
        auto *cbData = CreateCallbackData(env, [](const Napi::CallbackInfo &) {}, NAMEOF(VoidCallback));
        const auto result = ve_set_game_parameter_load_order(this->_pInstance, loadOrderCopy.get(), cbData, VoidCallback);
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }
}
#endif