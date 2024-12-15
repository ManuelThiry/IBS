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
    public IActionResult GetTinyMceConfig()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Unauthorized(); // Retourne une erreur 401 si l'utilisateur n'est pas authentifié
        }

        var apiKey = Environment.GetEnvironmentVariable("TINYMCE_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            return StatusCode(500, "API key is missing"); // Vérifie si la clé est configurée
        }

        var version = _configuration["TinyMCE:Version"];
        var policy = _configuration["TinyMCE:ReferrerPolicy"];
        Response.Headers["Referrer-Policy"] = policy;

        // Préparer la configuration à renvoyer au client
        var config = new
        {
            ScriptUrl = $"https://cdn.tiny.cloud/1/{apiKey}/tinymce/{version}/tinymce.min.js",
            Options = new
            {
                Selector = "#ProductDescription",
                Toolbar = "undo redo | bold italic underline | forecolor backcolor | fontsize fontfamily | alignleft aligncenter alignright | removeformat",
                Menubar = false,
                Height = 300,
                Branding = false
            }
        };

        return Json(config); // Retourne la configuration en JSON
    }

    
    [HttpGet("APT-script.js")]
    public async Task<IActionResult> GetAPTCatalogApiKey()
    {

        var apiKey = Environment.GetEnvironmentVariable("CATALOG_API_KEY");
        var url = $"https://app.sectorcatalog.be/SectorCatalogBe/feed/v2/digestedcatalogItems?format=json&SecureGuid={apiKey}";
        using var httpClient = new HttpClient();
        try
        {
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Failed to fetch data from the external API.");
            }

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json"); // Retourne la réponse JSON au client
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

}