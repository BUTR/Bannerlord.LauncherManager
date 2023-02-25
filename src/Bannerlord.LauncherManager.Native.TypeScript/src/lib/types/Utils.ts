import { ModuleInfoExtended } from "./BannerlordModuleManager";

export interface IUtils {
    isLoadOrderCorrect(modules: Array<ModuleInfoExtended>): Array<string>;
    getDependencyHint(module: ModuleInfoExtended): string;
}