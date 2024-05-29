using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

namespace Bannerlord.LauncherManager.Native;

public static class ReaderUtils2
{
    private static readonly byte[] BOMMarkUtf8 = Encoding.UTF8.GetPreamble();

    public static XmlDocument Read(ReadOnlySpan<char> xml)
    {
        var doc = new XmlDocument();
        doc.LoadXml(new string(RemoveBOM2(xml)));
        return doc;
    }
    
    private static ReadOnlySpan<char> RemoveBOM2(ReadOnlySpan<char> xml)
    {
        var bom = MemoryMarshal.Cast<byte, char>(BOMMarkUtf8.AsSpan());
        if (xml.StartsWith(bom))
            xml = xml.Slice(bom.Length);
        return xml;
    }
}