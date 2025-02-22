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

  const setGameParameters = (_executable: string, gameParameters: any): Promise<void> => {
    //t.deepEqual(executable, 'bin\\Win64_Shipping_Client\\Bannerlord.exe');
    t.deepEqual(gameParameters, [
      '/singleplayer',
      '_MODULES_*Bannerlord.Harmony*Bannerlord.UIExtenderEx*_MODULES_',
      '',
      ''
    ]);
    return Promise.resolve();
  };
  const loadLoadOrder = (): Promise<LoadOrder> => {
    return Promise.resolve(loadOrder);
  };
  const saveLoadOrder = (loadOrderOverride: LoadOrder): Promise<void> => {
    loadOrder = loadOrderOverride;
    return Promise.resolve();
  };
  const sendNotification = (_id: string, _type: NotificationType, _message: string, _delayMS: number): Promise<void> => {
    return Promise.resolve();
  };
  const sendDialog = (_type: DialogType, _title: string, _message: string, _filters: types.FileFilter[]): Promise<string> => {
    return new Promise((resolve) => setTimeout(() => {
      resolve("Testing");
    }, 100));
  };
  const getInstallPath = (): Promise<string> => {
    return Promise.resolve(gamePath);
  };
  const readFileContent = async (filePath: string, offset: number, length: number): Promise<Uint8Array | null> => {
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
  const writeFileContent = (filePath: string, data: Uint8Array): Promise<void> => {
    fs.writeFileSync(filePath, data);
    return Promise.resolve();
  };
  const readDirectoryFileList = async (directoryPath: string): Promise<string[] | null> => {
    if (fs.existsSync(directoryPath)) {
      return fs.readdirSync(directoryPath, { withFileTypes: true }).filter(x => x.isFile()).map(x => path.join(directoryPath, x.name));
    }
    return null;
  };
  const readDirectoryList = async (directoryPath: string): Promise<string[] | null> => {
    if (fs.existsSync(directoryPath)) {
      return fs.readdirSync(directoryPath, { withFileTypes: true }).filter(x => x.isDirectory()).map(x => path.join(directoryPath, x.name));
    }
    return null;
  };
  const getAllModuleViewModels = (): Promise<ModuleViewModel[] | null> => {
    return Promise.resolve(moduleViewModels);
  };
  const getModuleViewModels = (): Promise<ModuleViewModel[] | null> => {
    return Promise.resolve(moduleViewModels);
  };
  const setModuleViewModels = (_moduleViewModels: ModuleViewModel[]): Promise<void> => {
    const tt = 0;
    t.is(tt, 0);
    return Promise.resolve();
  };
  const getOptions = (): Promise<LauncherOptions> => {
    return Promise.resolve({
      language: "English",
      unblockFiles: true,
      fixCommonIssues: true,
      betaSorting: false,
    });
  };
  const getState = (): Promise<LauncherState> => {
    return Promise.resolve({ isSingleplayer: true });
  };

  var manager = new NativeLauncherManager(
    setGameParameters,
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

  await manager.dialogTestWarningAsync();
  await manager.dialogTestFileOpenAsync();


  const modules = await manager.getModulesAsync();
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

  const version = await manager.getGameVersionAsync();
  if (version === undefined) {
    t.fail();
    return;
  }

  //manager.setGameStore("Steam");

  await manager.sortAsync();
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