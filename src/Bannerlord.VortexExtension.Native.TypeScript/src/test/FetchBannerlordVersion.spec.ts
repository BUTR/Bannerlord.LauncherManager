import test from 'ava';

import { FetchBannerlordVersion } from '../lib';

test('Main', async (t) => {
  const path = __dirname;
  const dllName = 'TaleWorlds.Library.dll';

  const changeSet = FetchBannerlordVersion.getChangeSet(path, dllName);
  t.is(changeSet, 321460);

  const version = FetchBannerlordVersion.getVersion(path, dllName);
  t.is(version, 'e1.8.0');

  const versionType = FetchBannerlordVersion.getVersionType(path, dllName);
  t.is(versionType, 4);

  t.pass();
});