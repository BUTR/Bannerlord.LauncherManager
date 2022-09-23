using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization.Metadata;

namespace Bannerlord.VortexExtension.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct param_string
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> ToSpan(param_string* ptr) => MemoryMarshal.CreateReadOnlySpanFromNullTerminated((char*) ptr);
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct param_json
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> ToSpan(param_json* ptr) => MemoryMarshal.CreateReadOnlySpanFromNullTerminated((char*) ptr);
    }


    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct return_value_void
    {
        public static return_value_void* AsValue() => Utils.Create(new return_value_void(null));
        public static return_value_void* AsError(char* error) => Utils.Create(new return_value_void(error));

        public readonly char* Error;

        private return_value_void(char* error)
        {
            Error = error;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct return_value_string
    {
        public static return_value_string* AsValue(char* value) => Utils.Create(new return_value_string(value, null));
        public static return_value_string* AsError(char* error) => Utils.Create(new return_value_string(null, error));

        public readonly char* Error;
        public readonly char* Value;

        private return_value_string(char* value, char* error)
        {
            Value = value;
            Error = error;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct return_value_json
    {
        public static return_value_json* AsValue<TValue>(TValue value, JsonTypeInfo<TValue> jsonTypeInfo) =>
            AsValue(Utils.SerializeJsonCopy<TValue>(value, jsonTypeInfo));
        public static return_value_json* AsValue(char* value) => Utils.Create(new return_value_json(value, null));
        public static return_value_json* AsError(char* error) => Utils.Create(new return_value_json(null, error));

        public readonly char* Error;
        public readonly char* Value;

        private return_value_json(char* value, char* error)
        {
            Value = value;
            Error = error;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct return_value_bool
    {
        public static return_value_bool* AsValue(bool value) => Utils.Create(new return_value_bool(value, null));
        public static return_value_bool* AsError(char* error) => Utils.Create(new return_value_bool(false, error));

        public readonly char* Error;
        [MarshalAs(UnmanagedType.U1)]
        public readonly bool Value;

        private return_value_bool(bool value, char* error)
        {
            Value = value;
            Error = error;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct return_value_int32
    {
        public static return_value_int32* AsValue(int value) => Utils.Create(new return_value_int32(value, null));
        public static return_value_int32* AsError(char* error) => Utils.Create(new return_value_int32(0, null));

        public readonly char* Error;
        public readonly int Value;

        private return_value_int32(int value, char* error)
        {
            Value = value;
            Error = error;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct return_value_uint32
    {
        public static return_value_uint32* AsValue(uint value) => Utils.Create(new return_value_uint32(value, null));
        public static return_value_uint32* AsError(char* error) => Utils.Create(new return_value_uint32(0, error));

        public readonly char* Error;
        public readonly uint Value;

        private return_value_uint32(uint value, char* error)
        {
            Value = value;
            Error = error;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct return_value_ptr
    {
        public static return_value_ptr* AsValue(void* value) => Utils.Create(new return_value_ptr(value, null));
        public static return_value_ptr* AsError(char* error) => Utils.Create(new return_value_ptr(null, error));

        public readonly char* Error;
        public readonly void* Value;

        private return_value_ptr(void* value, char* error)
        {
            Value = value;
            Error = error;
        }
    }
}