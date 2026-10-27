import * as path from 'path';
import type * as types from '../src/types';
import { generateLocalization, generateModules } from './data';

/**
 * What the hot paths need from a LauncherManager package, the WebAssembly one or the native one, so the same
 * workload compares the two
 */
export interface LauncherManagerApi {
  BannerlordModuleManager: {
    getModuleInfo(xml: string): types.ModuleInfoExtended | undefined;
    getModuleInfoWithPath(xml: string, path: string): types.ModuleInfoExtendedWithPath | undefined;
    getModuleInfoWithMetadata(xml: string, type: types.ModuleProviderType, path: string): types.ModuleInfoExtendedWithMetadata | undefined;
    sort(unsorted: types.ModuleInfoExtended[]): types.ModuleInfoExtended[];
    sortWithOptions(unsorted: types.ModuleInfoExtended[], options: types.ModuleSorterOptions): types.ModuleInfoExtended[];
    areAllDependenciesOfModulePresent(unsorted: types.ModuleInfoExtended[], module: types.ModuleInfoExtended): boolean;
    getDependentModulesOf(source: types.ModuleInfoExtended[], module: types.ModuleInfoExtended): types.ModuleInfoExtended[];
    getDependentModulesOfWithOptions(source: types.ModuleInfoExtended[], module: types.ModuleInfoExtended, options: types.ModuleSorterOptions): types.ModuleInfoExtended[];
    validateModule(modules: types.ModuleInfoExtended[], targetModule: types.ModuleInfoExtended, manager: types.IValidationManager): types.ModuleIssue[];
    validateLoadOrder(source: types.ModuleInfoExtended[], targetModule: types.ModuleInfoExtended): types.ModuleIssue[];
    parseApplicationVersion(content: string): types.ApplicationVersion;
    compareVersions(x: types.ApplicationVersion, y: types.ApplicationVersion): number;
    getDependenciesAll(module: types.ModuleInfoExtended): types.DependentModuleMetadata[];
    getDependenciesToLoadBeforeThis(module: types.ModuleInfoExtended): types.DependentModuleMetadata[];
    getDependenciesToLoadAfterThis(module: types.ModuleInfoExtended): types.DependentModuleMetadata[];
    getDependenciesIncompatibles(module: types.ModuleInfoExtended): types.DependentModuleMetadata[];
    // Packages without it get the calls with the list each time, see validatorOf
    createValidator?(modules: types.ModuleInfoExtended[]): ListValidator;
  };
  Utils: {
    isLoadOrderCorrect(modules: types.ModuleInfoExtended[]): string[];
    getDependencyHint(module: types.ModuleInfoExtended): string;
    renderModuleIssue(moduleIssue: types.ModuleIssue): string;
    loadLocalization(xml: string): void;
    setLanguage(language: string): void;
    localizeString(template: string, values: { [value: string]: string }): string;
  };
  NativeLauncherManager: new (...args: any[]) => {
    refreshModulesAsync(): Promise<void>;
    getModulesAsync(): Promise<types.ModuleInfoExtendedWithMetadata[]>;
    getAllModulesAsync(): Promise<types.ModuleInfoExtendedWithMetadata[]>;
    refreshGameParametersAsync(): Promise<void>;
    setGameParameterLoadOrderAsync(loadOrder: types.LoadOrder): Promise<void>;
    orderByLoadOrderAsync(loadOrder: types.LoadOrder): Promise<types.OrderByLoadOrderResult>;
    sortAsync(): Promise<void>;
    isSorting(): boolean;
    getGamePlatformAsync(): Promise<types.GamePlatform>;
    setGameStore(gameStore: types.GameStore): void;
  };
}

/**
 * The list calls of a ModuleValidator, the list given once
 */
export interface ListValidator {
  areAllDependenciesOfModulePresent(module: types.ModuleInfoExtended): boolean;
  getDependentModulesOf(module: types.ModuleInfoExtended): types.ModuleInfoExtended[];
  getDependentModulesOfWithOptions(module: types.ModuleInfoExtended, options: types.ModuleSorterOptions): types.ModuleInfoExtended[];
  validateModule(targetModule: types.ModuleInfoExtended, manager: types.IValidationManager): types.ModuleIssue[];
  validateLoadOrder(targetModule: types.ModuleInfoExtended): types.ModuleIssue[];
  dispose(): void;
}

/**
 * Runs the action with the package's validator for the list, or one passing the list to each call for a package
 * without validators
 */
const withValidator = <T>(api: LauncherManagerApi, modules: types.ModuleInfoExtended[], action: (validator: ListValidator) => T): T => {
  const bmm = api.BannerlordModuleManager;
  const validator: ListValidator = bmm.createValidator?.(modules) ?? {
    areAllDependenciesOfModulePresent: (module) => bmm.areAllDependenciesOfModulePresent(modules, module),
    getDependentModulesOf: (module) => bmm.getDependentModulesOf(modules, module),
    getDependentModulesOfWithOptions: (module, options) => bmm.getDependentModulesOfWithOptions(modules, module, options),
    validateModule: (targetModule, manager) => bmm.validateModule(modules, targetModule, manager),
    validateLoadOrder: (targetModule) => bmm.validateLoadOrder(modules, targetModule),
    dispose: () => {},
  };
  try {
    return action(validator);
  } finally {
    validator.dispose();
  }
};

export type Timings = Record<string, number>;

const GAME_ROOT = process.platform === 'win32' ? 'C:\\Games\\Mount & Blade II Bannerlord' : '/games/bannerlord';

const time = async <T>(timings: Timings, name: string, action: () => T | Promise<T>): Promise<T> => {
  const start = performance.now();
  const result = await action();
  timings[name] = (timings[name] ?? 0) + (performance.now() - start);
  return result;
};

/**
 * A game folder in memory with the generated modules, served through the LauncherManager's file system callbacks
 */
const createGameFolder = (modules: { id: string; xml: string }[]) => {
  const key = (value: string) => path.normalize(value).toLowerCase();
  const files = new Map<string, Uint8Array>();
  const directories = new Map<string, string[]>();
  const addDirectory = (parent: string, name: string) => {
    const list = directories.get(key(parent)) ?? [];
    list.push(path.join(parent, name));
    directories.set(key(parent), list);
  };

  const modulesPath = path.join(GAME_ROOT, 'Modules');
  for (const { id, xml } of modules) {
    addDirectory(modulesPath, id);
    files.set(key(path.join(modulesPath, id, 'SubModule.xml')), Buffer.from(xml, 'utf8'));
  }
  const binPath = path.join(GAME_ROOT, 'bin');
  addDirectory(binPath, 'Win64_Shipping_Client');
  files.set(key(path.join(binPath, 'Win64_Shipping_Client', 'Bannerlord.exe')), new Uint8Array(4));
  files.set(key(path.join(modulesPath, 'Native', 'Version.xml')), new Uint8Array(4));

  const filesIn = (directory: string): string[] | null => {
    const prefix = key(directory) + path.sep;
    const result = [...files.keys()].filter((x) => x.startsWith(prefix) && !x.slice(prefix.length).includes(path.sep));
    return result.length === 0 && !directories.has(key(directory)) ? null : result;
  };

  return {
    readFileContent: async (filePath: string, offset: number, length: number): Promise<Uint8Array | null> => {
      const data = files.get(key(filePath));
      if (data === undefined) return null;
      return data.subarray(offset, length === -1 ? undefined : offset + length);
    },
    readDirectoryFileList: async (directory: string) => filesIn(directory),
    readDirectoryList: async (directory: string) => directories.get(key(directory)) ?? null,
  };
};

const loadOrderOf = (modules: types.ModuleInfoExtended[], isSelected: (index: number) => boolean): types.LoadOrder =>
  Object.fromEntries(modules.map((x, index) => [x.id, { id: x.id, name: x.name, isSelected: isSelected(index), isDisabled: false, index }]));

/**
 * The calls Vortex makes often and waits on: parsing SubModule.xml, sorting, validating, localizing, and the
 * LauncherManager's module list and load order. Install, scanning, saves and dialogs are left out on purpose
 */
export const runHotPaths = async (api: LauncherManagerApi, modCount: number, timings: Timings = {}): Promise<Timings> => {
  const { BannerlordModuleManager: bmm, Utils } = api;
  const generated = generateModules(modCount);

  // Localization, the first localize loads every language
  const english = generateLocalization('English', 400);
  const german = generateLocalization('Deutsch', 400);
  await time(timings, 'loadLocalization', () => {
    Utils.loadLocalization(english.xml);
    Utils.loadLocalization(german.xml);
  });
  await time(timings, 'localizeString x800', () => {
    for (const language of ['English', 'Deutsch']) {
      Utils.setLanguage(language);
      for (const id of english.ids) {
        Utils.localizeString(`{=${id}}Fallback {VALUE}`, { VALUE: '42' });
      }
    }
    Utils.setLanguage('English');
    Utils.localizeString('{=missing}Not translated', {});
  });

  // Parsing
  const modules = await time(timings, 'getModuleInfoWithMetadata', () => generated.map(({ id, xml }, index) =>
    bmm.getModuleInfoWithMetadata(xml, (index % 3 === 0 ? 'Steam' : 'Vortex') as types.ModuleProviderType, path.join(GAME_ROOT, 'Modules', id, 'SubModule.xml'))!));
  await time(timings, 'getModuleInfo', () => {
    for (const { xml } of generated.slice(0, 50)) bmm.getModuleInfo(xml);
    bmm.getModuleInfoWithPath(generated[0].xml, GAME_ROOT);
  });
  await time(timings, 'parse/compare versions', () => {
    const versions = ['v1.2.12', 'e1.8.0.321460', 'b0.1.2', 'a1.0.0', 'v1.0.0.*', 'invalid', 'v2.1'].map((x) => bmm.parseApplicationVersion(x));
    for (const x of versions) for (const y of versions) bmm.compareVersions(x, y);
    for (const module of modules) bmm.compareVersions(module.version, versions[0]);
  });

  // Sorting
  const shuffled = [...modules].reverse();
  const sorted = await time(timings, 'sort', () => bmm.sort(shuffled));
  await time(timings, 'sortWithOptions', () => {
    bmm.sortWithOptions(shuffled, { skipOptionals: true, skipExternalDependencies: true });
    bmm.sortWithOptions(shuffled, { skipOptionals: false, skipExternalDependencies: false });
  });
  await time(timings, 'isLoadOrderCorrect', () => {
    Utils.isLoadOrderCorrect(sorted);
    Utils.isLoadOrderCorrect(shuffled);
  });

  // Validation, per module like the load order page: one validator per pass over a list, like the extension
  const selected = new Set(sorted.filter((_, index) => index % 5 !== 0).map((x) => x.id));
  const issues = await time(timings, 'validateModule', () => withValidator(api, sorted, (validator) =>
    sorted.flatMap((module) => validator.validateModule(module, { isSelected: (id) => selected.has(id) }))));
  await time(timings, 'validateLoadOrder', () => {
    withValidator(api, sorted, (validator) => {
      for (const module of sorted) issues.push(...validator.validateLoadOrder(module));
    });
    withValidator(api, shuffled, (validator) => {
      for (const module of shuffled.slice(0, 50)) issues.push(...validator.validateLoadOrder(module));
    });
  });
  await time(timings, 'renderModuleIssue', () => {
    for (const issue of issues) Utils.renderModuleIssue(issue);
  });
  await time(timings, 'dependencies', () => withValidator(api, sorted, (validator) => {
    for (const module of sorted) {
      validator.areAllDependenciesOfModulePresent(module);
      bmm.getDependenciesAll(module);
      bmm.getDependenciesToLoadBeforeThis(module);
      bmm.getDependenciesToLoadAfterThis(module);
      bmm.getDependenciesIncompatibles(module);
      Utils.getDependencyHint(module);
    }
    for (const module of sorted.slice(0, 30)) {
      validator.getDependentModulesOf(module);
      validator.getDependentModulesOfWithOptions(module, { skipOptionals: true, skipExternalDependencies: false });
    }
  }));

  // The LauncherManager over the game folder
  const game = createGameFolder(generated);
  let viewModels: types.ModuleViewModel[] = [];
  const manager = new api.NativeLauncherManager(
    async () => { },
    async () => { },
    async () => '',
    async () => GAME_ROOT,
    game.readFileContent,
    async () => { },
    game.readDirectoryFileList,
    game.readDirectoryList,
    async () => viewModels,
    async () => viewModels,
    async (value: types.ModuleViewModel[]) => { viewModels = value; },
    async () => ({ betaSorting: false }),
    async () => ({ isSingleplayer: true }),
  );
  manager.setGameStore('Steam');
  await time(timings, 'refreshModulesAsync', () => manager.refreshModulesAsync());
  const managerModules = await time(timings, 'getModulesAsync', async () => {
    await manager.getAllModulesAsync();
    return manager.getModulesAsync();
  });
  await time(timings, 'getGamePlatformAsync', () => manager.getGamePlatformAsync());
  viewModels = managerModules.map((moduleInfoExtended, index) => ({ moduleInfoExtended, isValid: true, isSelected: index % 4 !== 0, isDisabled: false, index }));

  const managerSorted = bmm.sort(managerModules) as types.ModuleInfoExtendedWithMetadata[];
  await time(timings, 'orderByLoadOrderAsync', async () => {
    await manager.orderByLoadOrderAsync(loadOrderOf(managerSorted, (index) => index % 4 !== 0));
    await manager.orderByLoadOrderAsync(loadOrderOf([...managerSorted].reverse(), () => true));
  });
  await time(timings, 'sortAsync', async () => {
    manager.isSorting();
    await manager.sortAsync();
  });
  await time(timings, 'setGameParameterLoadOrderAsync', async () => {
    await manager.setGameParameterLoadOrderAsync(loadOrderOf(managerSorted, (index) => index % 4 !== 0));
    await manager.refreshGameParametersAsync();
  });

  (manager as { dispose?: () => void }).dispose?.();
  return timings;
};
