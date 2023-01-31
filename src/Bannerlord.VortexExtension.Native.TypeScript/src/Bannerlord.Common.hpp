#ifndef VE_COMMON_GUARD_HPP_
#define VE_COMMON_GUARD_HPP_

#include <napi.h>
#include "Common.Native.h"

using namespace Napi;
using namespace Common;

namespace Bannerlord::Common
{

    Value AllocAliveCount(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto result = alloc_alive_count();
        return Number::New(env, result);
    }

    Object Init(const Env env, const Object exports)
    {
        exports.Set("allocAliveCount", Function::New(env, AllocAliveCount));

        return exports;
    }

}
#endif