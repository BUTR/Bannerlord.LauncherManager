import test from 'ava';
import fs from 'fs';
import path from 'path';
import { IProfile, NotificationType } from 'vortex-api/lib/types/api';
import { ILoadOrder, ILoadOrderEntry } from '../lib/types';
import { createVortexExtensionManager, allocAliveCount } from '../lib/';

test('Main', async (t) => {
  const gameId = 'mountandblade2bannerlord';
  const gamePath = __dirname;
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
        name: 'Harmony',
        enabled: true
      } as ILoadOrderEntry,
      'Bannerlord.UIExtenderEx': {
        pos: 2,
        name: 'UIExtenderEx',
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
      return fs.readdirSync(directoryPath, { withFileTypes: true }).filter(x => x.isFile()).map(x => path.join(directoryPath, x.name));
    }
    return null;
  };
  const readDirectoryList = (directoryPath: string): string[] | null => {
    if (fs.existsSync(directoryPath)) {
      return fs.readdirSync(directoryPath, { withFileTypes: true }).filter(x => x.isDirectory()).map(x => path.join(directoryPath, x.name));
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
    readDirectoryFileList,
    readDirectoryList);

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

  t.deepEqual(allocAliveCount(), 1); // manager is alive
  t.pass();
});