﻿using System;
using ProxyMe.Emit;

namespace ProxyMe.Caching
{
    public static class DynamicSubType<T>
    {
        private static readonly Type Type;
        private static readonly Func<T> Constructor;

        static DynamicSubType()
        {
            Type = CreateType();
            Constructor = Type.CreateConstructorDelegate<T>();
        }

        public static T CreateInstance()
        {
            return Constructor();
        }

        public static Type GetDynamicType()
        {
            return Type;
        }

        private static Type CreateType()
        {
            return ProxyModuleBuilder.
                Get().
                DefineSubType<T>().
                CreateType();
        }
    }
}