using Bannerlord.LauncherManager.Models;
using Bannerlord.ModuleManager;

using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Bannerlord.LauncherManager.Native;

public static partial class Bindings
{
    public class InstallInstructionJsonConverter : JsonConverter<IInstallInstruction>
    {
        public override IInstallInstruction Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotSupportedException();

        public override void Write(Utf8JsonWriter writer, IInstallInstruction value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case ModuleInfoInstallInstruction moduleInfoInstallInstruction:
                    JsonSerializer.Serialize(writer, moduleInfoInstallInstruction, CustomSourceGenerationContext.ModuleInfoInstallInstruction);
                    break;
                case CopyInstallInstruction copyInstallInstruction:
                    JsonSerializer.Serialize(writer, copyInstallInstruction, CustomSourceGenerationContext.CopyInstallInstruction);
                    break;
            }
        }
    }

    private static readonly JsonSerializerOptions _options = new()
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
            new JsonStringEnumConverter<DialogType>(),
            new JsonStringEnumConverter<GamePlatform>(),
            new JsonStringEnumConverter<GameStore>(),
            new JsonStringEnumConverter<InstallInstructionType>(),
            new JsonStringEnumConverter<NotificationType>(),

            new JsonStringEnumConverter<ApplicationVersionType>(),
            new JsonStringEnumConverter<ModuleIssueType>(),
            new JsonStringEnumConverter<LoadType>(),
        }
    };

    internal static readonly SourceGenerationContext CustomSourceGenerationContext = new(_options);
}