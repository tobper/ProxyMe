using System;
using System.Linq;
using System.Linq.Expressions;

namespace ProxyMe.Caching
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Creates a delegate to the default constructor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Func<T> CreateConstructorDelegate<T>(this Type type)
        {
            return CreateConstructorDelegate<Func<T>>(type, Type.EmptyTypes);
        }

        /// <summary>
        /// Creates a delegate to the constructor with the provided argument.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TArg"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Func<TArg, T> CreateConstructorDelegate<T, TArg>(this Type type)
        {
            return CreateConstructorDelegate<Func<TArg, T>>(type, new[] { typeof(TArg) });
        }

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