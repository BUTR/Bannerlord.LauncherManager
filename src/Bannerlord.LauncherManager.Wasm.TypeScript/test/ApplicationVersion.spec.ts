import test from 'ava';

import { initialize } from '../src';
import { compareVersions, parseApplicationVersion } from '../src/applicationVersion';
import { getExports } from '../src/runtime';
import { ApplicationVersion } from '../src/types';

test.before(() => initialize());

// The TS port against Bannerlord.ModuleManager's C#, which the runtime still has
const INPUTS = [
  '', 'v', 'v1', 'v1.2', 'v1.2.3', 'v1.2.3.4', 'v1.2.3.4.5', 'e1.8.0', 'e1.8.0.321460', 'a0.0.0', 'b1.0.0', 'd2.0.0',
  'x1.2.3', '1.2.3', 'v*', 'v*.2.3', 'v1.*', 'v1.*.3', 'v1.2.*', 'v1.2.3.*', 'v1.2.x', 'v1.x.3', 'vx.2.3',
  '.1.2', '..', '...', 'v 1. 2 .3 ', 'v+1.-2.+3', 'v1.2.3\t', 'v1.2.3 ', 'v2147483647.0.0', 'v2147483648.0.0',
  'v-2147483648.0.0', 'v1.2.3.2147483648', 'v01.002.0003', 'v1..3', 'v1.2.', 'v١.2.3', 'V1.2.3', 'e1.2.3.4',
];

const parseInCSharp = (content: string): ApplicationVersion | string => {
  try {
    return JSON.parse(getExports().ModuleManagerExports.ParseApplicationVersion(content));
  } catch {
    return 'throws';
  }
};

const parseInTs = (content: string): ApplicationVersion | string => {
  try {
    return parseApplicationVersion(content);
  } catch {
    return 'throws';
  }
};

test('parseApplicationVersion matches the C#', (t) => {
  for (const input of INPUTS) {
    t.deepEqual(parseInTs(input), parseInCSharp(input), JSON.stringify(input));
  }
});

test('compareVersions matches the C#', (t) => {
  const versions = INPUTS.map(parseInTs).filter((x): x is ApplicationVersion => typeof x !== 'string');
  for (const x of versions) {
    for (const y of versions) {
      const csharp = getExports().ModuleManagerExports.CompareVersions(JSON.stringify(x), JSON.stringify(y));
      t.is(compareVersions(x, y), csharp, `${JSON.stringify(x)} ${JSON.stringify(y)}`);
    }
  }
});
