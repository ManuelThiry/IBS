using System.ComponentModel.DataAnnotations;
using IBS_Europe.App.Resources;
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
    public async Task OnGetAsync()
    {
        await LoadPartners();
    }
    
    public async Task<IActionResult> OnPost(string direction, int priority)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage();
        }
        
        await _data.SwitchPriority(priority, direction);
        await LoadPartners();
        return Page();
    }
    
    public async Task<IActionResult> OnPostDelete(int priority)
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
        
        await _data.DeletePartner(priority);
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
                ModelState.AddModelError("Name", SharedResource.Pa_Exist);
                error = true;
            }

            using (var ms = new MemoryStream())
            {
                Input.Picture.CopyTo(ms);
                byte[] fileBytes = ms.ToArray();

                // Appeler la méthode IsPdpPngJpg avec le tableau de bytes
                if (!ImagesVerification.PngOrJpg(fileBytes))  // Appeler avec le tableau de bytes
                {
                    ModelState.AddModelError("Picture", SharedResource.Pa_FPNG);
                    error = true;
                }
                
                const int maxFileSizeInBytes = 20 * 1024 * 1024; // 20 Mo
                if (fileBytes.Length > maxFileSizeInBytes)
                {
                    ModelState.AddModelError("Picture", SharedResource.Pa_F20);
                    error = true;
                }
            }

            if (!error)
            {
                // Construire le chemin complet vers le dossier "wwwroot/Images/Partners"
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Partners");

                // Vérifier si le dossier existe, sinon le créer
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Nom du fichier (nom du partenaire avec l'extension du fichier original)
                var fileName = Cleanup.GenerateUniqueFileName(Input.Picture.FileName);
                var filePath = Path.Combine(uploadPath, fileName);

                // Enregistrer le fichier dans le répertoire "wwwroot/Images/Partners"
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Input.Picture.CopyTo(stream);
                }

                // Ajouter le partenaire dans la base de données avec le chemin du fichier
                await _data.AddPartner(new Domains.Partners()
                {
                    Name = Input.Name,
                    WebSite = Input.WebSite,
                    Path = "/images/Partners/" + fileName, // Stocker le chemin relatif dans la base de données
                    Priority = -1
                });

                return RedirectToPage();
            }
            
        }
        await LoadPartners();
        return Page();
    }
    
    public async Task OnPostAddButton()
    {
        ModelState.Clear();
        IsAddPartnerAction = true;
        await LoadPartners();
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
        [Required(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Pa_NR")]
        [StringLength(50, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Pa_N50")]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Pa_WR")]
        [StringLength(250, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Pa_W250")]
        [Url(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Pa_URL")]
        public string WebSite { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Pa_IR")]
        public IFormFile Picture { get; set; }
        
    }
}