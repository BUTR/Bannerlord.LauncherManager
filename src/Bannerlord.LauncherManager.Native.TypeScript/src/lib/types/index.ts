export * from './BannerlordModuleManager';
export * from './FetchBannerlordVersion';
export * from './LauncherManager';
export * from './Utils';

import { IBannerlordModuleManager } from './BannerlordModuleManager';
import { IFetchBannerlordVersion } from './FetchBannerlordVersion';
import { INativeExtension } from './LauncherManager';
import { IUtils } from './Utils';

export interface IExtension extends IBannerlordModuleManager, IFetchBannerlordVersion, IUtils, INativeExtension {
    allocAliveCount(): number;
}