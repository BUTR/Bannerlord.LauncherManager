#ifndef VE_LIB_UTILS_JS_GUARD_HPP_
#define VE_LIB_UTILS_JS_GUARD_HPP_

#include "Bannerlord.LauncherManager.Native.h"
#include "Logger.hpp"
#include <napi.h>

using namespace Napi;
using namespace Bannerlord::LauncherManager::Native;

namespace Utils
{
inline void ConsoleLog(const String message)
{
    const auto env = message.Env();
    const auto consoleObject = env.Global().Get("console").As<Object>();
    const auto log = consoleObject.Get("log").As<Function>();
    // log.Call(consoleObject, {message});
}

inline String JSONStringify(const Object object)
{
    const auto env = object.Env();

    if (object.IsUndefined() || object.IsNull())
    {
        Logger::Log(__FUNCTION__, "Null or undefined object");
        return env.Null().As<String>();
    }
    const auto jsonObject = env.Global().Get("JSON").As<Object>();
    const auto stringify = jsonObject.Get("stringify").As<Function>();
    return stringify.Call(jsonObject, {object}).As<String>();
}

// A module list parameter: an array goes as its JSON, a string, "#" and the handle of a list the native side keeps
// (see ModuleValidator), as it is
inline String ModuleListArgument(const Value value)
{
    return value.IsString() ? value.As<String>() : JSONStringify(value.As<Object>());
}

inline Object JSONParse(const String json)
{
    const auto env = json.Env();
    const auto jsonObject = env.Global().Get("JSON").As<Object>();
    const auto parse = jsonObject.Get("parse").As<Function>();
    return parse.Call(jsonObject, {json}).As<Object>();
}

// JSON of the native side, UTF-8 the caller keeps, as a JS value. V8 makes its own copy whatever the API: Electron's V8
// sandbox keeps native memory out of the heap, even an external string is copied there. The C# serializer writes ASCII,
// escaping everything else as \uXXXX, so the copy is a string of 1 byte per char
inline Object JSONParse(const Env env, const uint8_t *const json, const size_t length)
{
    return JSONParse(String::New(env, reinterpret_cast<const char *>(json), length));
}

// JSON of the native side, null-terminated UTF-8 the caller keeps, see above
inline Object JSONParse(const Env env, const uint8_t *const json)
{
    return JSONParse(String::New(env, reinterpret_cast<const char *>(json)));
}

// JSON of the native side the caller hands over, freed once V8 has its copy, see above
inline Object JSONParseOwned(const Env env, uint8_t *const json, const size_t length)
{
    struct Free
    {
        uint8_t *const json;
        ~Free()
        {
            common_dealloc(json);
        }
    } const free{json};
    return JSONParse(env, json, length);
}
} // namespace Utils
#endif
