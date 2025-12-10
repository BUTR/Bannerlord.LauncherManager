#ifndef VE_FETCHBLVERSION_GUARD_HPP_
#define VE_FETCHBLVERSION_GUARD_HPP_

#include <napi.h>
#include "Bannerlord.LauncherManager.Native.h"

using namespace Napi;
using namespace Bannerlord::LauncherManager::Native;

namespace Bindings::FetchVersion
{
    Value GetChangeSetWrapped(const CallbackInfo &info);

    Value GetVersionWrapped(const CallbackInfo &info);

    Value GetVersionTypeWrapped(const CallbackInfo &info);

    Object Init(const Env env, const Object exports);
}
#endif
