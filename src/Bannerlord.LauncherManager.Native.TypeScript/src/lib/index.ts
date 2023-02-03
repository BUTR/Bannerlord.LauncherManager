import * as types from './types';

const launchermanager: types.IExtension = require('./../../launchermanager.node');

export const createLauncherManager = (): types.LauncherManager => {
    return new launchermanager.LauncherManager();
}

export const allocAliveCount = (): number => {
    return launchermanager.allocAliveCount();
}

export class BannerlordModuleManager {
    /* istanbul ignore next */
    private constructor() { }

    static sort(unsorted: types.ModuleInfoExtended[]): types.ModuleInfoExtended[] {
        return launchermanager.sort(unsorted);
    }

    static sortWithOptions(unsorted: types.ModuleInfoExtended[], options: types.ModuleSorterOptions): types.ModuleInfoExtended[] {
        return launchermanager.sortWithOptions(unsorted, options);
    }

    static areAllDependenciesOfModulePresent(unsorted: types.ModuleInfoExtended[], module: types.ModuleInfoExtended): boolean {
        return launchermanager.areAllDependenciesOfModulePresent(unsorted, module);
    }

    static getDependentModulesOf(source: types.ModuleInfoExtended[], module: types.ModuleInfoExtended): types.ModuleInfoExtended[] {
        return launchermanager.getDependentModulesOf(source, module);
    }

    static getDependentModulesOfWithOptions(source: types.ModuleInfoExtended[], module: types.ModuleInfoExtended, options: types.ModuleSorterOptions): types.ModuleInfoExtended[] {
        return launchermanager.getDependentModulesOfWithOptions(source, module, options);
    }

    static validateLoadOrder(source: types.ModuleInfoExtended[], targetModule: types.ModuleInfoExtended): types.ModuleIssue[] {
        return launchermanager.validateLoadOrder(source, targetModule);
    }

    static validateModule(modules: types.ModuleInfoExtended[], targetModule: types.ModuleInfoExtended, manager: types.IValidationManager): types.ModuleIssue[] {
        return launchermanager.validateModule(modules, targetModule, manager);
    }

    static enableModule(modules: types.ModuleInfoExtended[], targetModule: types.ModuleInfoExtended, manager: types.IEnableDisableManager): types.ModuleIssue[] {
        return launchermanager.enableModule(modules, targetModule, manager);
    }

    static disableModule(modules: types.ModuleInfoExtended[], targetModule: types.ModuleInfoExtended, manager: types.IEnableDisableManager): types.ModuleIssue[] {
        return launchermanager.disableModule(modules, targetModule, manager);
    }

    static getModuleInfo(xml: string): types.ModuleInfoExtended | undefined {
        return launchermanager.getModuleInfo(xml);
    }

    static getSubModuleInfo(xml: string): types.SubModuleInfoExtended | undefined {
        return launchermanager.getSubModuleInfo(xml);
    }

    static compareVersions(x: types.ApplicationVersion, y: types.ApplicationVersion): number {
        return launchermanager.compareVersions(x, y);
    }

    static getDependenciesAll(module: types.ModuleInfoExtended): types.DependentModuleMetadata[] {
        return launchermanager.getDependenciesAll(module);
    }
    static getDependenciesToLoadBeforeThis(module: types.ModuleInfoExtended): types.DependentModuleMetadata[] {
        return launchermanager.getDependenciesToLoadBeforeThis(module);
    }
    static getDependenciesToLoadAfterThis(module: types.ModuleInfoExtended): types.DependentModuleMetadata[] {
        return launchermanager.getDependenciesToLoadAfterThis(module);
    }
    static getDependenciesIncompatibles(module: types.ModuleInfoExtended): types.DependentModuleMetadata[] {
        return launchermanager.getDependenciesIncompatibles(module);
    }
}

export class FetchBannerlordVersion {
    /* istanbul ignore next */
    private constructor() { }

    static getChangeSet(gameFolderPath: string, libAssembly: string): number {
        return launchermanager.getChangeSet(gameFolderPath, libAssembly);
    }

    static getVersion(gameFolderPath: string, libAssembly: string): string {
        return launchermanager.getVersion(gameFolderPath, libAssembly);
    }

    static getVersionType(gameFolderPath: string, libAssembly: string): number {
        return launchermanager.getVersionType(gameFolderPath, libAssembly);
    }
}

export {
    types
}