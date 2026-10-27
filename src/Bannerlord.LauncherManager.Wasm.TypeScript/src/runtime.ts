import * as fs from 'fs';
import * as path from 'path';
import { pathToFileURL } from 'url';

import { getLocalization } from './localization';

/**
 * The [JSExport] methods of Bannerlord.LauncherManager.Wasm. Objects cross as JSON strings
 */
export interface WasmExports {
  RuntimeExports: {
    Initialize(): void;
  };
  /** Module list parameters take the JSON, or "#" and the handle of a registered list, see moduleLists.ts */
  ModuleManagerExports: {
    RegisterModules(modules: string): number;
    ReleaseModules(handle: number): void;
    Sort(source: string): string;
    SortWithOptions(source: string, options: string): string;
    AreAllDependenciesOfModulePresent(source: string, module: string): boolean;
    GetDependentModulesOf(source: string, module: string): string;
    GetDependentModulesOfWithOptions(source: string, module: string, options: string): string;
    // The managers are the call's ModuleManagerImports, see moduleLists.ts
    ValidateModule(modules: string, targetModule: string): string;
    ValidateLoadOrder(modules: string, targetModule: string): string;
    EnableModule(modules: string, targetModule: string): void;
    DisableModule(modules: string, targetModule: string): void;
    GetModuleInfo(xml: string): string | null;
    GetModuleInfoWithPath(xml: string, path: string): string | null;
    GetModuleInfoWithMetadata(xml: string, moduleProvider: string, path: string): string | null;
    GetSubModuleInfo(xml: string): string;
    ParseApplicationVersion(content: string): string;
    CompareVersions(x: string, y: string): number;
    GetDependenciesAll(module: string): string;
    GetDependenciesToLoadBeforeThis(module: string): string;
    GetDependenciesToLoadAfterThis(module: string): string;
    GetDependenciesIncompatibles(module: string): string;
  };
  UtilsExports: {
    IsLoadOrderCorrect(modules: string): string;
    GetDependencyHint(module: string): string;
    RenderModuleIssue(moduleIssue: string): string;
    LoadLocalization(xml: string): void;
    SetLanguage(language: string): void;
    LocalizeString(template: string, values: string): string;
    SetLogCallback(log: ((level: number, message: string) => void) | null): void;
  };
  FetchBannerlordVersionExports: {
    GetChangeSet(libAssembly: Uint8Array | null, versionXml: Uint8Array | null, libAssemblyName: string): number;
    GetVersion(libAssembly: Uint8Array | null, versionXml: Uint8Array | null, libAssemblyName: string): string;
    GetVersionType(libAssembly: Uint8Array | null, versionXml: Uint8Array | null, libAssemblyName: string): number;
  };
  /**
   * The async methods complete through resolve and reject instead of returning a promise, the runtime would resolve it
   * from a setTimeout(0). See asyncCall
   */
  LauncherManagerExports: {
    CreateHandler(documentsPath: string): number;
    DisposeHandler(handler: number): void;
    CheckForRootHarmonyAsync(handler: number, call: number): void;
    GetGamePlatformAsync(handler: number, call: number): void;
    GetGameVersionAsync(handler: number, call: number): void;
    GetModulesAsync(handler: number, call: number): void;
    GetAllModulesAsync(handler: number, call: number): void;
    GetSaveFilePathAsync(handler: number, saveFile: string, call: number): void;
    GetSaveFilesAsync(handler: number, call: number): void;
    GetSaveMetadataAsync(handler: number, saveFile: string, data: Uint8Array, call: number): void;
    InstallModule(handler: number, files: string, moduleInfos: string): string;
    IsObfuscatedAsync(handler: number, module: string, call: number): void;
    ScanModAsync(handler: number, module: string, call: number): void;
    IsSorting(handler: number): boolean;
    ModuleListHandlerExportAsync(handler: number, call: number): void;
    ModuleListHandlerExportSaveFileAsync(handler: number, saveFile: string, call: number): void;
    ModuleListHandlerImportAsync(handler: number, call: number): void;
    ModuleListHandlerImportSaveFileAsync(handler: number, saveFile: string, call: number): void;
    OrderByLoadOrderAsync(handler: number, loadOrder: string, call: number): void;
    RefreshModulesAsync(handler: number, call: number): void;
    RefreshGameParametersAsync(handler: number, call: number): void;
    SetGameParameterExecutableAsync(handler: number, executable: string, call: number): void;
    SetGameParameterLoadOrderAsync(handler: number, loadOrder: string, call: number): void;
    SetGameParameterSaveFileAsync(handler: number, saveFile: string, call: number): void;
    SetGameParameterContinueLastSaveFileAsync(handler: number, value: boolean, call: number): void;
    SetGameStore(handler: number, gameStore: string): void;
    SortAsync(handler: number, call: number): void;
    SortHelperChangeModulePositionAsync(handler: number, moduleViewModel: string, insertIndex: number, call: number): void;
    SortHelperToggleModuleSelectionAsync(handler: number, moduleViewModel: string, call: number): void;
    SortHelperValidateModuleAsync(handler: number, moduleViewModel: string, call: number): void;
    TestModule(handler: number, files: string): string;
    DialogTestWarningAsync(handler: number, call: number): void;
    DialogTestFileOpenAsync(handler: number, call: number): void;
  };
}

interface PendingCall {
  resolve: (value: any) => void;
  reject: (error: Error) => void;
}

/**
 * The promises of the async exports in progress, by call id. The exports settle them through the asyncresults
 * imports: resolve and reject passed as function arguments became a proxy per call the runtime tracks, and that tracking
 * broke under some GC timing, "ReleaseCSOwnedObject expected to find registration", a call failing and then the process
 * aborting
 */
const pendingCalls = new Map<number, PendingCall>();
let nextCall = 1;

const settle = (call: number): PendingCall => {
  const pending = pendingCalls.get(call);
  if (pending === undefined) {
    throw new Error(`No async call ${call} in progress`);
  }
  pendingCalls.delete(call);
  return pending;
};

const asyncResultImports = {
  resolveVoid: (call: number): void => settle(call).resolve(undefined),
  resolveString: (call: number, value: string | null): void => settle(call).resolve(value),
  resolveBoolean: (call: number, value: boolean): void => settle(call).resolve(value),
  reject: (call: number, error: string): void => settle(call).reject(new Error(error)),
};

/**
 * A promise of an async export, which takes the call's id. The error is the .NET exception's text
 */
export const asyncCall = <T>(start: (call: number) => void): Promise<T> =>
  new Promise<T>((resolve, reject) => {
    const call = nextCall++;
    pendingCalls.set(call, { resolve, reject });
    try {
      start(call);
    } catch (e) {
      pendingCalls.delete(call);
      reject(e);
    }
  });

/**
 * The JS side of the [JSImport]s, the callbacks of a LauncherManager instance by its handler id
 */
export interface LauncherManagerImports {
  setGameParameters(handler: number, executable: string, gameParameters: string): Promise<void>;
  sendNotification(handler: number, id: string, type: string, message: string, displayMs: number): Promise<void>;
  sendDialog(handler: number, type: string, title: string, message: string, filters: string): Promise<string>;
  getInstallPath(handler: number): Promise<string>;
  readFileContent(handler: number, filePath: string, offset: number, length: number): Promise<Uint8Array | null>;
  writeFileContent(handler: number, filePath: string, data: Uint8Array | null): Promise<void>;
  readDirectoryFileList(handler: number, directoryPath: string): Promise<string | null>;
  readDirectoryList(handler: number, directoryPath: string): Promise<string | null>;
  getAllModuleViewModels(handler: number): Promise<string | null>;
  getModuleViewModels(handler: number): Promise<string | null>;
  setModuleViewModels(handler: number, moduleViewModels: string): Promise<void>;
  getOptions(handler: number): Promise<string>;
  getState(handler: number): Promise<string>;
}

/**
 * The JS side of the ModuleManager [JSImport]s: the validation and enable/disable managers of the call in progress.
 * They're imports and not function arguments: a function passed per call becomes a proxy the runtime tracks, and that
 * tracking broke under some GC timing, "ReleaseCSOwnedObject expected to find registration", a call failing and then
 * the process aborting. Synchronous, the calls wait for them
 */
export interface ModuleManagerImports {
  isSelected(moduleId: string): boolean;
  getSelected(moduleId: string): boolean;
  setSelected(moduleId: string, value: boolean): void;
  getDisabled(moduleId: string): boolean;
  setDisabled(moduleId: string, value: boolean): void;
}

export interface InitializeOptions {
  /**
   * The folder with launchermanager.wasm and launchermanager.html, the JS hosting it.
   * Defaults to the package's own, set it when the package is bundled
   */
  runtimePath?: string;
}

type AssemblyExports = { Bannerlord: { LauncherManager: { Wasm: WasmExports } } };

type AnyFunction = (...args: any[]) => any;

/**
 * The module in launchermanager.html, see runtime-host/host.js
 */
interface RuntimeHost {
  wrap<T extends AnyFunction>(fn: T): T;
  wrapAsync<T extends AnyFunction>(fn: T): T;
  toBytes(value: ArrayBufferView): Uint8Array;
  start(wasm: Uint8Array, moduleImports: Record<string, object>, assemblyName: string): Promise<AssemblyExports>;
}

const ASSEMBLY = 'Bannerlord.LauncherManager.Wasm.dll';
const WASM_FILE = 'launchermanager.wasm';
const HOST_FILE = 'launchermanager.html';
const HOST_SCRIPT_START = '<script type="module">';
const HOST_SCRIPT_END = '</script>';
// The import modules .NET calls and waits for, the others are async
const SYNC_IMPORTS = new Set(['modulemanager', 'asyncresults']);

let loading: Promise<void> | undefined;
let wasmExports: WasmExports | undefined;
let launcherManagerImports: LauncherManagerImports | undefined;
let moduleManagerImports: ModuleManagerImports | undefined;
// The frame the runtime runs in, in a page. Node runs it here and can't release it
let frame: { remove(): void } | undefined;
// A load that unload() overtook doesn't publish its exports
let generation = 0;
const unloadListeners = new Set<() => void>();

/**
 * Counts the unloads: what .NET kept for a generation, like a ModuleValidator's list, is gone in the next
 */
export const getGeneration = (): number => generation;

/**
 * Starts the .NET runtime. Everything else needs it, await it once before the first call
 */
export const initialize = (options?: InitializeOptions): Promise<void> => {
  loading ??= load(options).catch((e) => {
    loading = undefined;
    throw e;
  });
  return loading;
};

export const isInitialized = (): boolean => wasmExports !== undefined;

export const getExports = (): WasmExports => {
  if (wasmExports === undefined) {
    throw new Error('The LauncherManager runtime is not initialized, await initialize() before the first call');
  }
  return wasmExports;
};

/**
 * Releases the runtime and all its memory, initialize() starts a new one. Everything made with the old one is gone:
 * LauncherManager instances, loaded localization, the log callback.
 * Only in a page, where the runtime runs in a frame. In Node it can't be released, this does nothing and answers false
 */
export const unload = (): boolean => {
  if (frame === undefined) {
    return false;
  }
  generation++;
  frame.remove();
  frame = undefined;
  wasmExports = undefined;
  loading = undefined;
  // The runtime that would have settled them is gone
  for (const [call, pending] of pendingCalls) {
    pendingCalls.delete(call);
    pending.reject(new Error('The LauncherManager runtime was unloaded during the call'));
  }
  for (const listener of unloadListeners) {
    listener();
  }
  return true;
};

/**
 * Called when unload() released the runtime
 * @returns Removes the listener
 */
export const onUnload = (listener: () => void): (() => void) => {
  unloadListeners.add(listener);
  return () => unloadListeners.delete(listener);
};

/**
 * The LauncherManager module registers its imports, the runtime needs them before the first instance
 */
export const setLauncherManagerImports = (imports: LauncherManagerImports): void => {
  launcherManagerImports = imports;
};

/**
 * The ModuleManager module registers its imports, the runtime needs them before the first validation
 */
export const setModuleManagerImports = (imports: ModuleManagerImports): void => {
  moduleManagerImports = imports;
};

const load = async (options?: InitializeOptions): Promise<void> => {
  const started = generation;
  const runtimePath = options?.runtimePath ?? path.join(__dirname, '..', 'runtime');
  // Read here and handed over, a frame can't fetch file:// URLs
  const wasm = await fs.promises.readFile(path.join(runtimePath, WASM_FILE));
  const moduleImports: Record<string, Record<string, AnyFunction>> = { asyncresults: { ...asyncResultImports } };
  if (launcherManagerImports !== undefined) {
    moduleImports.launchermanager = { ...launcherManagerImports };
  }
  if (moduleManagerImports !== undefined) {
    moduleImports.modulemanager = { ...moduleManagerImports };
  }

  const page = (globalThis as { document?: Document }).document;
  const assemblyExports = page?.body
    ? await startInFrame(page, runtimePath, wasm, moduleImports)
    : await startHere(runtimePath, wasm, moduleImports);
  if (started !== generation) {
    throw new Error('The LauncherManager runtime was unloaded while it started');
  }
  const exports = assemblyExports.Bannerlord.LauncherManager.Wasm;
  // Await continuations run in microtasks, not the thread pool's setTimeout(0)
  exports.RuntimeExports.Initialize();
  // Its own texts, like issues and hints, in the localization the package was given
  const localization = getLocalization();
  for (const xml of localization.xmls) {
    exports.UtilsExports.LoadLocalization(xml);
  }
  exports.UtilsExports.SetLanguage(localization.language);
  wasmExports = exports;
};

// The page's inline module script, the build escaped any "</script" in it
const hostScriptOf = (html: string): string => {
  const start = html.indexOf(HOST_SCRIPT_START);
  const end = html.lastIndexOf(HOST_SCRIPT_END);
  if (start < 0 || end < start) {
    throw new Error(`${HOST_FILE} has no module script`);
  }
  return html.slice(start + HOST_SCRIPT_START.length, end);
};

// A real import(), TypeScript would turn one into a require() for CommonJS. Only made in Node: it's an eval, a page's
// CSP forbids it
const importModule = (specifier: string): Promise<unknown> =>
  (new Function('specifier', 'return import(specifier)') as (specifier: string) => Promise<unknown>)(specifier);

// In Node, the runtime in this realm: the page's script imported as a data: module. The glue resolves URLs against
// its own, a data: URL can't be a base, so it gets the page's
const startHere = async (runtimePath: string, wasm: Uint8Array, moduleImports: Record<string, object>): Promise<AssemblyExports> => {
  const hostPath = path.join(runtimePath, HOST_FILE);
  const html = await fs.promises.readFile(hostPath, 'utf8');
  const script = `import.meta.url = ${JSON.stringify(pathToFileURL(hostPath).href)};\n${hostScriptOf(html)}`;
  const source = Buffer.from(script, 'utf8').toString('base64');
  const { launcherManagerHost } = await importModule(`data:text/javascript;base64,${source}`) as { launcherManagerHost: RuntimeHost };
  return await launcherManagerHost.start(wasm, moduleImports, ASSEMBLY);
};

/**
 * In a page, the runtime in a hidden frame of it. Removing the frame frees the runtime, a realm's modules and
 * WebAssembly memory can't be released any other way. The frame shows launchermanager.html, a file of the page's
 * origin, so calls are synchronous. It has to be a document of its own: one that inherits the page's CSP, like an empty
 * frame's, can't compile WebAssembly. Its module script registers window.launcherManagerHost
 */
const startInFrame = async (page: Document, runtimePath: string, wasm: Uint8Array, moduleImports: Record<string, Record<string, AnyFunction>>): Promise<AssemblyExports> => {
  const element = page.createElement('iframe');
  element.style.display = 'none';
  element.src = pathToFileURL(path.join(runtimePath, HOST_FILE)).href;
  const loaded = new Promise<void>((resolve, reject) => {
    element.addEventListener('load', () => resolve(), { once: true });
    element.addEventListener('error', () => reject(new Error(`Failed to load ${element.src}`)), { once: true });
  });
  page.body.appendChild(element);
  frame = element;
  try {
    // A document's load waits for its module scripts
    await loaded;
    const host = (element.contentWindow as { launcherManagerHost?: RuntimeHost } | null)?.launcherManagerHost;
    if (host === undefined) {
      throw new Error(`The LauncherManager runtime's frame didn't start, is ${HOST_FILE} in ${runtimePath}?`);
    }
    // The imports are functions of this realm. The LauncherManager's are async, the frame needs its own promises
    const frameImports = Object.fromEntries(Object.entries(moduleImports).map(([name, imports]) =>
      [name, Object.fromEntries(Object.entries(imports).map(([key, fn]) => [key, SYNC_IMPORTS.has(name) ? host.wrap(fn) : host.wrapAsync(fn)]))]));
    return fromFrame(await host.start(wasm, frameImports, ASSEMBLY), host) as AssemblyExports;
  } catch (e) {
    element.remove();
    if (frame === element) {
      frame = undefined;
    }
    throw toLocalError(e);
  }
};

// The frame's errors aren't instances of this realm's Error
const toLocalError = (error: unknown): Error => {
  if (error instanceof Error) {
    return error;
  }
  const source = error as { message?: unknown; stack?: unknown } | null;
  const local = new Error(String(source?.message ?? error));
  if (typeof source?.stack === 'string') {
    local.stack = source.stack;
  }
  return local;
};

/**
 * The frame's exports as functions of this realm: functions and byte arrays passed to them become the frame's,
 * their errors this realm's
 */
const fromFrame = (value: unknown, host: RuntimeHost): unknown => {
  if (typeof value === 'function') {
    const fn = value as AnyFunction;
    return (...args: unknown[]) => {
      const frameArgs = args.map((arg) =>
        typeof arg === 'function' ? host.wrap(arg as AnyFunction) : ArrayBuffer.isView(arg) ? host.toBytes(arg) : arg);
      try {
        return fn(...frameArgs);
      } catch (e) {
        throw toLocalError(e);
      }
    };
  }
  if (value !== null && typeof value === 'object') {
    return Object.fromEntries(Object.entries(value).map(([key, member]) => [key, fromFrame(member, host)]));
  }
  return value;
};
