using System;
using System.Reflection.Emit;
using ProxyMe.Emit.Extensions;

namespace ProxyMe.Caching
{
    public static class ProxyModuleBuilder
    {
        private static readonly ModuleBuilder ModuleBuilder = AppDomain.CurrentDomain.
            DefineProxyAssembly("ProxyMe.DynamicTypes").
            DefineProxyModule();

        public static ModuleBuilder Get()
        {
            return ModuleBuilder;
        }
    }
}