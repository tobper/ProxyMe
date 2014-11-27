using System.Reflection;
using System.Reflection.Emit;

namespace ProxyMe.Emit
{
    public static partial class TypeBuilderExtensions
    {
        private static void DefinePropertiesWithBackingField(TypeBuilder type, TypeInfo contract)
        {
            foreach (var property in contract.DeclaredProperties)
            {
                DefinePropertyWithBackingField(type, property);
            }
        }

        private static void DefinePropertyWithBackingField(TypeBuilder type, PropertyInfo property)
        {
            var fieldName = "_" + property.Name;
            var backingField = type.DefineField(fieldName, property.PropertyType, FieldAttributes.Private);
            var proxyProperty = type.DefineProperty(property);

            if (property.GetMethod != null)
            {
                var method = type.DefineGetMethod(property);
                var il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, backingField);
                il.Emit(OpCodes.Ret);

                proxyProperty.SetGetMethod(method);
            }

            if (property.SetMethod != null)
            {
                var method = type.DefineSetMethod(property);
                var il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stfld, backingField);
                il.Emit(OpCodes.Ret);

                proxyProperty.SetSetMethod(method);
            }
        }

        private static void DefineDictionaryProperties(TypeBuilder type, TypeInfo contract, FieldInfo backingField)
        {
            foreach (var property in contract.DeclaredProperties)
            {
                DefineDictionaryProperty(type, property, backingField);
            }
        }

        private static void DefineDictionaryProperty(TypeBuilder type, PropertyInfo property, FieldInfo backingField)
        {
            var proxyProperty = type.DefineProperty(property);

            if (property.GetMethod != null)
            {
                var method = type.DefineGetMethod(property);
                var il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);                               // Load 'this'
                il.Emit(OpCodes.Ldfld, backingField);                   // Load backing field
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
                var method = type.DefineSetMethod(property);
                var il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);                           // Load 'this'
                il.Emit(OpCodes.Ldfld, backingField);               // Load backing field
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

        private static void DefineProxyProperties(TypeBuilder proxy, TypeInfo contract, FieldInfo target)
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

                il.Emit(OpCodes.Ldarg_0);                               // Load 'this'
                il.Emit(OpCodes.Ldfld, target);                         // Load target
                il.Emit(OpCodes.Callvirt, targetProperty.GetMethod);    // Call target 'get method'
                il.Emit(OpCodes.Ret);                                   // Return

                proxyProperty.SetGetMethod(method);
            }

            if (targetProperty.SetMethod != null)
            {
                var method = proxy.DefineSetMethod(targetProperty);
                var il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);                               // Load 'this'
                il.Emit(OpCodes.Ldfld, target);                         // Load target
                il.Emit(OpCodes.Ldarg_1);                               // Load value
                il.Emit(OpCodes.Callvirt, targetProperty.SetMethod);    // Call target 'set method'
                il.Emit(OpCodes.Ret);                                   // Return

                proxyProperty.SetSetMethod(method);
            }
        }
    }
}