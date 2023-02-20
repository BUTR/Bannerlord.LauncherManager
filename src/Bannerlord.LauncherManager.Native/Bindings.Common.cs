using Bannerlord.LauncherManager.Models;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Bannerlord.LauncherManager.Native;

public static partial class Bindings
{
    private class CallbackStorage
    {
        private readonly Action<object> _action;
        public CallbackStorage(Action<object> action) => _action = action;
        public void SetResult(object result) => _action(result);
    }

    public class InstallInstructionJsonConverter : JsonConverter<IInstallInstruction>
    {
        public override IInstallInstruction Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotSupportedException();

        public override void Write(Utf8JsonWriter writer, IInstallInstruction value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case CopyInstallInstruction copyInstallInstruction:
                    JsonSerializer.Serialize(writer, copyInstallInstruction, CustomSourceGenerationContext.CopyInstallInstruction);
                    break;
            }
        }
    }

    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "<Pending>")]
    internal static readonly JsonSerializerOptions _options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        IgnoreReadOnlyFields = true,
        IgnoreReadOnlyProperties = true,
        IncludeFields = false,
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin),
        Converters =
        {
            new InstallInstructionJsonConverter(),
            new JsonStringEnumConverter(),
        }
    };
    internal static readonly SourceGenerationContext CustomSourceGenerationContext = new(_options);
}