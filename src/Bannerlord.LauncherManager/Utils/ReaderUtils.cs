using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Bannerlord.LauncherManager.Utils;

public static class ReaderUtils
{
    private static readonly byte[] BOMMarkUtf8 = Encoding.UTF8.GetPreamble();
    
    public static XmlDocument Read(byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var sr = new StreamReader(ms);
        
        Span<byte> bom = BOMMarkUtf8.AsSpan();
#if NETSTANDARD2_1_OR_GREATER
        Span<byte> preamblePreallocated = stackalloc byte[BOMMarkUtf8.Length];
        ms.Read(preamblePreallocated);
#else
        var preamblePreallocated = new byte[BOMMarkUtf8.Length];
        ms.Read(preamblePreallocated, 0, BOMMarkUtf8.Length);
#endif

        ms.Seek(bom.SequenceEqual(preamblePreallocated) ? BOMMarkUtf8.Length : 0, SeekOrigin.Begin);
        var doc = new XmlDocument();
        doc.Load(new XmlTextReader(sr));
        return doc;
    }
}