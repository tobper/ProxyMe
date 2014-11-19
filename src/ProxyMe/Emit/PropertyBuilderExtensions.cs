using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ProxyMe.Emit
{
    public static class PropertyBuilderExtensions
    {
        const MethodAttributes DefaultMethodAttributes =
            MethodAttributes.Public |
            MethodAttributes.HideBySig |
            MethodAttributes.NewSlot |
            MethodAttributes.Virtual |
            MethodAttributes.Final;

        const MethodAttributes PropertyMethodAttributes =
            MethodAttributes.FamANDAssem |
            MethodAttributes.Family |
            MethodAttributes.Virtual |
            MethodAttributes.HideBySig |
            MethodAttributes.VtableLayoutMask |
            MethodAttributes.SpecialName;

        public static PropertyBuilder DefineProperty(this TypeBuilder type, PropertyInfo property)
        {
            return type.DefineProperty(property.Name, PropertyAttributes.None, property.PropertyType, null);
        }

        public static MethodBuilder DefineMethod(this TypeBuilder type, MethodInfo method, Type[] parameterTypes)
        {
            return type.DefineMethod(method.Name, DefaultMethodAttributes, method.ReturnType, parameterTypes);
        }

        public static MethodBuilder DefineGetMethod(this TypeBuilder type, PropertyInfo property)
        {
            return type.DefineMethod("get_" + property.Name, PropertyMethodAttributes, property.PropertyType, Type.EmptyTypes);
        }

        public static MethodBuilder DefineSetMethod(this TypeBuilder type, PropertyInfo property)
        {
            return type.DefineMethod("set_" + property.Name, PropertyMethodAttributes, null, new[] { property.PropertyType });
        }
    }
}