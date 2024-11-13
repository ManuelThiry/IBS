using Microsoft.AspNetCore.Mvc;

namespace IBS_Europe.App.Pages.Shared;

public class Controller1 : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}