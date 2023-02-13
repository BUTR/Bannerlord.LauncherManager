using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bannerlord.LauncherManager.Localization;

public partial class BUTRTextObject
{
    public static readonly BUTRTextObject Empty = new();

    internal string Value;

    public Dictionary<string, object>? Attributes { get; private set; }

    public int Length => Value.Length;

    public BUTRTextObject()
    {
        Value = string.Empty;
        Attributes = null;
    }
    public BUTRTextObject(string? value)
    {
        Value = value ?? string.Empty;
        Attributes = null;
    }
    public BUTRTextObject(string? value, Dictionary<string, object>? attributes = null)
    {
        Value = value ?? string.Empty;
        Attributes = attributes;
    }
    public BUTRTextObject(int value, Dictionary<string, object>? attributes = null)
    {
        Value = value.ToString();
        Attributes = attributes;
    }
    public BUTRTextObject(float value, Dictionary<string, object>? attributes = null)
    {
        Value = value.ToString("R");
        Attributes = attributes;
    }

    public BUTRTextObject(string? value, Dictionary<string, string>? attributes = null)
    {
        Value = value ?? string.Empty;
        Attributes = attributes?.ToDictionary(x => x.Key, x => x.Value as object);
    }

    public override string ToString()
    {
        string text;
        try
        {
            text = BUTRLocalizationManager.ProcessTextToString(this, true);
        }
        catch (Exception ex)
        {
            text = $"Error at id: {GetID()}. Lang: {BUTRLocalizationManager.ActiveLanguage}. Exception: {ex}";
        }

        return text;
    }

    public string ToStringWithoutClear()
    {
        string text;
        try
        {
            text = BUTRLocalizationManager.ProcessTextToString(this, false);
        }
        catch (Exception ex)
        {
            text = $"Error at id: {GetID()}. Lang: {BUTRLocalizationManager.ActiveLanguage}. Exception: {ex}";
        }

        return text;
    }

    public bool Contains(BUTRTextObject to)
    {
        return Value.Contains(to.Value);
    }

    public bool Contains(string text)
    {
        return Value.Contains(text);
    }

    public bool Equals(BUTRTextObject to)
    {
        return Value == to.Value && ((Attributes is null && to.Attributes is null) || (Attributes is not null && to.Attributes is not null && Attributes.SequenceEqual(to.Attributes)));
    }

    public bool HasSameValue(BUTRTextObject to) => Value == to.Value;

    private BUTRTextObject SetTextVariableFromObject(string tag, object variable)
    {
        Attributes ??= new Dictionary<string, object>();
        Attributes[tag] = variable;
        return this;
    }

    public BUTRTextObject SetTextVariable(string tag, BUTRTextObject variable)
    {
        return SetTextVariableFromObject(tag, variable);
    }

    public BUTRTextObject SetTextVariable(string tag, string variable)
    {
        SetTextVariableFromObject(tag, variable);
        return this;
    }

    public BUTRTextObject SetTextVariable(string tag, float variable)
    {
        SetTextVariableFromObject(tag, variable);
        return this;
    }

    public BUTRTextObject SetTextVariable(string tag, int variable)
    {
        SetTextVariableFromObject(tag, variable);
        return this;
    }

    public void AddIDToValue(string id)
    {
        if (!Value.Contains(id) && !Value.StartsWith("{="))
        {
            var value = Value;
            Value = $"{{={id}}}{value}";
        }
    }

    public int GetValueHashCode()
    {
        return Value.GetHashCode();
    }

    public BUTRTextObject CopyTextObject()
    {
        var dictionary = Attributes;
        if (Attributes is not null && Attributes.Any())
        {
            var dictionary2 = new Dictionary<string, object>();
            foreach (var keyValuePair in Attributes)
            {
                dictionary2.Add(keyValuePair.Key, keyValuePair.Value);
            }

            dictionary = dictionary2;
        }

        return new BUTRTextObject(Value, dictionary);
    }

    public string GetID()
    {
        var sb = new StringBuilder();
        if (Value is { Length: > 2 } && Value[0] == '{' && Value[1] == '=')
        {
            var num = 2;
            while (num < Value.Length && Value[num] != '}')
            {
                sb.Append(Value[num]);
                num++;
            }
        }

        return sb.ToString();
    }
}