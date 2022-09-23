import * as types from './types';

const vortexextension: types.ICommon & types.IBannerlordModuleManager & types.IFetchBannerlordVersion = require('./../../vortexextension.node');
//vortexextension.init();

export const createVortexExtensionManager = (): types.VortexExtensionManager => {
    return new (vortexextension as any).VortexExtensionManager();
}

export class BannerlordModuleManager {
    private constructor() { }

    static sort(unsorted: types.ModuleInfoExtended[]): types.ModuleInfoExtended[] {
        return vortexextension.sort(unsorted);
    }

    static sortWithOptions(unsorted: types.ModuleInfoExtended[], options: types.ModuleSorterOptions): types.ModuleInfoExtended[] {
        return vortexextension.sortWithOptions(unsorted, options);
    }

    static areAllDependenciesOfModulePresent(unsorted: types.ModuleInfoExtended[], module: types.ModuleInfoExtended): boolean {
        return vortexextension.areAllDependenciesOfModulePresent(unsorted, module);
    }

    static getDependentModulesOf(source: types.ModuleInfoExtended[], module: types.ModuleInfoExtended): types.ModuleInfoExtended[] {
        return vortexextension.getDependentModulesOf(source, module);
    }

    static getDependentModulesOfWithOptions(source: types.ModuleInfoExtended[], module: types.ModuleInfoExtended, options: types.ModuleSorterOptions): types.ModuleInfoExtended[] {
        return vortexextension.getDependentModulesOfWithOptions(source, module, options);
    }

    static validateModuleDependenciesDeclarations(module: types.ModuleInfoExtended): types.ModuleIssue[] {
        return vortexextension.validateModuleDependenciesDeclarations(module);
    }

    static validateModule(modules: types.ModuleInfoExtended[], targetModule: types.ModuleInfoExtended, manager: types.IValidationManager): types.ModuleIssue[] {
        return vortexextension.validateModule(modules, targetModule, manager);
    }

    static enableModule(modules: types.ModuleInfoExtended[], targetModule: types.ModuleInfoExtended, manager: types.IEnableDisableManager): types.ModuleIssue[] {
        return vortexextension.enableModule(modules, targetModule, manager);
    }

    static disableModule(modules: types.ModuleInfoExtended[], targetModule: types.ModuleInfoExtended, manager: types.IEnableDisableManager): types.ModuleIssue[] {
        return vortexextension.disableModule(modules, targetModule, manager);
    }

    static getModuleInfo(xml: string): types.ModuleInfoExtended | undefined {
        return vortexextension.getModuleInfo(xml);
    }

    static getSubModuleInfo(xml: string): types.SubModuleInfoExtended | undefined {
        return vortexextension.getSubModuleInfo(xml);
    }

    static compareVersions(x: types.ApplicationVersion, y: types.ApplicationVersion): number {
        return vortexextension.compareVersions(x, y);
    }
}

export class FetchBannerlordVersion {
    private constructor() { }

    static getChangeSet(gameFolderPath: string, libAssembly: string): number {
        return vortexextension.getChangeSet(gameFolderPath, libAssembly);
    }

    static getVersion(gameFolderPath: string, libAssembly: string): string {
        return vortexextension.getVersion(gameFolderPath, libAssembly);
    }

    static getVersionType(gameFolderPath: string, libAssembly: string): number {
        return vortexextension.getVersionType(gameFolderPath, libAssembly);
    }
}

export {
    types
}