export interface ModuleInfoExtended {
    id: string;
    name: string;
    isOfficial: boolean;
    version: ApplicationVersion;
    isSingleplayerModule: boolean;
    isMultiplayerModule: boolean;
    subModules: Array<SubModuleInfoExtended>;
    dependentModules: Array<DependentModule>;
    modulesToLoadAfterThis: Array<DependentModule>;
    incompatibleModules: Array<DependentModule>;
    url: string;
    dependentModuleMetadatas: Array<DependentModuleMetadata>;
}
export interface ModuleInfoExtendedWithPath extends ModuleInfoExtended {
    path: string;
}
export interface ModuleInfoExtendedWithMetadata extends ModuleInfoExtended {
    moduleProviderType: ModuleProviderType;
    path: string;
}
export enum ModuleProviderType {
    Default = 'Default',
    Steam = 'Steam',
}
export interface ApplicationVersion {
    applicationVersionType: ApplicationVersionType;
    major: number;
    minor: number;
    revision: number;
    changeSet: number;
}
export enum ApplicationVersionType {
    Alpha = 'Alpha',
    Beta = 'Beta',
    EarlyAccess = 'EarlyAccess',
    Release = 'Release',
    Development = 'Development',
    Invalid = 'Invalid',
}
export interface SubModuleInfoExtended {
    name: string;
    dLLName: string;
    assemblies: Array<string>;
    subModuleClassType: string;
    tags: Map<string, Array<string>>;
}
export interface DependentModule {
    id: string;
    version: ApplicationVersion;
    isOptional: boolean;
}
export interface DependentModuleMetadata {
    id: string;
    loadType: LoadType;
    isOptional: boolean;
    isIncompatible: boolean;
    version: ApplicationVersion;
    versionRange: ApplicationVersionRange;
}
export enum LoadType {
    None = 'None',
    LoadAfterThis = 'LoadAfterThis',
    LoadBeforeThis = 'LoadBeforeThis',
}
export interface ApplicationVersionRange {
    min: ApplicationVersion;
    max: ApplicationVersion;
}
export interface ModuleSorterOptions {
    skipOptionals: boolean;
    skipExternalDependencies: boolean;
}
export interface ModuleIssue {
    target: ModuleInfoExtended;
    sourceId: string;
    type: ModuleIssueType;
    reason: string;
    sourceVersion: ApplicationVersionRange;
}
export enum ModuleIssueType {
    MissingDependencies = 'MissingDependencies',
    DependencyMissingDependencies = 'DependencyMissingDependencies',
    DependencyValidationError = 'DependencyValidationError',
    VersionMismatch = 'VersionMismatch',
    Incompatible = 'Incompatible',
    DependencyConflict = 'DependencyConflict',
}

export interface IValidationManager {
    isSelected(moduleId: string): boolean,
}

export interface IEnableDisableManager {
    getSelected(moduleId: string): boolean,
    setSelected(moduleId: string, value: boolean): void,
    getDisabled(moduleId: string): boolean,
    setDisabled(moduleId: string, value: boolean): void,
}

export interface IBannerlordModuleManager {
    sort(unsorted: Array<ModuleInfoExtended>): Array<ModuleInfoExtended>;
    sortWithOptions(unsorted: Array<ModuleInfoExtended>, options: ModuleSorterOptions): Array<ModuleInfoExtended>;

    areAllDependenciesOfModulePresent(unsorted: Array<ModuleInfoExtended>, module: ModuleInfoExtended): boolean;

    getDependentModulesOf(source: Array<ModuleInfoExtended>, module: ModuleInfoExtended): Array<ModuleInfoExtended>;
    getDependentModulesOfWithOptions(source: Array<ModuleInfoExtended>, module: ModuleInfoExtended, options: ModuleSorterOptions): Array<ModuleInfoExtended>;

    validateLoadOrder(source: Array<ModuleInfoExtended>, targetModule: ModuleInfoExtended): Array<ModuleIssue>;
    validateModule(modules: Array<ModuleInfoExtended>, targetModule: ModuleInfoExtended, manager: IValidationManager): Array<ModuleIssue>;

    enableModule(modules: Array<ModuleInfoExtended>, targetModule: ModuleInfoExtended, manager: IEnableDisableManager): Array<ModuleIssue>;
    disableModule(modules: Array<ModuleInfoExtended>, targetModule: ModuleInfoExtended, manager: IEnableDisableManager): Array<ModuleIssue>;

    getModuleInfo(xml: string): ModuleInfoExtended | undefined;
    getModuleInfoWithPath(xml: string, path: string): ModuleInfoExtendedWithPath | undefined;
    getModuleInfoWithMetadata(xml: string, type: ModuleProviderType, path: string): ModuleInfoExtendedWithMetadata | undefined;
    getSubModuleInfo(xml: string): SubModuleInfoExtended | undefined;

    parseApplicationVersion(content: string): ApplicationVersion;
    compareVersions(x: ApplicationVersion, y: ApplicationVersion): number;

    getDependenciesAll(module: ModuleInfoExtended): DependentModuleMetadata[];
    getDependenciesToLoadBeforeThis(module: ModuleInfoExtended): DependentModuleMetadata[];
    getDependenciesToLoadAfterThis(module: ModuleInfoExtended): DependentModuleMetadata[];
    getDependenciesIncompatibles(module: ModuleInfoExtended): DependentModuleMetadata[];
}