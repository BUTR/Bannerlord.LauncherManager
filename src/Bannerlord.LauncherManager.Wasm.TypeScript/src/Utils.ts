import * as types from './types';
import { getExports, isInitialized } from './runtime';
import { localizeString, readLocalization, setLanguage } from './localization';
import { moduleListJson } from './moduleLists';

export class Utils {
  /* istanbul ignore next */
  private constructor() { }

  public static isLoadOrderCorrect(modules: Array<types.ModuleInfoExtended>): Array<string> {
    return JSON.parse(getExports().UtilsExports.IsLoadOrderCorrect(moduleListJson(modules)));
  }

  public static getDependencyHint(module: types.ModuleInfoExtended): string {
    return getExports().UtilsExports.GetDependencyHint(JSON.stringify(module));
  }

  public static renderModuleIssue(moduleIssue: types.ModuleIssue): string {
    return getExports().UtilsExports.RenderModuleIssue(JSON.stringify(moduleIssue));
  }

  /**
   * Works without the runtime, the runtimes it starts get the XML too
   */
  public static loadLocalization = (xml: string): void => {
    const keep = readLocalization(xml);
    if (isInitialized()) {
      getExports().UtilsExports.LoadLocalization(xml);
    }
    keep();
  };

  /**
   * Works without the runtime, the runtimes it starts get the language too
   */
  public static setLanguage = (language: string): void => {
    if (isInitialized()) {
      getExports().UtilsExports.SetLanguage(language);
    }
    setLanguage(language);
  };

  /**
   * Works without the runtime, see localization.ts
   */
  public static localizeString = (template: string, values: { [value: string]: string }): string => {
    return localizeString(template, values);
  };
}
