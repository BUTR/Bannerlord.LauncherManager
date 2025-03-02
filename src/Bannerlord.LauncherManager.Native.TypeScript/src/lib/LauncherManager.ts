import * as types from './types';

type Omit<T, K extends keyof T> = Pick<T, Exclude<keyof T, K>>
type LauncherManagerWithoutConstructor = Omit<types.LauncherManager, "constructor">;
export class NativeLauncherManager implements LauncherManagerWithoutConstructor {
  private manager: types.LauncherManager;

  public constructor(
    setGameParametersAsync: (executable: string, gameParameters: string[]) => Promise<void>,
    sendNotificationAsync: (id: string, type: types.NotificationType, message: string, delayMS: number) => Promise<void>,
    sendDialogAsync: (type: types.DialogType, title: string, message: string, filters: types.FileFilter[]) => Promise<string>,
    getInstallPathAsync: () => Promise<string>,
    readFileContentAsync: (filePath: string, offset: number, length: number) => Promise<Uint8Array | null>,
    writeFileContentAsync: (filePath: string, data: Uint8Array) => Promise<void>,
    readDirectoryFileListAsync: (directoryPath: string) => Promise<string[] | null>,
    readDirectoryListAsync: (directoryPath: string) => Promise<string[] | null>,
    getAllModuleViewModelsAsync: () => Promise<types.ModuleViewModel[] | null>,
    getModuleViewModelsAsync: () => Promise<types.ModuleViewModel[] | null>,
    setModuleViewModelsAsync: (moduleViewModels: types.ModuleViewModel[]) => Promise<void>,
    getOptionsAsync: () => Promise<types.LauncherOptions>,
    getStateAsync: () => Promise<types.LauncherState>) {

    const addon: types.INativeExtension = require('./../../launchermanager.node');
    this.manager = new addon.LauncherManager(
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
    );
  }
  public checkForRootHarmonyAsync = (): Promise<void> => {
    return this.manager.checkForRootHarmonyAsync();
  }
  public getGamePlatformAsync = (): Promise<types.GamePlatform> => {
    return this.manager.getGamePlatformAsync();
  }
  public getGameVersionAsync = (): Promise<string> => {
    return this.manager.getGameVersionAsync();
  }
  public getModulesAsync = (): Promise<types.ModuleInfoExtendedWithMetadata[]> => {
    return this.manager.getModulesAsync();
  }
  public getAllModulesAsync = (): Promise<types.ModuleInfoExtendedWithMetadata[]> => {
    return this.manager.getAllModulesAsync();
  }
  public getSaveFilePathAsync = (saveFile: string): Promise<string> => {
    return this.manager.getSaveFilePathAsync(saveFile);
  }
  public getSaveFilesAsync = (): Promise<types.SaveMetadata[]> => {
    return this.manager.getSaveFilesAsync();
  }
  public getSaveMetadataAsync = (saveFile: string, data: Uint8Array): Promise<types.SaveMetadata> => {
    return this.manager.getSaveMetadataAsync(saveFile, data);
  }
  public installModule = (files: string[], moduleInfos: types.ModuleInfoExtendedWithMetadata[]): types.InstallResult => {
    return this.manager.installModule(files, moduleInfos);
  }
  public isSorting = (): boolean => {
    return this.manager.isSorting();
  }
  public moduleListHandlerExportAsync = (): Promise<void> => {
    return this.manager.moduleListHandlerExportAsync();
  }
  public moduleListHandlerExportSaveFileAsync = (saveFile: string): Promise<void> => {
    return this.manager.moduleListHandlerExportSaveFileAsync(saveFile);
  }
  public moduleListHandlerImportAsync = (): Promise<boolean> => {
    return this.manager.moduleListHandlerImportAsync();
  }
  public moduleListHandlerImportSaveFileAsync = (saveFile: string): Promise<boolean> => {
    return this.manager.moduleListHandlerImportSaveFileAsync(saveFile);
  }
  public orderByLoadOrderAsync = (loadOrder: types.LoadOrder): Promise<types.OrderByLoadOrderResult> => {
    return this.manager.orderByLoadOrderAsync(loadOrder);
  }
  public refreshModulesAsync = (): Promise<void> => {
    return this.manager.refreshModulesAsync();
  }
  public refreshGameParametersAsync = (): Promise<void> => {
    return this.manager.refreshGameParametersAsync();
  }
  public setGameParameterExecutableAsync(executable: string): Promise<void> {
    return this.manager.setGameParameterExecutableAsync(executable);
  }
  public setGameParameterSaveFileAsync(saveName: string): Promise<void> {
    return this.manager.setGameParameterSaveFileAsync(saveName);
  }
  public setGameParameterContinueLastSaveFileAsync(value: boolean): Promise<void> {
    return this.manager.setGameParameterContinueLastSaveFileAsync(value);
  }
  public setGameStore(gameStore: types.GameStore): void {
    return this.manager.setGameStore(gameStore);
  }
  public sortAsync = (): Promise<void> => {
    return this.manager.sortAsync();
  }
  public sortHelperChangeModulePositionAsync = (moduleViewModel: types.ModuleViewModel, insertIndex: number): Promise<boolean> => {
    return this.manager.sortHelperChangeModulePositionAsync(moduleViewModel, insertIndex);
  }
  public sortHelperToggleModuleSelectionAsync = (moduleViewModel: types.ModuleViewModel): Promise<types.ModuleViewModel> => {
    return this.manager.sortHelperToggleModuleSelectionAsync(moduleViewModel);
  }
  public sortHelperValidateModuleAsync = (moduleViewModel: types.ModuleViewModel): Promise<string[]> => {
    return this.manager.sortHelperValidateModuleAsync(moduleViewModel);
  }
  public testModule = (files: string[]): types.SupportedResult => {
    return this.manager.testModule(files);
  }
  public dialogTestWarningAsync = (): Promise<string> => {
    return this.manager.dialogTestWarningAsync();
  }
  public dialogTestFileOpenAsync = (): Promise<string> => {
    return this.manager.dialogTestFileOpenAsync();
  }
  public setGameParameterLoadOrderAsync = (loadOrder: types.LoadOrder): Promise<void> => {
    return this.manager.setGameParameterLoadOrderAsync(loadOrder);
  }
}