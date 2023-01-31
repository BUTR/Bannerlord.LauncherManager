#ifndef VE_VORTEXEXTENSIONMANAGER_GUARD_HPP_
#define VE_VORTEXEXTENSIONMANAGER_GUARD_HPP_

#include "utils.hpp"
#include "Common.Native.h"
#include "Bannerlord.VortexExtension.Native.h"
#include <codecvt>

using namespace Napi;
using namespace Common;
using namespace Utils;

namespace Bannerlord::VortexExtension
{

    class VortexExtensionManager : public Napi::ObjectWrap<VortexExtensionManager>
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

        VortexExtensionManager(const CallbackInfo &info);
        ~VortexExtensionManager();

        void RegisterCallbacks(const CallbackInfo &info);
        Napi::Value GetGameVersion(const CallbackInfo &info);
        Napi::Value TestModule(const CallbackInfo &info);
        Napi::Value InstallModule(const CallbackInfo &info);
        Napi::Value IsSorting(const CallbackInfo &info);
        void Sort(const CallbackInfo &info);
        Napi::Value GetLoadOrder(const CallbackInfo &info);
        void SetLoadOrder(const CallbackInfo &info);
        Napi::Value GetModules(const CallbackInfo &info);

    private:
        void *_pInstance;
    };

    // Initialize native add-on
    Napi::Object Init(const Napi::Env env, const Napi::Object exports)
    {
        return VortexExtensionManager::Init(env, exports);
    }

    Object VortexExtensionManager::Init(const Napi::Env env, const Object exports)
    {
        // This method is used to hook the accessor and method callbacks
        const auto func = DefineClass(env, "VortexExtensionManager",
                                      {
                                          InstanceMethod<&VortexExtensionManager::RegisterCallbacks>("registerCallbacks", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&VortexExtensionManager::GetGameVersion>("getGameVersion", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&VortexExtensionManager::TestModule>("testModule", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&VortexExtensionManager::InstallModule>("installModule", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&VortexExtensionManager::IsSorting>("isSorting", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&VortexExtensionManager::Sort>("sort", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&VortexExtensionManager::GetLoadOrder>("getLoadOrder", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&VortexExtensionManager::SetLoadOrder>("setLoadOrder", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                          InstanceMethod<&VortexExtensionManager::GetModules>("getModules", static_cast<napi_property_attributes>(napi_writable | napi_configurable)),
                                      });

        auto *const constructor = new FunctionReference();

        // Create a persistent reference to the class constructor. This will allow
        // a function called on a class prototype and a function
        // called on instance of a class to be distinguished from each other.
        *constructor = Persistent(func);
        exports.Set("VortexExtensionManager", func);

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

    VortexExtensionManager::VortexExtensionManager(const CallbackInfo &info) : ObjectWrap<VortexExtensionManager>(info)
    {
        const auto env = info.Env();

        const auto result = Bannerlord::VortexExtension::ve_create_handler(this);
        this->_pInstance = ThrowOrReturnPtr(env, result);
    }

    VortexExtensionManager::~VortexExtensionManager()
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
        Bannerlord::VortexExtension::ve_dispose_handler(this->_pInstance);
    }

    static return_value_json *get_active_profile(void *const p_owner)
    {
        try
        {
            const auto manager = static_cast<VortexExtensionManager *>(p_owner);
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
    static return_value_json *getProfileById(void *const p_owner, const char16_t *const p_profile_id) noexcept
    {
        try
        {
            const auto manager = static_cast<VortexExtensionManager *>(p_owner);
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
    static return_value_string *getActiveGameId(void *const p_owner) noexcept
    {
        try
        {
            const auto manager = static_cast<VortexExtensionManager *>(p_owner);
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
    static return_value_void *setGameParameters(void *const p_owner, const char16_t *const p_game_id, const char16_t *const p_executable, const char16_t *const p_game_parameters) noexcept
    {
        try
        {
            const auto manager = static_cast<VortexExtensionManager *>(p_owner);
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
    static return_value_json *getLoadOrder(void *const p_owner) noexcept
    {
        try
        {
            const auto manager = static_cast<VortexExtensionManager *const>(p_owner);
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
    static return_value_void *setLoadOrder(void *const p_owner, const char16_t *const p_load_order) noexcept
    {
        try
        {
            const auto manager = static_cast<VortexExtensionManager *>(p_owner);
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
    static return_value_string *translateString(void *const p_owner, const char16_t *const p_text, const char16_t *const p_ns) noexcept
    {
        try
        {
            const auto manager = static_cast<VortexExtensionManager *>(p_owner);
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
    static return_value_void *sendNotification(void *const p_owner, const char16_t *const p_id, const char16_t *const p_type, const char16_t *const p_message, const uint32_t displayMs) noexcept
    {
        try
        {
            const auto manager = static_cast<VortexExtensionManager *>(p_owner);
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
    static return_value_string *getInstallPath(void *const p_owner) noexcept
    {
        try
        {
            const auto manager = static_cast<VortexExtensionManager *>(p_owner);
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
    static return_value_string *readFileContent(void *const p_owner, const char16_t *const p_file_path) noexcept
    {
        try
        {
            const auto manager = static_cast<VortexExtensionManager *>(p_owner);
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
    static return_value_json *readDirectoryFileList(void *const p_owner, const char16_t *const p_directory_path) noexcept
    {
        try
        {
            const auto manager = static_cast<VortexExtensionManager *>(p_owner);
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
    static return_value_json *readDirectoryList(void *const p_owner, const char16_t *const p_directory_path) noexcept
    {
        try
        {
            const auto manager = static_cast<VortexExtensionManager *>(p_owner);
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
    void VortexExtensionManager::RegisterCallbacks(const CallbackInfo &info)
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

        ThrowOrReturn(env, Bannerlord::VortexExtension::ve_register_callbacks(this->_pInstance,
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

    Value VortexExtensionManager::GetGameVersion(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto result = Bannerlord::VortexExtension::ve_get_game_version(this->_pInstance);
        return ThrowOrReturnString(env, result);
    }

    Value VortexExtensionManager::TestModule(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto files = JSONStringify(env, info[0].As<Object>());
        const auto gameId = info[1].As<String>();

        const auto filesCopy = CopyWithFree(files.Utf16Value());
        const auto gameIdCopy = CopyWithFree(gameId.Utf16Value());

        const auto result = Bannerlord::VortexExtension::ve_test_module(this->_pInstance, filesCopy.get(), gameIdCopy.get());
        return ThrowOrReturnJson(env, result);
    }

    Value VortexExtensionManager::InstallModule(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto files = JSONStringify(env, info[0].As<Object>());
        const auto destinationPath = info[1].As<String>();

        const auto filesCopy = CopyWithFree(files.Utf16Value());
        const auto destinationPathCopy = CopyWithFree(destinationPath.Utf16Value());

        const auto result = Bannerlord::VortexExtension::ve_install_module(this->_pInstance, filesCopy.get(), destinationPathCopy.get());
        return ThrowOrReturnJson(env, result);
    }

    Value VortexExtensionManager::IsSorting(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto result = Bannerlord::VortexExtension::ve_is_sorting(this->_pInstance);
        return ThrowOrReturnBoolean(env, result);
    }

    void VortexExtensionManager::Sort(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto result = Bannerlord::VortexExtension::ve_sort(this->_pInstance);
        ThrowOrReturn(env, result);
    }

    Value VortexExtensionManager::GetLoadOrder(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto result = Bannerlord::VortexExtension::ve_get_load_order(this->_pInstance);
        return ThrowOrReturnJson(env, result);
    }

    void VortexExtensionManager::SetLoadOrder(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto loadOrder = JSONStringify(env, info[0].As<Object>());

        const auto loadOrderCopy = CopyWithFree(loadOrder.Utf16Value());

        const auto result = Bannerlord::VortexExtension::ve_set_load_order(this->_pInstance, loadOrderCopy.get());
        ThrowOrReturn(env, result);
    }

    Value VortexExtensionManager::GetModules(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto result = Bannerlord::VortexExtension::ve_get_modules(this->_pInstance);
        return ThrowOrReturnJson(env, result);
    }

}
#endif