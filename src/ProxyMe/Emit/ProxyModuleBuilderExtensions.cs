using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using ProxyMe.Extensions;

namespace ProxyMe.Emit
{
    public static class ProxyModuleBuilderExtensions
    {
        private static readonly ConstructorInfo ObjectConstructor;

        static ProxyModuleBuilderExtensions()
        {
            var objectType = typeof(object);
            var defaultConstructor = objectType.GetConstructor(Type.EmptyTypes);

            ObjectConstructor = defaultConstructor;
        }

        public static TypeBuilder DefineProxyType<TReference>(this ModuleBuilder moduleBuilder)
        {
            var parent = typeof (TReference);
            var typeBuilder = DefineProxyType(moduleBuilder, parent);

            return typeBuilder;
        }

        public static TypeBuilder DefineProxyType(this ModuleBuilder moduleBuilder, Type contract)
        {
            if (contract.IsInterface == false)
                throw new InvalidOperationException("A dynamic proxy can only be created for interfaces.");

            var typeInfo = contract.GetTypeInfo();

            return DefineProxyType(moduleBuilder, typeInfo);
        }

        public static TypeBuilder DefineProxyType(ModuleBuilder moduleBuilder, TypeInfo contract)
        {
            var proxyName = contract.GetDynamicName("DynamicProxy");
            var proxy = DefineType(moduleBuilder, proxyName, contract);
            var target = DefineTargetField(proxy, contract);

            DefineConstructor(proxy, target);
            DefineProperties(proxy, contract, target);

            return proxy;
        }

        private static TypeBuilder DefineType(ModuleBuilder moduleBuilder, string proxyName, Type contract)
        {
            return moduleBuilder.DefineType(
                proxyName,
                TypeAttributes.Class | TypeAttributes.Public,
                null,
                new[] { contract });
        }

        private static FieldInfo DefineTargetField(TypeBuilder proxy, Type contract)
        {
            return proxy.DefineField("_target", contract, FieldAttributes.Private | FieldAttributes.InitOnly);
        }

        private static void DefineConstructor(TypeBuilder proxy, FieldInfo target)
        {
            var arguments = new[] { target.FieldType };
            var constructor = proxy.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, arguments);
            var il = constructor.GetILGenerator();

            // Call base constructor
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, ObjectConstructor);

            // Store target in field
            il.Emit(OpCodes.Ldarg_0); // Load 'this'
            il.Emit(OpCodes.Ldarg_1); // Load target from constructor argument
            il.Emit(OpCodes.Stfld, target); // Store target in field

            // Return
            il.Emit(OpCodes.Ret);
        }

        private static void DefineProperties(TypeBuilder proxy, TypeInfo contract, FieldInfo target)
        {
            foreach (var property in contract.DeclaredProperties)
            {
                DefineProxyProperty(proxy, property, target);
            }
        }

        private static void DefineProxyProperty(TypeBuilder proxy, PropertyInfo targetProperty, FieldInfo target)
        {
            var proxyProperty = proxy.DefineProperty(targetProperty);

            if (targetProperty.GetMethod != null)
            {
                var method = proxy.DefineGetMethod(proxyProperty);
                var il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, target);
                il.Emit(OpCodes.Callvirt, targetProperty.GetMethod);
                il.Emit(OpCodes.Ret);

                proxyProperty.SetGetMethod(method);
            }

            if (targetProperty.SetMethod != null)
            {
                var method = proxy.DefineSetMethod(targetProperty);
                var il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, target);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Callvirt, targetProperty.SetMethod);
                il.Emit(OpCodes.Ret);

                proxyProperty.SetSetMethod(method);
            }
        }
    }
}