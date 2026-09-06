using Microsoft.AspNetCore.Mvc;

namespace ShiftPlanner.Api.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}