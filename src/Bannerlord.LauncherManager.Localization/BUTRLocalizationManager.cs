﻿using Bannerlord.LauncherManager.Localization.Extensions;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Bannerlord.LauncherManager.Localization;

/// <summary>
/// Shared<br/>
/// </summary>
public static partial class BUTRLocalizationManager
{
    public const string English = "English";
    public const string ChineseTraditional = "繁體中文";
    public const string ChineseSimple = "简体中文";
    public const string Japanese = "日本語";
    public const string Korean = "한국어";

    public static readonly string DefaultLanguage = English;
    public static string ActiveLanguage = DefaultLanguage;

    [ThreadStatic]
    private static StringBuilder? _idStringBuilder;
    [ThreadStatic]
    private static StringBuilder? _targetStringBuilder;

    public static void LoadLanguage(XmlDocument xmlDocument)
    {
        BUTRLanguageData.LoadFromXml(xmlDocument);
    }

    [return: NotNullIfNotNull("to")]
    internal static string? ProcessTextToString(BUTRTextObject? to, bool shouldClear)
    {
        if (to is null)
        {
            return null;
        }

        if (BUTRTextObject.IsNullOrEmpty(to))
        {
            return string.Empty;
        }

        var localizedText = GetLocalizedText(to.Value);
        string text;
        if (!string.IsNullOrEmpty(to.Value))
        {
            text = Process(localizedText, to);
        }
        else
        {
            text = "";
        }

        return text;
    }

    private static string GetLocalizedText(string text)
    {
        if (text is not { Length: > 2 } || text[0] != '{' || text[1] != '=') return text;

        (_idStringBuilder ??= new StringBuilder(8)).Clear();
        (_targetStringBuilder ??= new StringBuilder(100)).Clear();

        var i = 2;
        while (i < text.Length)
        {
            if (text[i] != '}')
            {
                _idStringBuilder.Append(text[i]);
                i++;
            }
            else
            {
                for (i++; i < text.Length; i++)
                {
                    _targetStringBuilder.Append(text[i]);
                }

                var text2 = "";
                if (ActiveLanguage == DefaultLanguage)
                {
                    text2 = _targetStringBuilder.ToString();
                    return RemoveComments(text2);
                }

                if ((_idStringBuilder.Length != 1 || _idStringBuilder[0] != '*') && (_idStringBuilder.Length != 1 || _idStringBuilder[0] != '!'))
                {
                    if (ActiveLanguage != DefaultLanguage)
                    {
                        text2 = GetTranslatedText(ActiveLanguage, _idStringBuilder.ToString());
                    }

                    if (text2 is not null)
                    {
                        return RemoveComments(text2);
                    }
                }
                return _targetStringBuilder.ToString();
            }
        }

        return _targetStringBuilder.ToString();

    }

    private static string RemoveComments(string localizedText)
    {
        foreach (Match match in new Regex("{%.+?}", RegexOptions.Compiled).Matches(localizedText))
        {
            localizedText = localizedText.Replace(match.Value, "");
        }
        return localizedText;
    }

    // Keeping it simple
    private static string Process(string query, BUTRTextObject? parent = null)
    {
        foreach (var (name, value) in parent?.Attributes ?? new Dictionary<string, object>())
        {
            query = query.Replace($"{{{name}}}", value.ToString());
        }

        return query.Replace("{NL}", "\n");
    }
}