using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using Bannerlord.ModuleManager;

namespace Bannerlord.VortexExtension.Native
{
    public static unsafe partial class Bindings
    {
        private static readonly ApplicationVersionComparer _applicationVersionComparer = new();

        [UnmanagedCallersOnly(EntryPoint = "bmm_sort")]
        public static return_value_json* Sort(param_json* p_source)
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(Sort)}! p_source: {param_json.ToSpan(p_source)}");
#endif

            try
            {
                var source = Utils.DeserializeJson<ModuleInfoExtended[]>(p_source, _customSourceGenerationContext.ModuleInfoExtendedArray);

                var result = ModuleSorter.Sort(source).ToArray();
#if LOGGING
                Logger.Log($"Result of {nameof(Sort)}: {result}");
#endif
                return return_value_json.AsValue<ModuleInfoExtended[]>(result, _customSourceGenerationContext.ModuleInfoExtendedArray);
            }
            catch (Exception e)
            {
#if LOGGING
                Logger.Log($"Exception of {nameof(Sort)}: {e}");
#endif
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "bmm_sort_with_options")]
        public static return_value_json* SortWithOptions(param_json* p_source, param_json* p_options)
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(SortWithOptions)}! p_source: {param_json.ToSpan(p_source)}; p_options: {param_json.ToSpan(p_options)}");
#endif

            try
            {
                var source = Utils.DeserializeJson<ModuleInfoExtended[]>(p_source, _customSourceGenerationContext.ModuleInfoExtendedArray);
                var options = Utils.DeserializeJson<ModuleSorterOptions>(p_options, _customSourceGenerationContext.ModuleSorterOptions);

                var result = ModuleSorter.Sort(source, options).ToArray();
#if LOGGING
                Logger.Log($"Result of {nameof(SortWithOptions)}: {result}");
#endif
                return return_value_json.AsValue<ModuleInfoExtended[]>(result, _customSourceGenerationContext.ModuleInfoExtendedArray);
            }
            catch (Exception e)
            {
#if LOGGING
                Logger.Log($"Exception of {nameof(SortWithOptions)}: {e}");
#endif
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "bmm_are_all_dependencies_of_module_present")]
        public static return_value_bool* AreAllDependenciesOfModulePresent(param_json* p_source, param_json* p_module)
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(AreAllDependenciesOfModulePresent)}!");
            //Logger.Log($"Received call to {nameof(AreAllDependenciesOfModulePresent)}! p_source: {param_json.ToSpan(p_source)}; p_module: {param_json.ToSpan(p_module)}");
#endif

            try
            {
                var source = Utils.DeserializeJson<ModuleInfoExtended[]>(p_source, _customSourceGenerationContext.ModuleInfoExtendedArray);
                var module = Utils.DeserializeJson<ModuleInfoExtended>(p_module, _customSourceGenerationContext.ModuleInfoExtended);

                var result = ModuleUtilities.AreDependenciesPresent(source, module);
#if LOGGING
                Logger.Log($"Result of {nameof(AreAllDependenciesOfModulePresent)}: {result}");
#endif
                return return_value_bool.AsValue(result);
            }
            catch (Exception e)
            {
#if LOGGING
                Logger.Log($"Exception of {nameof(AreAllDependenciesOfModulePresent)}: {e}");
#endif
                return return_value_bool.AsError(Utils.Copy(e.ToString()));
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "bmm_get_dependent_modules_of")]
        public static return_value_json* GetDependentModulesOf(param_json* p_source, param_json* p_module)
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(GetDependentModulesOf)}! p_source: {param_json.ToSpan(p_source)}; p_module: {param_json.ToSpan(p_module)}");
#endif

            try
            {
                var source = Utils.DeserializeJson<ModuleInfoExtended[]>(p_source, _customSourceGenerationContext.ModuleInfoExtendedArray);
                var module = Utils.DeserializeJson<ModuleInfoExtended>(p_module, _customSourceGenerationContext.ModuleInfoExtended);

                var result = ModuleUtilities.GetDependencies(source, module).ToArray();
#if LOGGING
                Logger.Log($"Result of {nameof(GetDependentModulesOf)}: {result}");
#endif
                return return_value_json.AsValue<ModuleInfoExtended[]>(result, _customSourceGenerationContext.ModuleInfoExtendedArray);
            }
            catch (Exception e)
            {
#if LOGGING
                Logger.Log($"Exception of {nameof(GetDependentModulesOf)}: {e}");
#endif
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "bmm_get_dependent_modules_of_with_options")]
        public static return_value_json* GetDependentModulesOfWithOptions(param_json* p_source, param_json* p_module, param_json* p_options)
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(GetDependentModulesOfWithOptions)}! p_source: {param_json.ToSpan(p_source)}; p_module: {param_json.ToSpan(p_module)}; p_options: {param_json.ToSpan(p_options)}");
#endif

            try
            {
                var source = Utils.DeserializeJson<ModuleInfoExtended[]>(p_source, _customSourceGenerationContext.ModuleInfoExtendedArray);
                var module = Utils.DeserializeJson<ModuleInfoExtended>(p_module, _customSourceGenerationContext.ModuleInfoExtended);
                var options = Utils.DeserializeJson<ModuleSorterOptions>(p_options, _customSourceGenerationContext.ModuleSorterOptions);

                var result = ModuleUtilities.GetDependencies(source, module, options).ToArray();
#if LOGGING
                Logger.Log($"Result of {nameof(GetDependentModulesOfWithOptions)}: {result}");
#endif
                return return_value_json.AsValue<ModuleInfoExtended[]>(result, _customSourceGenerationContext.ModuleInfoExtendedArray);
            }
            catch (Exception e)
            {
#if LOGGING
                Logger.Log($"Exception of {nameof(GetDependentModulesOfWithOptions)}: {e}");
#endif
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "bmm_validate_module_dependencies_declarations")]
        public static return_value_json* ValidateModuleDependenciesDeclarations(param_json* p_target_module)
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(ValidateModuleDependenciesDeclarations)}! p_target_module: {param_json.ToSpan(p_target_module)}");
#endif

            try
            {
                var targetModule = Utils.DeserializeJson<ModuleInfoExtended>(p_target_module, _customSourceGenerationContext.ModuleInfoExtended);

                var result = ModuleUtilities.ValidateModuleDependenciesDeclarations(targetModule).ToArray();
#if LOGGING
                Logger.Log($"Result of {nameof(ValidateModuleDependenciesDeclarations)}: {result}");
#endif
                return return_value_json.AsValue<ModuleIssue[]>(result, _customSourceGenerationContext.ModuleIssueArray);
            }
            catch (Exception e)
            {
#if LOGGING
                Logger.Log($"Exception of {nameof(ValidateModuleDependenciesDeclarations)}: {e}");
#endif
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "bmm_validate_module")]
        public static return_value_json* ValidateModule(void* p_owner
            , param_json* p_modules
            , param_json* p_target_module
            , delegate* unmanaged[Cdecl]<return_value_bool*, void*, param_string*> p_is_selected)
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(ValidateModule)}! p_modules: {param_json.ToSpan(p_modules)};  p_target_module: {param_json.ToSpan(p_target_module)}");
#endif

            try
            {
                var modules = Utils.DeserializeJson<ModuleInfoExtended[]>(p_modules, _customSourceGenerationContext.ModuleInfoExtendedArray);
                var targetModule = Utils.DeserializeJson<ModuleInfoExtended>(p_target_module, _customSourceGenerationContext.ModuleInfoExtended);

                var isSelected = Marshal.GetDelegateForFunctionPointer<N_IsSelected>(new IntPtr(p_is_selected));

                var result = ModuleUtilities.ValidateModule(modules, targetModule, module =>
                {
#if LOGGING
                    Logger.Log($"Received callback to {nameof(isSelected)}!");
#endif

                    fixed (char* pModuleId = module.Id)
                    {
                        using var result = SafeStructMallocHandle.Create(isSelected(p_owner, (param_string*) pModuleId));
                        return result.ValueAsBool();
                    }
                }).ToArray();
#if LOGGING
                Logger.Log($"Result of {nameof(ValidateModule)}: {result}");
#endif
                return return_value_json.AsValue<ModuleIssue[]>(result, _customSourceGenerationContext.ModuleIssueArray);
            }
            catch (Exception e)
            {
#if LOGGING
                Logger.Log($"Exception of {nameof(ValidateModule)}: {e}");
#endif
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "bmm_enable_module")]
        public static return_value_json* EnableModule(void* p_owner
            , param_json* p_module
            , param_json* p_target_module
            , delegate* unmanaged[Cdecl]<return_value_bool*, void*, param_string*> p_get_selected
            , delegate* unmanaged[Cdecl]<return_value_void*, void*, param_string*, bool> p_set_selected
            , delegate* unmanaged[Cdecl]<return_value_bool*, void*, param_string*> p_get_disabled
            , delegate* unmanaged[Cdecl]<return_value_void*, void*, param_string*, bool> p_set_disabled
            )
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(EnableModule)}! p_module: {param_json.ToSpan(p_module)};  p_target_module: {param_json.ToSpan(p_target_module)}");
#endif

            try
            {
                var modules = Utils.DeserializeJson<ModuleInfoExtended[]>(p_module, _customSourceGenerationContext.ModuleInfoExtendedArray);
                var targetModule = Utils.DeserializeJson<ModuleInfoExtended>(p_target_module, _customSourceGenerationContext.ModuleInfoExtended);

                var getSelected = Marshal.GetDelegateForFunctionPointer<N_GetSelected>(new IntPtr(p_get_selected));
                var setSelected = Marshal.GetDelegateForFunctionPointer<N_SetSelected>(new IntPtr(p_set_selected));
                var getDisabled = Marshal.GetDelegateForFunctionPointer<N_GetDisabled>(new IntPtr(p_get_disabled));
                var setDisabled = Marshal.GetDelegateForFunctionPointer<N_SetDisabled>(new IntPtr(p_set_disabled));

                var result = ModuleUtilities.EnableModule(modules, targetModule,
                    module =>
                    {
#if LOGGING
                        Logger.Log($"Received callback to {nameof(getSelected)}!");
#endif

                        fixed (char* pModuleId = module.Id)
                        {
                            using var result = SafeStructMallocHandle.Create(getSelected(p_owner, (param_string*) pModuleId));
                            return result.ValueAsBool();
                        }
                    },
                    (module, value) =>
                    {
#if LOGGING
                        Logger.Log($"Received callback to {nameof(setSelected)}!");
#endif

                        fixed (char* pModuleId = module.Id)
                        {
                            var result = SafeStructMallocHandle.Create(setSelected(p_owner, (param_string*) pModuleId, value));
                            result.ValueAsVoid();
                        }
                    },
                    module =>
                    {
#if LOGGING
                        Logger.Log($"Received callback to {nameof(getDisabled)}!");
#endif

                        fixed (char* pModuleId = module.Id)
                        {
                            using var result = SafeStructMallocHandle.Create(getDisabled(p_owner, (param_string*) pModuleId));
                            return result.ValueAsBool();
                        }
                    },
                    (module, value) =>
                    {
#if LOGGING
                        Logger.Log($"Received callback to {nameof(setDisabled)}!");
#endif

                        fixed (char* pModuleId = module.Id)
                        {
                            var result = SafeStructMallocHandle.Create(setDisabled(p_owner, (param_string*) pModuleId, value));
                            result.ValueAsVoid();
                        }
                    }).ToArray();
#if LOGGING
                Logger.Log($"Result of {nameof(EnableModule)}: {result}");
#endif
                return return_value_json.AsValue<ModuleIssue[]>(result, _customSourceGenerationContext.ModuleIssueArray);
            }
            catch (Exception e)
            {
#if LOGGING
                Logger.Log($"Exception of {nameof(EnableModule)}: {e}");
#endif
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "bmm_disable_module")]
        public static return_value_json* DisableModule(void* p_owner
            , param_json* p_module
            , param_json* p_target_module
            , delegate* unmanaged[Cdecl]<return_value_bool*, void*, param_string*> p_get_selected
            , delegate* unmanaged[Cdecl]<return_value_void*, void*, param_string*, bool> p_set_selected
            , delegate* unmanaged[Cdecl]<return_value_bool*, void*, param_string*> p_get_disabled
            , delegate* unmanaged[Cdecl]<return_value_void*, void*, param_string*, bool> p_set_disabled
        )
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(DisableModule)}! p_module: {param_json.ToSpan(p_module)};  p_target_module: {param_json.ToSpan(p_target_module)}");
#endif

            try
            {
                var modules = Utils.DeserializeJson<ModuleInfoExtended[]>(p_module, _customSourceGenerationContext.ModuleInfoExtendedArray);
                var targetModule = Utils.DeserializeJson<ModuleInfoExtended>(p_target_module, _customSourceGenerationContext.ModuleInfoExtended);

                var getSelected = Marshal.GetDelegateForFunctionPointer<N_GetSelected>(new IntPtr(p_get_selected));
                var setSelected = Marshal.GetDelegateForFunctionPointer<N_SetSelected>(new IntPtr(p_set_selected));
                var getDisabled = Marshal.GetDelegateForFunctionPointer<N_GetDisabled>(new IntPtr(p_get_disabled));
                var setDisabled = Marshal.GetDelegateForFunctionPointer<N_SetDisabled>(new IntPtr(p_set_disabled));

                var result = ModuleUtilities.DisableModule(modules, targetModule,
                    module =>
                    {
#if LOGGING
                        Logger.Log($"Received callback to {nameof(getSelected)}!");
#endif

                        fixed (char* pModuleId = module.Id)
                        {
                            using var result = SafeStructMallocHandle.Create(getSelected(p_owner, (param_string*) pModuleId));
                            return result.ValueAsBool();
                        }
                    },
                    (module, value) =>
                    {
#if LOGGING
                        Logger.Log($"Received callback to {nameof(setSelected)}!");
#endif

                        fixed (char* pModuleId = module.Id)
                        {
                            var result = SafeStructMallocHandle.Create(setSelected(p_owner, (param_string*) pModuleId, value));
                            result.ValueAsVoid();
                        }
                    },
                    module =>
                    {
#if LOGGING
                        Logger.Log($"Received callback to {nameof(getDisabled)}!");
#endif

                        fixed (char* pModuleId = module.Id)
                        {
                            using var result = SafeStructMallocHandle.Create(getDisabled(p_owner, (param_string*) pModuleId));
                            return result.ValueAsBool();
                        }
                    },
                    (module, value) =>
                    {
#if LOGGING
                        Logger.Log($"Received callback to {nameof(setDisabled)}!");
#endif

                        fixed (char* pModuleId = module.Id)
                        {
                            var result = SafeStructMallocHandle.Create(setDisabled(p_owner, (param_string*) pModuleId, value));
                            result.ValueAsVoid();
                        }
                    }).ToArray();
#if LOGGING
                Logger.Log($"Result of {nameof(DisableModule)}: {result}");
#endif
                return return_value_json.AsValue<ModuleIssue[]>(result, _customSourceGenerationContext.ModuleIssueArray);
            }
            catch (Exception e)
            {
#if LOGGING
                Logger.Log($"Exception of {nameof(DisableModule)}: {e}");
#endif
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "bmm_get_module_info")]
        public static return_value_json* GetModuleInfo(param_string* p_xml_content)
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(GetModuleInfo)}!");
            //Logger.Log($"Received call to {nameof(GetModuleInfo)}! p_xml_content: {param_string.ToSpan(p_xml_content)}");
#endif

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(new string(param_string.ToSpan(p_xml_content)));

                var result = ModuleInfoExtended.FromXml(doc);
#if LOGGING
                Logger.Log($"Result of {nameof(GetModuleInfo)}: {result}");
#endif
                return return_value_json.AsValue<ModuleInfoExtended>(result, _customSourceGenerationContext.ModuleInfoExtended);
            }
            catch (Exception e)
            {
#if LOGGING
                Logger.Log($"Exception of {nameof(GetModuleInfo)}: {e}");
#endif
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "bmm_get_sub_module_info")]
        public static return_value_json* GetSubModuleInfo(param_string* p_xml_content)
        {
#if LOGGING
            Logger.Log($"Received call to {nameof(GetSubModuleInfo)}!");
            //Logger.Log($"Received call to {nameof(GetSubModuleInfo)}! p_xml_content: {param_string.ToSpan(p_xml_content)}");
#endif

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(new string(param_string.ToSpan(p_xml_content)));

                var result = SubModuleInfoExtended.FromXml(doc);
#if LOGGING
                Logger.Log($"Result of {nameof(GetSubModuleInfo)}: {result}");
#endif
                return return_value_json.AsValue<SubModuleInfoExtended>(result, _customSourceGenerationContext.SubModuleInfoExtended);
            }
            catch (Exception e)
            {
#if LOGGING
                Logger.Log($"Exception of {nameof(GetSubModuleInfo)}: {e}");
#endif
                return return_value_json.AsError(Utils.Copy(e.ToString()));
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "bmm_compare_versions")]
        public static return_value_int32* CompareVersions(param_json* p_x, param_json* p_y)
        {
#if LOGGING
            //Logger.Log($"Received call to {nameof(CompareVersions)}!");
            Logger.Log($"Received call to {nameof(CompareVersions)}! p_x: {param_json.ToSpan(p_x)}; p_y: {param_json.ToSpan(p_y)}");
#endif

            try
            {
                var x = Utils.DeserializeJson<ApplicationVersion>(p_x, _customSourceGenerationContext.ApplicationVersion);
                var y = Utils.DeserializeJson<ApplicationVersion>(p_y, _customSourceGenerationContext.ApplicationVersion);

                var result = _applicationVersionComparer.Compare(x, y);
#if LOGGING
                Logger.Log($"Result of {nameof(CompareVersions)}: {result}");
#endif
                return return_value_int32.AsValue(result);
            }
            catch (Exception e)
            {
#if LOGGING
                Logger.Log($"Exception of {nameof(CompareVersions)}: {e}");
#endif
                return return_value_int32.AsError(Utils.Copy(e.ToString()));
            }
        }
    }
}