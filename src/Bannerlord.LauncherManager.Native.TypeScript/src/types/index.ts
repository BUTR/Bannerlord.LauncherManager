export * from "./BannerlordModuleManager";
export * from "./Compression";
export * from "./FetchBannerlordVersion";
export * from "./LauncherManager";
export * from "./Logger";
//export * from "./ScanFinding";
export * from "./ScanResult";
export * from "./Utils";

import { IBannerlordModuleManager } from "./BannerlordModuleManager";
import { ICompression } from "./Compression";
import { IFetchBannerlordVersion } from "./FetchBannerlordVersion";
import { INativeExtension } from "./LauncherManager";
import { IUtils } from "./Utils";

export interface IExtension
  extends
    IBannerlordModuleManager,
    ICompression,
    IFetchBannerlordVersion,
    IUtils,
    INativeExtension {
  allocAliveCount(): number;
}
