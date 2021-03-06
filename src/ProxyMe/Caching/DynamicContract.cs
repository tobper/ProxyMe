﻿using System;
using ProxyMe.Emit;

namespace ProxyMe.Caching
{
    public static class DynamicContract<T>
        where T : class
    {
        private static readonly Type Type;
        private static readonly Func<T> DefaultConstructor;
        private static readonly Func<Action<T>, T> InitConstructor;

        static DynamicContract()
        {
            Type = CreateType();
            DefaultConstructor = Type.CreateConstructorDelegate<T>();
            InitConstructor = Type.CreateConstructorDelegate<T, Action<T>>();
        }

        public static T CreateInstance()
        {
            return DefaultConstructor();
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
            var contractType = typeof (T);
            var proxyBuilder = new DynamicContractBuilder();
            var proxyType = proxyBuilder.CreateType(contractType);

            return proxyType;
        }
    }
}