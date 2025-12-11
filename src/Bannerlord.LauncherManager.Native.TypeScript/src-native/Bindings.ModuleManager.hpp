#ifndef VE_BLMANAGER_GUARD_HPP_
#define VE_BLMANAGER_GUARD_HPP_

#include <napi.h>
#include "Bannerlord.LauncherManager.Native.h"

using namespace Napi;
using namespace Bannerlord::LauncherManager::Native;

namespace Bindings::ModuleManager
{
    Value Sort(const CallbackInfo &info);
    Value SortWithOptions(const CallbackInfo &info);

    Value AreAllDependenciesOfModulePresent(const CallbackInfo &info);

    Value GetDependentModulesOf(const CallbackInfo &info);
    Value GetDependentModulesOfWithOptions(const CallbackInfo &info);

    Value ValidateLoadOrder(const CallbackInfo &info);

    Value ValidateModule(const CallbackInfo &info);

    void EnableModule(const CallbackInfo &info);
    void DisableModule(const CallbackInfo &info);

    Value GetModuleInfo(const CallbackInfo &info);
    Value GetModuleInfoWithPath(const CallbackInfo &info);
    Value GetModuleInfoWithMetadata(const CallbackInfo &info);
    Value GetSubModuleInfo(const CallbackInfo &info);

    Value ParseApplicationVersion(const CallbackInfo &info);
    Value CompareVersions(const CallbackInfo &info);

    Value GetDependenciesAll(const CallbackInfo &info);
    Value GetDependenciesToLoadBeforeThis(const CallbackInfo &info);
    Value GetDependenciesToLoadAfterThis(const CallbackInfo &info);
    Value GetDependenciesIncompatibles(const CallbackInfo &info);

    Object Init(const Env env, const Object exports);
}
#endif
