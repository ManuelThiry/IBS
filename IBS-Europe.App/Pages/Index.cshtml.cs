
using IBS_Europe.App.Resources;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace IBS_Europe.App.Pages;

public class IndexModel : PageModel
{
    public List<string> Images { get; set; } = new List<string>();
    public void OnGet()
    {
        var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","images", "Index");
        
        if (Directory.Exists(imagesFolder))
        {
            // Récupérer tous les fichiers image dans le dossier
            var files = Directory.GetFiles(imagesFolder, "*.*")
                .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Créer un générateur de nombres aléatoires
            var random = new Random();

            // Mélanger les fichiers de manière aléatoire et stocker les chemins relatifs pour l'affichage
            Images = files
                .OrderBy(_ => random.Next())
                .Select(file => Path.GetRelativePath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), file))
                .ToList();
        }
        
    }
}