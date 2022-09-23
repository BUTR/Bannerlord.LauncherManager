#ifndef SRC_COMMON_BINDINGS_H_
#define SRC_COMMON_BINDINGS_H_

#include <memory>

namespace Common
{

#ifdef __cplusplus
    extern "C"
    {
#endif

        typedef char16_t param_string;
        typedef char16_t param_json;

        typedef struct return_value_void
        {
            param_string *error;
        } return_value_void;
        typedef struct return_value_string
        {
            param_string *error;
            param_string *value;
        } return_value_string;
        typedef struct return_value_json
        {
            param_string *error;
            param_json *value;
        } return_value_json;
        typedef struct return_value_bool
        {
            param_string *error;
            bool value;
        } return_value_bool;
        typedef struct return_value_int32
        {
            param_string *error;
            __int32 value;
        } return_value_int32;
        typedef struct return_value_uint32
        {
            param_string *error;
            unsigned __int32 value;
        } return_value_uint32;
        typedef struct return_value_ptr
        {
            param_string *error;
            void *value;
        } return_value_ptr;

#ifdef __cplusplus
    }
#endif

    template <typename T>
    struct deleter
    {
        void operator()(const T *ptr) const { free((void *)ptr); }
    };

    using del_void = std::unique_ptr<return_value_void, deleter<return_value_void>>;
    using del_string = std::unique_ptr<return_value_string, deleter<return_value_string>>;
    using del_json = std::unique_ptr<return_value_json, deleter<return_value_json>>;
    using del_bool = std::unique_ptr<return_value_bool, deleter<return_value_bool>>;
    using del_int32 = std::unique_ptr<return_value_int32, deleter<return_value_int32>>;
    using del_uint32 = std::unique_ptr<return_value_uint32, deleter<return_value_uint32>>;
    using del_ptr = std::unique_ptr<return_value_ptr, deleter<return_value_ptr>>;

}

#endif