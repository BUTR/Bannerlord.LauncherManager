export * from './BannerlordModuleManager';
export * from './FetchBannerlordVersion';
export * from './VortexExtensionManager';

import { IBannerlordModuleManager } from './BannerlordModuleManager';
import { IFetchBannerlordVersion } from './FetchBannerlordVersion';
import { IVortexExtension } from './VortexExtensionManager';

export interface IExtension extends IBannerlordModuleManager, IFetchBannerlordVersion, IVortexExtension {
    allocAliveCount(): number;
}