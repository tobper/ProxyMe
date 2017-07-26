using System;
using System.Collections.Concurrent;
using System.Reflection;
using ProxyMe.Caching.Extensions;
using ProxyMe.Emit;

namespace ProxyMe.Caching
{
    public static class DynamicContract
    {
        private static readonly ConcurrentDictionary<Type, Func<object>> Constructors =
            new ConcurrentDictionary<Type, Func<object>>();

        public static object CreateInstance(Type type)
        {
            var constructor = Constructors.GetOrAdd(type, GetConstructor);
            var instance = constructor();

            return instance;
        }

        public static T CreateInstance<T>()
        {
            return Compiled<T>.DefaultConstructor();
        }

        public static T CreateInstance<T>(Action<T> initializer)
        {
            return Compiled<T>.InitConstructor(initializer);
        }

        private static Func<object> GetConstructor(Type contractType)
        {
            return (Func<object>)typeof(Compiled<>).
                MakeGenericType(contractType).
                GetRuntimeField("DefaultConstructor").
                GetValue(null);
        }

        private static class Compiled<T>
        {
            public static readonly Func<T> DefaultConstructor;
            public static readonly Func<Action<T>, T> InitConstructor;

            static Compiled()
            {
                var contractType = typeof(T);
                var dynamicBuilder = new DynamicContractBuilder();
                var dynamicType = dynamicBuilder.CreateType(contractType);

                DefaultConstructor = dynamicType.CreateConstructorDelegate<T>();
                InitConstructor = dynamicType.CreateConstructorDelegate<T, Action<T>>();
            }
        }
    }
}