using System.ComponentModel.DataAnnotations;
using IBS_Europe.Domains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IBS_Europe.App.Pages;

public class Partners : PageModel
{
    private readonly IPartnersData _data;

    public List<PartnersViewModel> PartnersList { get; set; } = new List<PartnersViewModel>();
    
    public bool IsAddPartnerAction { get; set; }
    
    [BindProperty]
    public AddPartnerModel Input { get; set; } = new AddPartnerModel();
    
    public Partners(IPartnersData data)
    {
        _data = data;
    }
    public void OnGet()
    {
        LoadPartners();
    }
    
    public IActionResult OnPost(string direction, int priority)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage();
        }
        
        _data.SwitchPriority(priority, direction);
        LoadPartners();
        return Page();
    }
    
    public IActionResult OnPostDelete(int priority)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage();
        }
        
        var action = Request.Form["action"];

        if (action == "cancel")
        {
            return RedirectToPage();
        }
        
        _data.DeletePartner(priority);
        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPostAdd()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage();
        }
        
        var action = Request.Form["action"];

        if (action == "cancel")
        {
            return RedirectToPage();
        }
        
        IsAddPartnerAction = true;
        if (ModelState.IsValid)
        {
            bool error = false;
            
            if ( await _data.PartnerExists(Input.Name))
            {
                ModelState.AddModelError("Name", "Ce partenaire existe déjà.");
                error = true;
            }

            using (var ms = new MemoryStream())
            {
                Input.Picture.CopyTo(ms);
                byte[] fileBytes = ms.ToArray();

                // Appeler la méthode IsPdpPngJpg avec le tableau de bytes
                if (!ImagesVerification.PngOrJpg(fileBytes))  // Appeler avec le tableau de bytes
                {
                    ModelState.AddModelError("Picture", "Le fichier doit être une image PNG ou JPG.");
                    error = true;
                }
                
                const int maxFileSizeInBytes = 20 * 1024 * 1024; // 20 Mo
                if (fileBytes.Length > maxFileSizeInBytes)
                {
                    ModelState.AddModelError("Picture", "Le fichier est trop volumineux. La taille maximale autorisée est de 20 Mo.");
                    error = true;
                }
            }

            if (!error)
            {
                // Construire le chemin complet vers le dossier "wwwroot/Images/Partners"
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Partners");

                // Vérifier si le dossier existe, sinon le créer
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Nom du fichier (nom du partenaire avec l'extension du fichier original)
                var fileName = Path.GetFileNameWithoutExtension(Input.Name) + Path.GetExtension(Input.Picture.FileName);
                var filePath = Path.Combine(uploadPath, fileName);

                // Enregistrer le fichier dans le répertoire "wwwroot/Images/Partners"
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Input.Picture.CopyTo(stream);
                }

                // Ajouter le partenaire dans la base de données avec le chemin du fichier
                _data.AddPartner(new Domains.Partners()
                {
                    Name = Input.Name,
                    WebSite = Input.WebSite,
                    Path = "/Images/Partners/" + fileName, // Stocker le chemin relatif dans la base de données
                    Priority = -1
                });
                
                IsAddPartnerAction = false;
            }
            
        }
        LoadPartners();
        return RedirectToPage();
    }
    
    public void OnPostAddButton()
    {
        ModelState.Clear();
        IsAddPartnerAction = true;
        LoadPartners();
    }
    
    private async Task LoadPartners()
    {
        var products = await _data.GetAllPartners();
        foreach (var product in products)
        {
            PartnersList.Add(new PartnersViewModel(
                    Name: product.Name,
                    Path: product.Path,
                    Priority: product.Priority,
                    Website: product.WebSite
                )
            );
        }
    }

    
    public record PartnersViewModel
    (

        string Name,
        
        string Path,
        
        int Priority,
        
        string Website
    );
    
    public class AddPartnerModel
    {
        [Required(ErrorMessage = "Le nom est requis.")]
        [StringLength(50, ErrorMessage = "Le nom ne peut pas dépasser 50 caractères.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Le site web est requis.")]
        [StringLength(250, ErrorMessage = "L'URL ne peut pas dépasser 250 caractères.")]
        [Url(ErrorMessage = "Veuillez entrer une URL valide.")]
        public string WebSite { get; set; }
        
        [Required(ErrorMessage = "L'image est requise.")]
        public IFormFile Picture { get; set; }
        
    }
}