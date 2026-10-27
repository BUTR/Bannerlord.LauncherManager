import * as os from 'os';
import * as path from 'path';
import * as types from './types';
import { asyncCall, getExports, onUnload, setLauncherManagerImports } from './runtime';
import {
  installResultToNative,
  moduleToNative,
  moduleToWasm,
  toNativePath,
  toWasmPath,
  toWasmPaths,
  viewModelToNative,
  viewModelToWasm,
} from './paths';

interface Callbacks {
  setGameParametersAsync: (executable: string, gameParameters: string[]) => Promise<void>;
  sendNotificationAsync: (id: string, type: types.NotificationType, message: string, delayMS: number) => Promise<void>;
  sendDialogAsync: (type: types.DialogType, title: string, message: string, filters: types.FileFilter[]) => Promise<string>;
  getInstallPathAsync: () => Promise<string>;
  readFileContentAsync: (filePath: string, offset: number, length: number) => Promise<Uint8Array | null>;
  writeFileContentAsync: (filePath: string, data: Uint8Array) => Promise<void>;
  readDirectoryFileListAsync: (directoryPath: string) => Promise<string[] | null>;
  readDirectoryListAsync: (directoryPath: string) => Promise<string[] | null>;
  getAllModuleViewModelsAsync: () => Promise<types.ModuleViewModel[] | null>;
  getModuleViewModelsAsync: () => Promise<types.ModuleViewModel[] | null>;
  setModuleViewModelsAsync: (moduleViewModels: types.ModuleViewModel[]) => Promise<void>;
  getOptionsAsync: () => Promise<types.LauncherOptions>;
  getStateAsync: () => Promise<types.LauncherState>;
}

export interface LauncherManagerOptions {
  /**
   * The user's Documents folder, the game's saves are in it. Defaults to Documents in the home folder
   */
  documentsPath?: string;
}

const instances = new Map<number, Callbacks>();

// The runtime and every instance in it are gone, a NativeLauncherManager made before can't be used anymore
onUnload(() => instances.clear());

const callbacksOf = (handler: number): Callbacks => {
  const callbacks = instances.get(handler);
  if (callbacks === undefined) {
    throw new Error(`No LauncherManager with the handler ${handler}`);
  }
  return callbacks;
};

const viewModelsToWasm = (viewModels: types.ModuleViewModel[] | null): string | null =>
  viewModels === null ? null : JSON.stringify(viewModels.map(viewModelToWasm));

setLauncherManagerImports({
  setGameParameters: (handler, executable, gameParameters) =>
    callbacksOf(handler).setGameParametersAsync(toNativePath(executable), JSON.parse(gameParameters)),
  sendNotification: (handler, id, type, message, displayMs) =>
    callbacksOf(handler).sendNotificationAsync(id, type as types.NotificationType, message, displayMs),
  sendDialog: (handler, type, title, message, filters) =>
    callbacksOf(handler).sendDialogAsync(type as types.DialogType, title, message, JSON.parse(filters)),
  getInstallPath: async (handler) => toWasmPath(await callbacksOf(handler).getInstallPathAsync()),
  readFileContent: (handler, filePath, offset, length) =>
    callbacksOf(handler).readFileContentAsync(toNativePath(filePath), offset, length),
  writeFileContent: (handler, filePath, data) =>
    callbacksOf(handler).writeFileContentAsync(toNativePath(filePath), data ?? new Uint8Array(0)),
  readDirectoryFileList: async (handler, directoryPath) => {
    const files = toWasmPaths(await callbacksOf(handler).readDirectoryFileListAsync(toNativePath(directoryPath)));
    return files === null ? null : JSON.stringify(files);
  },
  readDirectoryList: async (handler, directoryPath) => {
    const directories = toWasmPaths(await callbacksOf(handler).readDirectoryListAsync(toNativePath(directoryPath)));
    return directories === null ? null : JSON.stringify(directories);
  },
  getAllModuleViewModels: async (handler) => viewModelsToWasm(await callbacksOf(handler).getAllModuleViewModelsAsync()),
  getModuleViewModels: async (handler) => viewModelsToWasm(await callbacksOf(handler).getModuleViewModelsAsync()),
  setModuleViewModels: (handler, moduleViewModels) =>
    callbacksOf(handler).setModuleViewModelsAsync((JSON.parse(moduleViewModels) as types.ModuleViewModel[]).map(viewModelToNative)),
  getOptions: async (handler) => JSON.stringify(await callbacksOf(handler).getOptionsAsync()),
  getState: async (handler) => JSON.stringify(await callbacksOf(handler).getStateAsync()),
});

export class NativeLauncherManager {
  private readonly handler: number;

  public constructor(
    setGameParametersAsync: Callbacks['setGameParametersAsync'],
    sendNotificationAsync: Callbacks['sendNotificationAsync'],
    sendDialogAsync: Callbacks['sendDialogAsync'],
    getInstallPathAsync: Callbacks['getInstallPathAsync'],
    readFileContentAsync: Callbacks['readFileContentAsync'],
    writeFileContentAsync: Callbacks['writeFileContentAsync'],
    readDirectoryFileListAsync: Callbacks['readDirectoryFileListAsync'],
    readDirectoryListAsync: Callbacks['readDirectoryListAsync'],
    getAllModuleViewModelsAsync: Callbacks['getAllModuleViewModelsAsync'],
    getModuleViewModelsAsync: Callbacks['getModuleViewModelsAsync'],
    setModuleViewModelsAsync: Callbacks['setModuleViewModelsAsync'],
    getOptionsAsync: Callbacks['getOptionsAsync'],
    getStateAsync: Callbacks['getStateAsync'],
    options?: LauncherManagerOptions,
  ) {
    const documentsPath = options?.documentsPath ?? path.join(os.homedir(), 'Documents');
    this.handler = getExports().LauncherManagerExports.CreateHandler(toWasmPath(documentsPath));
    instances.set(this.handler, {
      setGameParametersAsync,
      sendNotificationAsync,
      sendDialogAsync,
      getInstallPathAsync,
      readFileContentAsync,
      writeFileContentAsync,
      readDirectoryFileListAsync,
      readDirectoryListAsync,
      getAllModuleViewModelsAsync,
      getModuleViewModelsAsync,
      setModuleViewModelsAsync,
      getOptionsAsync,
      getStateAsync,
    });
  }

  private get exports() {
    return getExports().LauncherManagerExports;
  }

  /**
   * Releases the instance, the runtime keeps it until then
   */
  public dispose = (): void => {
    this.exports.DisposeHandler(this.handler);
    instances.delete(this.handler);
  };

  public checkForRootHarmonyAsync = (): Promise<void> => asyncCall<void>((call) => this.exports.CheckForRootHarmonyAsync(this.handler, call));

  public getGamePlatformAsync = async (): Promise<types.GamePlatform> =>
    (await asyncCall<string>((call) => this.exports.GetGamePlatformAsync(this.handler, call))) as types.GamePlatform;

  public getGameVersionAsync = (): Promise<string> => asyncCall<string>((call) => this.exports.GetGameVersionAsync(this.handler, call));

  public getModulesAsync = async (): Promise<types.ModuleInfoExtendedWithMetadata[]> =>
    (JSON.parse(await asyncCall<string>((call) => this.exports.GetModulesAsync(this.handler, call))) as types.ModuleInfoExtendedWithMetadata[]).map(moduleToNative);

  public getAllModulesAsync = async (): Promise<types.ModuleInfoExtendedWithMetadata[]> =>
    (JSON.parse(await asyncCall<string>((call) => this.exports.GetAllModulesAsync(this.handler, call))) as types.ModuleInfoExtendedWithMetadata[]).map(moduleToNative);

  public getSaveFilePathAsync = async (saveFile: string): Promise<string> => {
    const result = await asyncCall<string | null>((call) => this.exports.GetSaveFilePathAsync(this.handler, saveFile, call));
    return result === null ? (result as unknown as string) : toNativePath(result);
  };

  public getSaveFilesAsync = async (): Promise<types.SaveMetadata[]> => JSON.parse(await asyncCall<string>((call) => this.exports.GetSaveFilesAsync(this.handler, call)));

  public getSaveMetadataAsync = async (saveFile: string, data: Uint8Array): Promise<types.SaveMetadata> =>
    JSON.parse(await asyncCall<string>((call) => this.exports.GetSaveMetadataAsync(this.handler, toWasmPath(saveFile), data, call)));

  public installModule = (files: string[], moduleInfos: types.ModuleInfoExtendedWithMetadata[]): types.InstallResult =>
    installResultToNative(JSON.parse(this.exports.InstallModule(this.handler, JSON.stringify(toWasmPaths(files)), JSON.stringify(moduleInfos.map(moduleToWasm)))));

  public isObfuscatedAsync = (module: types.ModuleInfoExtendedWithMetadata): Promise<boolean> =>
    asyncCall<boolean>((call) => this.exports.IsObfuscatedAsync(this.handler, JSON.stringify(moduleToWasm(module)), call));

  public scanModAsync = async (module: types.ModuleInfoExtendedWithMetadata): Promise<types.ScanResult[]> =>
    JSON.parse(await asyncCall<string>((call) => this.exports.ScanModAsync(this.handler, JSON.stringify(moduleToWasm(module)), call)));

  public isSorting = (): boolean => this.exports.IsSorting(this.handler);

  public moduleListHandlerExportAsync = (): Promise<void> => asyncCall<void>((call) => this.exports.ModuleListHandlerExportAsync(this.handler, call));

  public moduleListHandlerExportSaveFileAsync = (saveFile: string): Promise<void> =>
    asyncCall<void>((call) => this.exports.ModuleListHandlerExportSaveFileAsync(this.handler, saveFile, call));

  public moduleListHandlerImportAsync = (): Promise<boolean> => asyncCall<boolean>((call) => this.exports.ModuleListHandlerImportAsync(this.handler, call));

  public moduleListHandlerImportSaveFileAsync = (saveFile: string): Promise<boolean> =>
    asyncCall<boolean>((call) => this.exports.ModuleListHandlerImportSaveFileAsync(this.handler, saveFile, call));

  public orderByLoadOrderAsync = async (loadOrder: types.LoadOrder): Promise<types.OrderByLoadOrderResult> => {
    const result = JSON.parse(await asyncCall<string>((call) => this.exports.OrderByLoadOrderAsync(this.handler, JSON.stringify(loadOrder), call))) as types.OrderByLoadOrderResult;
    if (result.orderedModuleViewModels) {
      result.orderedModuleViewModels = result.orderedModuleViewModels.map(viewModelToNative);
    }
    return result;
  };

  public refreshModulesAsync = (): Promise<void> => asyncCall<void>((call) => this.exports.RefreshModulesAsync(this.handler, call));

  public refreshGameParametersAsync = (): Promise<void> => asyncCall<void>((call) => this.exports.RefreshGameParametersAsync(this.handler, call));

  public setGameParameterExecutableAsync = (executable: string): Promise<void> =>
    asyncCall<void>((call) => this.exports.SetGameParameterExecutableAsync(this.handler, toWasmPath(executable), call));

  public setGameParameterLoadOrderAsync = (loadOrder: types.LoadOrder): Promise<void> =>
    asyncCall<void>((call) => this.exports.SetGameParameterLoadOrderAsync(this.handler, JSON.stringify(loadOrder), call));

  public setGameParameterSaveFileAsync = (saveName: string): Promise<void> =>
    asyncCall<void>((call) => this.exports.SetGameParameterSaveFileAsync(this.handler, saveName, call));

  public setGameParameterContinueLastSaveFileAsync = (value: boolean): Promise<void> =>
    asyncCall<void>((call) => this.exports.SetGameParameterContinueLastSaveFileAsync(this.handler, value, call));

  public setGameStore = (gameStore: types.GameStore): void => this.exports.SetGameStore(this.handler, gameStore);

  public sortAsync = (): Promise<void> => asyncCall<void>((call) => this.exports.SortAsync(this.handler, call));

  public sortHelperChangeModulePositionAsync = (moduleViewModel: types.ModuleViewModel, insertIndex: number): Promise<boolean> =>
    asyncCall<boolean>((call) => this.exports.SortHelperChangeModulePositionAsync(this.handler, JSON.stringify(viewModelToWasm(moduleViewModel)), insertIndex, call));

  public sortHelperToggleModuleSelectionAsync = async (moduleViewModel: types.ModuleViewModel): Promise<types.ModuleViewModel> =>
    viewModelToNative(JSON.parse(await asyncCall<string>((call) => this.exports.SortHelperToggleModuleSelectionAsync(this.handler, JSON.stringify(viewModelToWasm(moduleViewModel)), call))));

  public sortHelperValidateModuleAsync = async (moduleViewModel: types.ModuleViewModel): Promise<string[]> =>
    JSON.parse(await asyncCall<string>((call) => this.exports.SortHelperValidateModuleAsync(this.handler, JSON.stringify(viewModelToWasm(moduleViewModel)), call)));

  public testModule = (files: string[]): types.SupportedResult =>
    JSON.parse(this.exports.TestModule(this.handler, JSON.stringify(toWasmPaths(files))));

  public dialogTestWarningAsync = (): Promise<string> => asyncCall<string>((call) => this.exports.DialogTestWarningAsync(this.handler, call));

  public dialogTestFileOpenAsync = (): Promise<string> => asyncCall<string>((call) => this.exports.DialogTestFileOpenAsync(this.handler, call));
}
