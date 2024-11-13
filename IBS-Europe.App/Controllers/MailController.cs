using Microsoft.AspNetCore.Mvc;

namespace IBS_Europe.App;

public class MailController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}