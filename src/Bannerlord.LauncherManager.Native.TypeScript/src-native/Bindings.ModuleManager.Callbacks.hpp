#ifndef VE_BLMANAGER_CB_GUARD_HPP_
#define VE_BLMANAGER_CB_GUARD_HPP_

#include "Bannerlord.LauncherManager.Native.h"
#include "Bindings.ModuleManager.hpp"
#include "Logger.hpp"
#include "Utils.Callbacks.hpp"
#include "Utils.Converters.hpp"
#include <napi.h>

using namespace Napi;
using namespace Utils;
using namespace Bannerlord::LauncherManager::Native;

namespace Bindings::ModuleManager
{
struct ValidationData
{
    const Napi::Env Env;
    const Function FIsSelected;
};
static return_value_bool *isSelected(param_ptr *p_owner, param_string *p_module_id) noexcept
{
    LoggerScope logger(__FUNCTION__, true);

    try
    {
        const auto data = static_cast<const ValidationData *const>(p_owner);
        const auto env = data->Env;

        const auto moduleId = String::New(env, p_module_id);

        return Create(return_value_bool{nullptr, data->FIsSelected({moduleId}).As<Boolean>()});
    }
    catch (const Napi::Error &e)
    {
        logger.LogError(e);
        return Create(return_value_bool{Copy(GetErrorMessage(e))});
    }
    catch (const std::exception &e)
    {
        logger.LogException(e);
        return Create(return_value_bool{Copy(Utf8ToUtf16(e.what()))});
    }
    catch (...)
    {
        logger.Log("Unknown exception");
        return Create(return_value_bool{Copy(u"Unknown exception")});
    }
}

struct EnableDisableData
{
    const Napi::Env Env;
    const Function FGetSelected;
    const Function FSetSelected;
    const Function FGetDisabled;
    const Function FSetDisabled;
};
static return_value_bool *getSelected(param_ptr *p_owner, param_string *p_module_id) noexcept
{
    LoggerScope logger(__FUNCTION__, true);

    try
    {
        const auto data = static_cast<const EnableDisableData *const>(p_owner);
        const auto env = data->Env;

        const auto moduleId = String::New(env, p_module_id);

        return Create(return_value_bool{nullptr, data->FGetSelected({moduleId}).As<Boolean>()});
    }
    catch (const Napi::Error &e)
    {
        logger.LogError(e);
        return Create(return_value_bool{Copy(GetErrorMessage(e))});
    }
    catch (const std::exception &e)
    {
        logger.LogException(e);
        return Create(return_value_bool{Copy(Utf8ToUtf16(e.what()))});
    }
    catch (...)
    {
        logger.Log("Unknown exception");
        return Create(return_value_bool{Copy(u"Unknown exception")});
    }
}
static return_value_void *setSelected(param_ptr *p_owner, param_string *p_module_id, param_bool value_raw) noexcept
{
    LoggerScope logger(__FUNCTION__, true);

    try
    {
        const auto data = static_cast<const EnableDisableData *const>(p_owner);
        const auto env = data->Env;

        const auto moduleId = String::New(env, p_module_id);
        const auto value = Boolean::New(env, value_raw == 1);

        data->FSetSelected({moduleId, value});
        return Create(return_value_void{nullptr});
    }
    catch (const Napi::Error &e)
    {
        logger.LogError(e);
        return Create(return_value_void{Copy(GetErrorMessage(e))});
    }
    catch (const std::exception &e)
    {
        logger.LogException(e);
        return Create(return_value_void{Copy(Utf8ToUtf16(e.what()))});
    }
    catch (...)
    {
        logger.Log("Unknown exception");
        return Create(return_value_void{Copy(u"Unknown exception")});
    }
}
static return_value_bool *getDisabled(param_ptr *p_owner, param_string *p_module_id) noexcept
{
    LoggerScope logger(__FUNCTION__, true);

    try
    {
        const auto data = static_cast<const EnableDisableData *const>(p_owner);
        const auto env = data->Env;

        const auto moduleId = String::New(env, p_module_id);

        return Create(return_value_bool{nullptr, data->FGetDisabled({moduleId}).As<Boolean>()});
    }
    catch (const Napi::Error &e)
    {
        logger.LogError(e);
        return Create(return_value_bool{Copy(GetErrorMessage(e))});
    }
    catch (const std::exception &e)
    {
        logger.LogException(e);
        return Create(return_value_bool{Copy(Utf8ToUtf16(e.what()))});
    }
    catch (...)
    {
        logger.Log("Unknown exception");
        return Create(return_value_bool{Copy(u"Unknown exception")});
    }
}
static return_value_void *setDisabled(param_ptr *p_owner, param_string *p_module_id, param_bool value_raw) noexcept
{
    LoggerScope logger(__FUNCTION__, true);

    try
    {
        const auto data = static_cast<const EnableDisableData *const>(p_owner);
        const auto env = data->Env;

        const auto moduleId = String::New(env, p_module_id);
        const auto value = Boolean::New(env, value_raw == 1);

        data->FSetDisabled({moduleId, value});
        return Create(return_value_void{nullptr});
    }
    catch (const Napi::Error &e)
    {
        logger.LogError(e);
        return Create(return_value_void{Copy(GetErrorMessage(e))});
    }
    catch (const std::exception &e)
    {
        logger.LogException(e);
        return Create(return_value_void{Copy(Utf8ToUtf16(e.what()))});
    }
    catch (...)
    {
        logger.Log("Unknown exception");
        return Create(return_value_void{Copy(u"Unknown exception")});
    }
}
} // namespace Bindings::ModuleManager
#endif
