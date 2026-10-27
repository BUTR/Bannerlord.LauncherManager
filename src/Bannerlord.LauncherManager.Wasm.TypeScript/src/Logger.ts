import * as types from './types';
import { getExports } from './runtime';

/**
 * The runtime has no log file of its own. With setCallbacks() its errors go to the log callback, the level is
 * Microsoft.Extensions.Logging.LogLevel like the native module's
 */
export class NativeLogger implements types.Logger {
  private readonly log: (level: number, message: string) => void;

  public constructor(log: (level: number, message: string) => void) {
    this.log = log;
  }

  public setCallbacks(): void {
    getExports().UtilsExports.SetLogCallback(this.log);
  }

  public disposeDefaultLogger(): void { }

  public static setDefaultCallbacks = (): void => {
    getExports().UtilsExports.SetLogCallback(null);
  };
}
