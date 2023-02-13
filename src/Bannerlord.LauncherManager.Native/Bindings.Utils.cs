using Bannerlord.LauncherManager.Localization;
using Bannerlord.LauncherManager.Utils;

using BUTR.NativeAOT.Shared;

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bannerlord.LauncherManager.Native;

public static unsafe partial class Bindings
{
    [UnmanagedCallersOnly(EntryPoint = "utils_is_load_order_correct", CallConvs = new[] { typeof(CallConvCdecl) }), IsNotConst<IsPtrConst>]
    public static return_value_json* IsLoadOrderCorrect([IsConst<IsPtrConst>] param_json* p_modules)
    {
        Logger.LogInput(p_modules);
        try
        {
            var modules = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_modules, CustomSourceGenerationContext.ModuleInfoExtendedArray);

            var result = LoadOrderChecker.IsLoadOrderCorrect(modules).ToArray();

            Logger.LogOutput(result);
            return return_value_json.AsValue(result, CustomSourceGenerationContext.StringArray, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "utils_get_dependency_hint", CallConvs = new[] { typeof(CallConvCdecl) }), IsNotConst<IsPtrConst>]
    public static return_value_string* GetDependencyHint([IsConst<IsPtrConst>] param_json* p_module)
    {
        Logger.LogInput(p_module);
        try
        {
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

    [UnmanagedCallersOnly(EntryPoint = "utils_translate", CallConvs = new[] { typeof(CallConvCdecl) }), IsNotConst<IsPtrConst>]
    public static return_value_string* UtilsTranslate([IsConst<IsPtrConst>] param_string* p_text)
    {
        Logger.LogInput(p_text);
        try
        {
            var text = new string(param_string.ToSpan(p_text));

            var result = new BUTRTextObject(text).ToString();

            Logger.LogOutput(result, nameof(UtilsTranslate));
            return return_value_string.AsValue(result, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_string.AsException(e, false);
        }
    }
}