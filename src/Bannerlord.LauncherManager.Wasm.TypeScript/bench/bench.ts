import * as wasm from '../src';
import { LauncherManagerApi, runHotPaths, Timings } from './hotPaths';

/**
 * Times the hot paths of this package against the native one: node dist/bench/bench.js [path to @butr/vortexextensionnative]
 */
const ROUNDS = 5;
const MOD_COUNT = 300;

const measure = async (api: LauncherManagerApi): Promise<Timings> => {
  // The first round warms up both, the JIT of the native wrapper and the interpreter's tiering
  await runHotPaths(api, MOD_COUNT);
  const total: Timings = {};
  for (let i = 0; i < ROUNDS; i++) await runHotPaths(api, MOD_COUNT, total);
  return Object.fromEntries(Object.entries(total).map(([name, value]) => [name, value / ROUNDS]));
};

const main = async () => {
  const start = performance.now();
  await wasm.initialize();
  console.log(`initialize: ${Math.round(performance.now() - start)} ms`);

  const results: Record<string, Timings> = { wasm: await measure(wasm) };
  if (process.argv[2]) {
    results.native = await measure(require(process.argv[2]) as LauncherManagerApi);
  }

  const names = Object.keys(results.wasm);
  const rows = names.map((name) => ({
    step: name,
    wasm: +results.wasm[name].toFixed(1),
    ...(results.native && {
      native: +results.native[name].toFixed(1),
      ratio: +(results.wasm[name] / results.native[name]).toFixed(1),
    }),
  }));
  const sum = (timings: Timings) => +Object.values(timings).reduce((a, b) => a + b, 0).toFixed(1);
  rows.push({ step: 'total', wasm: sum(results.wasm), ...(results.native && { native: sum(results.native), ratio: +(sum(results.wasm) / sum(results.native)).toFixed(1) }) });
  console.log(`${MOD_COUNT} mods, ms per round, average of ${ROUNDS}`);
  console.table(rows);
};

main().catch((e) => {
  console.error(e);
  process.exit(1);
});
