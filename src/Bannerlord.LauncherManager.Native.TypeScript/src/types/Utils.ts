import { ModuleInfoExtended, ModuleIssue } from "./BannerlordModuleManager";

export interface IUtils {
    isLoadOrderCorrect(modules: Array<ModuleInfoExtended>): Array<string>;

    getDependencyHint(module: ModuleInfoExtended): string;

    renderModuleIssue(moduleIssue: ModuleIssue): string;

    loadLocalization(xml: string): void;
    setLanguage(language: string): void;
    localizeString(template: string, values: { [value: string]: string }): string;
}