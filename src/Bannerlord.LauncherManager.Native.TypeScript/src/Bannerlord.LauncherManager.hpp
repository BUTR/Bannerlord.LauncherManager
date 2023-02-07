#ifndef VE_VORTEXEXTENSIONMANAGER_GUARD_HPP_
#define VE_VORTEXEXTENSIONMANAGER_GUARD_HPP_

#include "utils.hpp"
#include "Bannerlord.LauncherManager.Native.h"
#include <codecvt>

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
        FunctionReference FTranslateString;
        FunctionReference FSendNotification;
        FunctionReference FGetInstallPath;
        FunctionReference FReadFileContent;
        FunctionReference FReadDirectoryFileList;
        FunctionReference FReadDirectoryList;

        static Object Init(const Napi::Env env, const Object exports);

        LauncherManager(const CallbackInfo &info);
        ~LauncherManager();

        void RegisterCallbacks(const CallbackInfo &info);
        Napi::Value GetGameVersion(const CallbackInfo &info);
        Napi::Value TestModule(const CallbackInfo &info);
        Napi::Value InstallModule(const CallbackInfo &info);
        Napi::Value IsSorting(const CallbackInfo &info);
        void Sort(const CallbackInfo &info);
        Napi::Value GetLoadOrder(const CallbackInfo &info);
        void SetLoadOrder(const CallbackInfo &info);
        Napi::Value GetModules(const CallbackInfo &info);
        void RefreshGameParameters(const CallbackInfo &info);

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
                                          InstanceMethod<&LauncherManager::RegisterCallbacks>("registerCallbacks", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::GetGameVersion>("getGameVersion", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::TestModule>("testModule", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::InstallModule>("installModule", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::IsSorting>("isSorting", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::Sort>("sort", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::GetLoadOrder>("getLoadOrder", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::SetLoadOrder>("setLoadOrder", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::GetModules>("getModules", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&LauncherManager::RefreshGameParameters>("refreshGameParameters", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
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
        const auto env = info.Env();

        const auto result = ve_create_handler(this);
        this->_pInstance = ThrowOrReturnPtr(env, result);
    }

    LauncherManager::~LauncherManager()
    {
        this->FGetActiveProfile.Unref();
        this->FGetProfileById.Unref();
        this->FGetActiveGameId.Unref();
        this->FSetGameParameters.Unref();
        this->FGetLoadOrder.Unref();
        this->FSetLoadOrder.Unref();
        this->FTranslateString.Unref();
        this->FSendNotification.Unref();
        this->FGetInstallPath.Unref();
        this->FReadFileContent.Unref();
        this->FReadDirectoryFileList.Unref();
        this->FReadDirectoryList.Unref();
        ve_dispose_handler(this->_pInstance);
    }

    static return_value_json *const get_active_profile(const void *const p_owner)
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();

            const auto result = manager->FGetActiveProfile({}).As<Object>();
            return Create(return_value_json{nullptr, Copy(JSONStringify(env, result).Utf16Value())});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_json{Copy(conv.from_bytes(e.what())), nullptr});
        }
    }
    static return_value_json *const getProfileById(const void *const p_owner, const char16_t *const p_profile_id) noexcept
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();

            const auto profileId = String::New(env, p_profile_id);
            const auto result = manager->FGetProfileById({profileId}).As<Object>();
            return Create(return_value_json{nullptr, Copy(JSONStringify(env, result).Utf16Value())});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_json{Copy(conv.from_bytes(e.what())), nullptr});
        }
    }
    static return_value_string *const getActiveGameId(const void *const p_owner) noexcept
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();

            const auto result = manager->FGetActiveGameId({}).As<String>();
            return Create(return_value_string{nullptr, Copy(result.Utf16Value())});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_string{Copy(conv.from_bytes(e.what())), nullptr});
        }
    }
    static return_value_void *const setGameParameters(const void *const p_owner, const char16_t *const p_game_id, const char16_t *const p_executable, const char16_t *const p_game_parameters) noexcept
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();

            const auto gameId = String::New(env, p_game_id);
            const auto executable = String::New(env, p_executable);
            const auto gameParameters = JSONParse(env, String::New(env, Copy(p_game_parameters)));

            manager->FSetGameParameters({gameId, executable, gameParameters}).As<Object>();
            return Create(return_value_void{nullptr});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_void{Copy(conv.from_bytes(e.what()))});
        }
    }
    static return_value_json *const getLoadOrder(const void *const p_owner) noexcept
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
    static return_value_void *const setLoadOrder(const void *const p_owner, const char16_t *const p_load_order) noexcept
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();

            const auto loadOrder = JSONParse(env, String::New(env, p_load_order));
            manager->FSetLoadOrder({loadOrder});
            return Create(return_value_void{nullptr});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_void{Copy(conv.from_bytes(e.what()))});
        }
    }
    static return_value_string *const translateString(const void *const p_owner, const char16_t *const p_text, const char16_t *const p_ns) noexcept
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();

            const auto text = String::New(env, p_text);
            const auto ns = String::New(env, p_ns);
            const auto result = manager->FTranslateString({text, ns}).As<String>();
            return Create(return_value_string{nullptr, Copy(result.Utf16Value())});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_string{Copy(conv.from_bytes(e.what())), nullptr});
        }
    }
    static return_value_void *const sendNotification(const void *const p_owner, const char16_t *const p_id, const char16_t *const p_type, const char16_t *const p_message, uint32_t displayMs) noexcept
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();

            const auto id = String::New(env, p_id);
            const auto type = String::New(env, p_type);
            const auto message = String::New(env, p_message);
            const auto displayMs_ = Number::New(env, displayMs);
            manager->FSendNotification({id, type, message, displayMs_}).As<Object>();
            return Create(return_value_void{nullptr});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_void{Copy(conv.from_bytes(e.what()))});
        }
    }
    static return_value_string *const getInstallPath(const void *const p_owner) noexcept
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
    static return_value_string *const readFileContent(const void *const p_owner, const char16_t *const p_file_path) noexcept
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();
            const auto filePath = String::New(env, p_file_path);

            const auto result = manager->FReadFileContent({filePath});
            if (result.IsNull())
            {
                return Create(return_value_string{nullptr, nullptr});
            }

            return Create(return_value_string{nullptr, Copy(result.As<String>().Utf16Value())});
        }
        catch (const std::exception &e)
        {
            std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> conv;
            return Create(return_value_string{Copy(conv.from_bytes(e.what())), nullptr});
        }
    }
    static return_value_json *const readDirectoryFileList(const void *const p_owner, const char16_t *const p_directory_path) noexcept
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();
            const auto directoryPath = String::New(env, p_directory_path);

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
    static return_value_json *const readDirectoryList(const void *const p_owner, const char16_t *const p_directory_path) noexcept
    {
        try
        {
            const auto manager = static_cast<const LauncherManager *const>(p_owner);
            const auto env = manager->Env();
            const auto directoryPath = String::New(env, p_directory_path);

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
    void LauncherManager::RegisterCallbacks(const CallbackInfo &info)
    {
        const auto env = info.Env();
        this->FGetActiveProfile = Persistent(info[0].As<Function>());
        this->FGetProfileById = Persistent(info[1].As<Function>());
        this->FGetActiveGameId = Persistent(info[2].As<Function>());
        this->FSetGameParameters = Persistent(info[3].As<Function>());
        this->FGetLoadOrder = Persistent(info[4].As<Function>());
        this->FSetLoadOrder = Persistent(info[5].As<Function>());
        this->FTranslateString = Persistent(info[6].As<Function>());
        this->FSendNotification = Persistent(info[7].As<Function>());
        this->FGetInstallPath = Persistent(info[8].As<Function>());
        this->FReadFileContent = Persistent(info[9].As<Function>());
        this->FReadDirectoryFileList = Persistent(info[10].As<Function>());
        this->FReadDirectoryList = Persistent(info[11].As<Function>());

        ThrowOrReturn(env, ve_register_callbacks(this->_pInstance,
                                                 get_active_profile,
                                                 getProfileById,
                                                 getActiveGameId,
                                                 setGameParameters,
                                                 getLoadOrder,
                                                 setLoadOrder,
                                                 translateString,
                                                 sendNotification,
                                                 getInstallPath,
                                                 readFileContent,
                                                 readDirectoryFileList,
                                                 readDirectoryList));
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
        const auto gameId = info[1].As<String>();

        const auto filesCopy = CopyWithFree(files.Utf16Value());
        const auto gameIdCopy = CopyWithFree(gameId.Utf16Value());

        const auto result = ve_test_module(this->_pInstance, filesCopy.get(), gameIdCopy.get());
        return ThrowOrReturnJson(env, result);
    }

    Value LauncherManager::InstallModule(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto files = JSONStringify(env, info[0].As<Object>());
        const auto destinationPath = info[1].As<String>();

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

    Value LauncherManager::GetLoadOrder(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto result = ve_get_load_order(this->_pInstance);
        return ThrowOrReturnJson(env, result);
    }

    void LauncherManager::SetLoadOrder(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto loadOrder = JSONStringify(env, info[0].As<Object>());

        const auto loadOrderCopy = CopyWithFree(loadOrder.Utf16Value());

        const auto result = ve_set_load_order(this->_pInstance, loadOrderCopy.get());
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
        const auto loadOrder = JSONStringify(env, info[0].As<Object>());

        const auto loadOrderCopy = CopyWithFree(loadOrder.Utf16Value());

        const auto result = ve_refresh_game_parameters(this->_pInstance, loadOrderCopy.get());
        ThrowOrReturn(env, result);
    }

}
#endif