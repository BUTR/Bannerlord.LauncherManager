#ifndef VE_UTILS_IMPL_GUARD_HPP_
#define VE_UTILS_IMPL_GUARD_HPP_

#include "Bannerlord.LauncherManager.Native.h"
#include "Bindings.Utils.hpp"
#include "Logger.hpp"
#include "Utils.Return.hpp"
#include <napi.h>

using namespace Napi;
using namespace Utils;
using namespace Bannerlord::LauncherManager::Native;

namespace Bindings::Utils
{
Value IsLoadOrderCorrect(const CallbackInfo &info)
{
    LoggerScope logger(__FUNCTION__);
    return WithExceptionHandling(logger, [&]() {
        const auto env = info.Env();
        const auto modules = JSONStringify(info[0].As<Object>());
        const auto modulesCopy = CopyWithFree(modules.Utf16Value());
        return ThrowOrReturnJson(env, utils_is_load_order_correct(modulesCopy.get()));
    });
}

Value GetDependencyHint(const CallbackInfo &info)
{
    LoggerScope logger(__FUNCTION__, true);
    return WithExceptionHandling(logger, [&]() {
        const auto env = info.Env();
        const auto module = JSONStringify(info[0].As<Object>());
        const auto moduleCopy = CopyWithFree(module.Utf16Value());
        return ThrowOrReturnString(env, utils_get_dependency_hint(moduleCopy.get()));
    });
}

Value RenderModuleIssue(const CallbackInfo &info)
{
    LoggerScope logger(__FUNCTION__);
    return WithExceptionHandling(logger, [&]() {
        const auto env = info.Env();
        const auto moduleIssue = JSONStringify(info[0].As<Object>());
        const auto moduleIssueCopy = CopyWithFree(moduleIssue.Utf16Value());
        return ThrowOrReturnString(env, utils_render_module_issue(moduleIssueCopy.get()));
    });
}

void LoadLocalization(const CallbackInfo &info)
{
    LoggerScope logger(__FUNCTION__);
    WithExceptionHandling(logger, [&]() {
        const auto env = info.Env();
        const auto xml = info[0].As<String>();
        const auto xmlCopy = CopyWithFree(xml.Utf16Value());
        ThrowOrReturn(env, utils_load_localization(xmlCopy.get()));
    });
}

void SetLanguage(const CallbackInfo &info)
{
    LoggerScope logger(__FUNCTION__);
    WithExceptionHandling(logger, [&]() {
        const auto env = info.Env();
        const auto language = info[0].As<String>();
        const auto languageCopy = CopyWithFree(language.Utf16Value());
        ThrowOrReturn(env, utils_set_language(languageCopy.get()));
    });
}

Value LocalizeString(const CallbackInfo &info)
{
    LoggerScope logger(__FUNCTION__, true);
    return WithExceptionHandling(logger, [&]() {
        const auto env = info.Env();
        const auto templateStr = info[0].As<String>();
        const auto values = JSONStringify(info[1].As<Object>());
        const auto templateStrCopy = CopyWithFree(templateStr.Utf16Value());
        const auto valuesCopy = CopyWithFree(values.Utf16Value());
        return ThrowOrReturnString(env, utils_localize_string(templateStrCopy.get(), valuesCopy.get()));
    });
}

Object Init(const Env env, const Object exports)
{
    exports.Set("isLoadOrderCorrect", Function::New(env, IsLoadOrderCorrect));

    exports.Set("getDependencyHint", Function::New(env, GetDependencyHint));

    exports.Set("renderModuleIssue", Function::New(env, RenderModuleIssue));

    exports.Set("loadLocalization", Function::New(env, LoadLocalization));
    exports.Set("setLanguage", Function::New(env, SetLanguage));
    exports.Set("localizeString", Function::New(env, LocalizeString));

    return exports;
}

} // namespace Bindings::Utils
#endif
