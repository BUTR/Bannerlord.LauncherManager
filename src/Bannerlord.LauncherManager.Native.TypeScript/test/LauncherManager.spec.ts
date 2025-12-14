import test from 'ava';
import { Dirent } from 'node:fs';
import { FileHandle, open, readdir, readFile, writeFile } from 'node:fs/promises';
import path from 'path';
import { LauncherOptions, LauncherState, LoadOrder, ModuleViewModel, NotificationType, DialogType } from '../src/types';
import { NativeLauncherManager, allocAliveCount, allocWithoutOwnership, types } from '../src';
import { GAME_ROOT } from './testData';

const isDebug = process.argv[2] == "Debug";

test('Main', async (t) => {
  const gamePath = GAME_ROOT;

  let moduleViewModels: ModuleViewModel[] = [];

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
    try {
      let fileHandle: FileHandle | null = null;
      try {
        fileHandle = await open(filePath, 'r');
        if (length === -1) {
          const stats = await fileHandle.stat();
          length = stats.size;
        }
        const buffer = allocWithoutOwnership(length) ?? new Uint8Array(length);
        await fileHandle.read(buffer, 0, length, offset);
        return buffer;
      } finally {
        await fileHandle?.close();
      }
    } catch (err) {
      t.fail();
    }
    return null;
  };
  const writeFileContent = async (filePath: string, data: Uint8Array): Promise<void> => {
    try {
      await writeFile(filePath, data);
    } catch (err) {
      t.fail();
    }
  };
  const readDirectoryFileList = async (directoryPath: string): Promise<string[] | null> => {
    try {
      const dirs: Dirent[] = await readdir(directoryPath, { withFileTypes: true });
      return dirs.filter((x: Dirent) => x.isFile()).map<string>((x: Dirent) => path.join(directoryPath, x.name));
    } catch (err) {
      t.fail();
    }
    return null;
  };
  const readDirectoryList = async (directoryPath: string): Promise<string[] | null> => {
    try {
      const dirs: Dirent[] = await readdir(directoryPath, { withFileTypes: true });
      return dirs.filter((x: Dirent) => x.isDirectory()).map<string>((x: Dirent) => path.join(directoryPath, x.name));
    } catch (err) {
      t.fail();
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
    moduleViewModels = _moduleViewModels;
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
  t.truthy(modules.length > 0);
  moduleViewModels = [
    {
      moduleInfoExtended: modules.find(x => x.id === "Bannerlord.Harmony")!,
      isValid: true,
      isSelected: true,
      isDisabled: false,
      index: 1,
    },
    {
      moduleInfoExtended: modules.find(x => x.id === "Bannerlord.UIExtenderEx")!,
      isValid: true,
      isSelected: true,
      isDisabled: false,
      index: 0,
    }
  ]

  const version = await manager.getGameVersionAsync();
  if (version === undefined) {
    t.fail();
    return;
  }

  //manager.setGameStore("Steam");

  await manager.sortAsync();
  let expectedModuleViewModels: ModuleViewModel[] = [
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
    },
  ];
  t.deepEqual(moduleViewModels, expectedModuleViewModels);

  if (isDebug)
    t.deepEqual(allocAliveCount(), 0); // manager is alive

  t.pass();
});
