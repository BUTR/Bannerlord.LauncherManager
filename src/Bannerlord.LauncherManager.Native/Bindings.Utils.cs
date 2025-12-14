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
#if DEBUG
        using var logger = LogMethod(p_modules);
#else
        using var logger = LogMethod();
#endif
        try
        {
            //if (p_modules is null)
            //    return return_value_json.AsValue(["Invalid module list!"], CustomSourceGenerationContext.StringArray, false);

            var modules = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_modules, CustomSourceGenerationContext.ModuleInfoExtendedArray)
                .Where(x => x != null!).ToArray();

            var result = LoadOrderChecker.IsLoadOrderCorrect(modules).ToArray();

            return return_value_json.AsValue(result, CustomSourceGenerationContext.StringArray, false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "utils_get_dependency_hint", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_string* GetDependencyHint([IsConst<IsPtrConst>] param_json* p_module)
    {
#if DEBUG
        using var logger = LogMethod(p_module);
#else
        using var logger = LogMethod();
#endif
        try
        {
            //if (p_module is null)
            //    return return_value_string.AsValue(string.Empty, false);

            var module = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module, CustomSourceGenerationContext.ModuleInfoExtended);

            var result = ModuleDependencyConstructor.GetDependencyHint(module);

            return return_value_string.AsValue(result, false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_string.AsException(e, false);
        }
    }
    [UnmanagedCallersOnly(EntryPoint = "utils_render_module_issue", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_string* RenderModuleIssue([IsConst<IsPtrConst>] param_json* p_module_issue)
    {
#if DEBUG
        using var logger = LogMethod(p_module_issue);
#else
        using var logger = LogMethod();
#endif
        try
        {
            var moduleIssue = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module_issue, CustomSourceGenerationContext.ModuleIssue);

            var result = ModuleIssueRenderer.Render(moduleIssue);

            return return_value_string.AsValue(result, false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_string.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "utils_load_localization", CallConvs = [typeof(CallConvCdecl)])]
    public static return_value_void* LoadLocalization(param_string* p_xml)
    {
#if DEBUG
        using var logger = LogMethod(p_xml);
#else
        using var logger = LogMethod();
#endif
        try
        {
            var doc = ReaderUtils2.Read(param_string.ToSpan(p_xml));
            BUTRLocalizationManager.LoadLanguage(doc);

            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "utils_set_language", CallConvs = [typeof(CallConvCdecl)])]
    public static return_value_void* SetLanguage(param_string* p_language)
    {
#if DEBUG
        using var logger = LogMethod(p_language);
#else
        using var logger = LogMethod();
#endif
        try
        {
            var language = new string(param_string.ToSpan(p_language));

            BUTRLocalizationManager.ActiveLanguage = language;

            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "utils_localize_string", CallConvs = [typeof(CallConvCdecl)])]
    public static return_value_string* LocalizeString(param_string* p_template, param_json* p_values)
    {
#if DEBUG
        using var logger = LogMethod(p_template, p_values);
#else
        using var logger = LogMethod();
#endif
        try
        {
            //if (p_values is null)
            //    return return_value_string.AsValue(string.Empty, false);

            var template = new string(param_string.ToSpan(p_template));
            var values = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_values, CustomSourceGenerationContext.DictionaryStringString);

            var result = new BUTRTextObject(template, values).ToString();

            return return_value_string.AsValue(result, false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_string.AsException(e, false);
        }
    }
}