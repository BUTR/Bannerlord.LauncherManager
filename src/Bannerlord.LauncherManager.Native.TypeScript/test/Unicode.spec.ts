import test from 'ava';

import { BannerlordModuleManager } from '../src';

// JSON crosses as UTF-8 both ways, the native side's is ASCII with everything else escaped as \uXXXX: text outside ASCII
// has to reach the native side and come back from JSON.parse as it was
const name = '中文模组 Кириллица Ünïcødé 🎮 "quoted" \\ back';
const xmlName = name.replace(/&/g, '&amp;').replace(/"/g, '&quot;').replace(/</g, '&lt;');
const moduleXml = (id: string, dependency?: string): string => `<?xml version="1.0" encoding="UTF-8"?>
<Module>
  <Id value="${id}"/>
  <Name value="${xmlName}"/>
  <Version value="v1.0.0"/>
  <DependedModules>${dependency ? `<DependedModule Id="${dependency}"/>` : ''}</DependedModules>
  <SubModules/>
</Module>`;

test('Text outside Latin-1 comes back as it was', (t) => {
  const module = BannerlordModuleManager.getModuleInfo(moduleXml('Unicode.Module'));
  t.is(module?.name, name);

  // A result built from a list: an issue naming the module
  const dependent = BannerlordModuleManager.getModuleInfo(moduleXml('Unicode.Dependent', 'Missing.Module'))!;
  const issues = BannerlordModuleManager.validateLoadOrder([module!, dependent], dependent);
  t.true(issues.length > 0);
  t.is(issues[0].target.name, name);
});

test('A large result comes back whole', (t) => {
  const modules = Array.from({ length: 200 }, (_, i) => BannerlordModuleManager.getModuleInfo(moduleXml(`Unicode.${i}`))!);
  for (const module of modules) t.is(module.name, name);
  const sorted = BannerlordModuleManager.sortWithOptions(modules, { skipOptionals: false, skipExternalDependencies: false });
  t.is(sorted.length, 200);
});
