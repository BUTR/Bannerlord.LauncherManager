#ifndef VE_UTILS_GUARD_HPP_
#define VE_UTILS_GUARD_HPP_

#include <napi.h>
#include "Bannerlord.LauncherManager.Native.h"

using namespace Napi;
using namespace Bannerlord::LauncherManager::Native;

namespace Bindings::Utils
{
    Value IsLoadOrderCorrect(const CallbackInfo &info);

    Value GetDependencyHint(const CallbackInfo &info);

    Value RenderModuleIssue(const CallbackInfo &info);

    void LoadLocalization(const CallbackInfo &info);

    void SetLanguage(const CallbackInfo &info);

    Value LocalizeString(const CallbackInfo &info);

    Object Init(const Env env, const Object exports);
}
#endif
