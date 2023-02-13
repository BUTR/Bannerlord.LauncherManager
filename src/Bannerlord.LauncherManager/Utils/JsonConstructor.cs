namespace System.Text.Json.Serialization
{
    internal abstract class JsonAttribute : Attribute { }

    /// <summary>
    /// When placed on a constructor, indicates that the constructor should be used to create
    /// instances of the type on deserialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
    internal sealed class JsonConstructorAttribute : JsonAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="JsonConstructorAttribute"/>.
        /// </summary>
        public JsonConstructorAttribute() { }
    }
}