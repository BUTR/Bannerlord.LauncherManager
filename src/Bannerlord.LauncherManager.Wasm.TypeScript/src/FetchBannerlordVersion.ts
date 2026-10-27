import * as fs from 'fs';
import * as path from 'path';
import { getExports } from './runtime';

type VersionFiles = [libAssembly: Uint8Array | null, versionXml: Uint8Array | null];

const readIfExists = (filePath: string): Uint8Array | null => {
  try {
    return fs.readFileSync(filePath);
  } catch {
    return null;
  }
};

/**
 * The runtime can't read the disk, so the files FetchBannerlordVersion looks at are read here: the library and
 * Version.xml from bin/Win64_Shipping_Client, or from the Xbox build's folder when that one is missing
 */
const readVersionFiles = (gameFolderPath: string, libAssembly: string): VersionFiles => {
  let binPath = path.join(gameFolderPath, 'bin', 'Win64_Shipping_Client');
  if (!fs.existsSync(binPath)) {
    binPath = path.join(gameFolderPath, 'bin', 'Gaming.Desktop.x64_Shipping_Client');
  }
  return [readIfExists(path.join(binPath, libAssembly)), readIfExists(path.join(binPath, 'Version.xml'))];
};

export class FetchBannerlordVersion {
  /* istanbul ignore next */
  private constructor() { }

  public static getChangeSet(gameFolderPath: string, libAssembly: string): number {
    return getExports().FetchBannerlordVersionExports.GetChangeSet(...readVersionFiles(gameFolderPath, libAssembly), libAssembly);
  }

  public static getVersion(gameFolderPath: string, libAssembly: string): string {
    return getExports().FetchBannerlordVersionExports.GetVersion(...readVersionFiles(gameFolderPath, libAssembly), libAssembly);
  }

  public static getVersionType(gameFolderPath: string, libAssembly: string): number {
    return getExports().FetchBannerlordVersionExports.GetVersionType(...readVersionFiles(gameFolderPath, libAssembly), libAssembly);
  }
}
