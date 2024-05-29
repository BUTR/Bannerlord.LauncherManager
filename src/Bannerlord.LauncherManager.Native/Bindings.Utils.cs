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
                .Where(x => x is not null).ToArray();

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
}