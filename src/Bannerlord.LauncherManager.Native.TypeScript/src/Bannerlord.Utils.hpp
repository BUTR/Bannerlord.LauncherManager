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
        const auto modules = JSONStringify(env, info[0].As<Object>());

        const auto modulesCopy = CopyWithFree(modules.Utf16Value());

        const auto result = utils_is_load_order_correct(modulesCopy.get());
        return ThrowOrReturnJson(env, result);
    }

    Value GetDependencyHint(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto module = JSONStringify(env, info[0].As<Object>());

        const auto moduleCopy = CopyWithFree(module.Utf16Value());

        const auto result = utils_get_dependency_hint(moduleCopy.get());
        return ThrowOrReturnString(env, result);
    }

    Value UtilsTranslate(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto text = info[0].As<String>();

        const auto textCopy = CopyWithFree(text.Utf16Value());

        const auto result = utils_translate(textCopy.get());
        return ThrowOrReturnString(env, result);
    }

    Object Init(const Env env, const Object exports)
    {
        exports.Set("isLoadOrderCorrect", Function::New(env, IsLoadOrderCorrect));

        exports.Set("getDependencyHint", Function::New(env, GetDependencyHint));

        exports.Set("utilsTranslate", Function::New(env, UtilsTranslate));

        return exports;
    }

}
#endif