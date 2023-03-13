using System.Linq.Expressions;

namespace ExpressionTreesPlayground;

class Program
{
    static void Main(string[] args)
    {
        Expression<Func<MyClass, string>> expr = c => c.MyMethod(42, "My tested ASP.NET");

        var myClass = new MyClass();
        var numberConstant = Expression.Constant(42);
        var textConstant = Expression.Constant("My tested ASP.NET");
        var myClassType = typeof(MyClass);
        var parameterExpression = Expression.Parameter(myClassType, "c");
        var methodInfo = myClassType.GetMethod(nameof(MyClass.MyMethod));
        var callExpression = Expression.Call(parameterExpression, methodInfo, numberConstant, textConstant);
        var lambdaExpression = Expression.Lambda<Func<MyClass, string>>(callExpression, parameterExpression);
        var func = lambdaExpression.Compile();

        Console.WriteLine(func(myClass));
    }

    private static void ParseExpression(Expression expression)
    {
        if (expression.NodeType == ExpressionType.Lambda) 
        { 
            var lambdaExpression = (LambdaExpression)expression;

            Console.Write("Lambda ");
            Console.WriteLine(lambdaExpression.Parameters[0].Name);

            ParseExpression(lambdaExpression.Body);
        }
        else if (expression.NodeType == ExpressionType.Call)
        {
            var methodCallExpression = (MethodCallExpression)expression;

            Console.Write("Method ");
            Console.WriteLine(methodCallExpression.Method.Name);

            for (int i = 0; i < methodCallExpression.Arguments.Count; i++)
            {
                ParseExpression(methodCallExpression.Arguments[i]);
            }
        }
        else if (expression.NodeType == ExpressionType.MemberAccess)
        {
            var memberExpression = (MemberExpression)expression;

            Console.Write("Property ");
            Console.WriteLine(memberExpression.Member.Name);
        }
        else if (expression.NodeType == ExpressionType.Constant)
        {
            var constantExpression = (ConstantExpression)expression;

            Console.Write("Constant ");
            Console.WriteLine(constantExpression.Value);
        }
    }
}


public class MyClass
{
    public bool MyProperty { get; set; }

    public string MyMethod(int number, string text) => $"From method: {number + text}";
}
