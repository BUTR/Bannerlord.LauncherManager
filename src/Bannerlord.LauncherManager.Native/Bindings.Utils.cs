using Bannerlord.LauncherManager.Localization;
using Bannerlord.LauncherManager.Native.Utils;
using Bannerlord.LauncherManager.Utils;

using BUTR.NativeAOT.Shared;

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bannerlord.LauncherManager.Native;

public static unsafe partial class Bindings
{
    [UnmanagedCallersOnly(EntryPoint = "utils_is_load_order_correct", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_json* IsLoadOrderCorrect([IsConst<IsPtrConst>] param_json* p_modules)
    {
        Logger.LogInput(p_modules);
        try
        {
            //if (p_modules is null)
            //    return return_value_json.AsValue(["Invalid module list!"], CustomSourceGenerationContext.StringArray, false);

            var modules = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_modules, CustomSourceGenerationContext.ModuleInfoExtendedArray)
                .Where(x => x != null!).ToArray();

            var result = LoadOrderChecker.IsLoadOrderCorrect(modules).ToArray();

            Logger.LogOutput();
            return return_value_json.AsValue(result, CustomSourceGenerationContext.StringArray, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "utils_get_dependency_hint", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_string* GetDependencyHint([IsConst<IsPtrConst>] param_json* p_module)
    {
        Logger.LogInput(p_module);
        try
        {
            //if (p_module is null)
            //    return return_value_string.AsValue(string.Empty, false);

            var module = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module, CustomSourceGenerationContext.ModuleInfoExtended);

            var result = ModuleDependencyConstructor.GetDependencyHint(module);

            Logger.LogOutput(result);
            return return_value_string.AsValue(result, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_string.AsException(e, false);
        }
    }
    [UnmanagedCallersOnly(EntryPoint = "utils_render_module_issue", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_string* RenderModuleIssue([IsConst<IsPtrConst>] param_json* p_module_issue)
    {
        Logger.LogInput(p_module_issue);
        try
        {
            var moduleIssue = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module_issue, CustomSourceGenerationContext.ModuleIssue);

            var result = ModuleIssueRenderer.Render(moduleIssue);

            Logger.LogOutput(result);
            return return_value_string.AsValue(result, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_string.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "utils_load_localization", CallConvs = [typeof(CallConvCdecl)])]
    public static return_value_void* LoadLocalization(param_string* p_xml)
    {
        Logger.LogInput();
        try
        {
            var doc = ReaderUtils2.Read(param_string.ToSpan(p_xml));
            BUTRLocalizationManager.LoadLanguage(doc);

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "utils_set_language", CallConvs = [typeof(CallConvCdecl)])]
    public static return_value_void* SetLanguage(param_string* p_language)
    {
        Logger.LogInput(p_language);
        try
        {
            var language = new string(param_string.ToSpan(p_language));

            BUTRLocalizationManager.ActiveLanguage = language;

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "utils_localize_string", CallConvs = [typeof(CallConvCdecl)])]
    public static return_value_string* LocalizeString(param_string* p_template, param_json* p_values)
    {
        Logger.LogInput(p_template, p_values);
        try
        {
            //if (p_values is null)
            //    return return_value_string.AsValue(string.Empty, false);

            var template = new string(param_string.ToSpan(p_template));
            var values = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_values, CustomSourceGenerationContext.DictionaryStringString);

            var result = new BUTRTextObject(template, values).ToString();

            Logger.LogOutput(result);
            return return_value_string.AsValue(result, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_string.AsException(e, false);
        }
    }
}