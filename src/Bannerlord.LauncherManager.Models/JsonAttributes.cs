﻿using System;

namespace Newtonsoft.Json
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
    internal sealed class JsonConstructorAttribute : Attribute;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    internal sealed class JsonIgnoreAttribute : Attribute;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    internal sealed class JsonPropertyAttribute : Attribute
    {
        public string? PropertyName { get; set; }

        public JsonPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}

namespace System.Text.Json.Serialization
{
    internal abstract class JsonAttribute : Attribute;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    internal sealed class JsonPropertyNameAttribute : JsonAttribute
    {
        public string Name { get; }

        public JsonPropertyNameAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    internal sealed class JsonIgnoreAttribute : JsonAttribute;

    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
    internal sealed class JsonConstructorAttribute : JsonAttribute;
}