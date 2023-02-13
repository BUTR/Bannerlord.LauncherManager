import * as types from './types';

export const allocAliveCount = (): number => {
  const addon: types.IExtension = require('./../../launchermanager.node');
  return addon.allocAliveCount();
}