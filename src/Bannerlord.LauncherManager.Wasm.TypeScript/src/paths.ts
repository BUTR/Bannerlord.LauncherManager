import * as types from './types';

/**
 * .NET in WebAssembly has Unix paths: it only splits on '/' and joins with '/'. On Windows the paths going into the
 * LauncherManager get '/' and the ones coming out get '\' back, so they look like the native module's
 */
const isWindows = process.platform === 'win32';

export const toWasmPath = (value: string): string => (isWindows ? value.replace(/\\/g, '/') : value);

export const toNativePath = (value: string): string => (isWindows ? value.replace(/\//g, '\\') : value);

export const toWasmPaths = (values: string[] | null): string[] | null => (isWindows && values ? values.map(toWasmPath) : values);

const convertModule = <T extends { path: string }>(module: T, convert: (value: string) => string): T =>
  isWindows && typeof module.path === 'string' ? { ...module, path: convert(module.path) } : module;

export const moduleToWasm = <T extends { path: string }>(module: T): T => convertModule(module, toWasmPath);

export const moduleToNative = <T extends { path: string }>(module: T): T => convertModule(module, toNativePath);

const convertViewModel = (viewModel: types.ModuleViewModel, convert: (value: string) => string): types.ModuleViewModel =>
  isWindows && viewModel.moduleInfoExtended ? { ...viewModel, moduleInfoExtended: convertModule(viewModel.moduleInfoExtended, convert) } : viewModel;

export const viewModelToWasm = (viewModel: types.ModuleViewModel): types.ModuleViewModel => convertViewModel(viewModel, toWasmPath);

export const viewModelToNative = (viewModel: types.ModuleViewModel): types.ModuleViewModel => convertViewModel(viewModel, toNativePath);

export const installResultToNative = (result: types.InstallResult): types.InstallResult => {
  if (!isWindows) return result;
  return {
    ...result,
    instructions: result.instructions.map((instruction) => ({
      ...instruction,
      ...(instruction.source !== undefined && { source: toNativePath(instruction.source) }),
      ...(instruction.destination !== undefined && { destination: toNativePath(instruction.destination) }),
    })),
  };
};
