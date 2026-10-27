import * as types from './types';
import { getExports } from './runtime';
import { moduleListCalls, moduleListJson } from './moduleLists';
import { ModuleValidator } from './ModuleValidator';
import { compareVersions, parseApplicationVersion } from './applicationVersion';

const parseOptional = <T>(json: string | null): T | undefined => (json === null ? undefined : JSON.parse(json) ?? undefined);

export class BannerlordModuleManager {
  /* istanbul ignore next */
  private constructor() { }

  /**
   * The module list parsed once for several calls, see ModuleValidator. The calls below parse it on every call.
   * Dispose the validator when done, or `using` it
   */
  public static createValidator(modules: types.ModuleInfoExtended[]): ModuleValidator {
    return new ModuleValidator(modules);
  }

  public static sort(unsorted: types.ModuleInfoExtended[]): types.ModuleInfoExtended[] {
    return moduleListCalls.sort(moduleListJson(unsorted), unsorted);
  }

  public static sortWithOptions(unsorted: types.ModuleInfoExtended[], options: types.ModuleSorterOptions): types.ModuleInfoExtended[] {
    return moduleListCalls.sortWithOptions(moduleListJson(unsorted), unsorted, options);
  }

  public static areAllDependenciesOfModulePresent(unsorted: types.ModuleInfoExtended[], module: types.ModuleInfoExtended): boolean {
    return moduleListCalls.areAllDependenciesOfModulePresent(moduleListJson(unsorted), module);
  }

  public static getDependentModulesOf(source: types.ModuleInfoExtended[], module: types.ModuleInfoExtended): types.ModuleInfoExtended[] {
    return moduleListCalls.getDependentModulesOf(moduleListJson(source), source, module);
  }

  public static getDependentModulesOfWithOptions(source: types.ModuleInfoExtended[], module: types.ModuleInfoExtended, options: types.ModuleSorterOptions): types.ModuleInfoExtended[] {
    return moduleListCalls.getDependentModulesOfWithOptions(moduleListJson(source), source, module, options);
  }

  public static validateLoadOrder(source: types.ModuleInfoExtended[], targetModule: types.ModuleInfoExtended): types.ModuleIssue[] {
    return moduleListCalls.validateLoadOrder(moduleListJson(source), targetModule);
  }

  public static validateModule(modules: types.ModuleInfoExtended[], targetModule: types.ModuleInfoExtended, manager: types.IValidationManager): types.ModuleIssue[] {
    return moduleListCalls.validateModule(moduleListJson(modules), targetModule, manager);
  }

  public static enableModule(modules: types.ModuleInfoExtended[], targetModule: types.ModuleInfoExtended, manager: types.IEnableDisableManager): types.ModuleIssue[] {
    return moduleListCalls.enableModule(moduleListJson(modules), targetModule, manager);
  }

  public static disableModule(modules: types.ModuleInfoExtended[], targetModule: types.ModuleInfoExtended, manager: types.IEnableDisableManager): types.ModuleIssue[] {
    return moduleListCalls.disableModule(moduleListJson(modules), targetModule, manager);
  }

  public static getModuleInfo(xml: string): types.ModuleInfoExtended | undefined {
    return parseOptional(getExports().ModuleManagerExports.GetModuleInfo(xml));
  }

  public static getModuleInfoWithPath(xml: string, path: string): types.ModuleInfoExtendedWithPath | undefined {
    return parseOptional(getExports().ModuleManagerExports.GetModuleInfoWithPath(xml, path));
  }

  public static getModuleInfoWithMetadata(xml: string, type: types.ModuleProviderType, path: string): types.ModuleInfoExtendedWithMetadata | undefined {
    return parseOptional(getExports().ModuleManagerExports.GetModuleInfoWithMetadata(xml, type, path));
  }

  public static getSubModuleInfo(xml: string): types.SubModuleInfoExtended | undefined {
    return parseOptional(getExports().ModuleManagerExports.GetSubModuleInfo(xml));
  }

  // In TS, they work without the runtime, see applicationVersion.ts
  public static parseApplicationVersion(content: string): types.ApplicationVersion {
    return parseApplicationVersion(content);
  }

  public static compareVersions(x: types.ApplicationVersion, y: types.ApplicationVersion): number {
    return compareVersions(x, y);
  }

  public static getDependenciesAll(module: types.ModuleInfoExtended): types.DependentModuleMetadata[] {
    return JSON.parse(getExports().ModuleManagerExports.GetDependenciesAll(JSON.stringify(module)));
  }

  public static getDependenciesToLoadBeforeThis(module: types.ModuleInfoExtended): types.DependentModuleMetadata[] {
    return JSON.parse(getExports().ModuleManagerExports.GetDependenciesToLoadBeforeThis(JSON.stringify(module)));
  }

  public static getDependenciesToLoadAfterThis(module: types.ModuleInfoExtended): types.DependentModuleMetadata[] {
    return JSON.parse(getExports().ModuleManagerExports.GetDependenciesToLoadAfterThis(JSON.stringify(module)));
  }

  public static getDependenciesIncompatibles(module: types.ModuleInfoExtended): types.DependentModuleMetadata[] {
    return JSON.parse(getExports().ModuleManagerExports.GetDependenciesIncompatibles(JSON.stringify(module)));
  }
}
