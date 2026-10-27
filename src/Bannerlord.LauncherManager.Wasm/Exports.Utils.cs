using Bannerlord.LauncherManager.Localization;
using Bannerlord.LauncherManager.Native.Utils;
using Bannerlord.LauncherManager.Utils;

using System;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;

using static Bannerlord.LauncherManager.Native.Bindings;

namespace Bannerlord.LauncherManager.Wasm;

public static partial class UtilsExports
{
    [JSExport]
    public static string IsLoadOrderCorrect(string modules) =>
        Json.Serialize(LoadOrderChecker.IsLoadOrderCorrect(Json.DeserializeModules(modules)).ToArray(), CustomSourceGenerationContext.StringArray);

    [JSExport]
    public static string GetDependencyHint(string module) => ModuleDependencyConstructor.GetDependencyHint(Json.DeserializeModule(module));

    [JSExport]
    public static string RenderModuleIssue(string moduleIssue) =>
        ModuleIssueRenderer.Render(Json.Deserialize(moduleIssue, CustomSourceGenerationContext.ModuleIssue));

    [JSExport]
    public static void LoadLocalization(string xml) => BUTRLocalizationManager.LoadLanguage(ReaderUtils2.Read(xml));

    [JSExport]
    public static void SetLanguage(string language) => BUTRLocalizationManager.ActiveLanguage = language;

    [JSExport]
    public static string LocalizeString(string template, string values) =>
        new BUTRTextObject(template, Json.Deserialize(values, CustomSourceGenerationContext.DictionaryStringString)).ToString();

    /// <summary>
    /// The JS log callback, the native Logger's equivalent. The level is Microsoft.Extensions.Logging.LogLevel
    /// </summary>
    [JSExport]
    public static void SetLogCallback([JSMarshalAs<JSType.Function<JSType.Number, JSType.String>>] Action<int, string>? log) => Native.Logger.SetCallback(log);
}
