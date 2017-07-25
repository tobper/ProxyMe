using System;
using System.Reflection;
using ProxyMe.Emit;

namespace ProxyMe.Caching
{
    public static class DynamicProxy<T>
    {
        private static readonly TypeInfo Type;
        private static readonly Func<T, T> Constructor;

        static DynamicProxy()
        {
            Type = CreateType();
            Constructor = Type.CreateConstructorDelegate<T, T>();
        }

        public static T CreateInstance(T target)
        {
            return Constructor(target);
        }

        public static Type GetDynamicType()
        {
            return Type.GetType();
        }

        private static TypeInfo CreateType()
        {
            var contractType = typeof(T);
            var proxyBuilder = new DynamicProxyBuilder();
            var proxyType = proxyBuilder.CreateType(contractType);

            return proxyType;
        }
    }
}