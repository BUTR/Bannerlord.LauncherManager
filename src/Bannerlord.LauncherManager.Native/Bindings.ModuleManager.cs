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

    /// <param name="p_modules">The JSON of the modules, or "#" and the handle of a list kept by bmm_register_modules</param>
    private static ModuleInfoExtended[] DeserializeModules(param_data* p_modules) =>
        (p_modules is null ? null : ModuleLists.FromArgument(MemoryMarshal.CreateReadOnlySpanFromNullTerminated((byte*) p_modules)))
        ?? DeserializeJsonNative(p_modules, CustomSourceGenerationContext.ModuleInfoExtendedArray).Where(x => x != null!).ToArray();

    /// <summary>
    /// The positions of the result's modules in the source, see ModuleLists.Positions, or the modules when it can't
    /// </summary>
    private static return_value_data* PositionsValue(ModuleInfoExtended[] source, ModuleInfoExtended[] result) =>
        ModuleLists.Positions(source, result) is { } positions
            ? JsonValue(positions, false)
            : JsonValue(result, CustomSourceGenerationContext.ModuleInfoExtendedArray, false);

    /// <summary>
    /// Parses a module list and keeps it until bmm_release_modules, the module list parameters then take "#" and the
    /// handle instead of the JSON
    /// </summary>
    [UnmanagedCallersOnly(EntryPoint = "bmm_register_modules", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_int32* RegisterModules([IsConst<IsPtrConst>] param_data* p_modules)
    {
#if DEBUG
        using var logger = LogMethod(p_modules);
#else
        using var logger = LogMethod();
#endif
        try
        {
            var modules = DeserializeJsonNative(p_modules, CustomSourceGenerationContext.ModuleInfoExtendedArray)
                .Where(x => x != null!).ToArray();

            return return_value_int32.AsValue(ModuleLists.Register(modules), false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_int32.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "bmm_release_modules", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_void* ReleaseModules(param_int handle)
    {
#if DEBUG
        using var logger = LogMethod(&handle);
#else
        using var logger = LogMethod();
#endif
        try
        {
            ModuleLists.Release(handle);

            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "bmm_sort", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_data* Sort([IsConst<IsPtrConst>] param_data* p_source)
    {
#if DEBUG
        using var logger = LogMethod(p_source);
#else
        using var logger = LogMethod();
#endif
        try
        {
            //if (p_source is null)
            //    return return_value_json.AsValue([], CustomSourceGenerationContext.ModuleInfoExtendedArray, false);

            var source = DeserializeModules(p_source);

            var result = ModuleSorter.Sort(source).ToArray();

            return PositionsValue(source, result);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_data.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "bmm_sort_with_options", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_data* SortWithOptions([IsConst<IsPtrConst>] param_data* p_source, [IsConst<IsPtrConst>] param_data* p_options)
    {
#if DEBUG
        using var logger = LogMethod(p_source, p_options);
#else
        using var logger = LogMethod();
#endif
        try
        {
            //if (p_source is null || p_options is null)
            //    return return_value_json.AsValue([], CustomSourceGenerationContext.ModuleInfoExtendedArray, false);

            var source = DeserializeModules(p_source);
            var options = DeserializeJsonNative(p_options, CustomSourceGenerationContext.ModuleSorterOptions);

            var result = ModuleSorter.Sort(source, options).ToArray();

            return PositionsValue(source, result);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_data.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "bmm_are_all_dependencies_of_module_present", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_bool* AreAllDependenciesOfModulePresent([IsConst<IsPtrConst>] param_data* p_source, [IsConst<IsPtrConst>] param_data* p_module)
    {
#if DEBUG
        using var logger = LogMethod(p_source, p_module);
#else
        using var logger = LogMethod();
#endif
        try
        {
            //if (p_source is null || p_module is null)
            //    return return_value_bool.AsValue(false, false);

            var source = DeserializeModules(p_source);
            var module = DeserializeJsonNative(p_module, CustomSourceGenerationContext.ModuleInfoExtended);

            var result = ModuleUtilities.AreDependenciesPresent(source, module);

            return return_value_bool.AsValue(result, false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_bool.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "bmm_get_dependent_modules_of", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_data* GetDependentModulesOf([IsConst<IsPtrConst>] param_data* p_source, [IsConst<IsPtrConst>] param_data* p_module)
    {
#if DEBUG
        using var logger = LogMethod(p_source, p_module);
#else
        using var logger = LogMethod();
#endif
        try
        {
            //if (p_source is null || p_module is null)
            //    return return_value_json.AsValue([], CustomSourceGenerationContext.ModuleInfoExtendedArray, false);

            var source = DeserializeModules(p_source);
            var module = DeserializeJsonNative(p_module, CustomSourceGenerationContext.ModuleInfoExtended);

            var result = ModuleUtilities.GetDependencies(source, module).ToArray();

            return PositionsValue(source, result);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_data.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "bmm_get_dependent_modules_of_with_options", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_data* GetDependentModulesOfWithOptions([IsConst<IsPtrConst>] param_data* p_source, [IsConst<IsPtrConst>] param_data* p_module, [IsConst<IsPtrConst>] param_data* p_options)
    {
#if DEBUG
        using var logger = LogMethod(p_source, p_module, p_options);
#else
        using var logger = LogMethod();
#endif
        try
        {
            //if (p_source is null || p_module is null || p_options is null)
            //    return return_value_json.AsValue([], CustomSourceGenerationContext.ModuleInfoExtendedArray, false);

            var source = DeserializeModules(p_source);
            var module = DeserializeJsonNative(p_module, CustomSourceGenerationContext.ModuleInfoExtended);
            var options = DeserializeJsonNative(p_options, CustomSourceGenerationContext.ModuleSorterOptions);

            var result = ModuleUtilities.GetDependencies(source, module, options).ToArray();

            return PositionsValue(source, result);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_data.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "bmm_validate_module", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_data* ValidateModule([IsConst<IsPtrConst>] param_ptr* p_owner, [IsConst<IsPtrConst>] param_data* p_modules, [IsConst<IsPtrConst>] param_data* p_target_module,
        [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst<IsPtrConst>>]
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, return_value_bool*> p_is_selected)
    {
#if DEBUG
        using var logger = LogMethod(p_modules, p_target_module);
#else
        using var logger = LogMethod();
#endif
        try
        {
            //if (p_modules is null || p_target_module is null || p_is_selected is null)
            //    return return_value_json.AsValue([new ModuleIssue(new ModuleInfoExtended(), "", ModuleIssueType.NONE, "Invalid input!", ApplicationVersionRange.Empty)], CustomSourceGenerationContext.ModuleIssueArray, false);

            var modules = DeserializeModules(p_modules);
            var targetModule = DeserializeJsonNative(p_target_module, CustomSourceGenerationContext.ModuleInfoExtended);

            var isSelected = Marshal.GetDelegateForFunctionPointer<N_IsSelected>(new IntPtr(p_is_selected));

            var result = ModuleUtilities.ValidateModule(modules, targetModule, module =>
            {
#if DEBUG
                using var logger = LogCallbackMethod(nameof(ValidateModule));
#else
                using var logger = LogCallbackMethod(nameof(ValidateModule));
#endif
                try
                {
                    fixed (char* pModuleId = module.Id ?? string.Empty)
                    {
                        using var resultStr = SafeStructMallocHandle.Create(isSelected(p_owner, (param_string*) pModuleId), true);
                        var result = resultStr.ValueAsBool();
                        return result;
                    }
                }
                catch (Exception e)
                {
                    logger.LogException(e);
                    return false;
                }
            }).ToArray();

            return JsonValue(result, CustomSourceGenerationContext.ModuleIssueArray, false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_data.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "bmm_validate_load_order", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_data* ValidateLoadOrder([IsConst<IsPtrConst>] param_data* p_modules, [IsConst<IsPtrConst>] param_data* p_target_module)
    {
#if DEBUG
        using var logger = LogMethod(p_modules, p_target_module);
#else
        using var logger = LogMethod();
#endif
        try
        {
            //if (p_modules is null || p_target_module is null)
            //    return return_value_json.AsValue([new ModuleIssue(new ModuleInfoExtended(), "", ModuleIssueType.NONE, "Invalid input!", ApplicationVersionRange.Empty)], CustomSourceGenerationContext.ModuleIssueArray, false);

            var modules = DeserializeModules(p_modules);
            var targetModule = DeserializeJsonNative(p_target_module, CustomSourceGenerationContext.ModuleInfoExtended);

            var result = ModuleUtilities.ValidateLoadOrder(modules, targetModule).ToArray();

            return JsonValue(result, CustomSourceGenerationContext.ModuleIssueArray, false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_data.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "bmm_enable_module", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_void* EnableModule([IsConst<IsPtrConst>] param_ptr* p_owner, [IsConst<IsPtrConst>] param_data* p_module, [IsConst<IsPtrConst>] param_data* p_target_module,
        [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst<IsPtrConst>>]
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, return_value_bool*> p_get_selected,
        [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst, IsNotConst<IsPtrConst>>]
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_bool, return_value_void*> p_set_selected,
        [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst<IsPtrConst>>]
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, return_value_bool*> p_get_disabled,
        [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst, IsNotConst<IsPtrConst>>]
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_bool, return_value_void*> p_set_disabled)
    {
#if DEBUG
        using var logger = LogMethod(p_module, p_target_module);
#else
        using var logger = LogMethod();
#endif
        try
        {
            //if (p_module is null || p_target_module is null || p_get_selected is null || p_set_selected is null || p_get_disabled is null || p_set_disabled is null)
            //    return return_value_void.AsValue(false);

            var modules = DeserializeModules(p_module);
            var targetModule = DeserializeJsonNative(p_target_module, CustomSourceGenerationContext.ModuleInfoExtended);

            var getSelected = Marshal.GetDelegateForFunctionPointer<N_GetSelected>(new IntPtr(p_get_selected));
            var setSelected = Marshal.GetDelegateForFunctionPointer<N_SetSelected>(new IntPtr(p_set_selected));
            var getDisabled = Marshal.GetDelegateForFunctionPointer<N_GetDisabled>(new IntPtr(p_get_disabled));
            var setDisabled = Marshal.GetDelegateForFunctionPointer<N_SetDisabled>(new IntPtr(p_set_disabled));

            ModuleUtilities.EnableModule(modules, targetModule, module =>
            {
#if DEBUG
                using var logger = LogCallbackMethod($"{nameof(EnableModule)}1");
#else
                using var logger = LogCallbackMethod($"{nameof(EnableModule)}1");
#endif
                try
                {
                    fixed (char* pModuleId = module.Id ?? string.Empty)
                    {
                        using var resultStr = SafeStructMallocHandle.Create(getSelected(p_owner, (param_string*) pModuleId), true);
                        var result = resultStr.ValueAsBool();
                        return result;
                    }
                }
                catch (Exception e)
                {
                    logger.LogException(e);
                    return false;
                }
            }, (module, value) =>
            {
#if DEBUG
                using var logger = LogCallbackMethod($"{nameof(EnableModule)}2");
#else
                using var logger = LogCallbackMethod($"{nameof(EnableModule)}2");
#endif
                try
                {
                    fixed (char* pModuleId = module.Id ?? string.Empty)
                    {
                        using var resultStr = SafeStructMallocHandle.Create(setSelected(p_owner, (param_string*) pModuleId, value), true);
                        resultStr.ValueAsVoid();
                    }
                }
                catch (Exception e)
                {
                    logger.LogException(e);
                }
            }, module =>
            {
#if DEBUG
                using var logger = LogCallbackMethod($"{nameof(EnableModule)}3");
#else
                using var logger = LogCallbackMethod($"{nameof(EnableModule)}3");
#endif
                try
                {
                    fixed (char* pModuleId = module.Id ?? string.Empty)
                    {
                        using var resultStr = SafeStructMallocHandle.Create(getDisabled(p_owner, (param_string*) pModuleId), true);
                        var result = resultStr.ValueAsBool();
                        return result;
                    }
                }
                catch (Exception e)
                {
                    logger.LogException(e);
                    return false;
                }
            }, (module, value) =>
            {
#if DEBUG
                using var logger = LogCallbackMethod($"{nameof(EnableModule)}4");
#else
                using var logger = LogCallbackMethod($"{nameof(EnableModule)}4");
#endif
                try
                {
                    fixed (char* pModuleId = module.Id ?? string.Empty)
                    {
                        using var resultStr = SafeStructMallocHandle.Create(setDisabled(p_owner, (param_string*) pModuleId, value), true);
                        resultStr.ValueAsVoid();
                    }
                }
                catch (Exception e)
                {
                    logger.LogException(e);
                }
            });

            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "bmm_disable_module", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_void* DisableModule([IsConst<IsPtrConst>] param_ptr* p_owner, [IsConst<IsPtrConst>] param_data* p_module, [IsConst<IsPtrConst>] param_data* p_target_module,
        [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst<IsPtrConst>>]
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, return_value_bool*> p_get_selected,
        [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst, IsNotConst<IsPtrConst>>]
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_bool, return_value_void*> p_set_selected,
        [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst<IsPtrConst>>]
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, return_value_bool*> p_get_disabled,
        [ConstMeta<IsConst<IsPtrConst>, IsConst<IsPtrConst>, IsNotConst, IsNotConst<IsPtrConst>>]
        delegate* unmanaged[Cdecl]<param_ptr*, param_string*, param_bool, return_value_void*> p_set_disabled)
    {
#if DEBUG
        using var logger = LogMethod(p_module, p_target_module);
#else
        using var logger = LogMethod();
#endif
        try
        {
            //if (p_module is null || p_target_module is null || p_get_selected is null || p_set_selected is null || p_get_disabled is null || p_set_disabled is null)
            //    return return_value_void.AsValue(false);

            var modules = DeserializeModules(p_module);
            var targetModule = DeserializeJsonNative(p_target_module, CustomSourceGenerationContext.ModuleInfoExtended);

            var getSelected = Marshal.GetDelegateForFunctionPointer<N_GetSelected>(new IntPtr(p_get_selected));
            var setSelected = Marshal.GetDelegateForFunctionPointer<N_SetSelected>(new IntPtr(p_set_selected));
            var getDisabled = Marshal.GetDelegateForFunctionPointer<N_GetDisabled>(new IntPtr(p_get_disabled));
            var setDisabled = Marshal.GetDelegateForFunctionPointer<N_SetDisabled>(new IntPtr(p_set_disabled));

            ModuleUtilities.DisableModule(modules, targetModule, module =>
            {
#if DEBUG
                using var logger = LogCallbackMethod($"{nameof(DisableModule)}1");
#else
                using var logger = LogCallbackMethod($"{nameof(DisableModule)}1");
#endif
                try
                {
                    fixed (char* pModuleId = module.Id ?? string.Empty)
                    {
                        using var resultStr = SafeStructMallocHandle.Create(getSelected(p_owner, (param_string*) pModuleId), true);
                        var result = resultStr.ValueAsBool();
                        return result;
                    }
                }
                catch (Exception e)
                {
                    logger.LogException(e);
                    return false;
                }
            }, (module, value) =>
            {
#if DEBUG
                using var logger = LogCallbackMethod($"{nameof(DisableModule)}2");
#else
                using var logger = LogCallbackMethod($"{nameof(DisableModule)}2");
#endif
                try
                {
                    fixed (char* pModuleId = module.Id ?? string.Empty)
                    {
                        using var resultStr = SafeStructMallocHandle.Create(setSelected(p_owner, (param_string*) pModuleId, value), true);
                        resultStr.ValueAsVoid();
                    }
                }
                catch (Exception e)
                {
                    logger.LogException(e);
                }
            }, module =>
            {
#if DEBUG
                using var logger = LogCallbackMethod($"{nameof(DisableModule)}3");
#else
                using var logger = LogCallbackMethod($"{nameof(DisableModule)}3");
#endif
                try
                {
                    fixed (char* pModuleId = module.Id ?? string.Empty)
                    {
                        using var resultStr = SafeStructMallocHandle.Create(getDisabled(p_owner, (param_string*) pModuleId), true);
                        var result = resultStr.ValueAsBool();
                        return result;
                    }
                }
                catch (Exception e)
                {
                    logger.LogException(e);
                    return false;
                }
            }, (module, value) =>
            {
#if DEBUG
                using var logger = LogCallbackMethod($"{nameof(DisableModule)}4");
#else
                using var logger = LogCallbackMethod($"{nameof(DisableModule)}4");
#endif
                try
                {
                    fixed (char* pModuleId = module.Id ?? string.Empty)
                    {
                        using var resultStr = SafeStructMallocHandle.Create(setDisabled(p_owner, (param_string*) pModuleId, value), true);
                        resultStr.ValueAsVoid();
                    }
                }
                catch (Exception e)
                {
                    logger.LogException(e);
                }
            });

            return return_value_void.AsValue(false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_void.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "bmm_get_module_info", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_data* GetModuleInfo([IsConst<IsPtrConst>] param_string* p_xml_content)
    {
#if DEBUG
        using var logger = LogMethod(p_xml_content);
#else
        using var logger = LogMethod();
#endif
        try
        {
            var doc = ReaderUtils2.Read(param_string.ToSpan(p_xml_content));

            var result = ModuleInfoExtended.FromXml(doc);

            return JsonValue(result, CustomSourceGenerationContext.ModuleInfoExtended, false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_data.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "bmm_get_module_info_with_path", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_data* GetModuleInfoWithPath([IsConst<IsPtrConst>] param_string* p_xml_content, [IsConst<IsPtrConst>] param_string* p_path)
    {
#if DEBUG
        using var logger = LogMethod(p_xml_content, p_path);
#else
        using var logger = LogMethod();
#endif
        try
        {
            var doc = ReaderUtils2.Read(param_string.ToSpan(p_xml_content));

            var module = ModuleInfoExtended.FromXml(doc);
            var result = new ModuleInfoExtendedWithPath(module, new string(param_string.ToSpan(p_path)));

            return JsonValue(result, CustomSourceGenerationContext.ModuleInfoExtendedWithPath, false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_data.AsException(e, false);
        }
    }
    [UnmanagedCallersOnly(EntryPoint = "bmm_get_module_info_with_metadata", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_data* GetModuleInfoWithMetadata([IsConst<IsPtrConst>] param_string* p_xml_content, [IsConst<IsPtrConst>] param_string* p_module_provider, [IsConst<IsPtrConst>] param_string* p_path)
    {
#if DEBUG
        using var logger = LogMethod(p_xml_content, p_path);
#else
        using var logger = LogMethod();
#endif
        try
        {
            var doc = ReaderUtils2.Read(param_string.ToSpan(p_xml_content));

            var module = ModuleInfoExtended.FromXml(doc);
            var result = new ModuleInfoExtendedWithMetadata(module, Enum.TryParse<ModuleProviderType>(new string(param_string.ToSpan(p_module_provider)), out var e) ? e : ModuleProviderType.Default, new string(param_string.ToSpan(p_path)));

            return JsonValue(result, CustomSourceGenerationContext.ModuleInfoExtendedWithMetadata, false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_data.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "bmm_get_sub_module_info", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_data* GetSubModuleInfo([IsConst<IsPtrConst>] param_string* p_xml_content)
    {
#if DEBUG
        using var logger = LogMethod(p_xml_content);
#else
        using var logger = LogMethod();
#endif
        try
        {
            var doc = ReaderUtils2.Read(param_string.ToSpan(p_xml_content));

            var result = SubModuleInfoExtended.FromXml(doc);
            if (result is null)
            {
                var e = new InvalidOperationException("Invalid xml content!");
                logger.LogException(e);
                return return_value_data.AsException(e, false);
            }

            return JsonValue(result, CustomSourceGenerationContext.SubModuleInfoExtended, false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_data.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "bmm_parse_application_version", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_data* ParseApplicationVersion([IsConst<IsPtrConst>] param_string* p_application_version)
    {
#if DEBUG
        using var logger = LogMethod(p_application_version);
#else
        using var logger = LogMethod();
#endif
        try
        {
            var result = ApplicationVersion.TryParse(new string(param_string.ToSpan(p_application_version)), out var v) ? v : ApplicationVersion.Empty;

            return JsonValue(result, CustomSourceGenerationContext.ApplicationVersion, false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_data.AsException(e, false);
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "bmm_compare_versions", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_int32* CompareVersions([IsConst<IsPtrConst>] param_data* p_x, [IsConst<IsPtrConst>] param_data* p_y)
    {
#if DEBUG
        using var logger = LogMethod(p_x, p_y);
#else
        using var logger = LogMethod();
#endif
        try
        {
            var x = DeserializeJsonNative(p_x, CustomSourceGenerationContext.ApplicationVersion);
            var y = DeserializeJsonNative(p_y, CustomSourceGenerationContext.ApplicationVersion);

            var result = _applicationVersionComparer.Compare(x, y);

            return return_value_int32.AsValue(result, false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_int32.AsException(e, false);
        }
    }


    [UnmanagedCallersOnly(EntryPoint = "bmm_get_dependencies_all", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_data* GetDependenciesAll([IsConst<IsPtrConst>] param_data* p_module)
    {
#if DEBUG
        using var logger = LogMethod(p_module);
#else
        using var logger = LogMethod();
#endif
        try
        {
            //if (p_module is null)
            //    return return_value_json.AsValue([], CustomSourceGenerationContext.DependentModuleMetadataArray, false);

            var module = DeserializeJsonNative(p_module, CustomSourceGenerationContext.ModuleInfoExtended);

            var result = module.DependenciesAllDistinct().ToArray();

            return JsonValue(result, CustomSourceGenerationContext.DependentModuleMetadataArray, false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_data.AsException(e, false);
        }
    }
    [UnmanagedCallersOnly(EntryPoint = "bmm_get_dependencies_load_before_this", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_data* GetDependenciesLoadBeforeThis([IsConst<IsPtrConst>] param_data* p_module)
    {
#if DEBUG
        using var logger = LogMethod(p_module);
#else
        using var logger = LogMethod();
#endif
        try
        {
            //if (p_module is null)
            //    return return_value_json.AsValue([], CustomSourceGenerationContext.DependentModuleMetadataArray, false);

            var module = DeserializeJsonNative(p_module, CustomSourceGenerationContext.ModuleInfoExtended);

            var result = module.DependenciesLoadBeforeThisDistinct().ToArray();

            return JsonValue(result, CustomSourceGenerationContext.DependentModuleMetadataArray, false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_data.AsException(e, false);
        }
    }
    [UnmanagedCallersOnly(EntryPoint = "bmm_get_dependencies_load_after_this", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_data* GetDependenciesLoadAfterThis([IsConst<IsPtrConst>] param_data* p_module)
    {
#if DEBUG
        using var logger = LogMethod(p_module);
#else
        using var logger = LogMethod();
#endif
        try
        {
            //if (p_module is null)
            //    return return_value_json.AsValue([], CustomSourceGenerationContext.DependentModuleMetadataArray, false);

            var module = DeserializeJsonNative(p_module, CustomSourceGenerationContext.ModuleInfoExtended);

            var result = module.DependenciesLoadAfterThisDistinct().ToArray();

            return JsonValue(result, CustomSourceGenerationContext.DependentModuleMetadataArray, false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_data.AsException(e, false);
        }
    }
    [UnmanagedCallersOnly(EntryPoint = "bmm_get_dependencies_incompatibles", CallConvs = [typeof(CallConvCdecl)]), IsNotConst<IsPtrConst>]
    public static return_value_data* GetDependenciesIncompatibles([IsConst<IsPtrConst>] param_data* p_module)
    {
#if DEBUG
        using var logger = LogMethod(p_module);
#else
        using var logger = LogMethod();
#endif
        try
        {
            //if (p_module is null)
            //    return return_value_json.AsValue([], CustomSourceGenerationContext.DependentModuleMetadataArray, false);

            var module = DeserializeJsonNative(p_module, CustomSourceGenerationContext.ModuleInfoExtended);

            var result = module.DependenciesIncompatiblesDistinct().ToArray();

            return JsonValue(result, CustomSourceGenerationContext.DependentModuleMetadataArray, false);
        }
        catch (Exception e)
        {
            logger.LogException(e);
            return return_value_data.AsException(e, false);
        }
    }
}