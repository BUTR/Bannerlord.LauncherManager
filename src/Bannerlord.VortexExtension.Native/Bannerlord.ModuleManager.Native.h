#ifndef SRC_MODULEMANAGER_BINDINGS_H_
#define SRC_MODULEMANAGER_BINDINGS_H_

#include "Common.Native.h"

#ifdef __cplusplus
using namespace Common;

namespace Bannerlord::ModuleManager
{
    extern "C"
    {
#endif

        // All char16_t* parameters do not transfer ownership to the callee
        // All char16_t* returns pass their ownership to the callee

        return_value_json *const __cdecl bmm_sort(const param_json *p_source);
        return_value_json *const __cdecl bmm_sort_with_options(const param_json *p_source, const param_json *p_options);

        return_value_bool *const __cdecl bmm_are_all_dependencies_of_module_present(const param_json *p_source, const param_json *p_module);

        return_value_json *const __cdecl bmm_get_dependent_modules_of(const param_json *p_source, const param_json *constp_module);
        return_value_json *const __cdecl bmm_get_dependent_modules_of_with_options(const param_json *p_source, const param_json *p_module, const param_json *p_options);

        return_value_json *const __cdecl bmm_validate_load_order(const param_json *p_source, const param_json *p_target_module);
        return_value_json *const __cdecl bmm_validate_module(const void *p_owner, const param_json *p_modules, const param_json *p_target_module
            , return_value_bool *(__cdecl *p_is_selected)(void *p_owner, const param_string *p_module_id));

        return_value_void *const __cdecl bmm_enable_module(const void *p_owner, const param_json *p_modules, const param_json *p_target_module
            , return_value_bool *(__cdecl *p_get_selected)(void *p_owner, const param_string *p_module_id)
            , return_value_void *(__cdecl *p_set_selected)(void *p_owner, const param_string *p_module_id, param_bool value)
            , return_value_bool *(__cdecl *p_get_disabled)(void *p_owner, const param_string *p_module_id)
            , return_value_void *(__cdecl *p_set_disabled)(void *p_owner, const param_string *p_module_id, param_bool value));
        return_value_void *const __cdecl bmm_disable_module(const void *p_owner, const param_json *p_modules, const param_json *p_target_module
            , return_value_bool *(__cdecl *p_get_selected)(void *p_owner, const param_string *p_module_id)
            , return_value_void *(__cdecl *p_set_selected)(void *p_owner, const param_string *p_module_id, param_bool value)
            , return_value_bool *(__cdecl *p_get_disabled)(void *p_owner, const param_string *p_module_id)
            , return_value_void *(__cdecl *p_set_disabled)(void *p_owner, const param_string *p_module_id, param_bool value));

        return_value_json *const __cdecl bmm_get_module_info(const param_string *p_xml_content);
        return_value_json *const __cdecl bmm_get_sub_module_info(const param_string *p_xml_content);

        return_value_int32 *const __cdecl bmm_compare_versions(const param_json *p_x, const param_json *p_y);

        return_value_json *const __cdecl bmm_get_dependencies_all(const param_json *p_module);
        return_value_json *const __cdecl bmm_get_dependencies_load_before_this(const param_json *p_module);
        return_value_json *const __cdecl bmm_get_dependencies_load_after_this(const param_json *p_module);
        return_value_json *const __cdecl bmm_get_dependencies_incompatibles(const param_json *p_module);

#ifdef __cplusplus
    }
}
#endif

#endif