using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ProxyMe.Emit.Extensions
{
    public static partial class TypeBuilderExtensions
    {
        public static ConstructorBuilder DefineDefaultConstructor(this TypeBuilder type)
        {
            return type.DefineDefaultConstructor(MethodAttributes.Public);
        }

        public static ConstructorBuilder DefineInitializationConstructor(this TypeBuilder typeBuilder, Type type)
        {
            var actionType = typeof(Action<>).MakeGenericType(type);
            var actionTypeInfo = actionType.GetTypeInfo();
            var actionInvoke = actionTypeInfo.DeclaredMethods.Single(m => m.Name == "Invoke");
            var arguments = new[] { actionType };
            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, arguments);
            var il = constructor.GetILGenerator();

            // Call base constructor
            il.CallDefaultObjectConstructor();

            // Call initialization delegate
            il.Emit(OpCodes.Ldarg_1);                // Load action method
            il.Emit(OpCodes.Ldarg_0);                // Load contract instance
            il.Emit(OpCodes.Callvirt, actionInvoke); // Call action

            // Return
            il.Emit(OpCodes.Ret);

            return constructor;
        }
    }
}