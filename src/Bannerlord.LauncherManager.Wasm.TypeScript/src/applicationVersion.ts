import * as types from './types';

/**
 * Bannerlord.ModuleManager's ApplicationVersion.TryParse and ApplicationVersionComparer in TS: they're pure, Vortex calls
 * them from selectors, at any time, and they work without the runtime, which only runs while Bannerlord is managed.
 * Kept to the C# of the Bannerlord.ModuleManager package the runtime has (6.0.249), test/ApplicationVersion.spec.ts
 * checks both give the same results
 */

const INT_MIN = -2147483648;
const INT_MAX = 2147483647;

// The order of the C# enum, the comparer compares their values
const TYPE_ORDER: types.ApplicationVersionType[] = [
  types.ApplicationVersionType.Alpha,
  types.ApplicationVersionType.Beta,
  types.ApplicationVersionType.EarlyAccess,
  types.ApplicationVersionType.Release,
  types.ApplicationVersionType.Development,
  types.ApplicationVersionType.Invalid,
];

const fromPrefix = (prefix: string): types.ApplicationVersionType => {
  switch (prefix) {
    case 'a': return types.ApplicationVersionType.Alpha;
    case 'b': return types.ApplicationVersionType.Beta;
    case 'e': return types.ApplicationVersionType.EarlyAccess;
    case 'v': return types.ApplicationVersionType.Release;
    case 'd': return types.ApplicationVersionType.Development;
    default: return types.ApplicationVersionType.Invalid;
  }
};

// int.TryParse with the invariant culture: a sign and .NET's whitespace (tab to carriage return, space) around ASCII
// digits, the value must fit an int
const tryParseInt = (value: string): number | undefined => {
  const match = /^[\t\n\v\f\r ]*([+-]?\d+)[\t\n\v\f\r ]*$/.exec(value);
  if (match === null) return undefined;
  const parsed = Number(match[1]);
  return parsed >= INT_MIN && parsed <= INT_MAX ? parsed : undefined;
};

const empty = (): types.ApplicationVersion => ({
  applicationVersionType: types.ApplicationVersionType.Alpha,
  major: 0,
  minor: 0,
  revision: 0,
  changeSet: 0,
});

export const parseApplicationVersion = (content: string): types.ApplicationVersion => {
  // TryParse(versionAsString, out version, asMin: true), the export answers ApplicationVersion.Empty when it fails
  let major: number;
  let minor = 0;
  let revision = 0;
  let changeSet = 0;
  let skipCheck = false;

  const array = content.split('.');
  if ((array.length !== 3 && array.length !== 4) || array[0].length === 0) return empty();

  const first = array[0];
  const applicationVersionType = fromPrefix(first[0]);

  const parsedMajor = tryParseInt(first.substring(1));
  if (parsedMajor === undefined) {
    if (first.substring(1) !== '*') return empty();
    major = INT_MIN;
    minor = INT_MIN;
    revision = INT_MIN;
    changeSet = INT_MIN;
    skipCheck = true;
  } else {
    major = parsedMajor;
  }
  if (!skipCheck) {
    const parsed = tryParseInt(array[1]);
    if (parsed === undefined) {
      if (array[1] !== '*') return empty();
      minor = 0;
      revision = 0;
      changeSet = 0;
      skipCheck = true;
    } else {
      minor = parsed;
    }
  }
  if (!skipCheck) {
    const parsed = tryParseInt(array[2]);
    if (parsed === undefined) {
      if (array[2] !== '*') return empty();
      revision = 0;
      changeSet = 0;
      skipCheck = true;
    } else {
      revision = parsed;
    }
  }
  if (!skipCheck && array.length === 4) {
    const parsed = tryParseInt(array[3]);
    if (parsed === undefined) {
      if (array[3] !== '*') return empty();
      changeSet = 0;
    } else {
      changeSet = parsed;
    }
  }

  return { applicationVersionType, major, minor, revision, changeSet };
};

const compareNumbers = (x: number, y: number): number => (x < y ? -1 : x > y ? 1 : 0);

export const compareVersions = (x: types.ApplicationVersion | null | undefined, y: types.ApplicationVersion | null | undefined): number => {
  if (x == null && y == null) return 0;
  if (x == null) return -1;
  if (y == null) return 1;

  return compareNumbers(TYPE_ORDER.indexOf(x.applicationVersionType), TYPE_ORDER.indexOf(y.applicationVersionType))
    || compareNumbers(x.major, y.major)
    || compareNumbers(x.minor, y.minor)
    || compareNumbers(x.revision, y.revision)
    || compareNumbers(x.changeSet, y.changeSet);
};
