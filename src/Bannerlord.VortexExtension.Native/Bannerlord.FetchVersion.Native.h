#ifndef SRC_FETCHBLVERSION_BINDINGS_H_
#define SRC_FETCHBLVERSION_BINDINGS_H_

#include "Common.Native.h"

using namespace Common;

namespace Bannerlord::FetchVersion {

#ifdef __cplusplus
    extern "C" {
#endif

        // All char16_t* parameters do not transfer ownership to the callee
        // All char16_t* returns pass their ownership to the callee

        return_value_uint32* __cdecl bfv_get_change_set(const param_string* p_game_folder_path, const param_string* p_lib_assembly);

        return_value_string* __cdecl bfv_get_version(const param_string* p_game_folder_path, const param_string* p_lib_assembly);

        return_value_uint32* __cdecl bfv_get_version_type(const param_string* p_game_folder_path, const param_string* p_lib_assembly);

#ifdef __cplusplus
    }
#endif

}

#endif