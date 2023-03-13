using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace ObjectFactoryWithExpressions;

public static class ObjectFactory
{
    // () => new T
    public static object CreateInstance<T>(this Type _)
        where T : new()
        => New<T>.Instance();

    public static object CreateInstance<TArg>(this Type @this, TArg argument)
        => CreateInstance<TArg, TypeToIgnore>(@this, argument, null);

    public static object CreateInstance<TArg1, TArg2>(this Type @this, TArg1 argument1, TArg2 argument2)
        => CreateInstance<TArg1, TArg2, TypeToIgnore>(@this, argument1, argument2, null);

    public static object CreateInstance<TArg1, TArg2, TArg3>(
        this Type @this,
        TArg1 argument1,
        TArg2 argument2,
        TArg3 argument3)
        => ObjectFactoryCreator<TArg1, TArg2, TArg3>.CreateInstance(@this, argument1, argument2, argument3);

    private class TypeToIgnore
    {
    }

    // (TArg1 arg1, TArg2 arg2, TArg3 arg3) => new Type(arg1, arg2, arg3);
    private static class ObjectFactoryCreator<TArg1, TArg2, TArg3>
    {
        private static Type TypeToIgnore = typeof(TypeToIgnore);

        private static ConcurrentDictionary<Type, Func<TArg1, TArg2, TArg3, object>> objectFactoryCache = new();

        //public Func<Type, TArg1, TArg2, TArg3, object> 
        public static object CreateInstance(Type @this, 
            TArg1 argument1, 
            TArg2 argument2, 
            TArg3 argument3)
        {
            var objectFactoryFunc = objectFactoryCache.GetOrAdd(@this, _ =>
            {
                var argumentTypes = new[]
                {
                    typeof(TArg1),
                    typeof(TArg2),
                    typeof(TArg3)
                };
                var constructorArgumentTypes = argumentTypes
                    .Where(t => t != TypeToIgnore)
                    .ToArray();
                var constructor = @this.GetConstructor(constructorArgumentTypes);

                if (constructor is null)
                {
                    throw new InvalidOperationException($"{@this.Name} does not contain a constructor for the provided argument types: {string.Join(",", constructorArgumentTypes.Select(select => select.Name))}.");
                }

                // (TArg1 arg1, TArg2 arg2, TArg3 arg3)
                var expressionParameters = argumentTypes
                    .Select((t, i) => Expression.Parameter(t, $"arg{i}"))
                    .ToArray();
                var expressionConstructorParameters = expressionParameters
                    .Take(constructorArgumentTypes.Length)
                    .ToArray();
                // new Type(arg1, arg2, arg3);
                var newExpression = Expression.New(constructor, expressionConstructorParameters);
                // (TArg1 arg1, TArg2 arg2, TArg3 arg3) => new Type(arg1, arg2, arg3);
                var lambdaExpression =
                    Expression.Lambda<Func<TArg1, TArg2, TArg3, object>>(newExpression, expressionParameters);

                return lambdaExpression.Compile();
            });

            return objectFactoryFunc(argument1, argument2, argument3);
        }
    }
}
