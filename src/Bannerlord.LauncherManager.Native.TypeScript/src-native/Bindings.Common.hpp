#ifndef VE_COMMON_GUARD_HPP_
#define VE_COMMON_GUARD_HPP_

#include "Bannerlord.LauncherManager.Native.h"
#include <napi.h>

using namespace Napi;
using namespace Bannerlord::LauncherManager::Native;
namespace Bindings::Common
{
Value AllocAliveCount(const CallbackInfo &info);

Object Init(const Env env, Object exports);
} // namespace Bindings::Common
#endif
