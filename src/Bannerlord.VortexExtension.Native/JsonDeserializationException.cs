using System;
using System.Text.Json;

namespace Bannerlord.VortexExtension.Native
{
    public class NativeCallException : Exception
    {
        public NativeCallException(string message) : base(message) { }
    }

    public class JsonDeserializationException : JsonException
    {
        public JsonDeserializationException(string message) : base(message) { }
        public JsonDeserializationException(string message, Exception exception) : base(message, exception) { }
    }
}