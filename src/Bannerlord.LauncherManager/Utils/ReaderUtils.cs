using System;
using System.IO;
using System.Xml;

namespace Bannerlord.LauncherManager.Utils;

public static class ReaderUtils
{
    private static readonly byte[] BOMMarkUtf8 = [0xEF, 0xBB, 0xBF];
    private static readonly byte[] BOMMarkUtf16BE = [0xFE, 0xFF];
    private static readonly byte[] BOMMarkUtf16LE = [0xFF, 0xFE];

    public static XmlDocument Read(byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var sr = new StreamReader(ms);

        var utf8BOM = BOMMarkUtf8.AsSpan();
        var utf16BEBOM = BOMMarkUtf16BE.AsSpan();
        var utf16LEBOM = BOMMarkUtf16LE.AsSpan();
        
#if NETSTANDARD2_1_OR_GREATER
        Span<byte> preamblePreallocated = stackalloc byte[4];
        ms.Read(preamblePreallocated);
#else
        var preamblePreallocatedArray = new byte[4];
        ms.Read(preamblePreallocatedArray, 0, 4);
        var preamblePreallocated = preamblePreallocatedArray.AsSpan();
#endif
        
        var bomOffset = preamblePreallocated.StartsWith(utf8BOM) ? BOMMarkUtf8.Length :
            preamblePreallocated.StartsWith(utf16BEBOM) ? BOMMarkUtf16BE.Length :
            preamblePreallocated.StartsWith(utf16LEBOM) ? BOMMarkUtf16LE.Length : 0;
        
        ms.Seek(bomOffset > 0 ? bomOffset : 0, SeekOrigin.Begin);
        var doc = new XmlDocument();
        doc.Load(new XmlTextReader(sr));
        return doc;
    }
}