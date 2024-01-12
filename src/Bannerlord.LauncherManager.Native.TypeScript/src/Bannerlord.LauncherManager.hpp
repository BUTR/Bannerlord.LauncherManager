#ifndef VE_LAUNCHERMANAGER_GUARD_HPP_
#define VE_LAUNCHERMANAGER_GUARD_HPP_

#include "utils.hpp"
#include "Bannerlord.LauncherManager.Native.h"
#include <codecvt>
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
        FunctionReference FGetLoadOrder;
        FunctionReference FSetLoadOrder;
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

        void CheckForRootHarmony(const CallbackInfo &info);
        Napi::Value GetGamePlatform(const CallbackInfo &info);
        Napi::Value GetGameVersion(const CallbackInfo &info);
        Napi::Value GetModules(const CallbackInfo &info);
        Napi::Value GetSaveFilePath(const CallbackInfo &info);
        Napi::Value GetSaveFiles(const CallbackInfo &info);
        Napi::Value GetSaveMetadata(const CallbackInfo &info);
        Napi::Value InstallModule(const CallbackInfo &info);
        Napi::Value IsSorting(const CallbackInfo &info);
        void LoadLocalization(const CallbackInfo &info);
        Napi::Value LocalizeString(const CallbackInfo &info);
        void ModuleListHandlerExport(const CallbackInfo &info);
        void ModuleListHandlerExportSaveFile(const CallbackInfo &info);
        Napi::Value ModuleListHandlerImport(const CallbackInfo &info);
        Napi::Value ModuleListHandlerImportSaveFile(const CallbackInfo &info);
        Napi::Value OrderByLoadOrder(const CallbackInfo &info);
        void RefreshGameParameters(const CallbackInfo &info);
        void RefreshModules(const CallbackInfo &info);
        void SetGameParameterExecutable(const CallbackInfo &info);
        void SetGameParameterSaveFile(const CallbackInfo &info);
        void SetGameParameterContinueLastSaveFile(const CallbackInfo &info);
        void SetGameStore(const CallbackInfo &info);
        void Sort(const CallbackInfo &info);
        Napi::Value SortHelperChangeModulePosition(const CallbackInfo &info);
        Napi::Value SortHelperToggleModuleSelection(const CallbackInfo &info);
        Napi::Value SortHelperValidateModule(const CallbackInfo &info);
        Napi::Value TestModule(const CallbackInfo &info);
        Napi::Value DialogTestFileOpen(const CallbackInfo &info);
        Napi::Value DialogTestWarning(const CallbackInfo &info);
        void SaveLoadOrder(const CallbackInfo &info);
        Napi::Value LoadLoadOrder(const CallbackInfo &info);

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
                                          InstanceMethod<&LauncherManager::CheckForRootHarmony>("checkForRootHarmony", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::GetGameVersion>("getGameVersion", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::GetModules>("getModules", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::GetSaveFilePath>("getSaveFilePath", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::GetSaveFiles>("getSaveFiles", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::GetSaveMetadata>("getSaveMetadata", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::InstallModule>("installModule", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::IsSorting>("isSorting", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::LoadLocalization>("loadLocalization", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::LocalizeString>("localizeString", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::ModuleListHandlerExport>("moduleListHandlerExport", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::ModuleListHandlerExportSaveFile>("moduleListHandlerExportSaveFile", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::ModuleListHandlerImport>("moduleListHandlerImport", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::ModuleListHandlerImportSaveFile>("moduleListHandlerImportSaveFile", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::OrderByLoadOrder>("orderByLoadOrder", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::RefreshGameParameters>("refreshGameParameters", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::RefreshModules>("refreshModules", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::SetGameParameterExecutable>("setGameParameterExecutable", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::SetGameParameterSaveFile>("setGameParameterSaveFile", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::Sort>("sort", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::SortHelperChangeModulePosition>("sortHelperChangeModulePosition", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::SortHelperToggleModuleSelection>("sortHelperToggleModuleSelection", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::SortHelperValidateModule>("sortHelperValidateModule", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::TestModule>("testModule", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::DialogTestFileOpen>("dialogTestFileOpen", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::DialogTestWarning>("dialogTestWarning", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::SetGameStore>("setGameStore", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::GetGamePlatform>("getGamePlatform", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::SetGameParameterContinueLastSaveFile>("setGameParameterContinueLastSaveFile", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::SaveLoadOrder>("saveLoadOrder", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::LoadLoadOrder>("loadLoadOrder", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
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

            ConsoleLog(env, String::New(env, "14"));
            std::stringstream ss;
            ss << data;
            ConsoleLog(env, String::New(env, ss.str()));
            ss = std::stringstream();
            ss << data->p_callback_ptr;
            ConsoleLog(env, String::New(env, ss.str()));
            ss = std::stringstream();
            ss << data->p_callback;
            ConsoleLog(env, String::New(env, ss.str()));

            data->p_callback(data->p_callback_ptr, strCopy.get());
            ConsoleLog(env, String::New(env, "15"));
            delete data;
            return info.Env().Null();
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            ConsoleLog(env, String::New(env, conv.from_bytes(e.what())));
            return info.Env().Null();
        }
    }

    static return_value_void *setGameParameters(void *p_owner, char16_t *p_executable, char16_t *p_game_parameters) noexcept
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();

            const auto executable = p_executable == nullptr ? String::New(env, "") : String::New(env, p_executable);
            const auto gameParameters = JSONParse(env, p_game_parameters == nullptr ? String::New(env, "") : String::New(env, p_game_parameters));

            manager->FSetGameParameters({executable, gameParameters});
            return Create(return_value_void{nullptr});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_void{Copy(conv.from_bytes(e.what()))});
        }
    }
    static return_value_json *getLoadOrder(void *p_owner) noexcept
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();

            const auto result = manager->FGetLoadOrder({}).As<Object>();
            return Create(return_value_json{nullptr, Copy(JSONStringify(env, result).Utf16Value())});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_json{Copy(conv.from_bytes(e.what())), nullptr});
        }
    }
    static return_value_void *setLoadOrder(void *p_owner, char16_t *p_load_order) noexcept
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();

            const auto loadOrder = JSONParse(env, p_load_order == nullptr ? String::New(env, "") : String::New(env, p_load_order));
            manager->FSetLoadOrder({loadOrder});
            return Create(return_value_void{nullptr});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_void{Copy(conv.from_bytes(e.what()))});
        }
    }
    static return_value_void *sendNotification(void *p_owner, char16_t *p_id, char16_t *p_type, char16_t *p_message, uint32_t displayMs) noexcept
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();

            const auto id = p_id == nullptr ? String::New(env, "") : String::New(env, p_id);
            const auto type = p_type == nullptr ? String::New(env, "") : String::New(env, p_type);
            const auto message = p_message == nullptr ? String::New(env, "") : String::New(env, p_message);
            const auto displayMs_ = Number::New(env, displayMs);
            manager->FSendNotification({id, type, message, displayMs_});
            return Create(return_value_void{nullptr});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_void{Copy(conv.from_bytes(e.what()))});
        }
    }
    static return_value_void *sendDialog(void *p_owner, char16_t *p_type, char16_t *p_title, char16_t *p_message, char16_t *p_filters, void *p_callback_ptr, void(__cdecl *p_callback)(void *, char16_t *)) noexcept
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();

            const auto type = p_type == nullptr ? String::New(env, "") : String::New(env, p_type);
            const auto title = p_title == nullptr ? String::New(env, "") : String::New(env, p_title);
            const auto message = p_message == nullptr ? String::New(env, "") : String::New(env, p_message);
            const auto filters = JSONParse(env, p_filters == nullptr ? String::New(env, "") : String::New(env, p_filters));

            const auto result = manager->FSendDialog({type, title, message, filters});
            const auto promise = result.As<Promise>();
            const auto then = promise.Get("then").As<Function>();
            auto data = new param_callback{p_callback_ptr, reinterpret_cast<void(__cdecl *)(void *, void *)>(p_callback)};
            /*
            std::stringstream ss;
            ss << &data;
            ConsoleLog(env, String::New(env, ss.str()));
            ss = std::stringstream();
            ss << data->p_callback_ptr;
            ConsoleLog(env, String::New(env, ss.str()));
            ss = std::stringstream();
            ss << data->p_callback;
            ConsoleLog(env, String::New(env, ss.str()));
            */
            const auto callback = Function::New(env, CallbackString, "cpp_callback", static_cast<void *>(data));
            then.Call(promise, {callback});

            return Create(return_value_void{nullptr});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_void{Copy(conv.from_bytes(e.what()))});
        }
    }
    static return_value_string *getInstallPath(void *p_owner) noexcept
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();

            const auto result = manager->FGetInstallPath({}).As<String>();
            return Create(return_value_string{nullptr, Copy(result.Utf16Value())});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_string{Copy(conv.from_bytes(e.what())), nullptr});
        }
    }
    static return_value_data *readFileContent(void *p_owner, char16_t *p_file_path, int32_t p_offset, int32_t p_length) noexcept
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();
            const auto filePath = p_file_path == nullptr ? String::New(env, "") : String::New(env, p_file_path);
            const auto offset = Number::New(env, p_offset);
            const auto length = Number::New(env, p_length);

            const auto result = manager->FReadFileContent({filePath, offset, length});

            if (result.IsNull())
            {
                return Create(return_value_data{nullptr, nullptr, 0});
            }

            if (!result.IsBuffer())
            {
                std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
                return Create(return_value_data{Copy(conv.from_bytes("Not an Buffer<uint8_t>")), nullptr, 0});
            }

            auto buffer = result.As<Buffer<uint8_t>>();
            return Create(return_value_data{nullptr, Copy(buffer.Data(), buffer.ByteLength()), static_cast<int>(buffer.ByteLength())});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_data{Copy(conv.from_bytes(e.what())), nullptr, 0});
        }
    }
    static return_value_void *writeFileContent(void *p_owner, char16_t *p_file_path, uint8_t *p_data, int32_t length) noexcept
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();
            const auto filePath = p_file_path == nullptr ? String::New(env, "") : String::New(env, p_file_path);
            const auto data = Buffer<uint8_t>::New(env, p_data, static_cast<size_t>(length));

            const auto result = manager->FWriteFileContent({filePath, data});
            if (result.IsNull())
            {
                return Create(return_value_void{nullptr});
            }

            return Create(return_value_void{nullptr});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_void{Copy(conv.from_bytes(e.what()))});
        }
    }
    static return_value_json *readDirectoryFileList(void *p_owner, char16_t *p_directory_path) noexcept
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();
            const auto directoryPath = p_directory_path == nullptr ? String::New(env, "") : String::New(env, p_directory_path);

            const auto result = manager->FReadDirectoryFileList({directoryPath});
            if (result.IsNull())
            {
                return Create(return_value_json{nullptr, nullptr});
            }

            return Create(return_value_json{nullptr, Copy(JSONStringify(env, result.As<Object>()).Utf16Value())});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_json{Copy(conv.from_bytes(e.what())), nullptr});
        }
    }
    static return_value_json *readDirectoryList(void *p_owner, char16_t *p_directory_path) noexcept
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();
            const auto directoryPath = p_directory_path == nullptr ? String::New(env, "") : String::New(env, p_directory_path);

            const auto result = manager->FReadDirectoryList({directoryPath});
            if (result.IsNull())
            {
                return Create(return_value_json{nullptr, nullptr});
            }

            return Create(return_value_json{nullptr, Copy(JSONStringify(env, result.As<Object>()).Utf16Value())});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_json{Copy(conv.from_bytes(e.what())), nullptr});
        }
    }
    static return_value_json *getAllModuleViewModels(void *p_owner) noexcept
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();

            const auto result = manager->FGetAllModuleViewModels({});
            if (result.IsNull())
            {
                return Create(return_value_json{nullptr, nullptr});
            }

            return Create(return_value_json{nullptr, Copy(JSONStringify(env, result.As<Object>()).Utf16Value())});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_json{Copy(conv.from_bytes(e.what())), nullptr});
        }
    }
    static return_value_json *getModuleViewModels(void *p_owner) noexcept
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();

            const auto result = manager->FGetModuleViewModels({});
            if (result.IsNull())
            {
                return Create(return_value_json{nullptr, nullptr});
            }

            return Create(return_value_json{nullptr, Copy(JSONStringify(env, result.As<Object>()).Utf16Value())});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_json{Copy(conv.from_bytes(e.what())), nullptr});
        }
    }
    static return_value_void *setModuleViewModels(void *p_owner, char16_t *p_module_view_models) noexcept
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();
            const auto moduleViewModels = JSONParse(env, p_module_view_models == nullptr ? String::New(env, "") : String::New(env, p_module_view_models));

            const auto result = manager->FSetModuleViewModels({moduleViewModels});
            if (result.IsNull())
            {
                return Create(return_value_void{nullptr});
            }

            return Create(return_value_void{nullptr});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_void{Copy(conv.from_bytes(e.what()))});
        }
    }
    static return_value_json *getOptions(void *p_owner) noexcept
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();

            const auto result = manager->FGetOptions({});
            if (result.IsNull())
            {
                return Create(return_value_json{nullptr, nullptr});
            }

            return Create(return_value_json{nullptr, Copy(JSONStringify(env, result.As<Object>()).Utf16Value())});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_json{Copy(conv.from_bytes(e.what())), nullptr});
        }
    }
    static return_value_json *getState(void *p_owner) noexcept
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();

            const auto result = manager->FGetState({});
            if (result.IsNull())
            {
                return Create(return_value_json{nullptr, nullptr});
            }

            return Create(return_value_json{nullptr, Copy(JSONStringify(env, result.As<Object>()).Utf16Value())});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_json{Copy(conv.from_bytes(e.what())), nullptr});
        }
    }

    LauncherManager::LauncherManager(const CallbackInfo &info) : ObjectWrap<LauncherManager>(info)
    {
        const auto env = info.Env();
        this->FSetGameParameters = Persistent(info[0].As<Function>());
        this->FGetLoadOrder = Persistent(info[1].As<Function>());
        this->FSetLoadOrder = Persistent(info[2].As<Function>());
        this->FSendNotification = Persistent(info[3].As<Function>());
        this->FSendDialog = Persistent(info[4].As<Function>());
        this->FGetInstallPath = Persistent(info[5].As<Function>());
        this->FReadFileContent = Persistent(info[6].As<Function>());
        this->FWriteFileContent = Persistent(info[7].As<Function>());
        this->FReadDirectoryFileList = Persistent(info[8].As<Function>());
        this->FReadDirectoryList = Persistent(info[9].As<Function>());
        this->FGetAllModuleViewModels = Persistent(info[10].As<Function>());
        this->FGetModuleViewModels = Persistent(info[11].As<Function>());
        this->FSetModuleViewModels = Persistent(info[12].As<Function>());
        this->FGetOptions = Persistent(info[13].As<Function>());
        this->FGetState = Persistent(info[14].As<Function>());

        const auto result = ve_create_handler(this,
                                              setGameParameters,
                                              getLoadOrder,
                                              setLoadOrder,
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
        this->FGetLoadOrder.Unref();
        this->FSetLoadOrder.Unref();
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

    Value LauncherManager::GetGameVersion(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto result = ve_get_game_version(this->_pInstance);
        return ThrowOrReturnString(env, result);
    }

    Value LauncherManager::TestModule(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto files = JSONStringify(env, info[0].As<Object>());

        const auto filesCopy = CopyWithFree(files.Utf16Value());

        const auto result = ve_test_module(this->_pInstance, filesCopy.get());
        return ThrowOrReturnJson(env, result);
    }

    Value LauncherManager::InstallModule(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto files = JSONStringify(env, info[0].As<Object>());
        const auto destinationPath = JSONStringify(env, info[1].As<Object>());

        const auto filesCopy = CopyWithFree(files.Utf16Value());
        const auto destinationPathCopy = CopyWithFree(destinationPath.Utf16Value());

        const auto result = ve_install_module(this->_pInstance, filesCopy.get(), destinationPathCopy.get());
        return ThrowOrReturnJson(env, result);
    }

    Value LauncherManager::IsSorting(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto result = ve_is_sorting(this->_pInstance);
        return ThrowOrReturnBoolean(env, result);
    }

    void LauncherManager::Sort(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto result = ve_sort(this->_pInstance);
        ThrowOrReturn(env, result);
    }

    Value LauncherManager::GetModules(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto result = ve_get_modules(this->_pInstance);
        return ThrowOrReturnJson(env, result);
    }

    void LauncherManager::RefreshGameParameters(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto result = ve_refresh_game_parameters(this->_pInstance);
        ThrowOrReturn(env, result);
    }

    void LauncherManager::CheckForRootHarmony(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto result = ve_check_for_root_harmony(this->_pInstance);
        ThrowOrReturn(env, result);
    }

    void LauncherManager::LoadLocalization(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto xml = info[0].As<String>();

        const auto xmlCopy = CopyWithFree(xml.Utf16Value());

        const auto result = ve_load_localization(this->_pInstance, xmlCopy.get());
        ThrowOrReturn(env, result);
    }

    Value LauncherManager::LocalizeString(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto templateStr = info[0].As<String>();
        const auto values = JSONStringify(env, info[1].As<Object>());

        const auto templateStrCopy = CopyWithFree(templateStr.Utf16Value());
        const auto valuesCopy = CopyWithFree(values.Utf16Value());

        const auto result = ve_localize_string(this->_pInstance, templateStrCopy.get(), valuesCopy.get());
        return ThrowOrReturnString(env, result);
    }

    void LauncherManager::ModuleListHandlerExport(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto result = ve_module_list_handler_export(this->_pInstance);
        ThrowOrReturn(env, result);
    }

    void LauncherManager::ModuleListHandlerExportSaveFile(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto saveFile = info[0].As<String>();

        const auto saveFileCopy = CopyWithFree(saveFile.Utf16Value());

        const auto result = ve_module_list_handler_export_save_file(this->_pInstance, saveFileCopy.get());
        ThrowOrReturn(env, result);
    }

    Napi::Value LauncherManager::ModuleListHandlerImport(const CallbackInfo &info)
    {
        const auto env = info.Env();

        auto deferred = Promise::Deferred::New(env);
        auto callbackStorage = CallbackStorage<param_bool>{env, deferred};
        const auto result = ve_module_list_handler_import(this->_pInstance, static_cast<void *>(&callbackStorage), static_cast<void(__cdecl *)(param_ptr *, param_bool)>(&callbackStorage.Callback));
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }

    Napi::Value LauncherManager::ModuleListHandlerImportSaveFile(const CallbackInfo &info)
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

    void LauncherManager::RefreshModules(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto result = ve_refresh_modules(this->_pInstance);
        ThrowOrReturn(env, result);
    }

    Napi::Value LauncherManager::SortHelperChangeModulePosition(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto moduleViewModel = JSONStringify(env, info[0].As<Object>());
        const auto insertIndex = info[1].As<Number>();

        const auto moduleViewModelCopy = CopyWithFree(moduleViewModel.Utf16Value());

        const auto result = ve_sort_helper_change_module_position(this->_pInstance, moduleViewModelCopy.get(), insertIndex.Int32Value());
        return ThrowOrReturnBoolean(env, result);
    }

    Napi::Value LauncherManager::SortHelperToggleModuleSelection(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto moduleViewModel = JSONStringify(env, info[0].As<Object>());

        const auto moduleViewModelCopy = CopyWithFree(moduleViewModel.Utf16Value());

        const auto result = ve_sort_helper_toggle_module_selection(this->_pInstance, moduleViewModelCopy.get());
        return ThrowOrReturnJson(env, result);
    }

    Napi::Value LauncherManager::SortHelperValidateModule(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto moduleViewModel = JSONStringify(env, info[0].As<Object>());

        const auto moduleViewModelCopy = CopyWithFree(moduleViewModel.Utf16Value());

        const auto result = ve_sort_helper_validate_module(this->_pInstance, moduleViewModelCopy.get());
        return ThrowOrReturnJson(env, result);
    }

    Napi::Value LauncherManager::GetSaveFiles(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto result = ve_get_save_files(this->_pInstance);
        return ThrowOrReturnJson(env, result);
    }

    Napi::Value LauncherManager::GetSaveMetadata(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto saveFile = info[0].As<String>();
        auto data = info[1].As<Buffer<uint8_t>>();

        const auto saveFileCopy = CopyWithFree(saveFile.Utf16Value());
        const auto dataCopy = CopyWithFree(data.Data(), data.ByteLength());

        const auto result = ve_get_save_metadata(this->_pInstance, saveFileCopy.get(), dataCopy.get(), static_cast<int32_t>(data.ByteLength()));
        return ThrowOrReturnJson(env, result);
    }

    Napi::Value LauncherManager::GetSaveFilePath(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto saveFile = info[0].As<String>();

        const auto saveFileCopy = CopyWithFree(saveFile.Utf16Value());

        const auto result = ve_get_save_file_path(this->_pInstance, saveFileCopy.get());
        return ThrowOrReturnString(env, result);
    }

    Napi::Value LauncherManager::OrderByLoadOrder(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto loadOrder = JSONStringify(env, info[0].As<Object>());

        const auto loadOrderCopy = CopyWithFree(loadOrder.Utf16Value());

        const auto result = ve_order_by_load_order(this->_pInstance, loadOrderCopy.get());
        return ThrowOrReturnJson(env, result);
    }

    void LauncherManager::SetGameParameterExecutable(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto executable = info[0].As<String>();

        const auto executableCopy = CopyWithFree(executable.Utf16Value());

        const auto result = ve_set_game_parameter_executable(this->_pInstance, executableCopy.get());
        ThrowOrReturn(env, result);
    }

    void LauncherManager::SetGameParameterSaveFile(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto saveName = info[0].As<String>();

        const auto saveNameCopy = CopyWithFree(saveName.Utf16Value());

        const auto result = ve_set_game_parameter_save_file(this->_pInstance, saveNameCopy.get());
        ThrowOrReturn(env, result);
    }

    Napi::Value LauncherManager::DialogTestWarning(const CallbackInfo &info)
    {
        const auto env = info.Env();

        auto deferred = Promise::Deferred::New(env);
        auto callbackStorage = new CallbackStorage<param_string *>{env, deferred};
        const auto result = ve_dialog_test_warning(this->_pInstance, static_cast<void *>(callbackStorage), static_cast<void(__cdecl *)(param_ptr *, param_string *)>(callbackStorage->Callback));
        ThrowOrReturn(env, result);
        return deferred.Promise();
    }
    Napi::Value LauncherManager::DialogTestFileOpen(const CallbackInfo &info)
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

    Napi::Value LauncherManager::GetGamePlatform(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto result = ve_get_game_platform(this->_pInstance);
        return ThrowOrReturnString(env, result);
    }

    void LauncherManager::SetGameParameterContinueLastSaveFile(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto value = info[0].As<Boolean>();

        const auto result = ve_set_game_parameter_continue_last_save_file(this->_pInstance, value == true ? 1 : 0);
        ThrowOrReturn(env, result);
    }

    void LauncherManager::SaveLoadOrder(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto loadOrder = JSONStringify(env, info[0].As<Object>());

        const auto loadOrderCopy = CopyWithFree(loadOrder.Utf16Value());

        const auto result = ve_save_load_order(this->_pInstance, loadOrderCopy.get());
        return ThrowOrReturn(env, result);
    }
    Napi::Value LauncherManager::LoadLoadOrder(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto result = ve_load_load_order(this->_pInstance);
        return ThrowOrReturnJson(env, result);
    }
}
#endif