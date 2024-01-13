import test from 'ava';
import fs from 'fs';
import path from 'path';
import { LauncherOptions, LauncherState, LoadOrder, ModuleViewModel, NotificationType, DialogType } from '../lib/types';
import { NativeLauncherManager, allocAliveCount, types } from '../lib';

const isDebug = process.argv[2] == "Debug";

test('Main', async (t) => {
  const gamePath = __dirname;
  let loadOrder: LoadOrder = {
    'Bannerlord.UIExtenderEx': {
      id: "Bannerlord.UIExtenderEx",
      name: "UIExtenderEx",
      isSelected: false,
      isDisabled: false,
      index: 0,
    },
    'Bannerlord.Harmony': {
      id: "Bannerlord.Harmony",
      name: "Harmony",
      isSelected: false,
      isDisabled: false,
      index: 1,
    },
  };

  const setGameParameters = (_executable: string, gameParameters: any): void => {
    //t.deepEqual(executable, 'bin\\Win64_Shipping_Client\\Bannerlord.exe');
    t.deepEqual(gameParameters, [
      '/singleplayer',
      '_MODULES_*Bannerlord.Harmony*Bannerlord.UIExtenderEx*_MODULES_',
      '',
      ''
    ]);
  };
  const loadLoadOrder = (): LoadOrder => {
    return loadOrder;
  };
  const saveLoadOrder = (loadOrderOverride: LoadOrder): void => {
    loadOrder = loadOrderOverride;
  };
  const sendNotification = (_id: string, _type: NotificationType, _message: string, _delayMS: number): void => {

  };
  const sendDialog = (_type: DialogType, _title: string, _message: string, _filters: types.FileFilter[]): Promise<string> => {
    return new Promise((resolve) => setTimeout(() => {
      resolve("Testing");
    }, 100));
  };
  const getInstallPath = (): string => {
    return gamePath;
  };
  const readFileContent = (filePath: string, offset: number, length: number): Uint8Array | null => {
    if (fs.existsSync(filePath)) {
      if (offset === 0 && length === -1) {
        return fs.readFileSync(filePath);
      } else if (offset >= 0 && length > 0) {
        const fd = fs.openSync(filePath, 'r');
        const buffer = Buffer.alloc(length);
        fs.readSync(fd, buffer, offset, length, 0);
        return buffer;
      } else {
        return null;
      }
    }
    return null;
  };
  const writeFileContent = (filePath: string, data: Uint8Array): void => {
    fs.writeFileSync(filePath, data);
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
  const getAllModuleViewModels = (): ModuleViewModel[] | null => {
    return moduleViewModels;
  };
  const getModuleViewModels = (): ModuleViewModel[] | null => {
    return moduleViewModels;
  };
  const setModuleViewModels = (_moduleViewModels: ModuleViewModel[]): void => {
    const tt = 0;
    t.is(tt, 0);
  };
  const getOptions = (): LauncherOptions => {
    return {
      language: "English",
      unblockFiles: true,
      fixCommonIssues: true,
      betaSorting: false,
    }
  };
  const getState = (): LauncherState => {
    return { isSingleplayer: true };
  };

  var manager = new NativeLauncherManager(
    setGameParameters,
    loadLoadOrder,
    saveLoadOrder,
    sendNotification,
    sendDialog,
    getInstallPath,
    readFileContent,
    writeFileContent,
    readDirectoryFileList,
    readDirectoryList,
    getAllModuleViewModels,
    getModuleViewModels,
    setModuleViewModels,
    getOptions,
    getState);

  await manager.dialogTestWarning();
  await manager.dialogTestFileOpen();


  const modules = manager.getModules();
  const moduleViewModels: ModuleViewModel[] = [
    {
      moduleInfoExtended: modules.find(x => x.id === "Bannerlord.Harmony")!,
      isValid: true,
      isSelected: true,
      isDisabled: false,
      index: 0,
    },
    {
      moduleInfoExtended: modules.find(x => x.id === "Bannerlord.UIExtenderEx")!,
      isValid: true,
      isSelected: true,
      isDisabled: false,
      index: 1,
    }
  ]

  const version = manager.getGameVersion();
  if (version === undefined) {
    t.fail();
    return;
  }

  //manager.setGameStore("Steam");

  manager.sort();
  let expectedLoadOrder: LoadOrder = {
    'Bannerlord.Harmony': {
      id: "Bannerlord.Harmony",
      name: "Harmony",
      isSelected: true,
      isDisabled: false,
      index: 0,
    },
    'Bannerlord.UIExtenderEx': {
      id: "Bannerlord.UIExtenderEx",
      name: "UIExtenderEx",
      isSelected: true,
      isDisabled: false,
      index: 1,
    },
  };
  t.deepEqual(loadOrder, expectedLoadOrder);

  if (isDebug)
    t.deepEqual(allocAliveCount(), 0); // manager is alive

  t.pass();
});