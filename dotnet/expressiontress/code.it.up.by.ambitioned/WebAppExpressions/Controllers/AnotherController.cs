using Microsoft.AspNetCore.Mvc;
using WebAppExpressions.Infrastructure;

namespace WebAppExpressions.Controllers;

public class AnotherController : Controller
{
    public IActionResult About()
    {
        int id = 5;

        return this.RedirectTo<HomeController>(c => c.Index(id, "MyTestedASP.NET"));
    }
}
