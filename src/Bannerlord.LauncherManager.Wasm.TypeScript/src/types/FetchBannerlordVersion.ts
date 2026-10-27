export interface IFetchBannerlordVersion {
  getChangeSet(gameFolderPath: string, libAssembly: string): number;
  getVersion(gameFolderPath: string, libAssembly: string): string;
  getVersionType(gameFolderPath: string, libAssembly: string): number;
}
