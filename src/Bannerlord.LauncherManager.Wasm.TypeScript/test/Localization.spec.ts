import test from 'ava';

import { Utils, initialize } from '../src';
import { getExports } from '../src/runtime';

// The TS port against Bannerlord.LauncherManager.Localization's C#, which the runtime still has

// Before the runtime starts: the runtime gets it when it starts
const RUSSIAN = `﻿<?xml version="1.0" encoding="utf-8"?>
<!-- A comment -->
<LanguageData id="Русский" name="Русский" supported_iso="ru,rus" under_development="false">
  <strings>
    <!-- A group -->
    <string id="aaa" text="Модуль {NAME}!{NL}Вторая строка"/>
    <string id="bbb" text="С комментарием {%a comment}и &lt;тегом&gt; &amp; &quot;кавычками&quot; &#x1F3AE; &#65;"/>
    <string id='ccc' text='Одинарные кавычки "внутри"'/>
    <string id="ddd" text="Перенос\r\nстроки\tи табуляция"/>
    <string id="aaa" text="Второй перевод того же id"/>
    <string id="*" text="Звёздочка"/>
    <string id="dollar" text="$&amp; {VALUE} $1 {VALUE}"/>
    <string id="nested" text="{NAME} и {VALUE}"/>
  </strings>
  <other>
    <string id="eee" text="Не в strings"/>
  </other>
</LanguageData>`;

// English's strings are the templates' own
const ENGLISH = `<LanguageData id="English" name="English" supported_iso="en" under_development="false">
  <strings><string id="aaa" text="English text"/></strings>
</LanguageData>`;

// After the runtime started: it gets it right away
const GERMAN = `<LanguageData id="Deutsch" name="Deutsch" supported_iso="de" under_development="true">
  <strings>
    <string id="aaa" text="Modul {NAME}"/>
    <string id="fff" text="Nur Deutsch {%x}"/>
  </strings>
</LanguageData>`;

// A second file of a language: the first file's texts stay
const RUSSIAN_MORE = `<LanguageData id="Русский" name="Русский" supported_iso="ru" under_development="false">
  <strings>
    <string id="aaa" text="Из второго файла"/>
    <string id="ggg" text="Новый {%x}id"/>
  </strings>
</LanguageData>`;

const LANGUAGES = ['English', 'Русский', 'Deutsch', 'Unknown', 'english'];

const TEMPLATES = [
  '', '{', '{=', '{=}', '{=}Text', '{=aaa}Module {NAME}!', '{=bbb}Fallback', '{=ccc}Fallback', '{=ddd}Fallback',
  '{=*}Star {NAME}', '{=!}Bang', '{=missing}Missing {NAME}{NL}line', '{=eee}Not in strings', '{=fff}German only',
  '{=ggg}New', '{=dollar}Dollar', '{=nested}Nested', 'Plain {NAME} {NL} {VALUE}', '{=unterminated', 'no{=aaa}prefix',
  '{=zzz}With a {%comment} in English', '{=aaa', ' {=aaa}Leading space',
];

const VALUES: Record<string, string>[] = [
  {},
  { NAME: 'Имя {NL} $& $1 {VALUE}', VALUE: '中文 🎮' },
  { VALUE: 'first', NAME: '{VALUE}' },
  { NAME: 'n', 1: 'one', 0: 'zero' },
];

test.before(async () => {
  Utils.loadLocalization(RUSSIAN);
  await initialize();
  Utils.loadLocalization(ENGLISH);
  Utils.loadLocalization(GERMAN);
  Utils.loadLocalization(RUSSIAN_MORE);
});

test.serial('localizeString matches the C#', (t) => {
  for (const language of LANGUAGES) {
    Utils.setLanguage(language);
    for (const template of TEMPLATES) {
      for (const values of VALUES) {
        const csharp = getExports().UtilsExports.LocalizeString(template, JSON.stringify(values));
        t.is(Utils.localizeString(template, values), csharp, `${language} ${JSON.stringify(template)} ${JSON.stringify(values)}`);
      }
    }
  }
});

test.serial('localizeString translates', (t) => {
  Utils.setLanguage('Русский');
  t.is(Utils.localizeString('{=aaa}Module {NAME}!', { NAME: 'Тест' }), 'Модуль Тест!\nВторая строка');
  t.is(Utils.localizeString('{=bbb}Fallback', {}), 'С комментарием и <тегом> & "кавычками" 🎮 A');
  t.is(Utils.localizeString('{=ddd}Fallback', {}), 'Перенос\r\nстроки\tи табуляция');
  t.is(Utils.localizeString('{=ggg}New', {}), 'Новый id');
  Utils.setLanguage('English');
  t.is(Utils.localizeString('{=aaa}Module {NAME}!', { NAME: 'Test' }), 'Module Test!');
});

const INVALID = [
  '',
  '<LanguageData id="X" under_development="false">',
  '<LanguageData id="X" under_development="false"></Other>',
  '<LanguageData id="X" under_development="false"/><Second/>',
  '<LanguageData id="X" under_development="&bogus;"/>',
  '<LanguageData id="X" under_development="a & b"/>',
  '<LanguageData id="X" under_development="false"><strings><string id="a" text="1 < 2"/></strings></LanguageData>',
  '<LanguageData id="X"/>',
  '<LanguageData id="X" under_development="false"><strings><string id="a"/></strings></LanguageData>',
];

test.serial('loadLocalization throws where the C# does', (t) => {
  for (const xml of INVALID) {
    t.throws(() => getExports().UtilsExports.LoadLocalization(xml), undefined, `C# ${JSON.stringify(xml)}`);
    t.throws(() => Utils.loadLocalization(xml), undefined, `TS ${JSON.stringify(xml)}`);
  }
});
