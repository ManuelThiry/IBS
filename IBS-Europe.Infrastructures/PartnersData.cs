using System.ComponentModel.DataAnnotations;
using IBS_Europe.Domains;
using Microsoft.EntityFrameworkCore;
using Partners = IBS_Europe.Domains.Partners;

namespace IBS_Europe.Infrastructures;

public class PartnersData : IPartnersData
{
    
    private readonly IBSDbContext _context;
    
    public PartnersData(IBSDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Partners>> GetAllPartners(int selectedCategory)
    {
        List <Partners> myList = new List<Partners>();
        var items = await _context.Partners.OrderBy(a => a.Priority).ThenBy(a => a.Name).Where(p=> p.Category == selectedCategory).ToListAsync();
        foreach (var item in items)
        {
            myList.Add(new Partners
            {
                Name = item.Name,
                Path = item.Path,
                Priority = item.Priority,
                WebSite = item.Website
            });
        }
        
        return myList;
    }
    
    public async Task SwitchPriority(int priority, string direction, int selectedCategory)
    {
        var item1 = await _context.Partners.Where(p=> p.Priority == priority).Where(p=> p.Category == selectedCategory).FirstOrDefaultAsync();
        Data.Partners? item2;

        if (direction.Equals("right"))
        {
            item2 = await _context.Partners.Where(p=> p.Category == selectedCategory).Where(p=> p.Priority == item1.Priority +1).FirstOrDefaultAsync();
        }
        else
        {
            item2 = await _context.Partners.Where(p=> p.Category == selectedCategory).Where(p=> p.Priority == item1.Priority -1).FirstOrDefaultAsync();
        }
        if (item1 != null && item2 != null)
        {
            int temp = item1.Priority;
            item1.Priority = item2.Priority;
            item2.Priority = temp;
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task DeletePartner(int priority, int selectedCategory)
    {
        var item = await _context.Partners.Where(p=>p.Category == selectedCategory).Where(p=> p.Priority == priority).FirstOrDefaultAsync();
        if (item != null)
        {
            _context.Partners.Remove(item);
            
            var itemsAbove = await _context.Partners.Where(p=> p.Category == selectedCategory).Where(p=> p.Priority > priority).ToListAsync();

            foreach (var i in itemsAbove ) 
            {
                i.Priority -= 1;
            }
            
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task AddPartner(Partners partner)
    {
        partner.WebSite = partner.WebSite ?? "";
        partner.Path = partner.Path ?? "";
        // Récupérer la priorité maximale des partenaires existants et ajouter 1 pour le nouveau partenaire
        int maxPriority = await _context.Partners.Where(p=> p.Category == partner.Category).AnyAsync() 
            ? _context.Partners.Max(p => p.Priority) 
            : 0; // Si aucun partenaire, définir à 0
    
        int newPriority = maxPriority + 1;

        // Créer un nouvel objet Data.Partners avec les données fournies
        Data.Partners data = new Data.Partners
        {
            Name = partner.Name,
            Path = partner.Path,
            Website = partner.WebSite, // Corriger le nom de propriété si nécessaire
            Priority = newPriority,
            Category = partner.Category
        };

        // Ajouter le nouveau partenaire à la base de données
        _context.Partners.Add(data);
        await _context.SaveChangesAsync();
    }
    
    public async Task<bool> PartnerExists(string name, int selectedCategory)
    {
        return await _context.Partners
            .Where(p => p.Category == selectedCategory)
            .AnyAsync(p => p.Name.ToLower().Trim() == name.ToLower().Trim());
    }
    
    public async Task<Dictionary<string,string>> GetcatalogPaths()
    {
        var dictionary = new Dictionary<string, string>();
        var items = await _context.Partners.OrderBy(a => a.Priority).ThenBy(a => a.Name).ToListAsync();
        foreach (var item in items)
        {
           dictionary.Add(item.Name, item.Path);
        }
        
        return dictionary;
    }


}