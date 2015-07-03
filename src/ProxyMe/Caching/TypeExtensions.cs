using System;
using System.Linq;
using System.Linq.Expressions;

namespace ProxyMe.Caching
{
    public static class TypeExtensions
    {
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
        public static Func<T> CreateConstructorDelegate<T>(this Type type)
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
        public static Func<TArg, T> CreateConstructorDelegate<T, TArg>(this Type type)
        {
            return CreateConstructorDelegate<Func<TArg, T>>(type, new[] { typeof(TArg) });
        }

        /// <summary>
        ///     Creates a delegate for the constructor with the provided arguments.
        /// </summary>
        /// <typeparam name="TFunc">
        ///     The type of delegate to return. This must match the provided
        ///     <paramref name="type"/> and <paramref name="constructorArguments"/>.
        /// </typeparam>
        /// <param name="type">
        ///     The <see cref="System.Type"/> to create a constructor delegate for.
        /// </param>
        /// <param name="constructorArguments">
        ///     The type of arguments of the constructor.
        /// </param>
        /// <returns>
        ///     Returns the delegate for the constructor.
        /// </returns>
        public static TFunc CreateConstructorDelegate<TFunc>(this Type type, Type[] constructorArguments)
        {
            var constructor = type.GetConstructor(constructorArguments);
            if (constructor == null)
                throw new InvalidOperationException("Type is missing constructor");

            var argumentExpressions = constructorArguments.
                Select(Expression.Parameter).
                ToArray();

            var constructionExpression = Expression.New(
                constructor,
                argumentExpressions.Cast<Expression>());

            return Expression.
                Lambda<TFunc>(constructionExpression, argumentExpressions).
                Compile();
        }
    }
}