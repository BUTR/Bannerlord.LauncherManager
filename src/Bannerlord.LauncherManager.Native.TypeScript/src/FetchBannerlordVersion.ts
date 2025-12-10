import * as types from './types';

export class FetchBannerlordVersion {
  private static addon: types.IFetchBannerlordVersion;

  /* istanbul ignore next */
  private constructor() { }

  private static initialize() {
    if (FetchBannerlordVersion.addon === undefined) {
      FetchBannerlordVersion.addon = require('./../launchermanager.node');
    }
  }

  public static getChangeSet(gameFolderPath: string, libAssembly: string): number {
    FetchBannerlordVersion.initialize();
    return FetchBannerlordVersion.addon.getChangeSet(gameFolderPath, libAssembly);
  }

  public static getVersion(gameFolderPath: string, libAssembly: string): string {
    FetchBannerlordVersion.initialize();
    return FetchBannerlordVersion.addon.getVersion(gameFolderPath, libAssembly);
  }

  public static getVersionType(gameFolderPath: string, libAssembly: string): number {
    FetchBannerlordVersion.initialize();
    return FetchBannerlordVersion.addon.getVersionType(gameFolderPath, libAssembly);
  }
}