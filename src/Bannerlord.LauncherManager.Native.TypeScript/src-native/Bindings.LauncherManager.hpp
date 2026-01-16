#ifndef VE_LAUNCHERMANAGER_GUARD_HPP_
#define VE_LAUNCHERMANAGER_GUARD_HPP_

#include "Bannerlord.LauncherManager.Native.h"
#include <napi.h>

using namespace Napi;
using namespace Bannerlord::LauncherManager::Native;

namespace Bindings::LauncherManager
{
class LauncherManager : public Napi::ObjectWrap<LauncherManager>
{
  public:
    FunctionReference FGetActiveProfile;
    FunctionReference FGetProfileById;
    FunctionReference FGetActiveGameId;
    FunctionReference FSetGameParameters;
    FunctionReference FSendNotification;
    FunctionReference FSendDialog;
    FunctionReference FGetInstallPath;
    FunctionReference FReadFileContent;
    FunctionReference FWriteFileContent;
    FunctionReference FReadDirectoryFileList;
    FunctionReference FReadDirectoryList;
    FunctionReference FGetAllModuleViewModels;
    FunctionReference FGetModuleViewModels;
    FunctionReference FSetModuleViewModels;
    FunctionReference FGetOptions;
    FunctionReference FGetState;

    Napi::ThreadSafeFunction TSFNGetActiveProfile;
    Napi::ThreadSafeFunction TSFNGetProfileById;
    Napi::ThreadSafeFunction TSFNGetActiveGameId;
    Napi::ThreadSafeFunction TSFNSetGameParameters;
    Napi::ThreadSafeFunction TSFNSendNotification;
    Napi::ThreadSafeFunction TSFNSendDialog;
    Napi::ThreadSafeFunction TSFNGetInstallPath;
    Napi::ThreadSafeFunction TSFNReadFileContent;
    Napi::ThreadSafeFunction TSFNWriteFileContent;
    Napi::ThreadSafeFunction TSFNReadDirectoryFileList;
    Napi::ThreadSafeFunction TSFNReadDirectoryList;
    Napi::ThreadSafeFunction TSFNGetAllModuleViewModels;
    Napi::ThreadSafeFunction TSFNGetModuleViewModels;
    Napi::ThreadSafeFunction TSFNSetModuleViewModels;
    Napi::ThreadSafeFunction TSFNGetOptions;
    Napi::ThreadSafeFunction TSFNGetState;
    Napi::ThreadSafeFunction TSFN; // Promise-handling trampoline

    std::thread::id MainThreadId;

    static Object Init(const Napi::Env env, const Object exports);

    LauncherManager(const CallbackInfo &info);
    ~LauncherManager();

    Napi::Value CheckForRootHarmonyAsync(const CallbackInfo &info);
    Napi::Value GetGamePlatformAsync(const CallbackInfo &info);
    Napi::Value GetGameVersionAsync(const CallbackInfo &info);
    Napi::Value GetModulesAsync(const CallbackInfo &info);
    Napi::Value GetAllModulesAsync(const CallbackInfo &info);
    Napi::Value GetSaveFilePathAsync(const CallbackInfo &info);
    Napi::Value GetSaveFilesAsync(const CallbackInfo &info);
    Napi::Value GetSaveMetadataAsync(const CallbackInfo &info);
    Napi::Value InstallModule(const CallbackInfo &info);
    Napi::Value IsObfuscatedAsync(const CallbackInfo &info);
    Napi::Value IsSorting(const CallbackInfo &info);
    Napi::Value ModuleListHandlerExportAsync(const CallbackInfo &info);
    Napi::Value ModuleListHandlerExportSaveFileAsync(const CallbackInfo &info);
    Napi::Value ModuleListHandlerImportAsync(const CallbackInfo &info);
    Napi::Value ModuleListHandlerImportSaveFileAsync(const CallbackInfo &info);
    Napi::Value OrderByLoadOrderAsync(const CallbackInfo &info);
    Napi::Value RefreshGameParametersAsync(const CallbackInfo &info);
    Napi::Value RefreshModulesAsync(const CallbackInfo &info);
    Napi::Value SetGameParameterExecutableAsync(const CallbackInfo &info);
    Napi::Value SetGameParameterSaveFileAsync(const CallbackInfo &info);
    Napi::Value SetGameParameterContinueLastSaveFileAsync(const CallbackInfo &info);
    void SetGameStore(const CallbackInfo &info);
    Napi::Value SortAsync(const CallbackInfo &info);
    Napi::Value SortHelperChangeModulePositionAsync(const CallbackInfo &info);
    Napi::Value SortHelperToggleModuleSelectionAsync(const CallbackInfo &info);
    Napi::Value SortHelperValidateModuleAsync(const CallbackInfo &info);
    Napi::Value TestModule(const CallbackInfo &info);
    Napi::Value DialogTestFileOpenAsync(const CallbackInfo &info);
    Napi::Value DialogTestWarningAsync(const CallbackInfo &info);
    Napi::Value SetGameParameterLoadOrderAsync(const CallbackInfo &info);

  private:
    void *_pInstance;
};
} // namespace Bindings::LauncherManager
#endif
