using System;
using System.Collections.Generic;
using ProxyMe.Caching;

namespace ProxyMe
{
    public static class Proxy
    {
        /// <summary>Creates a proxy of type <typeparamref name="T"/> for the provided <paramref name="target"/>.</summary>
        /// <typeparam name="T">The type of interface to create a proxy for.</typeparam>
        /// <returns>An proxy to the specified <paramref name="target"/>.</returns>
        public static T Create<T>(T target)
            where T : class
        {
            return DynamicProxy<T>.CreateInstance(target);
        }

        /// <summary>Creates an implementation of interface <typeparamref name="T"/>.</summary>
        /// <typeparam name="T">The type of interface to create an implementation for.</typeparam>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        public static T CreateContract<T>()
            where T : class
        {
            return DynamicContract<T>.CreateInstance();
        }

        /// <summary>Creates an implementation of interface <typeparamref name="T"/> and initializes it using the provided <paramref name="initializer"/>.</summary>
        /// <typeparam name="T">The type of interface to create an implementation for.</typeparam>
        /// <param name="initializer">The initializer to run for the constructed instance.</param>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        public static T CreateContract<T>(Action<T> initializer)
            where T : class
        {
            return DynamicContract<T>.CreateInstance(initializer);
        }

        /// <summary>Creates an implementation of interface <typeparamref name="T"/>.</summary>
        /// <typeparam name="T">The type of interface to create an implementation for.</typeparam>
        /// <param name="properties"></param>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        public static T CreateContract<T>(IDictionary<string, object> properties)
            where T : class
        {
            return DynamicDictionaryContract<T>.CreateInstance(properties);
        }

        /// <summary>Creates a subtype of <typeparamref name="T"/>.</summary>
        /// <typeparam name="T">The type of class to create a subtype for.</typeparam>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        public static T CreateSubType<T>()
            where T : class
        {
            return DynamicSubType<T>.CreateInstance();
        }
    }
}
