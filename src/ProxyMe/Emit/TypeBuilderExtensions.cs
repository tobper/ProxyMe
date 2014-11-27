using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ProxyMe.Emit
{
    public static partial class TypeBuilderExtensions
    {
        private static readonly ConstructorInfo ObjectConstructor;
        private static readonly MethodInfo DictionaryGetMethod;
        private static readonly MethodInfo DictionarySetMethod;
        private static readonly MethodInfo DictionaryContainsKeyMethod;
        private static readonly Type PropertiesType = typeof(IDictionary<string, object>);

        static TypeBuilderExtensions()
        {
            // Get reference to default Object constructor
            var objectType = typeof(object);
            var defaultConstructor = objectType.GetConstructor(Type.EmptyTypes);

            ObjectConstructor = defaultConstructor;

            // Get reference to dictiomary get/set methods
            var dictionaryType = typeof (IDictionary<string, object>);

            DictionaryGetMethod = dictionaryType.GetMethod("get_Item");
            DictionarySetMethod = dictionaryType.GetMethod("set_Item");
            DictionaryContainsKeyMethod = dictionaryType.GetMethod("ContainsKey");
        }

        public static TypeBuilder DefineContractType<TContract>(this ModuleBuilder moduleBuilder)
        {
            var contract = typeof(TContract);
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

            var typeName = GetDynamicName(contract, "DynamicContract");
            var typeBuilder = DefineType(moduleBuilder, typeName, null, contract);

            DefineDefaultConstructor(typeBuilder);
            DefineInitializationConstructor(typeBuilder, contract);
            DefinePropertiesWithBackingField(typeBuilder, typeInfo);

            return typeBuilder;
        }

        public static TypeBuilder DefineDictionaryContractType<TContract>(this ModuleBuilder moduleBuilder)
        {
            var contract = typeof(TContract);
            var typeBuilder = DefineDictionaryContractType(moduleBuilder, contract);

            return typeBuilder;
        }

        public static TypeBuilder DefineDictionaryContractType(this ModuleBuilder moduleBuilder, Type contract)
        {
            if (contract.IsInterface == false)
                throw new InvalidOperationException("A dynamic contract can only be created for interfaces.");

            var typeInfo = contract.GetTypeInfo();
            if (typeInfo.DeclaredMethods.Any(m => m.IsSpecialName == false))
                throw new InvalidOperationException("A dynamic contract can not be created for an interface with methods.");

            var typeName = GetDynamicName(contract, "DynamicDictionaryContract");
            var typeBuilder = DefineType(moduleBuilder, typeName, null, contract);
            var properties = DefinePropertiesField(typeBuilder);

            DefinePropertiesConstructor(typeBuilder, typeInfo, properties);
            DefineDictionaryProperties(typeBuilder, typeInfo, properties);

            return typeBuilder;
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

        public static TypeBuilder DefineProxyType(this ModuleBuilder moduleBuilder, TypeInfo contract)
        {
            var proxyName = GetDynamicName(contract, "DynamicProxy");
            var proxy = DefineType(moduleBuilder, proxyName, null, contract);
            var target = DefineTargetField(proxy, contract);

            DefineTargetConstructor(proxy, target);
            DefineProxyProperties(proxy, contract, target);
            DefineProxyMethods(proxy, contract, target);

            return proxy;
        }

        public static TypeBuilder DefineSubType<TSuper>(this ModuleBuilder moduleBuilder)
        {
            // http://stackoverflow.com/questions/6879279/using-typebuilder-to-create-a-pass-through-constructor-for-the-base-class

            var superType = typeof(TSuper);
            var typeBuilder = DefineSubType(moduleBuilder, superType);

            return typeBuilder;
        }

        public static TypeBuilder DefineSubType(this ModuleBuilder moduleBuilder, Type parent)
        {
            if (parent.IsInterface)
                throw new InvalidOperationException("A dynamic sub type can only be created for classes.");

            var typeName = GetDynamicName(parent, "DynamicSubType");
            var typeBuilder = DefineType(moduleBuilder, typeName, parent);

            DefineDefaultConstructor(typeBuilder);

            return typeBuilder;
        }

        private static TypeBuilder DefineType(ModuleBuilder moduleBuilder, string typeName, Type parent, params Type[] contracts)
        {
            return moduleBuilder.DefineType(
                typeName,
                TypeAttributes.Class | TypeAttributes.Public,
                parent,
                contracts);
        }

        /// <summary>Gets the name of a dynamic proxy based on specified type.</summary>
        private static string GetDynamicName(Type type, string suffix)
        {
            return type.FullName + "`" + suffix;
        }
    }
}