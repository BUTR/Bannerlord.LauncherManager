import test from 'ava';

import { FetchBannerlordVersion, allocAliveCount } from '../src';
import { GAME_ROOT } from './testData';

const isDebug = process.argv[2] == "Debug";

test('Main', async (t) => {
  const path = GAME_ROOT;
  const dllName = 'TaleWorlds.Library.dll';

  const changeSet = FetchBannerlordVersion.getChangeSet(path, dllName);
  t.is(changeSet, 321460);

  const version = FetchBannerlordVersion.getVersion(path, dllName);
  t.is(version, 'e1.8.0');

  const versionType = FetchBannerlordVersion.getVersionType(path, dllName);
  t.is(versionType, 4);

  if (isDebug)
    t.deepEqual(allocAliveCount(), 0);

  t.pass();
});
