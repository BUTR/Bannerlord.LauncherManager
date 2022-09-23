using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Bannerlord.VortexExtension.Native
{
    public static partial class Bindings
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
            IncludeFields = false,
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin),
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false) }
        };
        private static readonly SourceGenerationContext _customSourceGenerationContext = new(_options);
    }
}