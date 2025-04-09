using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Controllers;

namespace Nop.Web.Controllers;

public class AboutController : BasePublicController
{
    public IActionResult Index()
    {
        ViewBag.Message = "Hello from About view!";
        return View();
    }
}