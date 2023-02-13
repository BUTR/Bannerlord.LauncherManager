using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Bannerlord.LauncherManager.Utils;

public static class ReaderUtils
{
    public static XmlDocument Read(byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var sr = new StreamReader(ms);
        //var preamble = new UTF8Encoding(true).GetPreamble();
        //var preamblePreallocated = new byte[preamble.Length];
        //ms.Read(preamblePreallocated, 0, preamble.Length);
        //ms.Seek(preamble.SequenceEqual(preamblePreallocated) ? preamble.Length : 0, SeekOrigin.Begin);
        var doc = new XmlDocument();
        doc.Load(new XmlTextReader(sr));
        return doc;
    }
}