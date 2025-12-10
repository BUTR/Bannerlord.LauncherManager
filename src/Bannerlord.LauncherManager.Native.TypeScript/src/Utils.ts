import * as types from './types';

export class Utils {
  private static addon: types.IUtils;

  /* istanbul ignore next */
  private constructor() { }

  private static initialize() {
    if (Utils.addon === undefined) {
      Utils.addon = require('./../launchermanager.node');
    }
  }

  public static isLoadOrderCorrect(modules: Array<types.ModuleInfoExtended>): Array<string> {
    Utils.initialize();
    return Utils.addon.isLoadOrderCorrect(modules);
  }

  public static getDependencyHint(module: types.ModuleInfoExtended): string {
    Utils.initialize();
    return Utils.addon.getDependencyHint(module);
  }

  public static renderModuleIssue(moduleIssue: types.ModuleIssue): string {
    Utils.initialize();
    return Utils.addon.renderModuleIssue(moduleIssue);
  }

  public static loadLocalization = (xml: string): void => {
    Utils.initialize();
    return Utils.addon.loadLocalization(xml);
  }
  public static setLanguage = (language: string): void => {
    Utils.initialize();
    return Utils.addon.setLanguage(language);
  }
  public static localizeString = (template: string, values: { [value: string]: string }): string => {
    Utils.initialize();
    return Utils.addon.localizeString(template, values);
  }
}