#ifndef VE_LIB_UTILS_GUARD_HPP_
#define VE_LIB_UTILS_GUARD_HPP_

#include "Bannerlord.LauncherManager.Native.h"
#include "Logger.hpp"
#include <napi.h>

using namespace Napi;
using namespace Bannerlord::LauncherManager::Native;

namespace Utils
{
template <typename T> struct common_deallocor
{
    void operator()(T *const ptr) const
    {
        if (ptr != nullptr)
        {
            common_dealloc(static_cast<void *const>(ptr));
        }
    }
};

template <typename T> struct deleter
{
    void operator()(T *const ptr) const
    {
        delete ptr;
    }
};

using del_void = std::unique_ptr<return_value_void, common_deallocor<return_value_void>>;
using del_string = std::unique_ptr<return_value_string, common_deallocor<return_value_string>>;
using del_data = std::unique_ptr<return_value_data, common_deallocor<return_value_data>>;
using del_bool = std::unique_ptr<return_value_bool, common_deallocor<return_value_bool>>;
using del_int32 = std::unique_ptr<return_value_int32, common_deallocor<return_value_int32>>;
using del_uint32 = std::unique_ptr<return_value_uint32, common_deallocor<return_value_uint32>>;
using del_ptr = std::unique_ptr<return_value_ptr, common_deallocor<return_value_ptr>>;
using del_async = std::unique_ptr<return_value_async, common_deallocor<return_value_async>>;

inline uint8_t *const Copy(const uint8_t *src, const size_t length)
{
    auto dst = static_cast<uint8_t *const>(common_alloc(length));
    if (dst == nullptr)
    {
        Logger::Log(__FUNCTION__, "Failed to allocate memory");
        throw std::bad_alloc();
    }
    std::memmove(dst, src, length);
    return dst;
}

inline char16_t *const Copy(const std::u16string str)
{
    const auto src = str.c_str();
    const auto srcChar16Length = str.length();
    const auto srcByteLength = srcChar16Length * sizeof(char16_t);
    const auto size = srcByteLength + sizeof(char16_t);

    auto dst = static_cast<char16_t *const>(common_alloc(size));
    if (dst == nullptr)
    {
        Logger::Log(__FUNCTION__, "Failed to allocate memory");
        throw std::bad_alloc();
    }
    std::memmove(dst, src, srcByteLength);
    dst[srcChar16Length] = '\0';
    return dst;
}

// A JS string straight into native memory: Utf16Value() would make a std::u16string to copy from first, 1.5 MB more
// for the JSON of a 300 module list
inline char16_t *const Copy(const String &str)
{
    const auto env = str.Env();
    size_t length = 0;
    NAPI_THROW_IF_FAILED(env, napi_get_value_string_utf16(env, str, nullptr, 0, &length), nullptr);

    auto dst = static_cast<char16_t *const>(common_alloc((length + 1) * sizeof(char16_t)));
    if (dst == nullptr)
    {
        Logger::Log(__FUNCTION__, "Failed to allocate memory");
        throw std::bad_alloc();
    }
    size_t copied = 0;
    const auto status = napi_get_value_string_utf16(env, str, dst, length + 1, &copied);
    if (status != napi_ok)
    {
        common_dealloc(dst);
        NAPI_THROW_IF_FAILED(env, status, nullptr);
    }
    return dst;
}

// A JS string as null-terminated UTF-8 in native memory, the way the native side reads JSON: V8 writes it straight from
// its own encoding, nothing converts it again
inline uint8_t *const CopyUtf8(const String &str, size_t *const length = nullptr)
{
    const auto env = str.Env();
    size_t utf8Length = 0;
    NAPI_THROW_IF_FAILED(env, napi_get_value_string_utf8(env, str, nullptr, 0, &utf8Length), nullptr);

    auto dst = static_cast<uint8_t *const>(common_alloc(utf8Length + 1));
    if (dst == nullptr)
    {
        Logger::Log(__FUNCTION__, "Failed to allocate memory");
        throw std::bad_alloc();
    }
    size_t copied = 0;
    const auto status = napi_get_value_string_utf8(env, str, reinterpret_cast<char *>(dst), utf8Length + 1, &copied);
    if (status != napi_ok)
    {
        common_dealloc(dst);
        NAPI_THROW_IF_FAILED(env, status, nullptr);
    }
    if (length != nullptr)
    {
        *length = copied;
    }
    return dst;
}

inline std::unique_ptr<uint8_t[], common_deallocor<uint8_t>> CopyUtf8WithFree(const String &str)
{
    return std::unique_ptr<uint8_t[], common_deallocor<uint8_t>>(CopyUtf8(str));
}

inline std::unique_ptr<char16_t[], common_deallocor<char16_t>> CopyWithFree(const String &str)
{
    return std::unique_ptr<char16_t[], common_deallocor<char16_t>>(Copy(str));
}

inline std::unique_ptr<uint8_t[], common_deallocor<uint8_t>> CopyWithFree(const uint8_t *const data, size_t length)
{
    return std::unique_ptr<uint8_t[], common_deallocor<uint8_t>>(Copy(data, length));
}

inline std::unique_ptr<char16_t[], common_deallocor<char16_t>> CopyWithFree(const std::u16string str)
{
    return std::unique_ptr<char16_t[], common_deallocor<char16_t>>(Copy(str));
}

inline std::unique_ptr<char16_t[], common_deallocor<char16_t>> NullStringCopy()
{
    return std::unique_ptr<char16_t[], common_deallocor<char16_t>>(nullptr);
}

inline const char16_t *const NoCopy(const std::u16string str) noexcept
{
    return str.c_str();
}

template <typename T> inline T *const Create(const T val)
{
    const auto size = sizeof(T);
    auto dst = static_cast<T *const>(common_alloc(size));
    if (dst == nullptr)
    {
        Logger::Log(__FUNCTION__, "Failed to allocate memory");
        throw std::bad_alloc();
    }
    std::memcpy(dst, &val, sizeof(T));
    return dst;
}

// Exception handling wrapper to reduce code duplication
// Wraps a callable and handles Napi::Error, std::exception, and unknown exceptions
template <typename Func> inline decltype(auto) WithExceptionHandling(LoggerScope &logger, Func &&func)
{
    try
    {
        return func();
    }
    catch (const Napi::Error &e)
    {
        logger.LogError(e);
        throw;
    }
    catch (const std::exception &e)
    {
        logger.LogException(e);
        throw;
    }
    catch (...)
    {
        logger.Log("Unknown exception");
        throw;
    }
}

// Exception handling wrapper for NAPI functions - returns Napi::Value (null on error)
template <typename Func> inline Napi::Value WithExceptionHandlingReturningNull(LoggerScope &logger, const Napi::Env &env, Func &&func) noexcept
{
    try
    {
        return func();
    }
    catch (const Napi::Error &e)
    {
        logger.LogError(e);
        return env.Null();
    }
    catch (const std::exception &e)
    {
        logger.LogException(e);
        return env.Null();
    }
    catch (...)
    {
        logger.Log("Unknown exception");
        return env.Null();
    }
}
} // namespace Utils
#endif
