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
    
    public int SelectedCategory { get; set; }
    
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
        
        SelectedCategoryMethod();
        await _data.SwitchPriority(priority, direction, SelectedCategory);
        await LoadPartners();
        return Page();
    }
    
    public IActionResult OnPostSwitchCategory(int selectedCategory)
    {
        CookieOptions option = new CookieOptions
        {
            Expires = DateTime.Now.AddMonths(1)
        };
            
        Response.Cookies.Append("selectedCategoryPartners", selectedCategory.ToString(), option);
        return RedirectToPage();
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
        SelectedCategoryMethod();
        var path = await _data.DeletePartner(priority, SelectedCategory);
        
        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var filePath = Path.Combine(uploadPath, path.TrimStart('/')); 

        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }

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
        if (Input.Category == 0)
        {
            ModelState.AddModelError("Category", SharedResource.B_CR);
        }
        if (ModelState.IsValid && Input.Category != 0)
        {
            bool error = false;
            
            if ( await _data.PartnerExists(Input.Name,Input.Category))
            {
                ModelState.AddModelError("Name", SharedResource.Pa_Exist);
                error = true;
            }
            
            if ( Input.Picture != null )
            {
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
            }

            

            if (!error)
            {
                var fileName = "";
                if (Input.Picture != null)
                {
                    // Construire le chemin complet vers le dossier "wwwroot/Images/Partners"
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Partners");

                    // Vérifier si le dossier existe, sinon le créer
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Nom du fichier (nom du partenaire avec l'extension du fichier original)
                    fileName = Cleanup.GenerateUniqueFileName(Input.Picture.FileName);
                    var filePath = Path.Combine(uploadPath, fileName);

                    // Enregistrer le fichier dans le répertoire "wwwroot/Images/Partners"
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        Input.Picture.CopyTo(stream);
                    }
                }
               

                // Ajouter le partenaire dans la base de données avec le chemin du fichier
                await _data.AddPartner(new Domains.Partners()
                {
                    Name = Input.Name,
                    WebSite = Input.WebSite,
                    Path = fileName == "" ? "" : "/images/Partners/" + fileName, // Stocker le chemin relatif dans la base de données
                    Priority = -1,
                    Category = Input.Category
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
        SelectedCategoryMethod();
        var products = await _data.GetAllPartners(SelectedCategory);
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

    private void SelectedCategoryMethod()
    {
        if (Request.Cookies.ContainsKey("selectedCategoryPartners"))
        {
            SelectedCategory = int.Parse(Request.Cookies["selectedCategoryPartners"]);
        }
        else
        {
            SelectedCategory = 1;
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
        
        [StringLength(250, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Pa_W250")]
        [Url(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Pa_URL")]
        public string WebSite { get; set; }
        
        public IFormFile Picture { get; set; }
        
        public int Category { get; set; }
        
    }
}