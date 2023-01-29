#ifndef SRC_VEHANDLER_BINDINGS_H_
#define SRC_VEHANDLER_BINDINGS_H_

#include "Common.Native.h"

#ifdef __cplusplus
using namespace Common;

namespace Bannerlord::VortexExtension
{
    extern "C"
    {
#endif

        // All char16_t* parameters do not transfer ownership to the callee
        // All char16_t* returns pass their ownership to the callee

        return_value_ptr *const __cdecl ve_create_handler(const void *p_owner);
        return_value_void *const __cdecl ve_dispose_handler(const void *p_handler);

        return_value_string *const __cdecl ve_get_game_version(const void *p_handler);

        return_value_json *const __cdecl ve_test_module(const void *p_handler, const param_json *p_files, const param_string *p_game_id);
        return_value_json *const __cdecl ve_install_module(const void *p_handler, const param_json *p_files, const param_string *p_destination_path);

        return_value_bool *const __cdecl ve_is_sorting(const void *p_handler);
        return_value_void *const __cdecl ve_sort(const void *p_handler);

        return_value_json *const __cdecl ve_get_load_order(const void *p_handler);
        return_value_void *const __cdecl ve_set_load_order(const void *p_handler, const param_json *p_load_order);

        return_value_json *const __cdecl ve_get_modules(const void *p_handler);

        return_value_void *const __cdecl ve_register_callbacks(const void *p_handler
            , return_value_json *(__cdecl *p_get_active_profile)(void *p_owner)
            , return_value_json *(__cdecl *p_get_profile_by_id)(void *p_owner, const param_string *p_profile_id)
            , return_value_string *(__cdecl *p_get_active_game_id)(void *p_owner)
            , return_value_void *(__cdecl *p_set_game_parameters)(void *p_owner, const param_string *p_game_id, const param_string *p_executable, const param_json *p_game_parameters)
            , return_value_json *(__cdecl *p_get_load_order)(void *p_owner)
            , return_value_void *(__cdecl *p_set_load_order)(void *p_owner, const param_json *p_load_order)
            , return_value_string *(__cdecl *p_translate_string)(void *p_owner, const param_string *p_text, const param_string *p_ns)
            , return_value_void *(__cdecl *p_send_notification)(void *p_owner, const param_string *p_id, const param_string *p_type, const param_string *p_message, param_uint display_ms)
            , return_value_string *(__cdecl *p_get_install_path)(void *p_owner)
            , return_value_string *(__cdecl *p_read_file_content)(void *p_owner, const param_string *p_file_path)
            , return_value_json *(__cdecl *p_read_directory_file_list)(void *p_owner, const param_string *p_directory_path)
            , return_value_json *(__cdecl *p_read_directory_list)(void *p_owner, const param_string *p_directory_path));

#ifdef __cplusplus
    }
}
#endif

#endif