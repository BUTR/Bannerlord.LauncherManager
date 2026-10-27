/**
 * A made-up but realistic mod list for the benchmark: the official modules and a few hundred mods with dependencies,
 * load order metadata, optional and incompatible modules, sub-module tags, and some broken ones (missing dependencies,
 * version mismatches, cycles) so the code that reports issues runs too. Seeded, the same on every run
 */

export interface GeneratedModule {
  id: string;
  xml: string;
}

const createRandom = (seed: number) => () => {
  seed = (seed * 1664525 + 1013904223) % 4294967296;
  return seed / 4294967296;
};

const OFFICIAL = ['Native', 'SandBoxCore', 'Sandbox', 'CustomBattle', 'StoryMode', 'BirthAndDeath', 'Multiplayer'];
const GAME_VERSION = 'v1.2.12';
const COMMUNITY = ['Bannerlord.Harmony', 'Bannerlord.ButterLib', 'Bannerlord.UIExtenderEx', 'Bannerlord.MBOptionScreen'];

const tags = (random: () => number): string => {
  const values: string[] = [];
  if (random() < 0.3) values.push('<Tag key="DedicatedServerType" value="none" />');
  if (random() < 0.2) values.push('<Tag key="IsNoRenderModeElement" value="false" />');
  if (random() < 0.15) values.push('<Tag key="ExclusivePlatform" value="WindowsSteam" />');
  if (random() < 0.1) values.push('<Tag key="RejectedPlatform" value="GDKDesktop" />');
  return values.length === 0 ? '<Tags />' : `<Tags>${values.join('')}</Tags>`;
};

const subModules = (id: string, random: () => number): string => {
  const count = 1 + Math.floor(random() * 3);
  const items: string[] = [];
  for (let i = 0; i < count; i++) {
    items.push(`<SubModule>
      <Name value="${id} ${i}" />
      <DLLName value="${id}.dll" />
      <SubModuleClassType value="${id}.SubModule${i}" />
      ${random() < 0.3 ? `<Assemblies><Assembly value="${id}.Shared.dll" /></Assemblies>` : ''}
      ${tags(random)}
    </SubModule>`);
  }
  return `<SubModules>${items.join('')}</SubModules>`;
};

const moduleXml = (options: {
  id: string;
  version: string;
  official: boolean;
  multiplayer?: boolean;
  dependedModules?: string[];
  loadAfterThis?: string[];
  incompatible?: string[];
  metadatas?: string[];
  subModules: string;
}): string => `<?xml version="1.0" encoding="UTF-8"?>
<Module>
  <Id value="${options.id}" />
  <Name value="${options.id.replace(/\./g, ' ')}" />
  <Version value="${options.version}" />
  <Official value="${options.official}" />
  <DefaultModule value="${options.official}" />
  <SingleplayerModule value="${!options.multiplayer}" />
  <MultiplayerModule value="${!!options.multiplayer}" />
  <Url value="https://www.nexusmods.com/mountandblade2bannerlord/mods/${options.id.length * 37}" />
  <DependedModules>${(options.dependedModules ?? []).join('')}</DependedModules>
  <ModulesToLoadAfterThis>${(options.loadAfterThis ?? []).map((x) => `<Module Id="${x}" />`).join('')}</ModulesToLoadAfterThis>
  <IncompatibleModules>${(options.incompatible ?? []).map((x) => `<Module Id="${x}" />`).join('')}</IncompatibleModules>
  <DependedModuleMetadatas>${(options.metadatas ?? []).join('')}</DependedModuleMetadatas>
  ${options.subModules}
</Module>`;

const version = (random: () => number): string =>
  `${random() < 0.1 ? 'e' : 'v'}${Math.floor(random() * 4)}.${Math.floor(random() * 10)}.${Math.floor(random() * 20)}${random() < 0.3 ? `.${Math.floor(random() * 99999)}` : ''}`;

export const generateModules = (modCount: number, seed = 42): GeneratedModule[] => {
  const random = createRandom(seed);
  const result: GeneratedModule[] = [];

  OFFICIAL.forEach((id, index) => {
    const previous = OFFICIAL.slice(0, index).filter((x) => x !== 'Multiplayer');
    result.push({
      id,
      xml: moduleXml({
        id,
        version: `${GAME_VERSION}.${100000 + index}`,
        official: true,
        multiplayer: id === 'Multiplayer',
        dependedModules: previous.map((x) => `<DependedModule Id="${x}" DependentVersion="${GAME_VERSION}" />`),
        subModules: subModules(id, random),
      }),
    });
  });

  COMMUNITY.forEach((id, index) => {
    const previous = COMMUNITY.slice(0, index);
    result.push({
      id,
      xml: moduleXml({
        id,
        version: `v2.${index}.${Math.floor(random() * 10)}`,
        official: false,
        dependedModules: previous.map((x) => `<DependedModule Id="${x}" DependentVersion="v2.0.0" />`),
        loadAfterThis: OFFICIAL.slice(0, 5),
        metadatas: [
          ...previous.map((x) => `<DependedModuleMetadata id="${x}" order="LoadBeforeThis" version="v2.0.0" />`),
          ...OFFICIAL.slice(0, 5).map((x) => `<DependedModuleMetadata id="${x}" order="LoadAfterThis" version="v1.0.0.*" optional="true" />`),
        ],
        subModules: subModules(id, random),
      }),
    });
  });

  const modIds = Array.from({ length: modCount }, (_, i) => `Mod.${String.fromCharCode(65 + (i % 26))}${i}`);
  modIds.forEach((id, index) => {
    const dependedModules = ['Native', 'SandBoxCore', 'Sandbox', 'StoryMode']
      .filter(() => random() < 0.8)
      .map((x) => `<DependedModule Id="${x}" DependentVersion="${random() < 0.1 ? 'v9.9.9' : GAME_VERSION}" />`);
    const metadatas: string[] = [];
    const incompatible: string[] = [];

    for (const community of COMMUNITY) {
      if (random() < 0.35) {
        dependedModules.push(`<DependedModule Id="${community}" DependentVersion="v2.0.0" ${random() < 0.2 ? 'Optional="true"' : ''} />`);
        metadatas.push(`<DependedModuleMetadata id="${community}" order="LoadBeforeThis" version="v2.0.*" />`);
      }
    }
    // Other mods, mostly the earlier ones so the order can be solved
    const others = Math.floor(random() * 4);
    for (let i = 0; i < others && index > 0; i++) {
      const other = modIds[Math.floor(random() * index)];
      const roll = random();
      if (roll < 0.4) {
        dependedModules.push(`<DependedModule Id="${other}" ${random() < 0.5 ? `DependentVersion="${version(random)}"` : ''} />`);
      } else if (roll < 0.75) {
        metadatas.push(`<DependedModuleMetadata id="${other}" order="LoadBeforeThis" ${random() < 0.5 ? 'optional="true"' : ''} version="${random() < 0.5 ? `[v1.0.0,v3.0.0]` : version(random)}" />`);
      } else if (roll < 0.9) {
        metadatas.push(`<DependedModuleMetadata id="${other}" order="LoadAfterThis" optional="true" />`);
      } else {
        incompatible.push(other);
        metadatas.push(`<DependedModuleMetadata id="${other}" incompatible="true" />`);
      }
    }
    // Broken: a dependency that isn't installed, a cycle with the next mod
    if (random() < 0.05) dependedModules.push(`<DependedModule Id="Missing.${id}" DependentVersion="v1.0.0" />`);
    if (random() < 0.03 && index + 1 < modIds.length) {
      metadatas.push(`<DependedModuleMetadata id="${modIds[index + 1]}" order="LoadAfterThis" />`);
    }

    result.push({
      id,
      xml: moduleXml({
        id,
        version: version(random),
        official: false,
        multiplayer: random() < 0.05,
        dependedModules,
        incompatible,
        metadatas,
        subModules: subModules(id, random),
      }),
    });
  });

  return result;
};

/**
 * A language file like the extension's, the strings are the templates the workload localizes
 */
export const generateLocalization = (language: string, count: number): { xml: string; ids: string[] } => {
  const ids = Array.from({ length: count }, (_, i) => `id${i.toString(36).padStart(6, '0')}`);
  const strings = ids
    .map((id, i) => `<string id="${id}" text="${language} text ${i}${i % 3 === 0 ? ' {VALUE}' : ''}${i % 7 === 0 ? '{NL}line' : ''}"/>`)
    .join('\n    ');
  return {
    ids,
    xml: `<?xml version="1.0" encoding="utf-8"?>
<LanguageData id="${language}" name="${language}" subtitle_extension="${language.slice(0, 2).toLowerCase()}" supported_iso="${language.slice(0, 2).toLowerCase()}" under_development="false">
  <strings>
    ${strings}
  </strings>
</LanguageData>`,
  };
};
