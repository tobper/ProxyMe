using System.Reflection.Emit;

namespace ProxyMe.Emit
{
    public static class AssemblyBuilderExtensions
    {
        public static ModuleBuilder DefineProxyModule(this AssemblyBuilder assemblyBuilder, bool transientModule = true)
        {
            var assemblyName = assemblyBuilder.GetName().Name;

            return transientModule
                ? DefineTransientModule(assemblyBuilder, assemblyName)
                : DefinePersistedModule(assemblyBuilder, assemblyName);
        }

        private static ModuleBuilder DefinePersistedModule(AssemblyBuilder assemblyBuilder, string assemblyName)
        {
            var fileName = assemblyName + ".dll";
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName, fileName);

            return moduleBuilder;
        }

        private static ModuleBuilder DefineTransientModule(AssemblyBuilder assemblyBuilder, string assemblyName)
        {
            return assemblyBuilder.DefineDynamicModule(assemblyName);
        }
    }
}