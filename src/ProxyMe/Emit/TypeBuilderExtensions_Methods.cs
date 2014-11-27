using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ProxyMe.Emit
{
    public static partial class TypeBuilderExtensions
    {
        private static void DefineProxyMethods(TypeBuilder proxy, TypeInfo contract, FieldInfo target)
        {
            foreach (var method in contract.DeclaredMethods)
            {
                DefineProxyMethod(proxy, method, target);
            }
        }

        private static void DefineProxyMethod(TypeBuilder proxy, MethodInfo targetMethod, FieldInfo target)
        {
            var parameters = targetMethod.GetParameters();
            var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
            var proxyMethod = proxy.DefineMethod(targetMethod, parameterTypes);
            var il = proxyMethod.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);                // Load 'this'
            il.Emit(OpCodes.Ldfld, target);          // Load target field

            for (var i = 0; i < parameters.Length; i++)
            {
                il.Emit(OpCodes.Ldarg, i + 1);
            }

            il.Emit(OpCodes.Callvirt, targetMethod); // Call target method
            il.Emit(OpCodes.Ret);                    // Return
        }
    }
}