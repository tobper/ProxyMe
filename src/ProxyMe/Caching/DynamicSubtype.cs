using System;
using ProxyMe.Caching.Extensions;
using ProxyMe.Emit;

namespace ProxyMe.Caching
{
    public static class DynamicSubType
    {
        public static T CreateInstance<T>()
        {
            return Compiled<T>.Constructor();
        }

        private static class Compiled<T>
        {
            public static readonly Func<T> Constructor;

            static Compiled()
            {
                var superType = typeof(T);
                var dynamicBuilder = new DynamicSubTypeBuilder();
                var dynamicType = dynamicBuilder.CreateType(superType);

                Constructor = dynamicType.CreateConstructorDelegate<T>();
            }
        }
    }
}