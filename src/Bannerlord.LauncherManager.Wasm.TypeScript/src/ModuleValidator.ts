import * as types from './types';
import { getExports, getGeneration } from './runtime';
import { moduleListCalls } from './moduleLists';

interface Registration {
  handle: number;
  generation: number;
}

const release = ({ handle, generation }: Registration): void => {
  // A runtime unloaded since took the list with it
  if (generation === getGeneration()) {
    getExports().ModuleManagerExports.ReleaseModules(handle);
  }
};

// A validator collected without dispose() still releases its list
const undisposed = new FinalizationRegistry<Registration>(release);

/**
 * A module list .NET parses once and keeps until dispose(), for the calls of one pass over it: validating every
 * module of a list sends the list once instead of once per module. The calls work on the list as it was when the
 * validator was created, changes to the array or its modules after that aren't seen
 */
export class ModuleValidator {
  private readonly modules: types.ModuleInfoExtended[];
  private registration: Registration | undefined;

  public constructor(modules: types.ModuleInfoExtended[]) {
    this.modules = [...modules];
    const handle = getExports().ModuleManagerExports.RegisterModules(JSON.stringify(this.modules));
    this.registration = { handle, generation: getGeneration() };
    undisposed.register(this, this.registration, this);
  }

  private get list(): string {
    if (this.registration === undefined) {
      throw new Error('The ModuleValidator is disposed');
    }
    if (this.registration.generation !== getGeneration()) {
      throw new Error('The LauncherManager runtime was unloaded since the ModuleValidator was created, create another');
    }
    return `#${this.registration.handle}`;
  }

  public sort(): types.ModuleInfoExtended[] {
    return moduleListCalls.sort(this.list, this.modules);
  }

  public sortWithOptions(options: types.ModuleSorterOptions): types.ModuleInfoExtended[] {
    return moduleListCalls.sortWithOptions(this.list, this.modules, options);
  }

  public areAllDependenciesOfModulePresent(module: types.ModuleInfoExtended): boolean {
    return moduleListCalls.areAllDependenciesOfModulePresent(this.list, module);
  }

  public getDependentModulesOf(module: types.ModuleInfoExtended): types.ModuleInfoExtended[] {
    return moduleListCalls.getDependentModulesOf(this.list, this.modules, module);
  }

  public getDependentModulesOfWithOptions(module: types.ModuleInfoExtended, options: types.ModuleSorterOptions): types.ModuleInfoExtended[] {
    return moduleListCalls.getDependentModulesOfWithOptions(this.list, this.modules, module, options);
  }

  public validateLoadOrder(targetModule: types.ModuleInfoExtended): types.ModuleIssue[] {
    return moduleListCalls.validateLoadOrder(this.list, targetModule);
  }

  public validateModule(targetModule: types.ModuleInfoExtended, manager: types.IValidationManager): types.ModuleIssue[] {
    return moduleListCalls.validateModule(this.list, targetModule, manager);
  }

  public enableModule(targetModule: types.ModuleInfoExtended, manager: types.IEnableDisableManager): types.ModuleIssue[] {
    return moduleListCalls.enableModule(this.list, targetModule, manager);
  }

  public disableModule(targetModule: types.ModuleInfoExtended, manager: types.IEnableDisableManager): types.ModuleIssue[] {
    return moduleListCalls.disableModule(this.list, targetModule, manager);
  }

  /**
   * Releases the list in .NET, the validator can't be used after. Calling it again does nothing
   */
  public dispose(): void {
    if (this.registration === undefined) return;
    undisposed.unregister(this);
    release(this.registration);
    this.registration = undefined;
  }

  public [Symbol.dispose](): void {
    this.dispose();
  }
}
