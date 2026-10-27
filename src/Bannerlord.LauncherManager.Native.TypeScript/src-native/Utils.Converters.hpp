#ifndef LIB_UTILS_CONVERTERS_GUARD_HPP_
#define LIB_UTILS_CONVERTERS_GUARD_HPP_

#include "Logger.hpp"
#include "Utils.Generic.hpp"
#include "Utils.JS.hpp"
#include "Utils.Utf.hpp"
#include <napi.h>

using namespace Napi;

namespace Utils
{
inline return_value_string *ConvertToStringResult(const Napi::Value &result)
{
    if (result.IsNull())
    {
        Logger::Log(__FUNCTION__, "Value: NULL");
        return Create(return_value_string{nullptr, nullptr});
    }

    const auto resultStr = result.As<Napi::String>();
#if _DEBUG
    Logger::Log(__FUNCTION__, "Value: " + resultStr.Utf8Value());
#endif
    return Create(return_value_string{nullptr, Copy(resultStr)});
}

// JSON as UTF-8, see Bindings.Json.cs
inline return_value_data *ConvertToJsonResult(const Napi::Value &result)
{
    if (result.IsNull())
    {
        Logger::Log(__FUNCTION__, "Value: NULL");
        return Create(return_value_data{nullptr, nullptr, 0});
    }

    const auto resultObj = result.As<Object>();
#if _DEBUG
    Logger::Log(__FUNCTION__, "Value: " + JSONStringify(resultObj).Utf8Value());
#endif
    size_t length = 0;
    auto json = std::unique_ptr<uint8_t[], common_deallocor<uint8_t>>(CopyUtf8(JSONStringify(resultObj), &length));
    const auto converted = Create(return_value_data{nullptr, json.get(), static_cast<int>(length)});
    json.release();
    return converted;
}

inline return_value_data *ConvertToDataResult(const Napi::Value &result)
{
    if (result.IsNull())
    {
#if _DEBUG
        Logger::Log(__FUNCTION__, "Value: NULL");
#endif
        return Create(return_value_data{nullptr, nullptr, 0});
    }

    if (!result.IsBuffer())
    {
        Logger::Log(__FUNCTION__, "Value: Not a Buffer<uint8_t>");
        return Create(return_value_data{Copy(Utf8ToUtf16("Not a Buffer<uint8_t>")), nullptr, 0});
    }

    auto buffer = result.As<Buffer<uint8_t>>();
#if _DEBUG
    Logger::Log(__FUNCTION__, "Buffer size: " + std::to_string(buffer.ByteLength()));
#endif
    // Lent, not copied: the native side gets it in the same call, while the buffer is alive, and copies what it keeps
    return Create(return_value_data{nullptr, buffer.Data(), static_cast<int>(buffer.ByteLength())});
}

inline return_value_void *ConvertToVoidResult(LoggerScope &logger)
{
    return Create(return_value_void{nullptr});
}
} // namespace Utils
#endif
