using System;
using System.Collections.Generic;
using System.Reflection;
using ProxyMe.Emit;

namespace ProxyMe.Caching
{
    public static class DynamicDictionaryContract<T>
        where T : class
    {
        private static readonly TypeInfo Type;
        private static readonly Func<IDictionary<string, object>, T> Constructor;

        static DynamicDictionaryContract()
        {
            Type = CreateType();
            Constructor = Type.CreateConstructorDelegate<T, IDictionary<string, object>>();
        }

        public static T CreateInstance(IDictionary<string, object> properties)
        {
            return Constructor(properties);
        }

        public static Type GetDynamicType()
        {
            return Type.GetType();
        }

        private static TypeInfo CreateType()
        {
            var contractType = typeof(T);
            var proxyBuilder = new DynamicDictionaryContractBuilder();
            var proxyType = proxyBuilder.CreateType(contractType);

            return proxyType;
        }
    }
}