import * as types from './types';

type Omit<T, K extends keyof T> = Pick<T, Exclude<keyof T, K>>
type LauncherManagerWithoutConstructor = Omit<types.LauncherManager, "constructor">;
export class NativeLauncherManager implements LauncherManagerWithoutConstructor {
  private manager: types.LauncherManager;

  public constructor(
    setGameParameters: (executable: string, gameParameters: string[]) => void,
    getLoadOrder: () => types.LoadOrder,
    setLoadOrder: (loadOrder: types.LoadOrder) => void,
    sendNotification: (id: string, type: types.NotificationType, message: string, delayMS: number) => void,
    sendDialog: (type: types.DialogType, title: string, message: string, filters: types.FileFilter[]) => Promise<string>,
    getInstallPath: () => string,
    readFileContent: (filePath: string, offset: number, length: number) => Uint8Array | null,
    writeFileContent: (filePath: string, data: Uint8Array) => void,
    readDirectoryFileList: (directoryPath: string) => string[] | null,
    readDirectoryList: (directoryPath: string) => string[] | null,
    getAllModuleViewModels: () => types.ModuleViewModel[] | null,
    getModuleViewModels: () => types.ModuleViewModel[] | null,
    setModuleViewModels: (moduleViewModels: types.ModuleViewModel[]) => void,
    getOptions: () => types.LauncherOptions,
    getState: () => types.LauncherState) {

    const addon: types.INativeExtension = require('./../../launchermanager.node');
    this.manager = new addon.LauncherManager(
      setGameParameters,
      getLoadOrder,
      setLoadOrder,
      sendNotification,
      sendDialog,
      getInstallPath,
      readFileContent,
      writeFileContent,
      readDirectoryFileList,
      readDirectoryList,
      getAllModuleViewModels,
      getModuleViewModels,
      setModuleViewModels,
      getOptions,
      getState,
    );
  }
  public checkForRootHarmony = (): void => {
    return this.manager.checkForRootHarmony();
  }
  public getGamePlatform = (): types.GamePlatform => {
    return this.manager.getGamePlatform();
  }
  public getGameVersion = (): string => {
    return this.manager.getGameVersion();
  }
  public getModules = (): types.ModuleInfoExtendedWithMetadata[] => {
    return this.manager.getModules();
  }
  public getAllModules = (): types.ModuleInfoExtendedWithMetadata[] => {
    return this.manager.getAllModules();
  }
  public getSaveFilePath = (saveFile: string): string => {
    return this.manager.getSaveFilePath(saveFile);
  }
  public getSaveFiles = (): types.SaveMetadata[] => {
    return this.manager.getSaveFiles();
  }
  public getSaveMetadata = (saveFile: string, data: ArrayBuffer): types.SaveMetadata => {
    return this.manager.getSaveMetadata(saveFile, data);
  }
  public installModule = (files: string[], moduleInfos: types.ModuleInfoExtendedWithMetadata[]): types.InstallResult => {
    return this.manager.installModule(files, moduleInfos);
  }
  public isSorting = (): boolean => {
    return this.manager.isSorting();
  }
  public moduleListHandlerExport = (): void => {
    return this.manager.moduleListHandlerExport();
  }
  public moduleListHandlerExportSaveFile = (saveFile: string): void => {
    return this.manager.moduleListHandlerExportSaveFile(saveFile);
  }
  public moduleListHandlerImport = (): Promise<boolean> => {
    return this.manager.moduleListHandlerImport();
  }
  public moduleListHandlerImportSaveFile = (saveFile: string): Promise<boolean> => {
    return this.manager.moduleListHandlerImportSaveFile(saveFile);
  }
  public orderByLoadOrder = (loadOrder: types.LoadOrder): types.OrderByLoadOrderResult => {
    return this.manager.orderByLoadOrder(loadOrder);
  }
  public refreshModules = (): void => {
    return this.manager.refreshModules();
  }
  public refreshGameParameters = (): void => {
    return this.manager.refreshGameParameters();
  }
  public setGameParameterExecutable(executable: string): void {
    return this.manager.setGameParameterExecutable(executable);
  }
  public setGameParameterSaveFile(saveName: string): void {
    return this.manager.setGameParameterSaveFile(saveName);
  }
  public setGameParameterContinueLastSaveFile(value: boolean): void {
    return this.manager.setGameParameterContinueLastSaveFile(value);
  }
  public setGameStore(gameStore: types.GameStore): void {
    return this.manager.setGameStore(gameStore);
  }
  public sort = (): void => {
    return this.manager.sort();
  }
  public sortHelperChangeModulePosition = (moduleViewModel: types.ModuleViewModel, insertIndex: number): boolean => {
    return this.manager.sortHelperChangeModulePosition(moduleViewModel, insertIndex);
  }
  public sortHelperToggleModuleSelection = (moduleViewModel: types.ModuleViewModel): types.ModuleViewModel => {
    return this.manager.sortHelperToggleModuleSelection(moduleViewModel);
  }
  public sortHelperValidateModule = (moduleViewModel: types.ModuleViewModel): string[] => {
    return this.manager.sortHelperValidateModule(moduleViewModel);
  }
  public testModule = (files: string[]): types.SupportedResult => {
    return this.manager.testModule(files);
  }
  public dialogTestWarning = (): Promise<string> => {
    return this.manager.dialogTestWarning();
  }
  public dialogTestFileOpen = (): Promise<string> => {
    return this.manager.dialogTestFileOpen();
  }
  public setGameParameterLoadOrder = (loadOrder: types.LoadOrder): void => {
    return this.manager.setGameParameterLoadOrder(loadOrder);
  }
}