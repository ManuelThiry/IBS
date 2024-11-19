using System.ComponentModel.DataAnnotations;
using IBS_Europe.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IBS_Europe.App.Pages;

public class AboutUsTeam : PageModel
{
    private readonly IPeopleData _data;
    
    public bool IsAddPeopleAction { get; set; }
    public bool IsEditPeopleAction { get; set; }
    
    [BindProperty]
    public AddPeopleModel Input { get; set; } = new AddPeopleModel();

    public List<PeopleViewModel> PeopleList { get; set; } = new List<PeopleViewModel>();
    
    public AboutUsTeam(IPeopleData data)
    {
        _data = data;
    }
    public void OnGet()
    {
        Load();
    }

    public async Task Load()
    {
        var products = await _data.GetAllPeople();
        foreach (var product in products)
        {
            PeopleList.Add(new PeopleViewModel(
                    Id: product.Id,
                    FirstName: product.FirstName,
                    LastName: product.LastName,
                    Email: product.Email,
                    Role: product.Role,
                    Path: product.Path,
                    Phone: product.Phone,
                    Priority: product.Priority
                )
            ); 
        }
    }
    
    public IActionResult OnPostSwitch(string direction, int priority)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage();
        }
        
        _data.SwitchPriority(priority, direction);
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
        
        IsAddPeopleAction = true;
        if (ModelState.IsValid)
        {
            bool error = false; 
            
            if (await _data.PeopleExists(Input.Firstname, Input.Lastname, -1))
            {
                ModelState.AddModelError("Firstname", "Cette personne existe déjà.");
                error = true;
            }
            
            if ( Input.Picture == null )
            {
                ModelState.AddModelError("Picture", "L'image est requise.");
                error = true;
            }
            else
            {
                using (var ms = new MemoryStream())
                {
                    Input.Picture.CopyTo(ms);
                    byte[] fileBytes = ms.ToArray();
                
                    if (!ImagesVerification.PngOrJpg(fileBytes)) 
                    {
                        ModelState.AddModelError("Picture", "Le fichier doit être une image PNG ou JPG.");
                        error = true;
                    }
                
                    const int maxFileSizeInBytes = 20 * 1024 * 1024;
                    if (fileBytes.Length > maxFileSizeInBytes)
                    {
                        ModelState.AddModelError("Picture", "Le fichier est trop volumineux. La taille maximale autorisée est de 20 Mo.");
                        error = true;
                    }
                }
            }
           
            
            if (!error)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "People");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                
                var fileName = Path.GetFileNameWithoutExtension(Input.Firstname)+ Path.GetFileNameWithoutExtension(Input.Lastname) + Path.GetExtension(Input.Picture.FileName);
                var filePath = Path.Combine(uploadPath, fileName);
                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Input.Picture.CopyTo(stream);
                }
                
                await _data.AddPeople(new Domains.People()
                {
                    FirstName = Input.Firstname,
                    LastName = Input.Lastname,
                    Role = Input.Role,
                    Email = Input.Email,
                    Path = "/Images/People/" + fileName,
                    Phone = Input.Phone
                });
                return RedirectToPage();
            }
            
        }
        Load();
        return Page();
    }

    public async Task<IActionResult> OnPostSwitchImage(int id)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage();
        }
        
        if ( Input.Picture != null)
        {
            using (var ms = new MemoryStream())
            {
                Input.Picture.CopyTo(ms);
                byte[] fileBytes = ms.ToArray();
            
                if (!ImagesVerification.PngOrJpg(fileBytes))
                {
                    ModelState.AddModelError($"people_{id}", "Le fichier doit être une image PNG ou JPG.");
                    Load();
                    return Page();
                }
                
                const int maxFileSizeInBytes = 20 * 1024 * 1024;
                if (fileBytes.Length > maxFileSizeInBytes)
                {
                    ModelState.AddModelError($"people_{id}", "Le fichier est trop volumineux. La taille maximale autorisée est de 20 Mo.");
                    Load();
                    return Page();
                }
            }
            
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "People");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            var fileName = Path.GetFileNameWithoutExtension(await _data.GetName(id)) + Path.GetExtension(Input.Picture.FileName);
            var filePath = Path.Combine(uploadPath, fileName);
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
               Input.Picture.CopyTo(stream);
            }
            
            _data.UpdateImage(id, "/Images/People/" + fileName);
        }

        return RedirectToPage();
    }

    public void OnPostEditButton(int editId)
    {
        Load();
        ModelState.Clear();
        CookieOptions options = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(1)
        };

        Response.Cookies.Append("selectedTeamId", editId.ToString(), options);
        var people = PeopleList.Find(x => x.Id == editId);
        
        Input = new AddPeopleModel
        {
           Email = people.Email,
           Role = people.Role,
           Firstname = people.FirstName,
           Lastname = people.LastName,
           Phone = people.Phone
        };
        IsEditPeopleAction = true;
        
    }
    
    public async Task<IActionResult> OnPostEdit()
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
        
        int id = -1;
        if (Request.Cookies.ContainsKey("selectedTeamId"))
        {
            id = int.Parse(Request.Cookies["selectedTeamId"]);
        }
        else
        {
            return RedirectToPage();
        }
        if ( ModelState.IsValid )
        {
            bool error = false; 
            
            if (await _data.PeopleExists(Input.Firstname, Input.Lastname, id))
            {
                ModelState.AddModelError("Firstname", "Cette personne existe déjà.");
                error = true;
            }
           
            
            if (!error)
            {
                
                await _data.UpdatePeople(new Domains.People()
                {
                    Id = id,
                    FirstName = Input.Firstname,
                    LastName = Input.Lastname,
                    Role = Input.Role,
                    Email = Input.Email,
                    Phone = Input.Phone
                });

                return RedirectToPage();
            }
        }
        IsEditPeopleAction = true;
        Load();
        return Page();
    }
    
    public IActionResult OnPostDelete(int id)
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
        
        _data.DeletePeople(id);
        return RedirectToPage();
    }
    
    public void OnPostAddButton()
    {
        ModelState.Clear();
        Input = new AddPeopleModel();

        IsAddPeopleAction = true;
        Load();
    }
    
    public record PeopleViewModel
    (
        int Id,
        
        string FirstName,
        
        string LastName,

        string Email,
        
        string Role,
        
        string Path,
        
        string Phone,
        
        int Priority
    );
    
    public class AddPeopleModel
    {
        [Required(ErrorMessage = "Le prénom est requis.")]
        [StringLength(20, ErrorMessage = "Le prénom ne peut pas dépasser 20 caractères.")]
        public string Firstname { get; set; }
        
        [StringLength(20, ErrorMessage = "Le nom ne peut pas dépasser 20 caractères.")]
        public string Lastname { get; set; }
        
        [Required(ErrorMessage = "Le rôle est requis.")]
        [StringLength(25, ErrorMessage = "Le rôle ne peut pas dépasser 25 caractères.")]
        public string Role { get; set; }
        
        [MaxLength(20, ErrorMessage = "Le numéro de téléphone ne doit pas dépasser 20 caractères.")]
        [Phone(ErrorMessage = "Veuillez entrer un numéro de téléphone valide.")]
        public string Phone { get; set; }
        
        [MaxLength(25, ErrorMessage = "L'email ne doit pas dépasser 25 caractères.")]
        [EmailAddress(ErrorMessage = "Veuillez entrer une adresse email valide.")]
        public string Email { get; set; }
        
        public IFormFile Picture { get; set; }
        
    }
}