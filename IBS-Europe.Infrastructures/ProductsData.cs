
using IBS_Europe.Domains;
using IBS_Europe.Domains.Translation;
using IBS_Europe.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;
using Translator = IBS_Europe.Infrastructures.Data.Translator;

namespace IBS_Europe.Infrastructures;

public class ProductsData : IProductsData
{
    private readonly IBSDbContext _context;

    public ProductsData(IBSDbContext context)
    {
        _context = context;
    }

    public async Task<HashSet<Product>> GetAllProducts(int id, string culture)
    {
        HashSet <Product> myList = new HashSet<Product>(5);
        var items = new HashSet<Products>(5);
        
        if ( await _context.Products.CountAsync() == 0)
        {
            return myList;
        }
        
        if ( id == -1 || await _context.Products.FirstOrDefaultAsync(p => p.Id == id) == null)
        {
            items = (await _context.Products
                    .Include(p => p.Translator)
                    .OrderBy(x => x.Name.ToLower())
                    .Take(5)
                    .ToListAsync()) // Asynchrone ici
                .ToHashSet(); // Conversion en HashSet après récupération des données

        }
        else
        {
            var selectedProduct = await _context.Products.Include(p => p.Translator).FirstOrDefaultAsync(p => p.Id == id);
            
            var orderedItems = await _context.Products.Include(p => p.Translator).OrderBy(x => x.Name.ToLower()).ToListAsync();
                
            int index = orderedItems.IndexOf(selectedProduct);
            
            if ( index -2 < 0 && index -1 < 0 )
            {
                if (orderedItems.Count > 4)
                {
                    items.Add(orderedItems[orderedItems.Count - 2]);
                }
                if (orderedItems.Count > 2)
                {
                    items.Add(orderedItems[orderedItems.Count - 1]);
                }
                
            } else if ( index -2 < 0 ) {
                if (orderedItems.Count > 4)
                {
                    items.Add(orderedItems[orderedItems.Count - 1]);
                }
                
                if (orderedItems.Count > 2)
                {
                    items.Add(orderedItems[0]);
                }
            } else {
                if (orderedItems.Count > 4)
                {
                    items.Add(orderedItems[index - 2]);
                }
                if (orderedItems.Count > 2)
                {
                    items.Add(orderedItems[index - 1]);
                }
            }
            
            items.Add(selectedProduct);
            
            if ( index + 1 >= orderedItems.Count && index + 2 >= orderedItems.Count ) {
                if ( orderedItems.Count > 3 )
                {
                    items.Add(orderedItems[0]);
                }
                if ( orderedItems.Count > 1 )
                {
                    items.Add(orderedItems[1]);
                }
            } else if ( index + 2 >= orderedItems.Count ) {
                if ( orderedItems.Count > 3 )
                {
                    items.Add(orderedItems[orderedItems.Count - 1]);
                }
                if ( orderedItems.Count > 1 )
                {
                    items.Add(orderedItems[0]);
                }
            } else {
                if (orderedItems.Count > 3)
                {
                    items.Add(orderedItems[index + 1]);
                }
                if (orderedItems.Count > 1)
                {
                    items.Add(orderedItems[index + 2]);
                }
            }
            
        }
        
        foreach (var item in items)
        {
            var description = item.Text;
            if (culture.Equals("en-US"))
            {
                 description = item.Translator.Text;
            }
            myList.Add(new Product
            {
                Id = item.Id,
                Name = item.Name,
                Image = item.Path,
                Description = description
            });
        }
        
        return myList;
    }
    
    public async Task<Product> GetProduct(int id)
    {
        var item = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        return new Product
        {
            Name = item.Name,
            Image = item.Path,
            Description = item.Text
        };
    }
    
    public async Task<bool> ProductExists(string name, int id)
    {
        name = name.ToLower().Trim();
        return await _context.Products.AnyAsync(p => p.Name.ToLower().Trim() == name && p.Id != id);
    }
    
    public async Task UpdateImage(int id, string path)
    {
        var item = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        item.Path = path;
        await _context.SaveChangesAsync();
    }
    
    public async Task<string> GetPath(int id)
    {
        var item = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
    
        if (item != null)
        {
            return item.Path;
        }

        return string.Empty;
    }
    
    public async Task EditProduct(Product product)
    {
        var item = await _context.Products.Include(p => p.Translator).FirstOrDefaultAsync(p => p.Id == product.Id);
        if (item != null)
        {
            string traduction = await DeeplTranslate.TranslateTextWithDeeplAsync(product.Description, "EN");
            Translator translator = item.Translator;
            translator.Text = traduction;
            translator.IsChecked = false;
            item.Name = product.Name;
            item.Text = product.Description;
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<int> AddProduct(Product product)
    {
        product.Description = product.Description ?? string.Empty;
        string traduction = await DeeplTranslate.TranslateTextWithDeeplAsync(product.Description, "EN");
        Translator translator = new Translator
        {
            Text = traduction,
            IsChecked = false
        };
        var addedProduct = _context.Products.Add(new Products
        {
            Name = product.Name,
            Text = product.Description,
            Path = product.Image,
            Translator = translator
        });
        await _context.SaveChangesAsync();

        return addedProduct.Entity.Id;
    }
    
    public async Task DeleteProduct(int id)
    {
        var item = await _context.Products.Include(p => p.Translator).FirstOrDefaultAsync(p => p.Id == id);
        var translations = item.Translator;
        _context.Products.Remove(item);
        _context.Translator.Remove(translations);
        await _context.SaveChangesAsync();
    }


}