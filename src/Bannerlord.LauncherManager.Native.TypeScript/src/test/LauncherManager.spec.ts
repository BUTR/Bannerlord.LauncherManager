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
      index: 0,
    },
    'Bannerlord.Harmony': {
      id: "Bannerlord.Harmony",
      name: "Harmony",
      isSelected: false,
      index: 1,
    },
  };

  var manager = new NativeLauncherManager();
  const setGameParameters = (executable: string, gameParameters: string[]): void => {
    t.deepEqual(executable, 'bin\\Win64_Shipping_Client\\Bannerlord.exe');
    t.deepEqual(gameParameters, [
      '/singleplayer',
      '_MODULES_*Bannerlord.Harmony*Bannerlord.UIExtenderEx*_MODULES_',
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
  const readFileContent = (filePath: string): Uint8Array | null => {
    if (fs.existsSync(filePath)) {
      return fs.readFileSync(filePath);
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
  manager.registerCallbacks(
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
    getModuleViewModels,
    setModuleViewModels,
    getOptions,
    getState
  );

  const _warn = await manager.dialogTestWarning();
  const _warn2 = await manager.dialogTestFileOpen();


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

  manager.sort();
  let expectedLoadOrder: LoadOrder = {
    'Bannerlord.Harmony': {
      id: "Bannerlord.Harmony",
      name: "Harmony",
      isSelected: true,
      index: 0,
    },
    'Bannerlord.UIExtenderEx': {
      id: "Bannerlord.UIExtenderEx",
      name: "UIExtenderEx",
      isSelected: true,
      index: 1,
    },
  };
  t.deepEqual(loadOrder, expectedLoadOrder);

  if (isDebug)
    t.deepEqual(allocAliveCount(), 1); // manager is alive

  t.pass();
});