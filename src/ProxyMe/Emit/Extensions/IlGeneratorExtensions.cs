using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ProxyMe.Emit.Extensions
{
    public static class IlGeneratorExtensions
    {
        private static readonly ConstructorInfo ObjectConstructor;

        static IlGeneratorExtensions()
        {
            // Get reference to default Object constructor
            var objectType = typeof(object);
            var defaultConstructor = objectType.GetConstructor(Type.EmptyTypes);

            ObjectConstructor = defaultConstructor;
        }

        public static void CallDefaultObjectConstructor(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, ObjectConstructor);
        }
    }
}