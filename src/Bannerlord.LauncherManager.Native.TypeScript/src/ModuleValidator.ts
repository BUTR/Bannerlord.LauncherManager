import * as types from './types';
import { getModuleListAddon, modulesAt } from './moduleLists';

const release = (handle: number): void => getModuleListAddon().releaseModules(handle);

// A validator collected without dispose() still releases its list
const undisposed = new FinalizationRegistry<number>(release);

/**
 * A module list the native side parses once and keeps until dispose(), for the calls of one pass over it: validating
 * every module of a list sends the list once instead of once per module. The calls work on the list as it was when
 * the validator was created, changes to the array or its modules after that aren't seen
 */
export class ModuleValidator {
  private readonly modules: types.ModuleInfoExtended[];
  private handle: number | undefined;

  public constructor(modules: types.ModuleInfoExtended[]) {
    this.modules = [...modules];
    this.handle = getModuleListAddon().registerModules(this.modules);
    undisposed.register(this, this.handle, this);
  }

  private get list(): string {
    if (this.handle === undefined) {
      throw new Error('The ModuleValidator is disposed');
    }
    return `#${this.handle}`;
  }

  public sort(): types.ModuleInfoExtended[] {
    return modulesAt(this.modules, getModuleListAddon().sort(this.list));
  }

  public sortWithOptions(options: types.ModuleSorterOptions): types.ModuleInfoExtended[] {
    return modulesAt(this.modules, getModuleListAddon().sortWithOptions(this.list, options));
  }

  public areAllDependenciesOfModulePresent(module: types.ModuleInfoExtended): boolean {
    return getModuleListAddon().areAllDependenciesOfModulePresent(this.list, module);
  }

  public getDependentModulesOf(module: types.ModuleInfoExtended): types.ModuleInfoExtended[] {
    return modulesAt(this.modules, getModuleListAddon().getDependentModulesOf(this.list, module));
  }

  public getDependentModulesOfWithOptions(module: types.ModuleInfoExtended, options: types.ModuleSorterOptions): types.ModuleInfoExtended[] {
    return modulesAt(this.modules, getModuleListAddon().getDependentModulesOfWithOptions(this.list, module, options));
  }

  public validateLoadOrder(targetModule: types.ModuleInfoExtended): types.ModuleIssue[] {
    return getModuleListAddon().validateLoadOrder(this.list, targetModule);
  }

  public validateModule(targetModule: types.ModuleInfoExtended, manager: types.IValidationManager): types.ModuleIssue[] {
    return getModuleListAddon().validateModule(this.list, targetModule, manager);
  }

  public enableModule(targetModule: types.ModuleInfoExtended, manager: types.IEnableDisableManager): types.ModuleIssue[] {
    return getModuleListAddon().enableModule(this.list, targetModule, manager);
  }

  public disableModule(targetModule: types.ModuleInfoExtended, manager: types.IEnableDisableManager): types.ModuleIssue[] {
    return getModuleListAddon().disableModule(this.list, targetModule, manager);
  }

  /**
   * Releases the list on the native side, the validator can't be used after. Calling it again does nothing
   */
  public dispose(): void {
    if (this.handle === undefined) return;
    undisposed.unregister(this);
    release(this.handle);
    this.handle = undefined;
  }

  public [Symbol.dispose](): void {
    this.dispose();
  }
}
