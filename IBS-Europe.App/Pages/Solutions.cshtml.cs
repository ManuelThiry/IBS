using System.ComponentModel.DataAnnotations;
using IBS_Europe.App.Resources;
using IBS_Europe.Domains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IBS_Europe.App.Pages;

public class Solutions : PageModel
{
    private readonly IProductsData _data;
    
    public bool IsNew { get; set; }
    public bool IsUpdate { get; set; }
    [BindProperty]
    public EditModel Edit { get; set; } = new EditModel();
    public List<ProductViewModel> ProductsList { get; set; } = new List<ProductViewModel>();
    
    public Solutions(IProductsData data)
    {
        _data = data;
    }
    public async Task OnGetAsync()
    {
        await Load();
    }
    
    private async Task Load()
    {
        var culture = Thread.CurrentThread.CurrentCulture.Name;
        var products = await _data.GetAllProducts(culture);
        foreach (var product in products)
        {
            ProductsList.Add(new ProductViewModel(
                    Name: product.Name,
                    Image: product.Image,
                    Description: product.Description,
                    SmallDescription: product.SmallDescription,
                    Priority: product.Priority
                )
            ); 
        }
    }
    
    public async Task OnPostAddButton()
    {
        ModelState.Clear();
        Edit = new EditModel();

        IsNew = true;
        await Load();
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
        
        bool error = false;
        if (!ModelState.IsValid)
        {
            error = true;
        } else 
        
        if ( await _data.ProductExists(Edit.Name,""))
        {
            ModelState.AddModelError("Name", SharedResource.Pr_Exist);
            error = true;
        }

        if (Edit.Image != null)
        {
            using (var ms = new MemoryStream())
            {
                Edit.Image.CopyTo(ms);
                byte[] fileBytes = ms.ToArray();

                if (!ImagesVerification.PngOrJpg(fileBytes))
                {
                    ModelState.AddModelError("Edit.Image", SharedResource.Pr_IPNG);
                    error = true;
                }

                const int maxFileSizeInBytes = 20 * 1024 * 1024;
                if (fileBytes.Length > maxFileSizeInBytes)
                {
                    ModelState.AddModelError("Edit.Image",
                        SharedResource.Pa_F20);
                    error = true;
                }
            }
        }

        if (error)
        {
            IsNew = true;
            await Load();
            return Page();
        }
        
        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Products");
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }
        
        var filePath = "";
        var fileName = "";
        var start = "";

        if (Edit.Image != null)
        {
            fileName = Cleanup.GenerateUniqueFileName(Edit.Image.FileName);
            filePath = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                Edit.Image.CopyTo(stream);
            }
            start = "/images/Products/";
        }
        else
        {
            fileName = "IBS-logo-bleu-2_HD.JPG";
            start = "/images/";
        }
        
        
        var product = new Product
        {
            Image = start + fileName,
            Name = Edit.Name,
            Description = Edit.Description,
            SmallDescription = Edit.SmallDescription
        };
        
        string name = await  _data.AddProduct(product);
        return RedirectToPage("/SolutionDetails", new { name = name });

    }
    
    public async Task OnPostEditButton(string name)
    {
        ModelState.Clear();
        var product = await _data.GetProduct(name);
        
        Edit = new EditModel
        {
            Name = product.Name,
            Description = product.Description,
            SmallDescription = product.SmallDescription, 
            Reference= product.Name
        };

        IsUpdate = true;
        await Load();

    }
    
    public async Task<IActionResult> OnPostEdit(string actualName)
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
        
        
        bool error = false;
        
        if (!ModelState.IsValid)
        {
            error = true;
        } else 
        
        if ( await _data.ProductExists(Edit.Name,actualName))
        {
            ModelState.AddModelError("Name", SharedResource.Pr_Exist);
            error = true;
        }

        if (error)
        {
            await Load();
            IsUpdate = true;
            return Page();
        }
        
        var product = new Product
        {
            Name = Edit.Name,
            Description = Edit.Description,
            SmallDescription = Edit.SmallDescription
        };
        
        await _data.EditProduct(product, actualName);
        return RedirectToPage("/Solutions");
    }
    
    public async Task<IActionResult> OnPostDelete(string productName)
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
        
        var path = await _data.DeleteProduct(productName);
        
        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var filePath = Path.Combine(uploadPath, path.TrimStart('/')); 

        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }


        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPostSwitchImage(string name)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage();
        }
        
        if ( Edit.Image != null)
        {
            using (var ms = new MemoryStream())
            {
                Edit.Image.CopyTo(ms);
                byte[] fileBytes = ms.ToArray();
            
                if (!ImagesVerification.PngOrJpg(fileBytes))
                {
                    ModelState.AddModelError("Edit.Image", SharedResource.Pr_IPNG);
                    await Load();
                    return Page();
                }
                
                const int maxFileSizeInBytes = 20 * 1024 * 1024;
                if (fileBytes.Length > maxFileSizeInBytes)
                {
                    ModelState.AddModelError("Edit.Image", SharedResource.Pa_F20);
                    await Load();
                    return Page();
                }
            }
            
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Products");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var fileName = Edit.Image.FileName;
            var filePath = await _data.GetPath(name);
            var globalPath = "";
            if ( !Cleanup.IsPathInDirectory(filePath, "Products") )
            {
                var uniqueFileName = Cleanup.GenerateUniqueFileName(fileName);
                globalPath = Path.Combine(uploadPath, uniqueFileName);
                filePath = Path.Combine("/images/Products", uniqueFileName);
                await _data.UpdateImage(name, filePath);
            }
            else
            {
                var path  = filePath.Substring(filePath.IndexOf("Products") + "Products".Length + 1);
                globalPath = Path.Combine(uploadPath, path);
            }
            
            using (var stream = new FileStream(globalPath, FileMode.Create))
            {
                Edit.Image.CopyTo(stream);
            }
            
           
        }

        return RedirectToPage();
    }
    
    public record ProductViewModel
    (
        
        string Name,
        
        string Image,

        string Description,
        
        string SmallDescription,
        
        int Priority
    );
    
    public class EditModel
    {
        [Required(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "B_NR")]
        [StringLength(50, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Pa_N50")]
        public string Name { get; set; }

        public IFormFile Image { get; set; }
        
        [StringLength(250, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Pr_D200")]
        public string SmallDescription { get; set; }
        
        [StringLength(20000, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Pr_D20")]
        public string Description { get; set; }
        
        public string Reference { get; set; }
    }
}