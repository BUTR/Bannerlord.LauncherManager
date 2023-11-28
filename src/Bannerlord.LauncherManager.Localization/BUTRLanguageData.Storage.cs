using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Bannerlord.LauncherManager.Localization;

/// <summary>
/// Storage of all serialized languages
/// </summary>
partial class BUTRLanguageData
{
    public static IReadOnlyList<BUTRLanguageData> All => _all;
    private static List<BUTRLanguageData> _all { get; } = new();

    public static BUTRLanguageData? GetLanguageData(string stringId)
    {
        foreach (var languageData in All)
        {
            if (languageData.StringId == stringId)
            {
                return languageData;
            }
        }

        return null;
    }

    public static void LoadFromXml(XmlDocument doc)
    {
        foreach (var node in doc.ChildNodes.OfType<XmlNode>())
        {
            if (node.Name != "LanguageData" || node.NodeType == XmlNodeType.Comment || node.Attributes is null) continue;
            if (node.Attributes["id"]?.Value is not { } id || string.IsNullOrEmpty(id)) continue;
            var languageData = GetLanguageData(id);
            if (languageData is null)
            {
                _all.Add(languageData = new BUTRLanguageData(id));
            }

            languageData.Deserialize(node);
        }
    }

    public static void Clear()
    {
        _all.Clear();
    }
}