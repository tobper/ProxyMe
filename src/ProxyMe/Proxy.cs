using System;
using System.Collections.Generic;
using ProxyMe.Caching;

namespace ProxyMe
{
    /// <summary>
    ///     Defines and creates new instances of classes and interfaces during run time.
    /// </summary>
    public static class Proxy
    {
        /// <summary>
        ///     Creates a dynamic proxy of type <paramref name="type"/> for the
        ///     provided <paramref name="target"/> instance.
        /// </summary>
        /// <param name="type">
        ///     The type of interface to create a proxy for.
        /// </param>
        /// <param name="target">
        ///     The instance to create a proxy for.
        /// </param>
        /// <returns>
        ///     A proxy for the specified <paramref name="target"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     <paramref name="type"/> is not an interface.
        /// </exception>
        public static object Create(Type type, object target)
        {
            return DynamicProxy.CreateInstance(type, target);
        }

        /// <summary>
        ///     Creates a dynamic proxy of type <typeparamref name="T"/> for the
        ///     provided <paramref name="target"/> instance.
        /// </summary>
        /// <param name="target">
        ///     The instance to create a proxy for.
        /// </param>
        /// <typeparam name="T">
        ///     The type of interface to create a proxy for.
        /// </typeparam>
        /// <returns>
        ///     A proxy for the specified <paramref name="target"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     <typeref name="T"/> is not an interface.
        /// </exception>
        public static T Create<T>(T target)
            where T : class
        {
            return DynamicProxy.CreateInstance(target);
        }

        /// <summary>
        ///     Creates a dynamic implementation of interface specified as argument.
        /// </summary>
        /// <param name="type">
        ///     The type of interface to create an implementation for.
        /// </param>
        /// <returns>
        ///     An instance of <paramref name="type"></paramref>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     <paramref name="type"/> is not an interface.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <paramref name="type"/> contains methods.
        /// </exception>
        public static object CreateContract(Type type)
        {
            return DynamicContract.CreateInstance(type);
        }

        /// <summary>
        ///     Creates a dynamic implementation of interface <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of interface to create an implementation for.
        /// </typeparam>
        /// <returns>
        ///     An instance of <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     <typeref name="T"/> is not an interface.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <typeref name="T"/> contains methods.
        /// </exception>
        public static T CreateContract<T>()
            where T : class
        {
            return DynamicContract.CreateInstance<T>();
        }

        /// <summary>
        ///     Creates an implementation of interface <typeparamref name="T"/> and
        ///     initializes it using the provided <paramref name="initializer"/> delegate.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of interface to create an implementation for.
        /// </typeparam>
        /// <param name="initializer">
        ///     The initializer to run when an instance has been created.
        /// </param>
        /// <returns>
        ///     An instance of <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     <typeref name="T"/> is not an interface.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <typeref name="T"/> contains methods.
        /// </exception>
        public static T CreateContract<T>(Action<T> initializer)
            where T : class
        {
            return DynamicContract.CreateInstance(initializer);
        }

        /// <summary>
        ///     Creates an implementation of interface <paramref name="type"/> using
        ///     the specified <paramref name="properties"/> dictionary as backing for
        ///     the interface properties.
        /// </summary>
        /// <param name="type">
        ///     The type of interface to create an implementation for.
        /// </param>
        /// <param name="properties">
        ///     The dictionary to use as backing storage for the properties on the interface.
        /// </param>
        /// <returns>
        ///     An instance of <paramref name="type"></paramref>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     <paramref name="type"/> is not an interface.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <paramref name="type"/> contains methods.
        /// </exception>
        public static object CreateContract(Type type, IDictionary<string, object> properties)
        {
            return DynamicDictionaryContract.CreateInstance(type, properties);
        }

        /// <summary>
        ///     Creates an implementation of interface <typeparamref name="T"/> using
        ///     the specified <paramref name="properties"/> dictionary as backing for
        ///     the interface properties.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of interface to create an implementation for.
        /// </typeparam>
        /// <param name="properties">
        ///     The dictionary to use as backing storage for the properties on the interface.
        /// </param>
        /// <returns>
        ///     An instance of <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     <typeref name="T"/> is not an interface.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <typeref name="T"/> contains methods.
        /// </exception>
        public static T CreateContract<T>(IDictionary<string, object> properties)
            where T : class
        {
            return DynamicDictionaryContract.CreateInstance<T>(properties);
        }

        /// <summary>
        ///     Creates a subtype of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of class to create a subtype for.
        /// </typeparam>
        /// <returns>
        ///     An instance of <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     <typeref name="T"/> is not a class.
        /// </exception>
        public static T CreateSubType<T>()
            where T : class
        {
            return DynamicSubType.CreateInstance<T>();
        }
    }
}
