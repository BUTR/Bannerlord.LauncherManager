import * as types from './types';
import { getExports, setModuleManagerImports } from './runtime';

/**
 * A module list argument of the exports: the list's JSON, which .NET parses on every call, or "#" and the handle of a
 * list .NET keeps, see ModuleValidator
 */
export const moduleListJson = (modules: types.ModuleInfoExtended[]): string => JSON.stringify(modules);

/**
 * The modules an export returned as their positions in the list it got, see Positions in ModuleManagerExports. These
 * are the caller's objects, not copies. An export that couldn't returns the modules' JSON
 */
const modulesAt = (modules: types.ModuleInfoExtended[], json: string): types.ModuleInfoExtended[] => {
  const result: (number | types.ModuleInfoExtended)[] = JSON.parse(json);
  if (result.length === 0 || typeof result[0] !== 'number') return result as types.ModuleInfoExtended[];
  // .NET drops the list's nulls, the positions skip them
  const present = modules.filter((x) => x != null);
  return (result as number[]).map((index) => present[index]);
};

/**
 * The calls that take a module list. `list` is the argument the exports take, `modules` the list it stands for
 */
export const moduleListCalls = {
  sort: (list: string, modules: types.ModuleInfoExtended[]): types.ModuleInfoExtended[] =>
    modulesAt(modules, getExports().ModuleManagerExports.Sort(list)),

  sortWithOptions: (list: string, modules: types.ModuleInfoExtended[], options: types.ModuleSorterOptions): types.ModuleInfoExtended[] =>
    modulesAt(modules, getExports().ModuleManagerExports.SortWithOptions(list, JSON.stringify(options))),

  areAllDependenciesOfModulePresent: (list: string, module: types.ModuleInfoExtended): boolean =>
    getExports().ModuleManagerExports.AreAllDependenciesOfModulePresent(list, JSON.stringify(module)),

  getDependentModulesOf: (list: string, modules: types.ModuleInfoExtended[], module: types.ModuleInfoExtended): types.ModuleInfoExtended[] =>
    modulesAt(modules, getExports().ModuleManagerExports.GetDependentModulesOf(list, JSON.stringify(module))),

  getDependentModulesOfWithOptions: (list: string, modules: types.ModuleInfoExtended[], module: types.ModuleInfoExtended, options: types.ModuleSorterOptions): types.ModuleInfoExtended[] =>
    modulesAt(modules, getExports().ModuleManagerExports.GetDependentModulesOfWithOptions(list, JSON.stringify(module), JSON.stringify(options))),

  validateLoadOrder: (list: string, targetModule: types.ModuleInfoExtended): types.ModuleIssue[] =>
    JSON.parse(getExports().ModuleManagerExports.ValidateLoadOrder(list, JSON.stringify(targetModule))),

  validateModule: (list: string, targetModule: types.ModuleInfoExtended, manager: types.IValidationManager): types.ModuleIssue[] =>
    withManagers(manager, undefined, () => JSON.parse(getExports().ModuleManagerExports.ValidateModule(list, JSON.stringify(targetModule)))),

  enableModule: (list: string, targetModule: types.ModuleInfoExtended, manager: types.IEnableDisableManager): types.ModuleIssue[] => {
    withManagers(undefined, manager, () => getExports().ModuleManagerExports.EnableModule(list, JSON.stringify(targetModule)));
    return [];
  },

  disableModule: (list: string, targetModule: types.ModuleInfoExtended, manager: types.IEnableDisableManager): types.ModuleIssue[] => {
    withManagers(undefined, manager, () => getExports().ModuleManagerExports.DisableModule(list, JSON.stringify(targetModule)));
    return [];
  },
};

// The managers of the call in progress, the runtime's ModuleManagerImports ask them. A manager's callback may make a
// call of its own, which has its own managers until it returns
let validationManager: types.IValidationManager | undefined;
let enableDisableManager: types.IEnableDisableManager | undefined;

const withManagers = <T>(validation: types.IValidationManager | undefined, enableDisable: types.IEnableDisableManager | undefined, call: () => T): T => {
  const previous = [validationManager, enableDisableManager] as const;
  validationManager = validation;
  enableDisableManager = enableDisable;
  try {
    return call();
  } finally {
    [validationManager, enableDisableManager] = previous;
  }
};

const managerOfTheCall = <T>(manager: T | undefined): T => {
  if (manager === undefined) {
    throw new Error('A ModuleManager callback outside of the call it belongs to');
  }
  return manager;
};

setModuleManagerImports({
  isSelected: (moduleId) => managerOfTheCall(validationManager).isSelected(moduleId),
  getSelected: (moduleId) => managerOfTheCall(enableDisableManager).getSelected(moduleId),
  setSelected: (moduleId, value) => managerOfTheCall(enableDisableManager).setSelected(moduleId, value),
  getDisabled: (moduleId) => managerOfTheCall(enableDisableManager).getDisabled(moduleId),
  setDisabled: (moduleId, value) => managerOfTheCall(enableDisableManager).setDisabled(moduleId, value),
});
