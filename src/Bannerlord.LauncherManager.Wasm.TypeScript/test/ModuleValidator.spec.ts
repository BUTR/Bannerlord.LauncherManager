import test from 'ava';

import { generateModules } from '../bench/data';
import { ModuleInfoExtended, ModuleSorterOptions } from '../src/types';
import { BannerlordModuleManager, initialize, unload } from '../src';

test.before(() => initialize());

const modulesOf = (count: number): ModuleInfoExtended[] =>
  generateModules(count).map(({ id, xml }) => BannerlordModuleManager.getModuleInfoWithMetadata(xml, 'Vortex' as any, `C:/Games/Modules/${id}/SubModule.xml`)!);

const options: ModuleSorterOptions = { skipOptionals: true, skipExternalDependencies: false };

test.serial('A validator gives what the calls with the list give', (t) => {
  const modules = modulesOf(80);
  const isSelected = (id: string) => !id.endsWith('3');
  const validator = BannerlordModuleManager.createValidator(modules);
  try {
    t.deepEqual(validator.sort(), BannerlordModuleManager.sort(modules));
    t.deepEqual(validator.sortWithOptions(options), BannerlordModuleManager.sortWithOptions(modules, options));
    for (const module of modules) {
      t.deepEqual(validator.validateModule(module, { isSelected }), BannerlordModuleManager.validateModule(modules, module, { isSelected }));
      t.deepEqual(validator.validateLoadOrder(module), BannerlordModuleManager.validateLoadOrder(modules, module));
      t.is(validator.areAllDependenciesOfModulePresent(module), BannerlordModuleManager.areAllDependenciesOfModulePresent(modules, module));
      t.deepEqual(validator.getDependentModulesOf(module), BannerlordModuleManager.getDependentModulesOf(modules, module));
      t.deepEqual(validator.getDependentModulesOfWithOptions(module, options), BannerlordModuleManager.getDependentModulesOfWithOptions(modules, module, options));
    }
  } finally {
    validator.dispose();
  }
});

test.serial('A validator returns the modules it was given, not copies', (t) => {
  const modules = modulesOf(20);
  const validator = BannerlordModuleManager.createValidator(modules);
  try {
    for (const module of validator.sort()) t.true(modules.includes(module));
  } finally {
    validator.dispose();
  }
});

test.serial('A validator works on the list as it was created', (t) => {
  const modules = modulesOf(20);
  const validator = BannerlordModuleManager.createValidator(modules);
  try {
    const before = validator.sort();
    modules.length = 5;
    t.deepEqual(validator.sort(), before);
  } finally {
    validator.dispose();
  }
});

test.serial('A disposed validator throws, disposing again does nothing', (t) => {
  const modules = modulesOf(5);
  const validator = BannerlordModuleManager.createValidator(modules);
  validator.dispose();
  t.notThrows(() => validator.dispose());
  t.throws(() => validator.sort(), { message: /disposed/ });
});

test.serial('In Node unload() keeps the runtime, and the validators with it', (t) => {
  // In a page unload() removes the runtime and the validators made before throw, see the Electron check
  const modules = modulesOf(5);
  const validator = BannerlordModuleManager.createValidator(modules);
  t.false(unload());
  t.notThrows(() => validator.validateLoadOrder(modules[0]));
  validator.dispose();
});
