#define NODE_API_NO_EXTERNAL_BUFFERS_ALLOWED // Thanks Electron

#include "Platform.hpp"
#include <napi.h>
#include "Bindings.Common.Implementation.hpp"
#include "Bindings.FetchVersion.Implementation.hpp"
#include "Bindings.ModuleManager.Implementation.hpp"
#include "Bindings.LauncherManager.Implementation.hpp"
#include "Bindings.Utils.Implementation.hpp"

using namespace Napi;

Object InitAll(const Env env, const Object exports)
{
  Bindings::Common::Init(env, exports);
  Bindings::FetchVersion::Init(env, exports);
  Bindings::ModuleManager::Init(env, exports);
  Bindings::LauncherManager::Init(env, exports);
  Bindings::Utils::Init(env, exports);
  return exports;
}

NODE_API_MODULE(NODE_GYP_MODULE_NAME, InitAll)
