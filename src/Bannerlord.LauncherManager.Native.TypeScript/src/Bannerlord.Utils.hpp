#ifndef VE_UTILS_GUARD_HPP_
#define VE_UTILS_GUARD_HPP_

#include "utils.hpp"
#include "Bannerlord.LauncherManager.Native.h"
#include <codecvt>

using namespace Napi;
using namespace Utils;
using namespace Bannerlord::LauncherManager::Native;

namespace Bannerlord::Utils
{
    Value IsLoadOrderCorrect(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto modules = JSONStringify(info[0].As<Object>());

        const auto modulesCopy = CopyWithFree(modules.Utf16Value());

        const auto result = utils_is_load_order_correct(modulesCopy.get());
        return ThrowOrReturnJson(env, result);
    }

    Value GetDependencyHint(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto module = JSONStringify(info[0].As<Object>());

        const auto moduleCopy = CopyWithFree(module.Utf16Value());

        const auto result = utils_get_dependency_hint(moduleCopy.get());
        return ThrowOrReturnString(env, result);
    }

    Value RenderModuleIssue(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto moduleIssue = JSONStringify(info[0].As<Object>());

        const auto moduleIssueCopy = CopyWithFree(moduleIssue.Utf16Value());

        const auto result = utils_render_module_issue(moduleIssueCopy.get());
        return ThrowOrReturnString(env, result);
    }

    void LoadLocalization(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto xml = info[0].As<String>();

        const auto xmlCopy = CopyWithFree(xml.Utf16Value());

        const auto result = utils_load_localization(xmlCopy.get());
        ThrowOrReturn(env, result);
    }

    void SetLanguage(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto language = info[0].As<String>();

        const auto languageCopy = CopyWithFree(language.Utf16Value());

        const auto result = utils_set_language(languageCopy.get());
        return ThrowOrReturn(env, result);
    }

    Value LocalizeString(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto templateStr = info[0].As<String>();
        const auto values = JSONStringify(info[1].As<Object>());

        const auto templateStrCopy = CopyWithFree(templateStr.Utf16Value());
        const auto valuesCopy = CopyWithFree(values.Utf16Value());

        const auto result = utils_localize_string(templateStrCopy.get(), valuesCopy.get());
        return ThrowOrReturnString(env, result);
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

}
#endif
