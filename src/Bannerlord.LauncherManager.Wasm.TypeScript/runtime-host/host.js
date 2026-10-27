// The JS side of launchermanager.wasm: .NET's dotnet.js host and the two runtime modules it needs, bundled into one
// launchermanager.html by build.js. src/runtime.ts loads it, in Node as a module and in a page into a hidden frame,
// which frees the runtime when it's removed. A frame registers it as window.launcherManagerHost
import { dotnet } from 'dotnet-js/dotnet.js';
import * as nativeModule from 'dotnet-js/dotnet.native.js';
import * as runtimeModule from 'dotnet-js/dotnet.runtime.js';

// Values from the page are of its realm, the runtime checks them with instanceof against this one
const toBytes = (value) =>
  value === null || value === undefined || value instanceof Uint8Array
    ? value
    : new Uint8Array(value.buffer, value.byteOffset, value.byteLength);

export const launcherManagerHost = {
  // A function of the page, as one of this realm
  wrap: (fn) => (...args) => fn(...args),

  // An async function of the page, its promise and its bytes as this realm's
  wrapAsync: (fn) => async (...args) => {
    const result = await fn(...args);
    return ArrayBuffer.isView(result) ? toBytes(result) : result;
  },

  toBytes,

  /**
   * Starts the runtime of launchermanager.wasm's bytes and answers the assembly's exports
   */
  start: async (wasm, moduleImports, assemblyName) => {
    const runtime = await dotnet.withConfig({
      mainAssemblyName: assemblyName,
      globalizationMode: 'invariant',
      // The host doesn't look for its files, it's given them
      resources: {
        wasmNative: [{ name: 'launchermanager.wasm', virtualPath: 'launchermanager.wasm', buffer: toBytes(wasm).slice().buffer }],
        jsModuleNative: [{ name: 'dotnet.native.js', moduleExports: nativeModule }],
        jsModuleRuntime: [{ name: 'dotnet.runtime.js', moduleExports: runtimeModule }],
      },
    }).create();
    for (const [name, imports] of Object.entries(moduleImports)) {
      runtime.setModuleImports(name, imports);
    }
    // NativeAOT runs its module initializers, the exports exist after it
    await runtime.runMain(assemblyName, []);
    return await runtime.getAssemblyExports(assemblyName);
  },
};

if (typeof window !== 'undefined') {
  window.launcherManagerHost = launcherManagerHost;
}
