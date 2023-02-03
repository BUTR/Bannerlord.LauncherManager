import { types } from "vortex-api";
import { ModuleInfoExtended } from "./BannerlordModuleManager";

export interface IVortexExtension {
  LauncherManager: new () => LauncherManager
}

export interface ILoadOrderEntry<T = any> {
  pos: number;
  enabled: boolean;
  prefix?: string;
  data?: T;
  locked?: boolean;
  external?: boolean;
}

export interface ILoadOrder {
  [modId: string]: ILoadOrderEntry;
}

export type LauncherManager = {
  constructor(): LauncherManager;

  registerCallbacks(
    getActiveProfile: () => types.IProfile,
    getProfileById: (id: string) => types.IProfile,
    getActiveGameId: () => string,
    setGameParameters: (gameId: string, executable: string, gameParameters: string[]) => void,
    getLoadOrder: () => ILoadOrder,
    setLoadOrder: (loadOrder: ILoadOrder) => void,
    translateString: (text: string, ns: string) => string,
    sendNotification: (id: string, type: types.NotificationType, message: string, delayMS: number) => void,
    getInstallPath: () => string,
    readFileContent: (filePath: string) => string | null,
    readDirectoryFileList: (directoryPath: string) => string[] | null,
    readDirectoryList: (directoryPath: string) => string[] | null,
  ): void;

  getGameVersion(): string;

  testModule(files: string[], gameId: string): types.ISupportedResult;
  installModule(files: string[], destinationPath: string): types.IInstallResult;

  isSorting(): boolean;
  sort(): string;

  getLoadOrder(): ILoadOrder;
  setLoadOrder(loadOrder: ILoadOrder): void;

  getModules(): ModuleInfoExtended[];

  refreshGameParameters(loadOrder: ILoadOrder): void;
}
