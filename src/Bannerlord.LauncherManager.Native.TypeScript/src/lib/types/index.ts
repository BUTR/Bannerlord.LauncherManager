export * from './BannerlordModuleManager';
export * from './FetchBannerlordVersion';
export * from './LauncherManager';

import { IBannerlordModuleManager } from './BannerlordModuleManager';
import { IFetchBannerlordVersion } from './FetchBannerlordVersion';
import { INativeExtension } from './LauncherManager';

export interface IExtension extends IBannerlordModuleManager, IFetchBannerlordVersion, INativeExtension {
    allocAliveCount(): number;
}