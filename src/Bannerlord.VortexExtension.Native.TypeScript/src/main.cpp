#include "Bannerlord.FetchVersion.h"
#include "Bannerlord.ModuleManager.h"
#include "Bannerlord.VortexExtensionManager.h"
#include <napi.h>

using namespace Napi;

Object InitAll(const Env env, const Object exports)
{
  Bannerlord::FetchVersion::Init(env, exports);
  Bannerlord::ModuleManager::Init(env, exports);
  Bannerlord::VortexExtension::Init(env, exports);
  return exports;
}

NODE_API_MODULE(NODE_GYP_MODULE_NAME, InitAll)