using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ProxyMe.Emit
{
    public static class SubTypeModuleBuilderExtensions
    {
        // http://stackoverflow.com/questions/6879279/using-typebuilder-to-create-a-pass-through-constructor-for-the-base-class

        public static TypeBuilder DefineSubType<TSuper>(this ModuleBuilder moduleBuilder)
        {
            var superType = typeof (TSuper);
            var typeBuilder = DefineSubType(moduleBuilder, superType);

            return typeBuilder;
        }

        public static TypeBuilder DefineSubType(this ModuleBuilder moduleBuilder, Type parent)
        {
            if (parent.IsInterface)
                throw new InvalidOperationException("A dynamic sub type can only be created for classes.");

            var typeName = parent.GetDynamicName("DynamicSubType");
            var typeBuilder = DefineType(moduleBuilder, typeName, parent);

            DefineDefaultConstructor(typeBuilder);

            return typeBuilder;
        }

        private static TypeBuilder DefineType(ModuleBuilder moduleBuilder, string typeName, Type parent)
        {
            return moduleBuilder.DefineType(
                typeName,
                TypeAttributes.Class | TypeAttributes.Public,
                parent);
        }

        private static void DefineDefaultConstructor(TypeBuilder type)
        {
            type.DefineDefaultConstructor(MethodAttributes.Public);
        }
    }
}