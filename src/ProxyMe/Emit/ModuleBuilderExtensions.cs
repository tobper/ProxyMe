using System;

namespace ProxyMe.Emit
{
    public static class ModuleBuilderExtensions
    {
        /// <summary>Gets the name of a dynamic proxy based on specified type.</summary>
        public static string GetDynamicName(this Type type, string suffix)
        {
            return type.FullName + "`" + suffix;
        }
    }
}