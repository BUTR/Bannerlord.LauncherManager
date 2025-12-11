#ifndef VE_BLMANAGER_IMPL_GUARD_HPP_
#define VE_BLMANAGER_IMPL_GUARD_HPP_

#include <napi.h>
#include <thread>
#include "Bannerlord.LauncherManager.Native.h"
#include "Logger.hpp"
#include "Bindings.ModuleManager.hpp"
#include "Bindings.ModuleManager.Callbacks.hpp"

using namespace Napi;
using namespace Utils;
using namespace Bannerlord::LauncherManager::Native;

namespace Bindings::ModuleManager
{
    Value Sort(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);
        return WithExceptionHandling(logger, [&]() {
            const auto env = info.Env();
            const auto source = JSONStringify(info[0].As<Object>());
            const auto sourceCopy = CopyWithFree(source.Utf16Value());
            return ThrowOrReturnJson(env, bmm_sort(sourceCopy.get()));
        });
    }

    Value SortWithOptions(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);
        return WithExceptionHandling(logger, [&]() {
            const auto env = info.Env();
            const auto source = JSONStringify(info[0].As<Object>());
            const auto options = JSONStringify(info[1].As<Object>());
            const auto sourceCopy = CopyWithFree(source.Utf16Value());
            const auto optionsCopy = CopyWithFree(options.Utf16Value());
            return ThrowOrReturnJson(env, bmm_sort_with_options(sourceCopy.get(), optionsCopy.get()));
        });
    }

    Value AreAllDependenciesOfModulePresent(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);
        return WithExceptionHandling(logger, [&]() {
            const auto env = info.Env();
            const auto source = JSONStringify(info[0].As<Object>());
            const auto module = JSONStringify(info[1].As<Object>());
            const auto sourceCopy = CopyWithFree(source.Utf16Value());
            const auto moduleCopy = CopyWithFree(module.Utf16Value());
            return ThrowOrReturnBoolean(env, bmm_are_all_dependencies_of_module_present(sourceCopy.get(), moduleCopy.get()));
        });
    }

    Value GetDependentModulesOf(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);
        return WithExceptionHandling(logger, [&]() {
            const auto env = info.Env();
            const auto source = JSONStringify(info[0].As<Object>());
            const auto module = JSONStringify(info[1].As<Object>());
            const auto sourceCopy = CopyWithFree(source.Utf16Value());
            const auto moduleCopy = CopyWithFree(module.Utf16Value());
            return ThrowOrReturnJson(env, bmm_get_dependent_modules_of(sourceCopy.get(), moduleCopy.get()));
        });
    }

    Value GetDependentModulesOfWithOptions(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);
        return WithExceptionHandling(logger, [&]() {
            const auto env = info.Env();
            const auto source = JSONStringify(info[0].As<Object>());
            const auto module = JSONStringify(info[1].As<Object>());
            const auto options = JSONStringify(info[2].As<Object>());
            const auto sourceCopy = CopyWithFree(source.Utf16Value());
            const auto moduleCopy = CopyWithFree(module.Utf16Value());
            const auto optionsCopy = CopyWithFree(options.Utf16Value());
            return ThrowOrReturnJson(env, bmm_get_dependent_modules_of_with_options(sourceCopy.get(), moduleCopy.get(), optionsCopy.get()));
        });
    }

    Value ValidateLoadOrder(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);
        return WithExceptionHandling(logger, [&]() {
            const auto env = info.Env();
            const auto source = JSONStringify(info[0].As<Object>());
            const auto targetModule = JSONStringify(info[1].As<Object>());
            const auto sourceCopy = CopyWithFree(source.Utf16Value());
            const auto targetModuleCopy = CopyWithFree(targetModule.Utf16Value());
            return ThrowOrReturnJson(env, bmm_validate_load_order(sourceCopy.get(), targetModuleCopy.get()));
        });
    }

    Value ValidateModule(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);
        return WithExceptionHandling(logger, [&]() {
            const auto env = info.Env();
            const auto source = JSONStringify(info[0].As<Object>());
            const auto targetModule = JSONStringify(info[1].As<Object>());
            const auto manager = info[2].As<Object>();
            const auto fIsSelected = manager.Get("isSelected").As<Function>();
            const auto sourceCopy = CopyWithFree(source.Utf16Value());
            const auto targetModuleCopy = CopyWithFree(targetModule.Utf16Value());
            auto data = ValidationData{env, fIsSelected};
            return ThrowOrReturnJson(env, bmm_validate_module(static_cast<void *const>(&data), sourceCopy.get(), targetModuleCopy.get(), isSelected));
        });
    }

    void EnableModule(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);
        WithExceptionHandling(logger, [&]() {
            const auto env = info.Env();
            const auto source = JSONStringify(info[0].As<Object>());
            const auto targetModule = JSONStringify(info[1].As<Object>());
            const auto manager = info[2].As<Object>();
            const auto fGetSelected = manager.Get("getSelected").As<Function>();
            const auto fSetSelected = manager.Get("setSelected").As<Function>();
            const auto fGetDisabled = manager.Get("getDisabled").As<Function>();
            const auto fSetDisabled = manager.Get("setDisabled").As<Function>();
            const auto sourceCopy = CopyWithFree(source.Utf16Value());
            const auto targetModuleCopy = CopyWithFree(targetModule.Utf16Value());
            auto data = EnableDisableData{env, fGetSelected, fSetSelected, fGetDisabled, fSetDisabled};
            ThrowOrReturn(env, bmm_enable_module(static_cast<void *const>(&data), sourceCopy.get(), targetModuleCopy.get(), getSelected, setSelected, getDisabled, setDisabled));
        });
    }

    void DisableModule(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);
        WithExceptionHandling(logger, [&]() {
            const auto env = info.Env();
            const auto source = JSONStringify(info[0].As<Object>());
            const auto targetModule = JSONStringify(info[1].As<Object>());
            const auto manager = info[2].As<Object>();
            const auto fGetSelected = manager.Get("getSelected").As<Function>();
            const auto fSetSelected = manager.Get("setSelected").As<Function>();
            const auto fGetDisabled = manager.Get("getDisabled").As<Function>();
            const auto fSetDisabled = manager.Get("setDisabled").As<Function>();
            const auto sourceCopy = CopyWithFree(source.Utf16Value());
            const auto targetModuleCopy = CopyWithFree(targetModule.Utf16Value());
            auto data = EnableDisableData{env, fGetSelected, fSetSelected, fGetDisabled, fSetDisabled};
            ThrowOrReturn(env, bmm_disable_module(static_cast<void *const>(&data), sourceCopy.get(), targetModuleCopy.get(), getSelected, setSelected, getDisabled, setDisabled));
        });
    }

    Value GetModuleInfo(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);
        return WithExceptionHandling(logger, [&]() {
            const auto env = info.Env();
            const auto source = info[0].As<String>();
            const auto sourceCopy = CopyWithFree(source.Utf16Value());
            return ThrowOrReturnJson(env, bmm_get_module_info(sourceCopy.get()));
        });
    }

    Value GetModuleInfoWithPath(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);
        return WithExceptionHandling(logger, [&]() {
            const auto env = info.Env();
            const auto source = info[0].As<String>();
            const auto path = info[1].As<String>();
            const auto sourceCopy = CopyWithFree(source.Utf16Value());
            const auto pathCopy = CopyWithFree(path.Utf16Value());
            return ThrowOrReturnJson(env, bmm_get_module_info_with_path(sourceCopy.get(), pathCopy.get()));
        });
    }

    Value GetModuleInfoWithMetadata(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);
        return WithExceptionHandling(logger, [&]() {
            const auto env = info.Env();
            const auto source = info[0].As<String>();
            const auto moduleProvider = info[1].As<String>();
            const auto path = info[2].As<String>();
            const auto sourceCopy = CopyWithFree(source.Utf16Value());
            const auto moduleProviderCopy = CopyWithFree(moduleProvider.Utf16Value());
            const auto pathCopy = CopyWithFree(path.Utf16Value());
            return ThrowOrReturnJson(env, bmm_get_module_info_with_metadata(sourceCopy.get(), moduleProviderCopy.get(), pathCopy.get()));
        });
    }

    Value GetSubModuleInfo(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);
        return WithExceptionHandling(logger, [&]() {
            const auto env = info.Env();
            const auto source = info[0].As<String>();
            const auto sourceCopy = CopyWithFree(source.Utf16Value());
            return ThrowOrReturnJson(env, bmm_get_sub_module_info(sourceCopy.get()));
        });
    }

    Value ParseApplicationVersion(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);
        return WithExceptionHandling(logger, [&]() {
            const auto env = info.Env();
            const auto content = info[0].As<String>();
            const auto contentCopy = CopyWithFree(content.Utf16Value());
            return ThrowOrReturnJson(env, bmm_parse_application_version(contentCopy.get()));
        });
    }

    Value CompareVersions(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);
        return WithExceptionHandling(logger, [&]() {
            const auto env = info.Env();
            const auto x = JSONStringify(info[0].As<Object>());
            const auto y = JSONStringify(info[1].As<Object>());
            const auto xCopy = CopyWithFree(x.Utf16Value());
            const auto yCopy = CopyWithFree(y.Utf16Value());
            return ThrowOrReturnInt32(env, bmm_compare_versions(xCopy.get(), yCopy.get()));
        });
    }

    Value GetDependenciesAll(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);
        return WithExceptionHandling(logger, [&]() {
            const auto env = info.Env();
            const auto module = JSONStringify(info[0].As<Object>());
            const auto moduleCopy = CopyWithFree(module.Utf16Value());
            return ThrowOrReturnJson(env, bmm_get_dependencies_all(moduleCopy.get()));
        });
    }

    Value GetDependenciesToLoadBeforeThis(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);
        return WithExceptionHandling(logger, [&]() {
            const auto env = info.Env();
            const auto module = JSONStringify(info[0].As<Object>());
            const auto moduleCopy = CopyWithFree(module.Utf16Value());
            return ThrowOrReturnJson(env, bmm_get_dependencies_load_before_this(moduleCopy.get()));
        });
    }

    Value GetDependenciesToLoadAfterThis(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);
        return WithExceptionHandling(logger, [&]() {
            const auto env = info.Env();
            const auto module = JSONStringify(info[0].As<Object>());
            const auto moduleCopy = CopyWithFree(module.Utf16Value());
            return ThrowOrReturnJson(env, bmm_get_dependencies_load_after_this(moduleCopy.get()));
        });
    }

    Value GetDependenciesIncompatibles(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);
        return WithExceptionHandling(logger, [&]() {
            const auto env = info.Env();
            const auto module = JSONStringify(info[0].As<Object>());
            const auto moduleCopy = CopyWithFree(module.Utf16Value());
            return ThrowOrReturnJson(env, bmm_get_dependencies_incompatibles(moduleCopy.get()));
        });
    }

    Object Init(const Env env, const Object exports)
    {
        exports.Set("sort", Function::New(env, Sort));
        exports.Set("sortWithOptions", Function::New(env, SortWithOptions));

        exports.Set("areAllDependenciesOfModulePresent", Function::New(env, AreAllDependenciesOfModulePresent));

        exports.Set("getDependentModulesOf", Function::New(env, GetDependentModulesOf));
        exports.Set("getDependentModulesOfWithOptions", Function::New(env, GetDependentModulesOfWithOptions));

        exports.Set("validateModule", Function::New(env, ValidateModule));
        exports.Set("validateLoadOrder", Function::New(env, ValidateLoadOrder));

        exports.Set("enableModule", Function::New(env, EnableModule));
        exports.Set("disableModule", Function::New(env, DisableModule));

        exports.Set("getModuleInfo", Function::New(env, GetModuleInfo));
        exports.Set("getModuleInfoWithPath", Function::New(env, GetModuleInfoWithPath));
        exports.Set("getModuleInfoWithMetadata", Function::New(env, GetModuleInfoWithMetadata));
        exports.Set("getSubModuleInfo", Function::New(env, GetSubModuleInfo));

        exports.Set("parseApplicationVersion", Function::New(env, ParseApplicationVersion));
        exports.Set("compareVersions", Function::New(env, CompareVersions));

        exports.Set("getDependenciesAll", Function::New(env, GetDependenciesAll));
        exports.Set("getDependenciesToLoadBeforeThis", Function::New(env, GetDependenciesToLoadBeforeThis));
        exports.Set("getDependenciesToLoadAfterThis", Function::New(env, GetDependenciesToLoadAfterThis));
        exports.Set("getDependenciesIncompatibles", Function::New(env, GetDependenciesIncompatibles));

        return exports;
    }
}
#endif
