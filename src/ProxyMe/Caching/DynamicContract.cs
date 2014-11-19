using System;
using ProxyMe.Emit;

namespace ProxyMe.Caching
{
    public static class DynamicContract<T>
        where T : class
    {
        private static readonly Type Type;
        private static readonly Func<T> Constructor;
        private static readonly Func<Action<T>, T> InitConstructor;

        static DynamicContract()
        {
            Type = CreateType();
            Constructor = Type.CreateConstructorDelegate<T>();
            InitConstructor = Type.CreateConstructorDelegate<T, Action<T>>();
        }

        public static T CreateInstance()
        {
            return Constructor();
        }

        public static T CreateInstance(Action<T> initializer)
        {
            return InitConstructor(initializer);
        }

        public static Type GetDynamicType()
        {
            return Type;
        }

        private static Type CreateType()
        {
            return ProxyModuleBuilder.
                Get().
                DefineContractType<T>().
                CreateType();
        }
    }
}