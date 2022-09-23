import test from 'ava';
import fs from 'fs';
import { IProfile, NotificationType } from 'vortex-api/lib/types/api';

import { createVortexExtensionManager } from '../lib/';
import { ILoadOrder, ILoadOrderEntry } from '../lib/types';

test('sort', async (t) => {
  const gameId = 'mountandblade2bannerlord';
  const gamePath = "D:\\SteamLibrary\\steamapps\\common\\Mount & Blade II Bannerlord";
  let profile: IProfile = {
    id: '123',
    gameId: gameId,
    name: '123',
    modState: {},
    lastActivated: 0
  };
  let loadOrder: ILoadOrder = {
    'Bannerlord.Harmony': {
      pos: 1,
      enabled: false
    } as ILoadOrderEntry,
    'Bannerlord.UIExtenderEx': {
      pos: 0,
      enabled: false
    } as ILoadOrderEntry,
  };

  var manager = createVortexExtensionManager();
  const getActiveProfile = (): IProfile => {
    return profile;
  };
  const getProfileById = (_id: string): IProfile => {
    return profile;
  };
  const getActiveGameId = (): string => {
    return "";
  };
  const setGameParameters = (gameId: string, executable: string, gameParameters: string[]): void => {
    t.deepEqual(gameId, gameId);
    t.deepEqual(executable, 'bin\\Win64_Shipping_Client\\Bannerlord.exe');
    t.deepEqual(gameParameters, [
      '/singleplayer',
      '_MODULES_*Bannerlord.Harmony*Bannerlord.UIExtenderEx*_MODULES_'
    ]);
  };
  const getLoadOrder = (): ILoadOrder => {
    return loadOrder;
  };
  const setLoadOrder = (loadOrder: ILoadOrder): void => {
    let expectedLoadOrder: ILoadOrder = {
      'Bannerlord.Harmony': {
        pos: 1,
        enabled: true
      } as ILoadOrderEntry,
      'Bannerlord.UIExtenderEx': {
        pos: 2,
        enabled: true
      } as ILoadOrderEntry,
    };
    t.deepEqual(loadOrder, expectedLoadOrder)
  };
  const translateString = (text: string, _ns: string): string => {
    return text;
  };
  const sendNotification = (_id: string, _type: NotificationType, _message: string, _delayMS: number): void => {
    const tt = 0;
    t.is(tt, 0);
  };
  const getInstallPath = (): string => {
    return gamePath;
  };
  const readFileContent = (filePath: string): string | null => {
    if (fs.existsSync(filePath)) {
      return fs.readFileSync(filePath, 'utf8');
    }
    return null;
  };
  const readDirectoryFileList = (directoryPath: string): string[] | null => {
    if (fs.existsSync(directoryPath)) {
      return fs.readdirSync(directoryPath);
    }
    return null;
  };
  manager.registerCallbacks(
    getActiveProfile,
    getProfileById,
    getActiveGameId,
    setGameParameters,
    getLoadOrder,
    setLoadOrder,
    translateString,
    sendNotification,
    getInstallPath,
    readFileContent,
    readDirectoryFileList);

  const version = manager.getGameVersion();
  if (version === undefined) {
    t.fail();
    return;
  }

  const loadOrder2 = manager.getLoadOrder();
  if (loadOrder2 === undefined) {
    t.fail();
    return;
  }

  manager.sort();

  const tt = 6;
  if (tt === undefined) {
    t.fail();
    return;
  }
});