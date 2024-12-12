using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace IBS_Europe.App.Controllers;

public class HomeController : Controller
{
    private readonly IConfiguration _configuration;
    
    public HomeController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [HttpPost]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );

        return LocalRedirect(returnUrl);
    }

    [HttpGet("tinymce-script.js")]
    public IActionResult GetTinyMceScript()
    {
        if ( !User.Identity.IsAuthenticated )
        {
            return NotFound();
        }

        var apiKey = Environment.GetEnvironmentVariable("TINYMCE_API_KEY");
        var version = _configuration["TinyMCE:Version"];
        var policy = _configuration["TinyMCE:ReferrerPolicy"];
        var url = $"https://cdn.tiny.cloud/1/{apiKey}/tinymce/{version}/tinymce.min.js";
        Response.Headers["Referrer-Policy"] = policy;
        return Redirect(url);
    }

}