using IBS_Europe.App.Resources;
using IBS_Europe.Domains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IBS_Europe.App.Pages;

public class SolutionDetails : PageModel
{
    private readonly IProductsData _productsData;
    
    public Model Product { get; set; }
    
    public bool IsUpdate { get; set; }
    
    [BindProperty]
    public Solutions.EditModel Edit { get; set; } = new Solutions.EditModel();
    
    public SolutionDetails(IProductsData productsData)
    {
        _productsData = productsData;
    }

    public async Task OnGetAsync(string name)
    {
        await Load(name);
    }

    public async Task Load(string name)
    {
        var product = await _productsData.GetProduct(name);
        var brokers = await _productsData.GetBrokers(name);
        Product = new Model
        {
            Name = product.Name,
            Image = product.Image,
            Description = product.Description,
            Brokers = brokers
        };
    }
    
    public async Task<IActionResult> OnPostDelete(string productName)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage("/SolutionDetails", new { name = productName });
        }
        
        var action = Request.Form["action"];

        if (action == "cancel")
        {
            return RedirectToPage("/SolutionDetails", new { name = productName });
        }
        
        await _productsData.DeleteProduct(productName);

        return RedirectToPage("/Solutions");
    }
    
    public async Task OnPostEditButton(string name)
    {
        ModelState.Clear();
        var product = await _productsData.GetProduct(name);
        
        Edit = new Solutions.EditModel
        {
            Name = product.Name,
            Description = product.Description,
            SmallDescription = product.SmallDescription, 
            Reference= product.Name
        };

        IsUpdate = true;
        await Load(name);

    }
    
    public async Task<IActionResult> OnPostEdit(string actualName)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage("/SolutionDetails", new { name = actualName });
        }
        
        var action = Request.Form["action"];

        if (action == "cancel")
        {
            return RedirectToPage("/SolutionDetails", new { name = actualName });
        }
        
        
        bool error = false;
        
        if (!ModelState.IsValid)
        {
            error = true;
        } else 
        
        if ( await _productsData.ProductExists(Edit.Name,actualName))
        {
            ModelState.AddModelError("Name", SharedResource.Pr_Exist);
            error = true;
        }

        if (error)
        {
            await Load(actualName);
            IsUpdate = true;
            return Page();
        }
        
        var product = new Product
        {
            Name = Edit.Name,
            Description = Edit.Description,
            SmallDescription = Edit.SmallDescription
        };
        
        await _productsData.EditProduct(product, actualName);
        return RedirectToPage("/SolutionDetails", new { name = actualName });
    }
    
    public async Task<IActionResult> OnPostSwitchImage(string name)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage("/SolutionDetails", new { name = name });
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
                    await Load(name);
                    return Page();
                }
                
                const int maxFileSizeInBytes = 20 * 1024 * 1024;
                if (fileBytes.Length > maxFileSizeInBytes)
                {
                    ModelState.AddModelError("Edit.Image", SharedResource.Pa_F20);
                    await Load(name);
                    return Page();
                }
            }
            
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Products");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var fileName = Edit.Image.FileName;
            var filePath = await _productsData.GetPath(name);
            var globalPath = "";
            if ( !Cleanup.IsPathInDirectory(filePath, "Products") )
            {
                var uniqueFileName = Cleanup.GenerateUniqueFileName(fileName);
                globalPath = Path.Combine(uploadPath, uniqueFileName);
                filePath = Path.Combine("/images/Products", uniqueFileName);
                await _productsData.UpdateImage(name, filePath);
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

        return RedirectToPage("/SolutionDetails", new { name = name });
    }
}

public class Model
{
    public string Name { get; set; }
    public string Image { get; set; }
    public string Description { get; set; }
    
    public Dictionary<string,string> Brokers { get; set; }
}