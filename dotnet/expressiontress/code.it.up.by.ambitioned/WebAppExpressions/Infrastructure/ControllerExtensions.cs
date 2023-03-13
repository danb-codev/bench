using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace WebAppExpressions.Infrastructure;

public static class ControllerExtensions
{
    public static IActionResult RedirectTo<TController>(
        this Controller @this,
        Expression<Action<TController>> redirectExpression)
    {
        if (redirectExpression.Body.NodeType != ExpressionType.Call)
        {
            throw new InvalidOperationException($"The provided expression is not a valid method call: {redirectExpression.Body}");
        }

        var methodCallExpression = (MethodCallExpression)redirectExpression.Body;
        string actionName = GetActionName(methodCallExpression);
        string controllerName = typeof(TController).Name.Replace(nameof(Controller), "");
        var routeValues = ExtractRouteValues(methodCallExpression);

        return @this.RedirectToAction(actionName, controllerName, routeValues);
    }

    private static readonly ConcurrentDictionary<string, string> actionNameCache =
        new ConcurrentDictionary<string, string>();

    private static string GetActionName(MethodCallExpression expression)
    {
        string cacheKey = $"{expression.Method.Name}_{expression.Object.Type.Name}";

        return actionNameCache.GetOrAdd(cacheKey, _ =>
        {
            string methodName = expression.Method.Name;
            string? actionName = expression
                .Method
                .GetCustomAttributes(true)
                .OfType<ActionNameAttribute>()
                .FirstOrDefault()?.Name;

            return actionName ?? methodName;
        });
    }

    private static RouteValueDictionary ExtractRouteValues(MethodCallExpression expression)
    {
        // ["id", "query"]
        var names = expression.Method
            .GetParameters()
            .Select(p => p.Name)
            .ToArray();
        var values = expression.Arguments
            .Select(arg =>
            {
                if (arg.NodeType == ExpressionType.Constant)
                {
                    var constantExpression = (ConstantExpression)arg;

                    return constantExpression.Value;
                }

                // () => arg -> Func<object>
                var convertExpression = Expression.Convert(arg, typeof(object));
                var funcExpression = Expression.Lambda<Func<object>>(convertExpression);

                return funcExpression.Compile().Invoke();
            })
            .ToArray();
        var routeValueDictionary = new RouteValueDictionary();

        for (int i = 0; i < names.Length; i++)
        {
            routeValueDictionary.Add(names[i], values[i]);
        }

        return routeValueDictionary;
    }
}
