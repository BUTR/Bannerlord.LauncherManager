#ifndef VE_FETCHBLVERSION_GUARD_H_
#define VE_FETCHBLVERSION_GUARD_H_

#include "utils.h"
#include "Bannerlord.FetchVersion.Native.h"

using namespace Napi;
using namespace Utils;
using namespace Bannerlord::FetchVersion;

namespace Bannerlord::FetchVersion
{

    const Value getChangeSetWrapped(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto gameFolderPath = info[0].As<String>();
        const auto libAssembly = info[1].As<String>();

        const auto gameFolderPathCopy = CopyWithFree(gameFolderPath.Utf16Value());
        const auto libAssemblyCopy = CopyWithFree(libAssembly.Utf16Value());

        const auto result = bfv_get_change_set(gameFolderPathCopy.get(), libAssemblyCopy.get());
        return ThrowOrReturnUInt32(env, result);
    }

    const Value getVersionWrapped(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto gameFolderPath = info[0].As<String>();
        const auto libAssembly = info[1].As<String>();

        const auto gameFolderPathCopy = CopyWithFree(gameFolderPath.Utf16Value());
        const auto libAssemblyCopy = CopyWithFree(libAssembly.Utf16Value());

        const auto result = bfv_get_version(gameFolderPathCopy.get(), libAssemblyCopy.get());
        return ThrowOrReturnString(env, result);
    }

    const Value getVersionTypeWrapped(const CallbackInfo &info)
    {
        const auto env = info.Env();
        const auto gameFolderPath = info[0].As<String>();
        const auto libAssembly = info[1].As<String>();

        const auto gameFolderPathCopy = CopyWithFree(gameFolderPath.Utf16Value());
        const auto libAssemblyCopy = CopyWithFree(libAssembly.Utf16Value());

        const auto result = bfv_get_version_type(gameFolderPathCopy.get(), libAssemblyCopy.get());
        return ThrowOrReturnUInt32(env, result);
    }

    const Object Init(Env env, Object exports)
    {
        exports.Set("getChangeSet", Function::New(env, getChangeSetWrapped));

        exports.Set("getVersion", Function::New(env, getVersionWrapped));

        exports.Set("getVersionType", Function::New(env, getVersionTypeWrapped));

        return exports;
    }

}
#endif