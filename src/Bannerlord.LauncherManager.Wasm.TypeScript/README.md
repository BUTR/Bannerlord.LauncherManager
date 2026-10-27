# @butr/vortexextensionwasm

Bannerlord.LauncherManager compiled to one WebAssembly file by NativeAOT-LLVM, with the API of
[`@butr/vortexextensionnative`](https://www.npmjs.com/package/@butr/vortexextensionnative): module sorting and
validation, the load order, game parameters, saves, localization. No native binaries, it runs in Node 22.12+ and in
Electron's renderer, Vortex's Content Security Policy included.

## Usage

The runtime starts asynchronously. Await `initialize()` once before the first call, everything else is the same as the
native package, synchronous calls included.

```ts
import { initialize, BannerlordModuleManager, NativeLauncherManager } from '@butr/vortexextensionwasm';

await initialize();
const sorted = BannerlordModuleManager.sort(modules);
```

The runtime is two files in `dist/runtime`: `launchermanager.wasm`, the app, and `launchermanager.html`, the JS that
hosts it. When the package is bundled, copy both next to the bundle and pass their folder in:
`initialize({ runtimePath: __dirname })`.

### Differences from the native package

- `NativeLauncherManager` takes an optional last argument, `{ documentsPath }`: the user's Documents folder, where the
  game's saves are. The native module asked Windows for it, this defaults to `~/Documents`.
- In a page (Electron's renderer), the runtime runs in a hidden frame that shows `launchermanager.html`, and `unload()`
  releases it with all its memory, `onUnload()` tells when. Everything made with it goes too, `initialize()` starts a
  new one. In Node it can't be released.
- `BannerlordModuleManager.parseApplicationVersion` and `compareVersions` work before `initialize()`.

## Building

Needs the .NET 10 SDK with the `wasm-tools` workload (`dotnet workload install wasm-tools`), which brings the
Emscripten the compiler links with. The compiler, NativeAOT-LLVM, is experimental: it comes from the `dotnet-experimental`
feed, see `../Bannerlord.LauncherManager.Wasm/Bannerlord.LauncherManager.Wasm.csproj`.

```
npm ci
npm run build   # the runtime into dist/runtime, the wrapper into dist/main
npm test
node dist/bench/bench.js <path to @butr/vortexextensionnative>   # optional, the hot paths against the native module
```
