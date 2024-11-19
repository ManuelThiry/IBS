using System.ComponentModel.DataAnnotations;
using IBS_Europe.Domains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IBS_Europe.App.Pages;

public class Products : PageModel
{
    private readonly IProductsData _data;
    
    public bool IsUpdate { get; set; }
    public bool IsNew { get; set; }

    public List<ProductViewModel> ProductsList { get; set; } = new List<ProductViewModel>();
    
    [BindProperty]
    public EditModel Edit { get; set; } = new EditModel();
    
    public Products(IProductsData data)
    {
        _data = data;
    }
    public void OnGet()
    {
        Load();
    }

    public async Task Load()
    {
        var culture = Thread.CurrentThread.CurrentCulture.Name;
        var products = await _data.GetAllProducts(GetIdFromCookie(), culture);
        foreach (var product in products)
        {
            ProductsList.Add(new ProductViewModel(
                    Name: product.Name,
                    Image: product.Image,
                    Description: product.Description,
                    Id: product.Id
                )
            ); 
        }
    }

    public void OnPostAddButton()
    {
        ModelState.Clear();
        Edit = new EditModel();

        IsNew = true;
        Load();
    }

    public IActionResult OnPostDelete(int productId)
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
        
        _data.DeleteProduct(productId);
        CookieOptions options = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(-1)
        };

        Response.Cookies.Append("selectedProduct", "", options);

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
        
        if ( await _data.ProductExists(Edit.Name, -1))
        {
            ModelState.AddModelError("Name", "Ce produit existe déjà.");
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
                    ModelState.AddModelError("Edit.Image", "Le fichier doit être une image PNG ou JPG.");
                    error = true;
                }

                const int maxFileSizeInBytes = 20 * 1024 * 1024;
                if (fileBytes.Length > maxFileSizeInBytes)
                {
                    ModelState.AddModelError("Edit.Image",
                        "Le fichier est trop volumineux. La taille maximale autorisée est de 20 Mo.");
                    error = true;
                }
            }
        }

        if (error)
        {
            IsNew = true;
            Load();
            return Page();
        }
        
        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Products");
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }
        
        var filePath = "";
        var fileName = "";
        var start = "";

        if (Edit.Image != null)
        {
            fileName = Path.GetFileNameWithoutExtension(Edit.Name) + Path.GetExtension(Edit.Image.FileName);
            filePath = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                Edit.Image.CopyTo(stream);
            }
            start = "/Images/Products/";
        }
        else
        {
            fileName = "IBS-logo-bleu-2_HD.JPG";
            start = "/Images/";
        }
        
        
        var product = new Product
        {
            Image = start + fileName,
            Name = Edit.Name,
            Description = Edit.Description
        };
        
        int id = await  _data.AddProduct(product);
        CookieOptions option = new CookieOptions();
        option.Expires = DateTime.Now.AddMonths(1);
        Response.Cookies.Append("selectedProduct", id.ToString(), option);
        return RedirectToPage("/Products");
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
        
        int id = GetIdFromCookie();
        
        if (id == -1)
        {
            return RedirectToPage("/Products");
        }
        
        bool error = false;
        
        if (!ModelState.IsValid)
        {
            error = true;
        } else 
        
        if ( await _data.ProductExists(Edit.Name, id))
        {
            ModelState.AddModelError("Name", "Ce produit existe déjà.");
            error = true;
        }

        if (error)
        {
            Load();
            IsUpdate = true;
            return Page();
        }
        
        var product = new Product
        {
            Id = id,
            Name = Edit.Name,
            Description = Edit.Description
        };
        
        await _data.EditProduct(product);
        return RedirectToPage("/Products");
    }

    private int GetIdFromCookie()
    {
        string selectedProductIdStr = Request.Cookies["selectedProduct"];
        if (!string.IsNullOrEmpty(selectedProductIdStr))
        {
            if (int.TryParse(selectedProductIdStr, out int selectedProductId))
            {
                return selectedProductId;
            }
            else
            {
                return -1;
            }
        }
        else
        {
            return -1;
        }
    }

    public async Task<IActionResult> OnPostSwitchImage()
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
                    ModelState.AddModelError("Edit.Image", "Le fichier doit être une image PNG ou JPG.");
                    Load();
                    return Page();
                }
                
                const int maxFileSizeInBytes = 20 * 1024 * 1024;
                if (fileBytes.Length > maxFileSizeInBytes)
                {
                    ModelState.AddModelError("Edit.Image", "Le fichier est trop volumineux. La taille maximale autorisée est de 20 Mo.");
                    Load();
                    return Page();
                }
            }

            int id = GetIdFromCookie();
            if (id == -1)
            {
                return RedirectToPage();
            }
            
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Products");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            var fileName = Path.GetFileNameWithoutExtension(await _data.GetName(id)) + Path.GetExtension(Edit.Image.FileName);
            var filePath = Path.Combine(uploadPath, fileName);
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                Edit.Image.CopyTo(stream);
            }
            
            _data.UpdateImage(id, "/Images/Products/" + fileName);
        }

        return RedirectToPage();
    }
    
    public async Task OnPostButton(int id)
    {
        ModelState.Clear();
        var product = await _data.GetProduct(id);
        
        Edit = new EditModel
        {
            Name = product.Name,
            Description = product.Description
        };

        IsUpdate = true;
        Load();

    }
    
    public record ProductViewModel
    (

        int Id,
        
        string Name,
        
        string Image,

        string Description
    );
    
    public class EditModel
    {
        [Required(ErrorMessage = "Le nom est requis.")]
        [StringLength(50, ErrorMessage = "Le nom ne doit pas dépasser 50 caractères.")]
        public string Name { get; set; }

        public IFormFile Image { get; set; }
        
        [StringLength(20000, ErrorMessage = "La description ne doit pas dépasser de 20 000 caractères")]
        public string Description { get; set; }
    }
}