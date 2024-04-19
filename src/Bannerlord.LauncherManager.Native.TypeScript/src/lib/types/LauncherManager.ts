import { ModuleInfoExtended, ModuleInfoExtendedWithMetadata } from "./BannerlordModuleManager";

export interface INativeExtension {
  LauncherManager: new (
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
    getAllModuleViewModels: () => ModuleViewModel[] | null,
    getModuleViewModels: () => ModuleViewModel[] | null,
    setModuleViewModels: (moduleViewModels: ModuleViewModel[]) => void,
    getOptions: () => LauncherOptions,
    getState: () => LauncherState,
  ) => LauncherManager
}

export interface LoadOrderEntry {
  id: string;
  name: string;
  isSelected: boolean;
  isDisabled: boolean;
  index: number;
}

export interface LoadOrder {
  [id: string]: LoadOrderEntry;
}

export interface ModuleViewModel {
  moduleInfoExtended: ModuleInfoExtendedWithMetadata;
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

export type GameStore = 'Steam' | 'GOG' | 'Epic' | 'Xbox' | 'Unknown';
export type GamePlatform = 'Win64' | 'Xbox' | 'Unknown';

export type NotificationType = 'hint' | 'info';
export type DialogType = 'warning' | 'fileOpen' | 'fileSave';

export type InstructionType = 'Copy' | 'ModuleInfo';
export interface InstallInstruction {
  type: InstructionType;
  moduleInfo?: ModuleInfoExtended;
  source?: string;
  destination?: string;
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
  getGamePlatform(): GamePlatform;
  getGameVersion(): string;
  getModules(): ModuleInfoExtendedWithMetadata[];
  getSaveFilePath(saveFile: string): string;
  getSaveFiles(): SaveMetadata[];
  getSaveMetadata(saveFile: string, data: ArrayBuffer): SaveMetadata;
  installModule(files: string[], moduleInfos: ModuleInfoExtendedWithMetadata[]): InstallResult;
  isSorting(): boolean;
  loadLocalization(xml: string): void;
  localizeString(template: string, values: { [value: string]: string }): string;
  moduleListHandlerExport(): void;
  moduleListHandlerExportSaveFile(saveFile: string): void;
  moduleListHandlerImport(): Promise<boolean>;
  moduleListHandlerImportSaveFile(saveFile: string): Promise<boolean>;
  orderByLoadOrder(loadOrder: LoadOrder): OrderByLoadOrderResult;
  refreshModules(): void;
  refreshGameParameters(): void;
  setGameParameterExecutable(executable: string): void;
  setGameParameterSaveFile(saveName: string): void;
  setGameParameterContinueLastSaveFile(value: boolean): void;
  setGameStore(gameStore: GameStore): void;
  sort(): void;
  sortHelperChangeModulePosition(moduleViewModel: ModuleViewModel, insertIndex: number): boolean;
  sortHelperToggleModuleSelection(moduleViewModel: ModuleViewModel): ModuleViewModel;
  sortHelperValidateModule(moduleViewModel: ModuleViewModel): string[];
  testModule(files: string[]): SupportedResult;

  dialogTestWarning(): Promise<string>;
  dialogTestFileOpen(): Promise<string>;

  saveLoadOrder(loadOrder: LoadOrder): void;
  loadLoadOrder(): LoadOrder;
  setGameParameterLoadOrder(loadOrder: LoadOrder): void;
}
