using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ProxyMe.Emit.Extensions;

namespace ProxyMe.Emit
{
    public class DynamicProxyBuilder : DynamicTypeBuilder
    {
        private FieldBuilder _targetField;

        protected override void DefineConstructors(TypeBuilder typeBuilder)
        {
            var arguments = new[] { _targetField.FieldType };
            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, arguments);
            var il = constructor.GetILGenerator();

            // Call base constructor
            il.CallDefaultObjectConstructor();

            // Store target in field
            il.Emit(OpCodes.Ldarg_0);                                   // Load 'this'
            il.Emit(OpCodes.Ldarg_1);                                   // Load target from constructor argument
            il.Emit(OpCodes.Stfld, _targetField);                       // Store target in field

            // Return
            il.Emit(OpCodes.Ret);
        }

        protected override void DefineFields(TypeBuilder typeBuilder)
        {
            _targetField = typeBuilder.DefineField(
                "_target",
                ReferenceType,
                FieldAttributes.Private | FieldAttributes.InitOnly);
        }

        protected override void DefineMethod(TypeBuilder typeBuilder, MethodInfo method)
        {
            var parameters = method.GetParameters();
            var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
            var proxyMethod = typeBuilder.DefineMethod(method, parameterTypes);
            var il = proxyMethod.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);                                   // Load 'this'
            il.Emit(OpCodes.Ldfld, _targetField);                       // Load target field

            for (var i = 0; i < parameters.Length; i++)
            {
                il.Emit(OpCodes.Ldarg, i + 1);                          // Load arguments
            }

            il.Emit(OpCodes.Callvirt, method);                          // Call target method
            il.Emit(OpCodes.Ret);                                       // Return
        }

        protected override void DefineProperty(TypeBuilder typeBuilder, PropertyInfo property)
        {
            var proxyProperty = typeBuilder.DefineProperty(property);

            if (property.GetMethod != null)
            {
                var method = typeBuilder.DefineGetMethod(proxyProperty);
                var il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);                               // Load 'this'
                il.Emit(OpCodes.Ldfld, _targetField);                   // Load target
                il.Emit(OpCodes.Callvirt, property.GetMethod);          // Call target 'get method'
                il.Emit(OpCodes.Ret);                                   // Return

                proxyProperty.SetGetMethod(method);
            }

            if (property.SetMethod != null)
            {
                var method = typeBuilder.DefineSetMethod(property);
                var il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);                               // Load 'this'
                il.Emit(OpCodes.Ldfld, _targetField);                   // Load target
                il.Emit(OpCodes.Ldarg_1);                               // Load value
                il.Emit(OpCodes.Callvirt, property.SetMethod);          // Call target 'set method'
                il.Emit(OpCodes.Ret);                                   // Return

                proxyProperty.SetSetMethod(method);
            }
        }

        protected override TypeBuilder DefineType(ModuleBuilder moduleBuilder, string typeName)
        {
            if (ReferenceTypeInfo.IsInterface == false)
                throw new InvalidOperationException("A dynamic proxy can only be created for interfaces.");

            return base.DefineType(moduleBuilder, typeName);
        }

        protected override string GetTypeName()
        {
            return ReferenceType.GetProxyTypeName("DynamicProxy");
        }
    }
}