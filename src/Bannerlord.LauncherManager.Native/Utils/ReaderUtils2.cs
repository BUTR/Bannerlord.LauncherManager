using System;
using System.Runtime.InteropServices;
using System.Xml;

namespace Bannerlord.LauncherManager.Native.Utils;

public static class ReaderUtils2
{
    private static readonly byte[] BOMMarkUtf8 = [0xEF, 0xBB, 0xBF];
    private static readonly byte[] BOMMarkUtf16BE = [0xFE, 0xFF];
    private static readonly byte[] BOMMarkUtf16LE = [0xFF, 0xFE];

    public static XmlDocument Read(ReadOnlySpan<char> xml)
    {
        var doc = new XmlDocument();
        doc.LoadXml(new string(RemoveBOM2(xml)));
        return doc;
    }

    private static ReadOnlySpan<char> RemoveBOM2(ReadOnlySpan<char> xml)
    {
        var utf8BOM = MemoryMarshal.Cast<byte, char>(BOMMarkUtf8.AsSpan());
        var utf16BEBOM = MemoryMarshal.Cast<byte, char>(BOMMarkUtf16BE.AsSpan());
        var utf16LEBOM = MemoryMarshal.Cast<byte, char>(BOMMarkUtf16LE.AsSpan());

        var bomOffset = xml.StartsWith(utf8BOM) ? utf8BOM.Length :
            xml.StartsWith(utf16BEBOM) ? utf16BEBOM.Length :
            xml.StartsWith(utf16LEBOM) ? utf16LEBOM.Length : 0;

        xml = xml.Slice(bomOffset > 0 ? bomOffset : 0);

        return xml;
    }
}