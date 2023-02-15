import { ModuleInfoExtendedWithPath } from "./BannerlordModuleManager";

export interface INativeExtension {
  LauncherManager: new () => LauncherManager
}

export interface LoadOrderEntry {
  id: string;
  name: string;
  isSelected: boolean;
  index: number;
}

export interface LoadOrder {
  [id: string]: LoadOrderEntry;
}

export interface ModuleViewModel {
  moduleInfoExtended: ModuleInfoExtendedWithPath;
  isValid: boolean;
  isSelected: boolean;
  isDisabled: boolean;
  index: number;
}

export interface LauncherOptions {
  language: string;
  unblockFiles: boolean;
  fixCommonIssues: boolean;
  betaSorting: boolean;
}

export interface LauncherState {
  isSingleplayer: boolean;
}

export interface SaveMetadata {
  [key: string]: string;
  Name: string;
}

export type NotificationType = 'hint' | 'info';
export type DialogType = 'warning' | 'fileOpen' | 'fileSave';

export type InstructionType = 'Copy' | 'Attribute';
export interface InstallInstruction {
  type: InstructionType;
  source?: string;
  destination?: string;
  key?: string;
  value?: any;
}
export interface InstallResult {
  instructions: InstallInstruction[];
}

export interface SupportedResult {
  supported: boolean;
  requiredFiles: string[];
}

export interface OrderByLoadOrderResult {
  result: boolean;
  issues?: string[];
  orderedModuleViewModels?: ModuleViewModel[]
}

export interface FileFilter {
  name: string;
  extensions: string[];
}

export type LauncherManager = {
  constructor(): LauncherManager;

  checkForRootHarmony(): void;
  getGameVersion(): string;
  getModules(): ModuleInfoExtendedWithPath[];
  getSaveFilePath(saveFile: string): string;
  getSaveFiles(): SaveMetadata[];
  getSaveMetadata(saveFile: string, data: ArrayBuffer): SaveMetadata;
  installModule(files: string[], destinationPath: string): InstallResult;
  isSorting(): boolean;
  loadLocalization(xml: string): void;
  moduleListHandlerExport(): void;
  moduleListHandlerExportSaveFile(saveFile: string): void;
  moduleListHandlerImport(): Promise<boolean>;
  moduleListHandlerImportSaveFile(saveFile: string): Promise<boolean>;
  orderByLoadOrder(loadOrder: LoadOrder): OrderByLoadOrderResult;
  refreshModules(): void;
  refreshGameParameters(): void;
  registerCallbacks(
    setGameParameters: (executable: string, gameParameters: string[]) => void,
    getLoadOrder: () => LoadOrder,
    setLoadOrder: (loadOrder: LoadOrder) => void,
    sendNotification: (id: string, type: NotificationType, message: string, delayMS: number) => void,
    sendDialog: (type: DialogType, title: string, message: string, filters: FileFilter[]) => Promise<string>,
    getInstallPath: () => string,
    readFileContent: (filePath: string, offset: number, length: number) => Uint8Array | null,
    writeFileContent: (filePath: string, data: Uint8Array) => void,
    readDirectoryFileList: (directoryPath: string) => string[] | null,
    readDirectoryList: (directoryPath: string) => string[] | null,
    getModuleViewModels: () => ModuleViewModel[] | null,
    setModuleViewModels: (moduleViewModels: ModuleViewModel[]) => void,
    getOptions: () => LauncherOptions,
    getState: () => LauncherState,
  ): void;
  sort(): void;
  sortHelperChangeModulePosition(moduleViewModel: ModuleViewModel, insertIndex: number): boolean;
  sortHelperToggleModuleSelection(moduleViewModel: ModuleViewModel): ModuleViewModel;
  sortHelperValidateModule(moduleViewModel: ModuleViewModel): string[];
  testModule(files: string[]): SupportedResult;

  dialogTestWarning(): Promise<string>;
  dialogTestFileOpen(): Promise<string>;
}
