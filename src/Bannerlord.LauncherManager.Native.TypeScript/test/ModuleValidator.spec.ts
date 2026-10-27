import test from 'ava';

import { ModuleInfoExtended, ModuleSorterOptions } from '../src/types';
import { BannerlordModuleManager } from '../src';

// Module i depends on i - 1. With issues, every 7th also on a missing one, every 11th is incompatible with module 0
const moduleXml = (i: number, issues: boolean): string => `<?xml version="1.0" encoding="UTF-8"?>
<Module>
  <Id value="Mod${i}"/>
  <Name value="Mod ${i}"/>
  <Version value="v1.0.${i}"/>
  <DependedModules>${i > 0 ? `<DependedModule Id="Mod${i - 1}" DependentVersion="v1.0.0"/>` : ''}${issues && i % 7 === 0 ? '<DependedModule Id="Missing"/>' : ''}</DependedModules>
  <IncompatibleModules>${issues && i % 11 === 10 ? '<IncompatibleModule Id="Mod0"/>' : ''}</IncompatibleModules>
  <SubModules/>
</Module>`;

const modulesOf = (count: number, issues = true): ModuleInfoExtended[] =>
  Array.from({ length: count }, (_, i) => BannerlordModuleManager.getModuleInfo(moduleXml(i, issues))!).reverse();

const options: ModuleSorterOptions = { skipOptionals: true, skipExternalDependencies: false };

test('A validator gives what the calls with the list give', (t) => {
  const modules = modulesOf(40);
  const isSelected = (id: string) => !id.endsWith('3');
  const validator = BannerlordModuleManager.createValidator(modules);
  try {
    t.deepEqual(validator.sort(), BannerlordModuleManager.sort(modules));
    t.deepEqual(validator.sortWithOptions(options), BannerlordModuleManager.sortWithOptions(modules, options));
    let issues = 0;
    for (const module of modules) {
      const moduleIssues = validator.validateModule(module, { isSelected });
      issues += moduleIssues.length;
      t.deepEqual(moduleIssues, BannerlordModuleManager.validateModule(modules, module, { isSelected }));
      t.deepEqual(validator.validateLoadOrder(module), BannerlordModuleManager.validateLoadOrder(modules, module));
      t.is(validator.areAllDependenciesOfModulePresent(module), BannerlordModuleManager.areAllDependenciesOfModulePresent(modules, module));
      t.deepEqual(validator.getDependentModulesOf(module), BannerlordModuleManager.getDependentModulesOf(modules, module));
      t.deepEqual(validator.getDependentModulesOfWithOptions(module, options), BannerlordModuleManager.getDependentModulesOfWithOptions(modules, module, options));
    }
    t.true(issues > 0);
  } finally {
    validator.dispose();
  }
});

test('Sorting returns the modules it was given, not copies', (t) => {
  const modules = modulesOf(10, false);
  const sorted = BannerlordModuleManager.sort(modules);
  t.is(sorted.length, 10);
  for (const module of sorted) t.true(modules.includes(module));
  t.deepEqual(sorted.map((x) => x.id), Array.from({ length: 10 }, (_, i) => `Mod${i}`));
});

test('A validator works on the list as it was created', (t) => {
  const modules = modulesOf(10);
  const validator = BannerlordModuleManager.createValidator(modules);
  try {
    const before = validator.sort();
    modules.length = 3;
    t.deepEqual(validator.sort(), before);
  } finally {
    validator.dispose();
  }
});

test('A disposed validator throws, disposing again does nothing', (t) => {
  const validator = BannerlordModuleManager.createValidator(modulesOf(3));
  validator.dispose();
  t.notThrows(() => validator.dispose());
  t.throws(() => validator.sort(), { message: /disposed/ });
});
