using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ProxyMe.Emit
{
    public static class ContractModuleBuilderExtensions
    {
        public static TypeBuilder DefineContractType<TContract>(this ModuleBuilder moduleBuilder)
        {
            var contract = typeof (TContract);
            var typeBuilder = DefineContractType(moduleBuilder, contract);

            return typeBuilder;
        }

        public static TypeBuilder DefineContractType(this ModuleBuilder moduleBuilder, Type contract)
        {
            if (contract.IsInterface == false)
                throw new InvalidOperationException("A dynamic contract can only be created for interfaces.");

            var typeInfo = contract.GetTypeInfo();
            if (typeInfo.DeclaredMethods.Any(m => m.IsSpecialName == false))
                throw new InvalidOperationException("A dynamic contract can not be created for an interface with methods.");

            var typeName = contract.GetDynamicName("DynamicContract");
            var typeBuilder = DefineType(moduleBuilder, typeName, contract);

            DefineDefaultConstructor(typeBuilder);
            DefineInitializationConstructor(typeBuilder, contract);
            DefineProperties(typeBuilder, typeInfo);

            return typeBuilder;
        }

        private static TypeBuilder DefineType(ModuleBuilder moduleBuilder, string typeName, Type contract)
        {
            return moduleBuilder.DefineType(
                typeName,
                TypeAttributes.Class | TypeAttributes.Public,
                null,
                new[] { contract });
        }

        private static void DefineDefaultConstructor(TypeBuilder type)
        {
            type.DefineDefaultConstructor(MethodAttributes.Public);
        }

        private static void DefineInitializationConstructor(TypeBuilder type, Type contractType)
        {
            var actionType = typeof (Action<>).MakeGenericType(contractType);
            var actionInvoke = actionType.GetMethod("Invoke");
            var arguments = new[] { actionType };
            var constructor = type.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, arguments);

            var il = constructor.GetILGenerator();

            il.Emit(OpCodes.Ldarg_1); // Load action method
            il.Emit(OpCodes.Ldarg_0); // Load contract instance
            il.Emit(OpCodes.Callvirt, actionInvoke); // Call action
            il.Emit(OpCodes.Ret);
        }

        private static void DefineProperties(TypeBuilder type, TypeInfo contractType)
        {
            foreach (var property in contractType.DeclaredProperties)
            {
                type.DefinePropertyWithBackingField(property);
            }
        }

        public static void DefinePropertyWithBackingField(this TypeBuilder type, PropertyInfo contractProperty)
        {
            var fieldName = "_" + contractProperty.Name;
            var backingField = type.DefineField(fieldName, contractProperty.PropertyType, FieldAttributes.Private);
            var proxyProperty = type.DefineProperty(contractProperty);

            if (contractProperty.GetMethod != null)
            {
                var method = type.DefineGetMethod(contractProperty);
                var il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, backingField);
                il.Emit(OpCodes.Ret);

                proxyProperty.SetGetMethod(method);
            }

            if (contractProperty.SetMethod != null)
            {
                var method = type.DefineSetMethod(contractProperty);
                var il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stfld, backingField);
                il.Emit(OpCodes.Ret);

                proxyProperty.SetSetMethod(method);
            }
        }
    }
}