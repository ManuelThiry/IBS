using System.ComponentModel.DataAnnotations;
using IBS_Europe.Domains;
using Partners = IBS_Europe.Domains.Partners;

namespace IBS_Europe.Infrastructures;

public class PartnersData : IPartnersData
{
    
    private readonly IBSDbContext _context;
    
    public PartnersData(IBSDbContext context)
    {
        _context = context;
    }
    
    public List<Partners> GetAllPartners()
    {
        List <Partners> myList = new List<Partners>();
        var items = _context.Partners.OrderBy(a => a.Priority).ThenBy(a => a.Name).ToList();
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
    
    public void SwitchPriority(int priority, string direction)
    {
        var item1 = _context.Partners.Where(p=> p.Priority == priority).FirstOrDefault();
        Data.Partners? item2;

        if (direction.Equals("right"))
        {
            item2 = _context.Partners.Where(p=> p.Priority == item1.Priority +1).FirstOrDefault();
        }
        else
        {
            item2 = _context.Partners.Where(p=> p.Priority == item1.Priority -1).FirstOrDefault();
        }
        if (item1 != null && item2 != null)
        {
            int temp = item1.Priority;
            item1.Priority = item2.Priority;
            item2.Priority = temp;
            _context.SaveChanges();
        }
    }
    
    public void DeletePartner(int priority)
    {
        var item = _context.Partners.Where(p=> p.Priority == priority).FirstOrDefault();
        if (item != null)
        {
            _context.Partners.Remove(item);
            
            var itemsAbove = _context.Partners.Where(p=> p.Priority > priority).ToList();

            foreach (var i in itemsAbove ) 
            {
                i.Priority -= 1;
            }
            
            _context.SaveChanges();
        }
    }
    
    public void AddPartner(Partners partner)
    {
        // Récupérer la priorité maximale des partenaires existants et ajouter 1 pour le nouveau partenaire
        int maxPriority = _context.Partners.Any() 
            ? _context.Partners.Max(p => p.Priority) 
            : 0; // Si aucun partenaire, définir à 0
    
        int newPriority = maxPriority + 1;

        // Créer un nouvel objet Data.Partners avec les données fournies
        Data.Partners data = new Data.Partners
        {
            Name = partner.Name,
            Path = partner.Path,
            Website = partner.WebSite, // Corriger le nom de propriété si nécessaire
            Priority = newPriority
        };

        // Ajouter le nouveau partenaire à la base de données
        _context.Partners.Add(data);
        _context.SaveChanges();
    }
    
    public bool PartnerExists(string name)
    {
        return _context.Partners.Any(p => p.Name.ToLower().Trim() == name.ToLower().Trim());
    }


}