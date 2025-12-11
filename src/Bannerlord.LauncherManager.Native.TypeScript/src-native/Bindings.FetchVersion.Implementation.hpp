#ifndef VE_FETCHBLVERSION_IMPL_GUARD_HPP_
#define VE_FETCHBLVERSION_IMPL_GUARD_HPP_

#include <napi.h>
#include "Bannerlord.LauncherManager.Native.h"
#include "Logger.hpp"
#include "Utils.Return.hpp"
#include "Bindings.FetchVersion.hpp"

using namespace Napi;
using namespace Utils;
using namespace Bannerlord::LauncherManager::Native;

namespace Bindings::FetchVersion
{
    Value GetChangeSetWrapped(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);
        return WithExceptionHandling(logger, [&]() {
            const auto env = info.Env();
            const auto gameFolderPath = info[0].As<String>();
            const auto libAssembly = info[1].As<String>();
            const auto gameFolderPathCopy = CopyWithFree(gameFolderPath.Utf16Value());
            const auto libAssemblyCopy = CopyWithFree(libAssembly.Utf16Value());
            return ThrowOrReturnUInt32(env, bfv_get_change_set(gameFolderPathCopy.get(), libAssemblyCopy.get()));
        });
    }

    Value GetVersionWrapped(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);
        return WithExceptionHandling(logger, [&]() {
            const auto env = info.Env();
            const auto gameFolderPath = info[0].As<String>();
            const auto libAssembly = info[1].As<String>();
            const auto gameFolderPathCopy = CopyWithFree(gameFolderPath.Utf16Value());
            const auto libAssemblyCopy = CopyWithFree(libAssembly.Utf16Value());
            return ThrowOrReturnString(env, bfv_get_version(gameFolderPathCopy.get(), libAssemblyCopy.get()));
        });
    }

    Value GetVersionTypeWrapped(const CallbackInfo &info)
    {
        LoggerScope logger(__FUNCTION__);
        return WithExceptionHandling(logger, [&]() {
            const auto env = info.Env();
            const auto gameFolderPath = info[0].As<String>();
            const auto libAssembly = info[1].As<String>();
            const auto gameFolderPathCopy = CopyWithFree(gameFolderPath.Utf16Value());
            const auto libAssemblyCopy = CopyWithFree(libAssembly.Utf16Value());
            return ThrowOrReturnUInt32(env, bfv_get_version_type(gameFolderPathCopy.get(), libAssemblyCopy.get()));
        });
    }

    Object Init(const Env env, const Object exports)
    {
        exports.Set("getChangeSet", Function::New(env, GetChangeSetWrapped));

        exports.Set("getVersion", Function::New(env, GetVersionWrapped));

        exports.Set("getVersionType", Function::New(env, GetVersionTypeWrapped));

        return exports;
    }

}
#endif
