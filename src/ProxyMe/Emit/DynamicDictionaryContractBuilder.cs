using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ProxyMe.Emit.Extensions;

namespace ProxyMe.Emit
{
    public class DynamicDictionaryContractBuilder : DynamicTypeBuilder
    {
        private static readonly MethodInfo DictionaryGetMethod;
        private static readonly MethodInfo DictionarySetMethod;
        private static readonly MethodInfo DictionaryContainsKeyMethod;
        private static readonly Type PropertiesType = typeof(IDictionary<string, object>);
        private FieldBuilder _propertiesField;

        static DynamicDictionaryContractBuilder()
        {
            // Get reference to dictionary get/set methods
            var dictionaryType = typeof(IDictionary<string, object>);

            DictionaryGetMethod = dictionaryType.GetMethod("get_Item");
            DictionarySetMethod = dictionaryType.GetMethod("set_Item");
            DictionaryContainsKeyMethod = dictionaryType.GetMethod("ContainsKey");
        }

        protected override void DefineConstructors(TypeBuilder typeBuilder)
        {
            DefinePropertiesConstructor(typeBuilder);
        }

        protected override void DefineFields(TypeBuilder typeBuilder)
        {
            _propertiesField = typeBuilder.DefineField(
                "_properties",
                PropertiesType,
                FieldAttributes.Private | FieldAttributes.InitOnly);
        }

        private ConstructorBuilder DefinePropertiesConstructor(TypeBuilder type)
        {
            var argumentType = typeof(IDictionary<string, object>);
            var arguments = new[] { argumentType };
            var constructor = type.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, arguments);
            var il = constructor.GetILGenerator();

            // Call base constructor
            il.CallDefaultObjectConstructor();

            // Store dictionary in field
            il.Emit(OpCodes.Ldarg_0);                                       // Load 'this'
            il.Emit(OpCodes.Ldarg_1);                                       // Load dictionary
            il.Emit(OpCodes.Stfld, _propertiesField);                       // Store argument in field

            // Add default values for all properties if dictionary does not already contain matching values
            foreach (var property in ReferenceTypeInfo.DeclaredProperties)
            {
                var label = il.DefineLabel();

                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldstr, property.Name);                      // Load property name
                il.Emit(OpCodes.Callvirt, DictionaryContainsKeyMethod);
                il.Emit(OpCodes.Brtrue_S, label);

                il.Emit(OpCodes.Ldarg_1);                                   // Load dictionary
                il.Emit(OpCodes.Ldstr, property.Name);                      // Load property name

                if (property.PropertyType.IsValueType)
                {
                    il.Emit(OpCodes.Ldc_I4_0);                              // Load default value for value types
                    il.Emit(OpCodes.Box, property.PropertyType);            // Box default value
                }
                else
                {
                    il.Emit(OpCodes.Ldnull);                                // Load null for reference types
                }

                il.Emit(OpCodes.Callvirt, DictionarySetMethod);             // Set default value
                il.MarkLabel(label);
            }

            // Return
            il.Emit(OpCodes.Ret);

            return constructor;
        }

        protected override void DefineProperty(TypeBuilder typeBuilder, PropertyInfo property)
        {
            var proxyProperty = typeBuilder.DefineProperty(property);

            if (property.GetMethod != null)
            {
                var method = typeBuilder.DefineGetMethod(property);
                var il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);                               // Load 'this'
                il.Emit(OpCodes.Ldfld, _propertiesField);               // Load backing field
                il.Emit(OpCodes.Ldstr, property.Name);                  // Load name of property
                il.Emit(OpCodes.Callvirt, DictionaryGetMethod);         // Call method

                if (property.PropertyType.IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, property.PropertyType);  // Unbox value type
                }
                else
                {
                    il.Emit(OpCodes.Castclass, property.PropertyType);  // Cast to reference type
                }

                il.Emit(OpCodes.Ret);

                proxyProperty.SetGetMethod(method);
            }

            if (property.SetMethod != null)
            {
                var method = typeBuilder.DefineSetMethod(property);
                var il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);                           // Load 'this'
                il.Emit(OpCodes.Ldfld, _propertiesField);           // Load backing field
                il.Emit(OpCodes.Ldstr, property.Name);              // Load name of property
                il.Emit(OpCodes.Ldarg_1);                           // Load value

                if (property.PropertyType.IsValueType)
                {
                    il.Emit(OpCodes.Box, property.PropertyType);    // Box value type
                }

                il.Emit(OpCodes.Callvirt, DictionarySetMethod);     // Call method
                il.Emit(OpCodes.Ret);

                proxyProperty.SetSetMethod(method);
            }
        }

        protected override TypeBuilder DefineType(ModuleBuilder moduleBuilder, string typeName)
        {
            if (ReferenceType.IsInterface == false)
                throw new InvalidOperationException("A dynamic contract can only be created for interfaces.");

            if (ReferenceTypeInfo.DeclaredMethods.Any(m => m.IsSpecialName == false))
                throw new InvalidOperationException("A dynamic contract can not be created for an interface with methods.");

            return base.DefineType(moduleBuilder, typeName);
        }

        protected override string GetTypeName()
        {
            return ReferenceType.GetProxyTypeName("DynamicDictionaryContract");
        }
    }
}