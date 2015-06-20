using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ProxyMe.Emit.Extensions
{
    public static class AppDomainExtensions
    {
        public static AssemblyBuilder DefineProxyAssembly(this AppDomain appDomain, string assemblyName)
        {
            var name = new AssemblyName(assemblyName);
            var assembly = appDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);

            return assembly;
        }

        public static AssemblyBuilder DefineProxyAssembly(this AppDomain appDomain, string assemblyName, string directory)
        {
            var name = new AssemblyName(assemblyName);
            var assembly = appDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndSave, directory);

            return assembly;
        }
    }
}