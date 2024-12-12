using System.ComponentModel.DataAnnotations;
using IBS_Europe.App.Resources;
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
    public async Task OnGetAsync()
    {
        await Load();
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
    
    public async Task<IActionResult> OnPostSwitch(string direction, int priority)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage();
        }
        
        await _data.SwitchPriority(priority, direction);
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
                ModelState.AddModelError("Firstname", SharedResource.AT_PersonExist);
                error = true;
            }
            
            if ( Input.Picture == null )
            {
                ModelState.AddModelError("Picture", SharedResource.AT_IR);
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
                        ModelState.AddModelError("Picture", SharedResource.AT_Format);
                        error = true;
                    }
                
                    const int maxFileSizeInBytes = 20 * 1024 * 1024;
                    if (fileBytes.Length > maxFileSizeInBytes)
                    {
                        ModelState.AddModelError("Picture", SharedResource.AT_Size);
                        error = true;
                    }
                }
            }
           
            
            if (!error)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "People");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                
                var fileName = Cleanup.GenerateUniqueFileName(Input.Picture.FileName);
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
                    Path = "/images/People/" + fileName,
                    Phone = Input.Phone
                });
                return RedirectToPage();
            }
            
        }
        await Load();
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
                    ModelState.AddModelError($"people_{id}", SharedResource.AT_Format);
                    await Load();
                    return Page();
                }
                
                const int maxFileSizeInBytes = 20 * 1024 * 1024;
                if (fileBytes.Length > maxFileSizeInBytes)
                {
                    ModelState.AddModelError($"people_{id}", SharedResource.AT_Size);
                    await Load();
                    return Page();
                }
            }
            
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "People");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var path = await _data.GetPath(id);
            var fileName = path.Substring(path.IndexOf("People") + "People".Length + 1);;
            var filePath = Path.Combine(uploadPath, fileName);
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
               Input.Picture.CopyTo(stream);
            }
        }

        return RedirectToPage();
    }

    public async Task OnPostEditButton(int editId)
    {
        await Load();
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
                ModelState.AddModelError("Firstname", SharedResource.AT_PersonExist);
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
        await Load();
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
    
    public async Task OnPostAddButton()
    {
        ModelState.Clear();
        Input = new AddPeopleModel();

        IsAddPeopleAction = true;
        await Load();
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
        [Required(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "AT_FNR")]
        [StringLength(20, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "AT_FN20")]
        public string Firstname { get; set; }
        
        [StringLength(20, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "AT_N20")]
        public string Lastname { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "AT_RR")]
        [StringLength(25, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "AT_R20")]
        public string Role { get; set; }
        
        [MaxLength(20, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "AT_Nu20")]
        [Phone(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "AT_NuG")]
        public string Phone { get; set; }
        
        [MaxLength(25, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "AT_ER")]
        [EmailAddress(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "AT_EG")]
        public string Email { get; set; }
        
        public IFormFile Picture { get; set; }
        
    }
}