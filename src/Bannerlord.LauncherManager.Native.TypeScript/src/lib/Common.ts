import * as types from './types';

export const allocWithOwnership = (length: number): Uint8Array | null => {
  const addon: types.IExtension = require('./../../launchermanager.node');
  return addon.allocWithOwnership(length);
}
export const allocWithoutOwnership = (length: number): Uint8Array | null => {
  const addon: types.IExtension = require('./../../launchermanager.node');
  return addon.allocWithoutOwnership(length);
}
export const allocAliveCount = (): number => {
  const addon: types.IExtension = require('./../../launchermanager.node');
  return addon.allocAliveCount();
}