using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using ProxyMe.Caching.Extensions;
using ProxyMe.Emit;

namespace ProxyMe.Caching
{
    public static class DynamicProxy
    {
        private static readonly ConcurrentDictionary<Type, Func<object, object>> Constructors =
            new ConcurrentDictionary<Type, Func<object, object>>();

        public static object CreateInstance(Type type, object target)
        {
            var constructor = Constructors.GetOrAdd(type, GetConstructor);
            var instance = constructor(target);

            return instance;
        }

        public static T CreateInstance<T>(T target)
        {
            return Compiled<T>.Constructor(target);
        }

        private static Func<object, object> GetConstructor(Type proxyType)
        {
            return (Func<object, object>)typeof(Compiled<>).
                MakeGenericType(proxyType).
                GetRuntimeField("ObjectConstructor").
                GetValue(null);
        }

        private static class Compiled<T>
        {
            public static readonly Func<T, T> Constructor;
            public static readonly Func<object, object> ObjectConstructor;

            static Compiled()
            {
                var proxyType = typeof(T);
                var dynamicBuilder = new DynamicProxyBuilder();
                var dynamicType = dynamicBuilder.CreateType(proxyType);

                Constructor = dynamicType.CreateConstructorDelegate<T, T>();
                ObjectConstructor = obj => Constructor((T)obj);
            }
        }
    }
}