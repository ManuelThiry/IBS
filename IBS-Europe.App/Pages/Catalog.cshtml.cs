using IBS_Europe.Domains;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IBS_Europe.App.Pages;

public class Catalog : PageModel
{
    private readonly IPartnersData _partnersData;
    
    public Dictionary<string,string> CatalogPaths { get; set; } = new Dictionary<string, string>();
    public string APIKey { get; set; }
    
    public Catalog(IPartnersData partnersData)
    {
        _partnersData = partnersData;
    }
    
    public void OnGet()
    {
        CatalogPaths = _partnersData.GetcatalogPaths().Result;
        APIKey = Environment.GetEnvironmentVariable("CATALOG_API_KEY");
    }
}