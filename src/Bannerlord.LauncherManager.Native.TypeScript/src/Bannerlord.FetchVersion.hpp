#ifndef VE_FETCHBLVERSION_GUARD_HPP_
#define VE_FETCHBLVERSION_GUARD_HPP_

#include "utils.hpp"
#include "Bannerlord.LauncherManager.Native.h"
#include <codecvt>

using namespace Napi;
using namespace Utils;
using namespace Bannerlord::LauncherManager::Native;

namespace Bannerlord::FetchVersion
{
    Value GetChangeSetWrapped(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto gameFolderPath = info[0].As<String>();
        const auto libAssembly = info[1].As<String>();

        const auto gameFolderPathCopy = CopyWithFree(gameFolderPath.Utf16Value());
        const auto libAssemblyCopy = CopyWithFree(libAssembly.Utf16Value());

        const auto result = bfv_get_change_set(gameFolderPathCopy.get(), libAssemblyCopy.get());
        return ThrowOrReturnUInt32(env, result);
    }

    Value GetVersionWrapped(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto gameFolderPath = info[0].As<String>();
        const auto libAssembly = info[1].As<String>();

        const auto gameFolderPathCopy = CopyWithFree(gameFolderPath.Utf16Value());
        const auto libAssemblyCopy = CopyWithFree(libAssembly.Utf16Value());

        const auto result = bfv_get_version(gameFolderPathCopy.get(), libAssemblyCopy.get());
        return ThrowOrReturnString(env, result);
    }

    Value GetVersionTypeWrapped(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto gameFolderPath = info[0].As<String>();
        const auto libAssembly = info[1].As<String>();

        const auto gameFolderPathCopy = CopyWithFree(gameFolderPath.Utf16Value());
        const auto libAssemblyCopy = CopyWithFree(libAssembly.Utf16Value());

        const auto result = bfv_get_version_type(gameFolderPathCopy.get(), libAssemblyCopy.get());
        return ThrowOrReturnUInt32(env, result);
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
