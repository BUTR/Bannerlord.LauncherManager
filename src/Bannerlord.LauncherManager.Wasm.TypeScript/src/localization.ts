/**
 * Bannerlord.LauncherManager.Localization's BUTRLocalizationManager in TS: Vortex localizes while it renders, a string
 * has to come back at once, and it works without the runtime, which only runs while Bannerlord is managed.
 * The runtime still gets the XMLs, its own texts like issues and hints use them: Utils sends them while it runs and
 * runtime.ts again to every runtime it starts. test/Localization.spec.ts checks both give the same results
 */

const DEFAULT_LANGUAGE = 'English';

// The string id's text by language
const storage = new Map<string, Map<string, string>>();
const loaded: string[] = [];
let activeLanguage = DEFAULT_LANGUAGE;

interface LanguageStrings {
  language: string;
  strings: [id: string, text: string][];
}

const ENTITIES: Record<string, string> = { lt: '<', gt: '>', amp: '&', quot: '"', apos: "'" };

// An attribute's value as XmlDocument.LoadXml reads it: the references resolved, line breaks and tabs kept as they are
const attributeValue = (raw: string): string =>
  raw.replace(/&(#x[0-9A-Fa-f]+|#[0-9]+|[A-Za-z]+);|&/g, (match, ref: string | undefined) => {
    if (ref === undefined) throw new Error('The localization XML has an & that starts no reference');
    if (ref.startsWith('#')) {
      return String.fromCodePoint(ref[1] === 'x' ? parseInt(ref.slice(2), 16) : parseInt(ref.slice(1), 10));
    }
    const entity = ENTITIES[ref];
    if (entity === undefined) throw new Error(`The localization XML has an undeclared entity ${match}`);
    return entity;
  });

const attributesOf = (raw: string): Map<string, string> => {
  const attributes = new Map<string, string>();
  for (const [, name, double, single] of raw.matchAll(/([^\s=]+)\s*=\s*(?:"([^"]*)"|'([^']*)')/g)) {
    attributes.set(name, attributeValue(double ?? single));
  }
  return attributes;
};

// The markup between the tags, the tags themselves are matched on their own
const MARKUP = /<!--[\s\S]*?-->|<\?[\s\S]*?\?>|<!DOCTYPE[^>]*>|<!\[CDATA\[[\s\S]*?\]\]>|<(\/?)([^\s/>]+)((?:\s+[^\s=/>]+\s*=\s*(?:"[^"<]*"|'[^'<]*'))*)\s*(\/?)>|</g;

/**
 * What BUTRLanguageData reads: the <LanguageData> at the top with an id, the <string>s in its <strings>.
 * Throws where XmlDocument or the C# would
 */
const parse = (xml: string): LanguageStrings[] => {
  const languages: LanguageStrings[] = [];
  const open: string[] = [];
  let roots = 0;
  let language: LanguageStrings | undefined;
  for (const [markup, closing, name, rawAttributes, selfClosing] of xml.replace(/^﻿/, '').matchAll(MARKUP)) {
    if (name === undefined) {
      if (markup === '<') throw new Error('The localization XML has a < that starts no tag');
      continue;
    }
    if (closing) {
      if (open.pop() !== name) throw new Error(`The localization XML closes <${name}> where it isn't open`);
      continue;
    }

    if (open.length === 0) {
      if (++roots > 1) throw new Error('The localization XML has more than one root element');
      const attributes = attributesOf(rawAttributes);
      const id = attributes.get('id');
      language = undefined;
      if (name === 'LanguageData' && id) {
        if (!attributes.has('under_development')) throw new Error('The localization XML\'s LanguageData has no under_development');
        language = { language: id, strings: [] };
        languages.push(language);
      }
    } else if (language !== undefined && name === 'string' && open.length === 2 && open[1] === 'strings'
      && language.language.toUpperCase() !== DEFAULT_LANGUAGE.toUpperCase()) {
      const attributes = attributesOf(rawAttributes);
      const id = attributes.get('id');
      const text = attributes.get('text');
      if (id === undefined || text === undefined) throw new Error('The localization XML has a <string> without an id or text');
      language.strings.push([id, text]);
    }

    if (!selfClosing) {
      open.push(name);
    }
  }
  if (open.length > 0) throw new Error(`The localization XML doesn't close <${open[open.length - 1]}>`);
  if (roots === 0) throw new Error('The localization XML has no root element');
  return languages;
};

/**
 * Reads the XML, Utils.loadLocalization keeps it only when the runtime took it too
 * @returns Keeps it, for localizeString and the runtimes to come
 */
export const readLocalization = (xml: string): (() => void) => {
  const languages = parse(xml);
  return () => {
    for (const { language, strings } of languages) {
      for (const [id, text] of strings) {
        let texts = storage.get(id);
        if (texts === undefined) {
          storage.set(id, texts = new Map());
        }
        if (!texts.has(language)) {
          texts.set(language, text);
        }
      }
    }
    loaded.push(xml);
  };
};

export const setLanguage = (language: string): void => {
  activeLanguage = language;
};

/**
 * The XMLs and the language, for a runtime that starts
 */
export const getLocalization = (): { xmls: readonly string[]; language: string } => ({ xmls: loaded, language: activeLanguage });

const removeComments = (text: string): string => text.replace(/\{%.+?\}/g, '');

const getLocalizedText = (text: string): string => {
  if (text.length <= 2 || text[0] !== '{' || text[1] !== '=') return text;
  const end = text.indexOf('}', 2);
  // No end to the id, no text after it
  if (end < 0) return '';
  const id = text.slice(2, end);
  const target = text.slice(end + 1);
  if (activeLanguage === DEFAULT_LANGUAGE) return removeComments(target);
  if (id !== '*' && id !== '!') {
    const translated = storage.get(id)?.get(activeLanguage);
    if (translated !== undefined) return removeComments(translated);
  }
  return target;
};

/**
 * "{=id}Text {VALUE}": the active language's text for the id, or the text after it, with the values and {NL} filled in
 */
export const localizeString = (template: string, values: { [value: string]: string }): string => {
  let text = getLocalizedText(template);
  for (const [name, value] of Object.entries(values)) {
    if (value === undefined) continue;
    text = text.split(`{${name}}`).join(String(value));
  }
  return text.split('{NL}').join('\n');
};
