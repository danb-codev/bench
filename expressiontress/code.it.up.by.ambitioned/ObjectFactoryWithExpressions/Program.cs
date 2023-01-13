using System.Diagnostics;
using ObjectFactoryWithExpressions;

const int itemCount = 1;

var list = new List<Cat>();
var stopwatch = Stopwatch.StartNew();

for (int i = 0; i < itemCount; i++)
{
    var cat = Activator.CreateInstance<Cat>();

    list.Add(cat);
}

Console.WriteLine($"{stopwatch.Elapsed} - Activator - No parameters in constructor");
Console.WriteLine(list.Count);

New<Cat>.Instance();
list = new List<Cat>();

stopwatch = Stopwatch.StartNew();

var catType = typeof(Cat);

catType.CreateInstance<Cat>();
 

for (int i = 0; i < itemCount; i++)
{
    var cat = New<Cat>.Instance();

    list.Add(cat);
}

Console.WriteLine($"{stopwatch.Elapsed} - Expression Trees - No parameters in constructor");
Console.WriteLine(list.Count);

list = new List<Cat>();
stopwatch = Stopwatch.StartNew();

for (int i = 0; i < itemCount; i++)
{
    var cat = (Cat)Activator.CreateInstance(typeof(Cat), "My Cool Cat", 2);

    list.Add(cat);
}

Console.WriteLine($"{stopwatch.Elapsed} - Activator - 2 parameters in constructor");

list = new List<Cat>();
ObjectFactory.CreateInstance(catType, "My Cool Cat", 2);
stopwatch = Stopwatch.StartNew();

for (int i = 0; i < itemCount; i++)
{
    var cat = (Cat)ObjectFactory.CreateInstance(typeof(Cat), "My Cool Cat", 2);

    list.Add(cat);
}

Console.WriteLine($"{stopwatch.Elapsed} - Expression Trees - 2 parameters in constructor");
Console.WriteLine(list.Count);
