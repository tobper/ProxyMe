using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ProxyMe.Emit
{
    public static partial class TypeBuilderExtensions
    {
        private static FieldInfo DefineTargetField(TypeBuilder proxy, Type contract)
        {
            return proxy.DefineField("_target", contract, FieldAttributes.Private | FieldAttributes.InitOnly);
        }

        private static FieldInfo DefinePropertiesField(TypeBuilder proxy)
        {
            return proxy.DefineField("_properties", PropertiesType, FieldAttributes.Private | FieldAttributes.InitOnly);
        }
    }
}