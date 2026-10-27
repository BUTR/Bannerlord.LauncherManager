#ifndef VE_COMMON_IMPL_GUARD_HPP_
#define VE_COMMON_IMPL_GUARD_HPP_

#include "Bannerlord.LauncherManager.Native.h"
#include "Bindings.Common.hpp"
#include "Logger.hpp"
#include "Utils.Return.hpp"
#include <napi.h>

using namespace Napi;
using namespace Utils;
using namespace Bannerlord::LauncherManager::Native;
namespace Bindings::Common
{
Value AllocAliveCount(const CallbackInfo &info)
{
    LoggerScope logger(__FUNCTION__);
    const auto env = info.Env();

    return WithExceptionHandlingReturningNull(logger, env, [&]() {
        const auto result = common_alloc_alive_count();
        return Number::New(env, result);
    });
}

Object Init(const Env env, Object exports)
{
    exports.Set("allocAliveCount", Function::New(env, AllocAliveCount));

    return exports;
}
} // namespace Bindings::Common
#endif
