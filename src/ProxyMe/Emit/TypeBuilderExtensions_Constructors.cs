using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ProxyMe.Emit
{
    public static partial class TypeBuilderExtensions
    {
        private static ConstructorBuilder DefineDefaultConstructor(TypeBuilder type)
        {
            return type.DefineDefaultConstructor(MethodAttributes.Public);
        }

        private static ConstructorBuilder DefineInitializationConstructor(TypeBuilder type, Type contractType)
        {
            var actionType = typeof(Action<>).MakeGenericType(contractType);
            var actionInvoke = actionType.GetMethod("Invoke");
            var arguments = new[] { actionType };
            var constructor = type.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, arguments);
            var il = constructor.GetILGenerator();

            // Call base constructor
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, ObjectConstructor);

            // Call initialization delegate
            il.Emit(OpCodes.Ldarg_1);                // Load action method
            il.Emit(OpCodes.Ldarg_0);                // Load contract instance
            il.Emit(OpCodes.Callvirt, actionInvoke); // Call action

            // Return
            il.Emit(OpCodes.Ret);

            return constructor;
        }

        private static ConstructorBuilder DefinePropertiesConstructor(TypeBuilder type, TypeInfo contract, FieldInfo dictionary)
        {
            var argumentType = typeof(IDictionary<string, object>);
            var arguments = new[] { argumentType };
            var constructor = type.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, arguments);
            var il = constructor.GetILGenerator();

            // Call base constructor
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, ObjectConstructor);

            // Store dictionary in field
            il.Emit(OpCodes.Ldarg_0);           // Load 'this'
            il.Emit(OpCodes.Ldarg_1);           // Load dictionary
            il.Emit(OpCodes.Stfld, dictionary); // Store argument in field

            // Add default values for all properties if dictionary does not already contain matching values
            foreach (var property in contract.DeclaredProperties)
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

        private static ConstructorBuilder DefineTargetConstructor(TypeBuilder proxy, FieldInfo target)
        {
            var arguments = new[] { target.FieldType };
            var constructor = proxy.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, arguments);
            var il = constructor.GetILGenerator();

            // Call base constructor
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, ObjectConstructor);

            // Store target in field
            il.Emit(OpCodes.Ldarg_0);       // Load 'this'
            il.Emit(OpCodes.Ldarg_1);       // Load target from constructor argument
            il.Emit(OpCodes.Stfld, target); // Store target in field

            // Return
            il.Emit(OpCodes.Ret);

            return constructor;
        }
    }
}