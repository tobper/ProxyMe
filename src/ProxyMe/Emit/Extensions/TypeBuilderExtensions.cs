using System;

namespace ProxyMe.Emit.Extensions
{
    public static partial class TypeBuilderExtensions
    {
        /// <summary>Gets the name of a dynamic proxy based on specified type.</summary>
        public static string GetProxyTypeName(this Type type, string suffix)
        {
            return type.FullName + "`" + suffix;
        }
    }
}