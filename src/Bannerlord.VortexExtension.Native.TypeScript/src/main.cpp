#include "Bannerlord.Common.hpp"
#include "Bannerlord.FetchVersion.hpp"
#include "Bannerlord.ModuleManager.hpp"
#include "Bannerlord.VortexExtensionManager.hpp"
#include <napi.h>

using namespace Napi;

Object InitAll(const Env env, const Object exports)
{
  Bannerlord::Common::Init(env, exports);
  Bannerlord::FetchVersion::Init(env, exports);
  Bannerlord::ModuleManager::Init(env, exports);
  Bannerlord::VortexExtension::Init(env, exports);
  return exports;
}

NODE_API_MODULE(NODE_GYP_MODULE_NAME, InitAll)