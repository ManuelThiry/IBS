﻿
using IBS_Europe.Domains;
using IBS_Europe.Domains.Translation;
using IBS_Europe.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;
using Broker = IBS_Europe.Domains.Broker;
using Translator = IBS_Europe.Infrastructures.Data.Translator;

namespace IBS_Europe.Infrastructures;

public class ProductsData : IProductsData
{
    private readonly IBSDbContext _context;

    public ProductsData(IBSDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllProducts(string culture)
    {
       var products = await _context.Products.Include(p => p.FirstTranslator).Include(p => p.SecondTranslator).OrderBy(p=> p.Priority).ToListAsync();
       
       var productsToReturn = new List<Product>();

       foreach (var product in products)
       {
           productsToReturn.Add(new Product
           {
               Name = product.Name,
               Image = product.Path,
               Description = culture == "fr-FR" ? product.Text : product.FirstTranslator.Text,
               Priority = product.Priority,
               SmallDescription =  culture == "fr-FR" ? product.SmallDescription : product.SecondTranslator.Text
           });
       }

       return productsToReturn;

    }
    
    public async Task SwitchPriority(int priority, string direction)
    {
        var item1 = await _context.Products.Where(p=> p.Priority == priority).FirstOrDefaultAsync();
        Data.Products? item2;

        if (direction.Equals("right"))
        {
            item2 = await _context.Products.Where(p=> p.Priority == item1.Priority +1).FirstOrDefaultAsync();
        }
        else
        {
            item2 = await _context.Products.Where(p=> p.Priority == item1.Priority -1).FirstOrDefaultAsync();
        }
        if (item1 != null && item2 != null)
        {
            int temp = item1.Priority;
            item1.Priority = item2.Priority;
            item2.Priority = temp;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Dictionary<string,string>> GetBrokers(string productName)
    {
        var brokers = await _context.Products.Include(p => p.Brokers).ToListAsync();
        var brokersToReturn = new Dictionary<string, string>();
        foreach (var product in brokers)
        {
            if (product.Name == productName)
            {
                foreach (var broker in product.Brokers)
                {
                    brokersToReturn.Add(broker.Name, broker.Path);
                }
            }
        }

        return brokersToReturn;
    }

    public async Task<Product> GetProduct(string name)
    {
        var item = await _context.Products.Include(p=> p.FirstTranslator).Include(p=> p.SecondTranslator).FirstOrDefaultAsync(p => p.Name == name);
        var culture = Thread.CurrentThread.CurrentCulture.Name;
        return new Product
        {
            Name = item.Name,
            Image = item.Path,
            Description = culture == "fr-FR" ? item.Text : item.FirstTranslator.Text,
            SmallDescription = culture == "fr-FR" ? item.SmallDescription : item.SecondTranslator.Text
        };
    }
    
    public async Task<bool> ProductExists(string name, string actualName)
    {
        name = name.ToLower().Trim();
        return await _context.Products.AnyAsync(p => p.Name.ToLower().Trim() == name && p.Name.ToLower().Trim() != actualName.ToLower().Trim());
    }
    
    public async Task UpdateImage(string name, string path)
    {
        var item = await _context.Products.FirstOrDefaultAsync(p => p.Name == name);
        item.Path = path;
        await _context.SaveChangesAsync();
    }
    
    public async Task<string> GetPath(string name)
    {
        var item = await _context.Products.FirstOrDefaultAsync(p => p.Name == name);
    
        if (item != null)
        {
            return item.Path;
        }

        return string.Empty;
    }
    
    public async Task EditProduct(Product product, string actualName)
    {
        product.Description = product.Description ?? string.Empty;
        product.SmallDescription = product.SmallDescription ?? string.Empty;
        var item = await _context.Products.Include(p => p.FirstTranslator).Include(p=> p.SecondTranslator).FirstOrDefaultAsync(p => p.Name == actualName);
        if (item != null)
        {
            string traduction = item.Text != product.Description ? await DeeplTranslate.TranslateTextWithDeeplAsync(product.Description, "EN") : item.FirstTranslator.Text;
            string traduction2 = item.SmallDescription != product.SmallDescription ? await DeeplTranslate.TranslateTextWithDeeplAsync(product.SmallDescription, "EN") : item.SecondTranslator.Text;
            Translator translator = item.FirstTranslator;
            Translator translator2 = item.SecondTranslator;
            translator.Text = traduction;
            translator.IsChecked = product.Description == string.Empty ? true : false;
            translator2.Text = traduction2;
            translator2.IsChecked = product.SmallDescription == string.Empty ? true : false;
            item.Name = product.Name;
            item.Text = product.Description;
            item.SmallDescription = product.SmallDescription;
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<string> AddProduct(Product product)
    {
        product.Description = product.Description ?? string.Empty;
        product.SmallDescription = product.SmallDescription ?? string.Empty;
        string traduction = await DeeplTranslate.TranslateTextWithDeeplAsync(product.Description, "EN");
        string traduction2 = await DeeplTranslate.TranslateTextWithDeeplAsync(product.SmallDescription, "EN");
        Translator translator = new Translator
        {
            Text = traduction,
            IsChecked = product.Description == string.Empty ? true : false
        };
        Translator translator2 = new Translator
        {
            Text = traduction2,
            IsChecked = product.SmallDescription == string.Empty ? true : false
        };
        var priority = await _context.Products.MaxAsync(p => (int?)p.Priority) ?? 0;
        var addedProduct = _context.Products.Add(new Products
        {
            Name = product.Name,
            Text = product.Description,
            Path = product.Image,
            FirstTranslator = translator,
            SecondTranslator = translator2,
            SmallDescription = product.SmallDescription,
            Priority = priority + 1
        });
        await _context.SaveChangesAsync();

        return addedProduct.Entity.Name;
    }
    
    public async Task<string> DeleteProduct(string name)
    {
        var item = await _context.Products.Include(p => p.FirstTranslator).Include(p=> p.SecondTranslator).FirstOrDefaultAsync(p => p.Name == name);
        var path = item.Path;
        var priority = item.Priority;
        var translations = item.FirstTranslator;
        var translation2 = item.SecondTranslator;
        
        var brokers = await _context.Brokers.Where(b => b.Products.Id == item.Id).ToListAsync();
        if ( brokers != null)
        {
            foreach (var broker in brokers)
            {
                broker.Products = null;
            }
        }
        {
            
        }
        _context.Products.Remove(item);
        _context.Translator.Remove(translations);
        _context.Translator.Remove(translation2);
        
        
        
        var itemsAbove = await _context.Products.Where(p=> p.Priority > priority).ToListAsync();

        foreach (var i in itemsAbove ) 
        {
            i.Priority -= 1;
        }

        await _context.SaveChangesAsync();
        
        return path.Contains("Products") ? path : "";
    }
    
    public async Task<Dictionary<string,string>> GetProductsList()
    {
        var products = await _context.Products
            .OrderBy(p => p.Priority) 
            .ToListAsync(); 
        var productsList = new Dictionary<string,string>();
        foreach (var product in products)
        {
            productsList.Add(product.Name, product.Path);
        }

        return productsList;
    }


}