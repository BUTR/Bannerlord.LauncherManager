using Bannerlord.LauncherManager.Models;
using Bannerlord.LauncherManager.Native.Utils;
using Bannerlord.ModuleManager;

using BUTR.NativeAOT.Shared;

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bannerlord.LauncherManager.Native;

public static unsafe partial class Bindings
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate return_value_bool* N_IsSelected(param_ptr* p_owner, param_string* p_module_id);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate return_value_bool* N_GetSelected(param_ptr* p_owner, param_string* p_module_id);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate return_value_void* N_SetSelected(param_ptr* p_owner, param_string* p_module_id, param_bool value);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate return_value_bool* N_GetDisabled(param_ptr* p_owner, param_string* p_module_id);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate return_value_void* N_SetDisabled(param_ptr* p_owner, param_string* p_module_id, param_bool value);

    private static readonly ApplicationVersionComparer _applicationVersionComparer = new();

    [UnmanagedCallersOnly(EntryPoint = "bmm_sort", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_json* Sort([IsConst<IsPtrConst>] param_json* p_source)
    {
        Logger.LogInput(p_source);
        try
        {
            //if (p_source is null)
            //    return return_value_json.AsValue([], CustomSourceGenerationContext.ModuleInfoExtendedArray, false);

            var source = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_source, CustomSourceGenerationContext.ModuleInfoExtendedArray)
                .Where(x => x is not null).ToArray();

            var result = ModuleSorter.Sort(source).ToArray();

            Logger.LogOutput(result);
            return return_value_json.AsValue(result, CustomSourceGenerationContext.ModuleInfoExtendedArray, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "bmm_sort_with_options", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_json* SortWithOptions([IsConst<IsPtrConst>] param_json* p_source, [IsConst<IsPtrConst>] param_json* p_options)
    {
        Logger.LogInput(p_source, p_options);
        try
        {
            //if (p_source is null || p_options is null)
            //    return return_value_json.AsValue([], CustomSourceGenerationContext.ModuleInfoExtendedArray, false);

            var source = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_source, CustomSourceGenerationContext.ModuleInfoExtendedArray)
                .Where(x => x is not null).ToArray();
            var options = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_options, CustomSourceGenerationContext.ModuleSorterOptions);

            var result = ModuleSorter.Sort(source, options).ToArray();

            Logger.LogOutput(result);
            return return_value_json.AsValue(result, CustomSourceGenerationContext.ModuleInfoExtendedArray, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "bmm_are_all_dependencies_of_module_present", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_bool* AreAllDependenciesOfModulePresent([IsConst<IsPtrConst>] param_json* p_source, [IsConst<IsPtrConst>] param_json* p_module)
    {
        Logger.LogInput(p_source, p_module);
        try
        {
            //if (p_source is null || p_module is null)
            //    return return_value_bool.AsValue(false, false);

            var source = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_source, CustomSourceGenerationContext.ModuleInfoExtendedArray)
                .Where(x => x is not null).ToArray();
            var module = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module, CustomSourceGenerationContext.ModuleInfoExtended);

            var result = ModuleUtilities.AreDependenciesPresent(source, module);

            Logger.LogOutput(result);
            return return_value_bool.AsValue(result, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_bool.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "bmm_get_dependent_modules_of", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_json* GetDependentModulesOf([IsConst<IsPtrConst>] param_json* p_source, [IsConst<IsPtrConst>] param_json* p_module)
    {
        Logger.LogInput(p_source, p_module);
        try
        {
            //if (p_source is null || p_module is null)
            //    return return_value_json.AsValue([], CustomSourceGenerationContext.ModuleInfoExtendedArray, false);

            var source = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_source, CustomSourceGenerationContext.ModuleInfoExtendedArray)
                .Where(x => x is not null).ToArray();
            var module = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module, CustomSourceGenerationContext.ModuleInfoExtended);

            var result = ModuleUtilities.GetDependencies(source, module).ToArray();

            Logger.LogOutput(result);
            return return_value_json.AsValue(result, CustomSourceGenerationContext.ModuleInfoExtendedArray, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "bmm_get_dependent_modules_of_with_options", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_json* GetDependentModulesOfWithOptions([IsConst<IsPtrConst>] param_json* p_source, [IsConst<IsPtrConst>] param_json* p_module, [IsConst<IsPtrConst>] param_json* p_options)
    {
        Logger.LogInput(p_source, p_module, p_options);
        try
        {
            //if (p_source is null || p_module is null || p_options is null)
            //    return return_value_json.AsValue([], CustomSourceGenerationContext.ModuleInfoExtendedArray, false);

            var source = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_source, CustomSourceGenerationContext.ModuleInfoExtendedArray)
                .Where(x => x is not null).ToArray();
            var module = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module, CustomSourceGenerationContext.ModuleInfoExtended);
            var options = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_options, CustomSourceGenerationContext.ModuleSorterOptions);

            var result = ModuleUtilities.GetDependencies(source, module, options).ToArray();

            Logger.LogOutput(result);
            return return_value_json.AsValue(result, CustomSourceGenerationContext.ModuleInfoExtendedArray, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "bmm_validate_module", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_json* ValidateModule([IsConst<IsPtrConst>] param_ptr* p_owner, [IsConst<IsPtrConst>] param_json* p_modules, [IsConst<IsPtrConst>] param_json* p_target_module,
        [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst<IsPtrConst>>]
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, return_value_bool*> p_is_selected)
    {
        Logger.LogInput(p_modules, p_target_module);
        try
        {
            //if (p_modules is null || p_target_module is null || p_is_selected is null)
            //    return return_value_json.AsValue([new ModuleIssue(new ModuleInfoExtended(), "", ModuleIssueType.NONE, "Invalid input!", ApplicationVersionRange.Empty)], CustomSourceGenerationContext.ModuleIssueArray, false);

            var modules = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_modules, CustomSourceGenerationContext.ModuleInfoExtendedArray)
                .Where(x => x is not null).ToArray();
            var targetModule = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_target_module, CustomSourceGenerationContext.ModuleInfoExtended);

            var isSelected = Marshal.GetDelegateForFunctionPointer<N_IsSelected>(new IntPtr(p_is_selected));

            var result = ModuleUtilities.ValidateModule(modules, targetModule, module =>
            {
                Logger.LogInput();
                fixed (char* pModuleId = module.Id ?? string.Empty)
                {
                    Logger.LogPinned(pModuleId);

                    using var resultStr = SafeStructMallocHandle.Create(isSelected(p_owner, (param_string*) pModuleId), true);
                    var result = resultStr.ValueAsBool();
                    Logger.LogOutput(result);
                    return result;
                }
            }).ToArray();

            Logger.LogOutput(result);
            return return_value_json.AsValue(result, CustomSourceGenerationContext.ModuleIssueArray, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "bmm_validate_load_order", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_json* ValidateLoadOrder([IsConst<IsPtrConst>] param_json* p_modules, [IsConst<IsPtrConst>] param_json* p_target_module)
    {
        Logger.LogInput(p_modules, p_target_module);
        try
        {
            //if (p_modules is null || p_target_module is null)
            //    return return_value_json.AsValue([new ModuleIssue(new ModuleInfoExtended(), "", ModuleIssueType.NONE, "Invalid input!", ApplicationVersionRange.Empty)], CustomSourceGenerationContext.ModuleIssueArray, false);

            var modules = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_modules, CustomSourceGenerationContext.ModuleInfoExtendedArray)
                .Where(x => x is not null).ToArray();
            var targetModule = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_target_module, CustomSourceGenerationContext.ModuleInfoExtended);

            var result = ModuleUtilities.ValidateLoadOrder(modules, targetModule).ToArray();

            Logger.LogOutput(result);
            return return_value_json.AsValue(result, CustomSourceGenerationContext.ModuleIssueArray, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "bmm_enable_module", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_void* EnableModule([IsConst<IsPtrConst>] param_ptr* p_owner, [IsConst<IsPtrConst>] param_json* p_module, [IsConst<IsPtrConst>] param_json* p_target_module,
        [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst<IsPtrConst>>]
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, return_value_bool*> p_get_selected,
        [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst, IsNotConst<IsPtrConst>>]
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_bool, return_value_void*> p_set_selected,
        [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst<IsPtrConst>>]
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, return_value_bool*> p_get_disabled,
        [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst, IsNotConst<IsPtrConst>>]
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_bool, return_value_void*> p_set_disabled)
    {
        Logger.LogInput(p_module, p_target_module);
        try
        {
            //if (p_module is null || p_target_module is null || p_get_selected is null || p_set_selected is null || p_get_disabled is null || p_set_disabled is null)
            //    return return_value_void.AsValue(false);

            var modules = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module, CustomSourceGenerationContext.ModuleInfoExtendedArray)
                .Where(x => x is not null).ToArray();
            var targetModule = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_target_module, CustomSourceGenerationContext.ModuleInfoExtended);

            var getSelected = Marshal.GetDelegateForFunctionPointer<N_GetSelected>(new IntPtr(p_get_selected));
            var setSelected = Marshal.GetDelegateForFunctionPointer<N_SetSelected>(new IntPtr(p_set_selected));
            var getDisabled = Marshal.GetDelegateForFunctionPointer<N_GetDisabled>(new IntPtr(p_get_disabled));
            var setDisabled = Marshal.GetDelegateForFunctionPointer<N_SetDisabled>(new IntPtr(p_set_disabled));

            ModuleUtilities.EnableModule(modules, targetModule, module =>
            {
                Logger.LogInput();
                fixed (char* pModuleId = module.Id ?? string.Empty)
                {
                    Logger.LogPinned(pModuleId);

                    using var resultStr = SafeStructMallocHandle.Create(getSelected(p_owner, (param_string*) pModuleId), true);
                    var result = resultStr.ValueAsBool();
                    Logger.LogOutput(result);
                    return result;
                }
            }, (module, value) =>
            {
                Logger.LogInput();
                fixed (char* pModuleId = module.Id ?? string.Empty)
                {
                    Logger.LogPinned(pModuleId);

                    using var resultStr = SafeStructMallocHandle.Create(setSelected(p_owner, (param_string*) pModuleId, value), true);
                    resultStr.ValueAsVoid();
                    Logger.LogOutput();
                }
            }, module =>
            {
                Logger.LogInput();
                fixed (char* pModuleId = module.Id ?? string.Empty)
                {
                    Logger.LogPinned(pModuleId);

                    using var resultStr = SafeStructMallocHandle.Create(getDisabled(p_owner, (param_string*) pModuleId), true);
                    var result = resultStr.ValueAsBool();
                    Logger.LogOutput(result);
                    return result;
                }
            }, (module, value) =>
            {
                Logger.LogInput();
                fixed (char* pModuleId = module.Id ?? string.Empty)
                {
                    Logger.LogPinned(pModuleId);

                    using var resultStr = SafeStructMallocHandle.Create(setDisabled(p_owner, (param_string*) pModuleId, value), true);
                    resultStr.ValueAsVoid();
                    Logger.LogOutput();
                }
            });

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "bmm_disable_module", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_void* DisableModule([IsConst<IsPtrConst>] param_ptr* p_owner, [IsConst<IsPtrConst>] param_json* p_module, [IsConst<IsPtrConst>] param_json* p_target_module,
        [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst<IsPtrConst>>]
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, return_value_bool*> p_get_selected,
        [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst, IsNotConst<IsPtrConst>>]
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_bool, return_value_void*> p_set_selected,
        [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst<IsPtrConst>>]
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, return_value_bool*> p_get_disabled,
        [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst, IsNotConst<IsPtrConst>>]
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_bool, return_value_void*> p_set_disabled)
    {
        Logger.LogInput(p_module, p_target_module);
        try
        {
            //if (p_module is null || p_target_module is null || p_get_selected is null || p_set_selected is null || p_get_disabled is null || p_set_disabled is null)
            //    return return_value_void.AsValue(false);

            var modules = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module, CustomSourceGenerationContext.ModuleInfoExtendedArray)
                .Where(x => x is not null).ToArray();
            var targetModule = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_target_module, CustomSourceGenerationContext.ModuleInfoExtended);

            var getSelected = Marshal.GetDelegateForFunctionPointer<N_GetSelected>(new IntPtr(p_get_selected));
            var setSelected = Marshal.GetDelegateForFunctionPointer<N_SetSelected>(new IntPtr(p_set_selected));
            var getDisabled = Marshal.GetDelegateForFunctionPointer<N_GetDisabled>(new IntPtr(p_get_disabled));
            var setDisabled = Marshal.GetDelegateForFunctionPointer<N_SetDisabled>(new IntPtr(p_set_disabled));

            ModuleUtilities.DisableModule(modules, targetModule, module =>
            {
                Logger.LogInput();
                fixed (char* pModuleId = module.Id ?? string.Empty)
                {
                    Logger.LogPinned(pModuleId);

                    using var resultStr = SafeStructMallocHandle.Create(getSelected(p_owner, (param_string*) pModuleId), true);
                    var result = resultStr.ValueAsBool();
                    Logger.LogOutput(result);
                    return result;
                }
            }, (module, value) =>
            {
                Logger.LogInput();
                fixed (char* pModuleId = module.Id ?? string.Empty)
                {
                    Logger.LogPinned(pModuleId);

                    using var resultStr = SafeStructMallocHandle.Create(setSelected(p_owner, (param_string*) pModuleId, value), true);
                    resultStr.ValueAsVoid();
                    Logger.LogOutput();
                }
            }, module =>
            {
                Logger.LogInput();
                fixed (char* pModuleId = module.Id ?? string.Empty)
                {
                    Logger.LogPinned(pModuleId);

                    using var resultStr = SafeStructMallocHandle.Create(getDisabled(p_owner, (param_string*) pModuleId), true);
                    var result = resultStr.ValueAsBool();
                    Logger.LogOutput(result);
                    return result;
                }
            }, (module, value) =>
            {
                Logger.LogInput();
                fixed (char* pModuleId = module.Id ?? string.Empty)
                {
                    Logger.LogPinned(pModuleId);

                    using var resultStr = SafeStructMallocHandle.Create(setDisabled(p_owner, (param_string*) pModuleId, value), true);
                    resultStr.ValueAsVoid();
                    Logger.LogOutput();
                }
            });

            Logger.LogOutput();
            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "bmm_get_module_info", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_json* GetModuleInfo([IsConst<IsPtrConst>] param_string* p_xml_content)
    {
        Logger.LogInput(p_xml_content);
        try
        {
            var doc = ReaderUtils2.Read(param_string.ToSpan(p_xml_content));

            var result = ModuleInfoExtended.FromXml(doc);

            Logger.LogOutput(result);
            return return_value_json.AsValue(result, CustomSourceGenerationContext.ModuleInfoExtended, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "bmm_get_module_info_with_path", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_json* GetModuleInfoWithPath([IsConst<IsPtrConst>] param_string* p_xml_content, [IsConst<IsPtrConst>] param_string* p_path)
    {
        Logger.LogInput(p_xml_content, p_path);
        try
        {
            var doc = ReaderUtils2.Read(param_string.ToSpan(p_xml_content));

            var module = ModuleInfoExtended.FromXml(doc);
            var result = new ModuleInfoExtendedWithPath(module, new string(param_string.ToSpan(p_path)));

            Logger.LogOutput(result);
            return return_value_json.AsValue(result, CustomSourceGenerationContext.ModuleInfoExtendedWithPath, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }
    [UnmanagedCallersOnly(EntryPoint = "bmm_get_module_info_with_metadata", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_json* GetModuleInfoWithMetadata([IsConst<IsPtrConst>] param_string* p_xml_content, [IsConst<IsPtrConst>] param_string* p_module_provider, [IsConst<IsPtrConst>] param_string* p_path)
    {
        Logger.LogInput(p_xml_content, p_path);
        try
        {
            var doc = ReaderUtils2.Read(param_string.ToSpan(p_xml_content));

            var module = ModuleInfoExtended.FromXml(doc);
            var result = new ModuleInfoExtendedWithMetadata(module, Enum.TryParse<ModuleProviderType>(new string(param_string.ToSpan(p_module_provider)), out var e) ? e : ModuleProviderType.Default, new string(param_string.ToSpan(p_path)));

            Logger.LogOutput(result);
            return return_value_json.AsValue(result, CustomSourceGenerationContext.ModuleInfoExtendedWithMetadata, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "bmm_get_sub_module_info", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_json* GetSubModuleInfo([IsConst<IsPtrConst>] param_string* p_xml_content)
    {
        Logger.LogInput(p_xml_content);
        try
        {
            var doc = ReaderUtils2.Read(param_string.ToSpan(p_xml_content));

            var result = SubModuleInfoExtended.FromXml(doc);
            if (result is null)
            {
                var e = new InvalidOperationException("Invalid xml content!");
                Logger.LogException(e);
                return return_value_json.AsException(e, false);
            }

            return return_value_json.AsValue(result, CustomSourceGenerationContext.SubModuleInfoExtended, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "bmm_parse_application_version", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_json* ParseApplicationVersion([IsConst<IsPtrConst>] param_string* p_application_version)
    {
        Logger.LogInput(p_application_version);
        try
        {
            var result = ApplicationVersion.TryParse(new string(param_string.ToSpan(p_application_version)), out var v) ? v : ApplicationVersion.Empty;

            return return_value_json.AsValue(result, CustomSourceGenerationContext.ApplicationVersion, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "bmm_compare_versions", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_int32* CompareVersions([IsConst<IsPtrConst>] param_json* p_x, [IsConst<IsPtrConst>] param_json* p_y)
    {
        Logger.LogInput(p_x, p_y);
        try
        {
            var x = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_x, CustomSourceGenerationContext.ApplicationVersion);
            var y = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_y, CustomSourceGenerationContext.ApplicationVersion);

            var result = _applicationVersionComparer.Compare(x, y);

            Logger.LogOutput(result);
            return return_value_int32.AsValue(result, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_int32.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "bmm_get_dependencies_all", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_json* GetDependenciesAll([IsConst<IsPtrConst>] param_json* p_module)
    {
        Logger.LogInput(p_module);
        try
        {
            //if (p_module is null)
            //    return return_value_json.AsValue([], CustomSourceGenerationContext.DependentModuleMetadataArray, false);

            var module = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module, CustomSourceGenerationContext.ModuleInfoExtended);

            var result = module.DependenciesAllDistinct().ToArray();

            Logger.LogOutput(result);
            return return_value_json.AsValue(result, CustomSourceGenerationContext.DependentModuleMetadataArray, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }
    [UnmanagedCallersOnly(EntryPoint = "bmm_get_dependencies_load_before_this", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_json* GetDependenciesLoadBeforeThis([IsConst<IsPtrConst>] param_json* p_module)
    {
        Logger.LogInput(p_module);
        try
        {
            //if (p_module is null)
            //    return return_value_json.AsValue([], CustomSourceGenerationContext.DependentModuleMetadataArray, false);

            var module = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module, CustomSourceGenerationContext.ModuleInfoExtended);

            var result = module.DependenciesLoadBeforeThisDistinct().ToArray();

            Logger.LogOutput(result);
            return return_value_json.AsValue(result, CustomSourceGenerationContext.DependentModuleMetadataArray, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }
    [UnmanagedCallersOnly(EntryPoint = "bmm_get_dependencies_load_after_this", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_json* GetDependenciesLoadAfterThis([IsConst<IsPtrConst>] param_json* p_module)
    {
        Logger.LogInput(p_module);
        try
        {
            //if (p_module is null)
            //    return return_value_json.AsValue([], CustomSourceGenerationContext.DependentModuleMetadataArray, false);

            var module = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module, CustomSourceGenerationContext.ModuleInfoExtended);

            var result = module.DependenciesLoadAfterThisDistinct().ToArray();

            Logger.LogOutput(result);
            return return_value_json.AsValue(result, CustomSourceGenerationContext.DependentModuleMetadataArray, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }
    [UnmanagedCallersOnly(EntryPoint = "bmm_get_dependencies_incompatibles", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_json* GetDependenciesIncompatibles([IsConst<IsPtrConst>] param_json* p_module)
    {
        Logger.LogInput(p_module);
        try
        {
            //if (p_module is null)
            //    return return_value_json.AsValue([], CustomSourceGenerationContext.DependentModuleMetadataArray, false);

            var module = BUTR.NativeAOT.Shared.Utils.DeserializeJson(p_module, CustomSourceGenerationContext.ModuleInfoExtended);

            var result = module.DependenciesIncompatiblesDistinct().ToArray();

            Logger.LogOutput(result);
            return return_value_json.AsValue(result, CustomSourceGenerationContext.DependentModuleMetadataArray, false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return return_value_json.AsException(e, false);
        }
    }
}