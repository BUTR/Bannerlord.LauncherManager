import * as types from './types';

export class BannerlordModuleManager {
    private static addon: types.IBannerlordModuleManager;

    /* istanbul ignore next */
    private constructor() { }

    private static initialize() {
        if (BannerlordModuleManager.addon === undefined) {
            BannerlordModuleManager.addon = require('./../launchermanager.node');
        }
    }

    public static sort(unsorted: types.ModuleInfoExtended[]): types.ModuleInfoExtended[] {
        BannerlordModuleManager.initialize();
        return BannerlordModuleManager.addon.sort(unsorted);
    }

    public static sortWithOptions(unsorted: types.ModuleInfoExtended[], options: types.ModuleSorterOptions): types.ModuleInfoExtended[] {
        BannerlordModuleManager.initialize();
        return BannerlordModuleManager.addon.sortWithOptions(unsorted, options);
    }

    public static areAllDependenciesOfModulePresent(unsorted: types.ModuleInfoExtended[], module: types.ModuleInfoExtended): boolean {
        BannerlordModuleManager.initialize();
        return BannerlordModuleManager.addon.areAllDependenciesOfModulePresent(unsorted, module);
    }

    public static getDependentModulesOf(source: types.ModuleInfoExtended[], module: types.ModuleInfoExtended): types.ModuleInfoExtended[] {
        BannerlordModuleManager.initialize();
        return BannerlordModuleManager.addon.getDependentModulesOf(source, module);
    }

    public static getDependentModulesOfWithOptions(source: types.ModuleInfoExtended[], module: types.ModuleInfoExtended, options: types.ModuleSorterOptions): types.ModuleInfoExtended[] {
        BannerlordModuleManager.initialize();
        return BannerlordModuleManager.addon.getDependentModulesOfWithOptions(source, module, options);
    }

    public static validateLoadOrder(source: types.ModuleInfoExtended[], targetModule: types.ModuleInfoExtended): types.ModuleIssue[] {
        BannerlordModuleManager.initialize();
        return BannerlordModuleManager.addon.validateLoadOrder(source, targetModule);
    }

    public static validateModule(modules: types.ModuleInfoExtended[], targetModule: types.ModuleInfoExtended, manager: types.IValidationManager): types.ModuleIssue[] {
        BannerlordModuleManager.initialize();
        return BannerlordModuleManager.addon.validateModule(modules, targetModule, manager);
    }

    public static enableModule(modules: types.ModuleInfoExtended[], targetModule: types.ModuleInfoExtended, manager: types.IEnableDisableManager): types.ModuleIssue[] {
        BannerlordModuleManager.initialize();
        return BannerlordModuleManager.addon.enableModule(modules, targetModule, manager);
    }

    public static disableModule(modules: types.ModuleInfoExtended[], targetModule: types.ModuleInfoExtended, manager: types.IEnableDisableManager): types.ModuleIssue[] {
        BannerlordModuleManager.initialize();
        return BannerlordModuleManager.addon.disableModule(modules, targetModule, manager);
    }

    public static getModuleInfo(xml: string): types.ModuleInfoExtended | undefined {
        BannerlordModuleManager.initialize();
        return BannerlordModuleManager.addon.getModuleInfo(xml);
    }
    public static getModuleInfoWithPath(xml: string, path: string): types.ModuleInfoExtendedWithPath | undefined {
        BannerlordModuleManager.initialize();
        return BannerlordModuleManager.addon.getModuleInfoWithPath(xml, path);
    }
    public static getModuleInfoWithMetadata(xml: string, type: types.ModuleProviderType, path: string): types.ModuleInfoExtendedWithMetadata | undefined {
        BannerlordModuleManager.initialize();
        return BannerlordModuleManager.addon.getModuleInfoWithMetadata(xml, type, path);
    }

    public static getSubModuleInfo(xml: string): types.SubModuleInfoExtended | undefined {
        BannerlordModuleManager.initialize();
        return BannerlordModuleManager.addon.getSubModuleInfo(xml);
    }

    public static parseApplicationVersion(content: string): types.ApplicationVersion {
        BannerlordModuleManager.initialize();
        return BannerlordModuleManager.addon.parseApplicationVersion(content);
    }
    public static compareVersions(x: types.ApplicationVersion, y: types.ApplicationVersion): number {
        BannerlordModuleManager.initialize();
        return BannerlordModuleManager.addon.compareVersions(x, y);
    }

    public static getDependenciesAll(module: types.ModuleInfoExtended): types.DependentModuleMetadata[] {
        BannerlordModuleManager.initialize();
        return BannerlordModuleManager.addon.getDependenciesAll(module);
    }
    public static getDependenciesToLoadBeforeThis(module: types.ModuleInfoExtended): types.DependentModuleMetadata[] {
        BannerlordModuleManager.initialize();
        return BannerlordModuleManager.addon.getDependenciesToLoadBeforeThis(module);
    }
    public static getDependenciesToLoadAfterThis(module: types.ModuleInfoExtended): types.DependentModuleMetadata[] {
        BannerlordModuleManager.initialize();
        return BannerlordModuleManager.addon.getDependenciesToLoadAfterThis(module);
    }
    public static getDependenciesIncompatibles(module: types.ModuleInfoExtended): types.DependentModuleMetadata[] {
        BannerlordModuleManager.initialize();
        return BannerlordModuleManager.addon.getDependenciesIncompatibles(module);
    }
}