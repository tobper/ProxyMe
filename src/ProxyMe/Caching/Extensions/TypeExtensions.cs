using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ProxyMe.Caching.Extensions
{
    public static class TypeExtensions
    {
        ///// <summary>
        /////     Creates a delegate for the default constructor.
        ///// </summary>
        ///// <param name="type">
        /////     The <see cref="System.Type"/> to create a constructor delegate for.
        ///// </param>
        ///// <returns>
        /////     Returns the delegate for the constructor.
        ///// </returns>
        //public static Func<object> CreateConstructorDelegate(this TypeInfo type)
        //{
        //    return CreateConstructorDelegate<Func<object>>(type, Type.EmptyTypes);
        //}

        /// <summary>
        ///     Creates a delegate for the default constructor.
        /// </summary>
        /// <typeparam name="T">
        ///     The statically typed version of <paramref name="type"/>.
        /// </typeparam>
        /// <param name="type">
        ///     The <see cref="System.Type"/> to create a constructor delegate for.
        /// </param>
        /// <returns>
        ///     Returns the delegate for the constructor.
        /// </returns>
        public static Func<T> CreateConstructorDelegate<T>(this TypeInfo type)
        {
            return CreateConstructorDelegate<Func<T>>(type, Type.EmptyTypes);
        }

        /// <summary>
        ///     Creates a delegate for the constructor with the provided argument.
        /// </summary>
        /// <typeparam name="T">
        ///     The statically typed version of <paramref name="type"/>.
        /// </typeparam>
        /// <typeparam name="TArg">
        ///     The type of the argument to the constructor.
        /// </typeparam>
        /// <param name="type">
        ///     The <see cref="System.Type"/> to create a constructor delegate for.
        /// </param>
        /// <returns>
        ///     Returns the delegate for the constructor.
        /// </returns>
        public static Func<TArg, T> CreateConstructorDelegate<T, TArg>(this TypeInfo type)
        {
            return CreateConstructorDelegate<Func<TArg, T>>(type, new[] { typeof(TArg) });
        }

        /// <summary>
        ///     Creates a delegate for the constructor with the provided arguments.
        /// </summary>
        /// <typeparam name="TFunc">
        ///     The type of delegate to return. This must match the provided
        ///     <paramref name="type"/> and <paramref name="parameters"/>.
        /// </typeparam>
        /// <param name="type">
        ///     The <see cref="System.Type"/> to create a constructor delegate for.
        /// </param>
        /// <param name="parameters">
        ///     The type of arguments of the constructor.
        /// </param>
        /// <returns>
        ///     Returns the delegate for the constructor.
        /// </returns>
        public static TFunc CreateConstructorDelegate<TFunc>(this TypeInfo type, IReadOnlyList<Type> parameters)
        {
            var constructor = GetConstructor(type, parameters);
            if (constructor == null)
                throw new InvalidOperationException("Type is missing constructor");

            var argumentExpressions = parameters.
                Select(Expression.Parameter).
                ToArray();

            var constructionExpression = Expression.New(
                constructor,
                argumentExpressions.Cast<Expression>());

            return Expression.
                Lambda<TFunc>(constructionExpression, argumentExpressions).
                Compile();
        }

        private static ConstructorInfo GetConstructor(TypeInfo typeInfo, IReadOnlyList<Type> parameters)
        {
            bool CollectionEquals<T>(IReadOnlyList<T> types1, IReadOnlyList<T> types2) where T: class
            {
                if (types1.Count != types2.Count)
                    return false;

                for (var i = 0; i < types1.Count; i++)
                {
                    if (types1[i] != types2[i])
                        return false;
                }

                return true;
            }

            foreach (var declaredConstructor in typeInfo.DeclaredConstructors)
            {
                var declaredParameters = declaredConstructor.
                    GetParameters().
                    Select(p => p.ParameterType).
                    ToArray();

                if (CollectionEquals(parameters, declaredParameters))
                    return declaredConstructor;
            }

            return null;
        }
    }
}