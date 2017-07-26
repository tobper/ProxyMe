using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using ProxyMe.Caching.Extensions;
using ProxyMe.Emit;

namespace ProxyMe.Caching
{
    using ParameterDictionary = IDictionary<string, object>;

    public static class DynamicDictionaryContract
    {
        private static readonly ConcurrentDictionary<Type, Func<ParameterDictionary, object>> Constructors =
            new ConcurrentDictionary<Type, Func<ParameterDictionary, object>>();

        public static object CreateInstance(Type type, ParameterDictionary properties)
        {
            var constructor = Constructors.GetOrAdd(type, GetConstructor);
            var instance = constructor(properties);

            return instance;
        }

        public static T CreateInstance<T>(IDictionary<string, object> properties)
        {
            return Compiled<T>.Constructor(properties);
        }

        private static Func<ParameterDictionary, object> GetConstructor(Type contractType)
        {
            return (Func<ParameterDictionary, object>)typeof(Compiled<>).
                MakeGenericType(contractType).
                GetRuntimeField("Constructor").
                GetValue(null);
        }

        private static class Compiled<T>
        {
            public static readonly Func<IDictionary<string, object>, T> Constructor;

            static Compiled()
            {
                var contractType = typeof(T);
                var dynamicBuilder = new DynamicDictionaryContractBuilder();
                var dynamicType = dynamicBuilder.CreateType(contractType);

                Constructor = dynamicType.CreateConstructorDelegate<T, IDictionary<string, object>>();
            }
        }
    }
}