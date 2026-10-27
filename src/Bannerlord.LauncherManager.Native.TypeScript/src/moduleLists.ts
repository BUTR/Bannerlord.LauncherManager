import * as types from './types';

/**
 * A module list parameter of the addon: the list, sent as its JSON, or "#" and the handle of a list the native side
 * keeps, see ModuleValidator
 */
export type ModuleListArgument = types.ModuleInfoExtended[] | string;

/**
 * Positions of modules in the list a call got, or the modules when the native side couldn't, see modulesAt
 */
export type ModulePositions = (number | types.ModuleInfoExtended)[];

/**
 * The addon's module list calls as launchermanager.node exports them
 */
export interface IModuleListAddon {
  registerModules(modules: types.ModuleInfoExtended[]): number;
  releaseModules(handle: number): void;

  sort(source: ModuleListArgument): ModulePositions;
  sortWithOptions(source: ModuleListArgument, options: types.ModuleSorterOptions): ModulePositions;
  areAllDependenciesOfModulePresent(source: ModuleListArgument, module: types.ModuleInfoExtended): boolean;
  getDependentModulesOf(source: ModuleListArgument, module: types.ModuleInfoExtended): ModulePositions;
  getDependentModulesOfWithOptions(source: ModuleListArgument, module: types.ModuleInfoExtended, options: types.ModuleSorterOptions): ModulePositions;
  validateLoadOrder(source: ModuleListArgument, targetModule: types.ModuleInfoExtended): types.ModuleIssue[];
  validateModule(source: ModuleListArgument, targetModule: types.ModuleInfoExtended, manager: types.IValidationManager): types.ModuleIssue[];
  enableModule(source: ModuleListArgument, targetModule: types.ModuleInfoExtended, manager: types.IEnableDisableManager): types.ModuleIssue[];
  disableModule(source: ModuleListArgument, targetModule: types.ModuleInfoExtended, manager: types.IEnableDisableManager): types.ModuleIssue[];
}

let addon: IModuleListAddon | undefined;

export const getModuleListAddon = (): IModuleListAddon => addon ??= require('./../launchermanager.node');

/**
 * The modules a call returned as their positions in the list it got (see ModuleLists.Positions on the native side):
 * the caller's objects, not copies. A call that couldn't returns the modules
 */
export const modulesAt = (modules: types.ModuleInfoExtended[], result: ModulePositions): types.ModuleInfoExtended[] => {
  if (result.length === 0 || typeof result[0] !== 'number') return result as types.ModuleInfoExtended[];
  // The native side drops the list's nulls, the positions skip them
  const present = modules.filter((x) => x != null);
  return (result as number[]).map((index) => present[index]);
};
