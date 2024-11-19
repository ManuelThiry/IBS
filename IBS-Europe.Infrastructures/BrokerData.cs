
using IBS_Europe.Domains;
using IBS_Europe.Domains.Translation;
using IBS_Europe.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;
using Broker = IBS_Europe.Domains.Broker;
using Translator = IBS_Europe.Infrastructures.Data.Translator;

namespace IBS_Europe.Infrastructures;

public class BrokerData : IBrokerData
{
    private readonly IBSDbContext _context;
    
    public BrokerData(IBSDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Broker>> GetBrokers(int category, string culture)
    {
        List <Broker> myList = new List<Broker>();
        var items = await _context.Brokers.Include(b => b.Translator).Where(a => a.Category == category).OrderBy(x=> x.Priority).ToListAsync();
        foreach (var item in items)
        {
            var description = item.Name;
            if (culture.Equals("en-US"))
            {
                description = item.Translator.Text;
            }
            myList.Add(new Broker()
            {
                Id = item.Id,
                Name = description,
                Path = item.Path
            });
        }
        
        return myList;
    }
    
    public async Task<List<Broker>> GetGeneralBrokers(string culture)
    {
        List <Broker> myList = new List<Broker>();
        var items = await _context.Brokers.Include(b=> b.Translator).Where(c => c.Category == (int)Category.Généralistes).OrderBy(x=> x.Priority).ToListAsync();
        foreach (var item in items)
        {
            var description = item.Name;
            if (culture.Equals("en-US"))
            {
                description = item.Translator.Text;
            }
            myList.Add(new Broker()
            {
                Id = item.Id,
                Name = description,
                Path = item.Path
            });
        }
        
        return myList;
    }
    
    public async Task DeleteBroker(int id)
    {
        var item = await _context.Brokers.Include(b => b.Translator).FirstOrDefaultAsync(a => a.Id == id);
        
        if (item != null)
        {
            var priority = item.Priority;
            _context.Translator.Remove(item.Translator);
            _context.Brokers.Remove(item);
            
            var itemsAbove = await _context.Brokers.Where(p=> p.Priority > priority).ToListAsync();

            foreach (var i in itemsAbove ) 
            {
                i.Priority -= 1;
            }
        
            await _context.SaveChangesAsync();
            
        }
    }
    
    public async Task<bool> BrokerExists(int id, string name, int category)
    {
        name = name.ToLower().Trim();
        if (id == -1)
        {
            return await _context.Brokers.AnyAsync(a => a.Name.ToLower().Trim() == name && a.Category == category);
        }
        
        return await _context.Brokers.AnyAsync(a => a.Name.ToLower().Trim() == name && a.Id != id && a.Category == category);
    }
    
    public async Task AddBroker(Broker broker)
    {
        int maxPriority = await _context.Brokers
            .AnyAsync() // Vérifie si des éléments existent
            ? _context.Brokers.Where(x => x.Category == broker.Category).Max(p => p.Priority) // Applique Max seulement s'il y a des éléments
            : 0; // Sinon retourne 0

    
        int newPriority = maxPriority + 1;
        var translator = new Translator
        {
            Text = await DeeplTranslate.TranslateTextWithDeeplAsync(broker.Name, "EN"),
        };
        
        _context.Brokers.Add(new Data.Broker()
        {
            Name = broker.Name,
            Path = broker.Path,
            Category = broker.Category,
            Priority = newPriority,
            Translator = translator
        });
        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateBroker(int id, string newName)
    {
        
        var item = await _context.Brokers.Include(t=> t.Translator).FirstOrDefaultAsync(a =>  a.Id == id);
        if (item != null && item.Name != newName)
        {
            item.Translator.Text = await DeeplTranslate.TranslateTextWithDeeplAsync(newName, "EN");
            item.Name = newName;
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task SwitchPriority(int id, string direction)
    {
        var item = await _context.Brokers.FirstOrDefaultAsync(a => a.Id == id);
        if (item != null)
        {
            if (direction == "left")
            {
                var previousItem = await _context.Brokers.FirstOrDefaultAsync(a => a.Priority == item.Priority - 1 && a.Category == item.Category);
                if (previousItem != null)
                {
                    previousItem.Priority++;
                    item.Priority--;
                    await _context.SaveChangesAsync();
                }
            }
            else if (direction == "right")
            {
                var nextItem = await _context.Brokers.FirstOrDefaultAsync(a => a.Priority == item.Priority + 1 && a.Category == item.Category);
                if (nextItem != null)
                {
                    nextItem.Priority--;
                    item.Priority++;
                    await _context.SaveChangesAsync();
                }
            }
        }
    }

    
    public async Task<int> GetCategory(int id)
    {
        var item = await _context.Brokers.FirstOrDefaultAsync(a => a.Id == id);
        if (item != null)
        {
            return item.Category;
        }
        
        return -1;
    }
    
    public async Task<string> GetBrokerName(int id)
    {
        var item = await _context.Brokers.FirstOrDefaultAsync(a => a.Id == id);
        if (item != null)
        {
            return item.Name;
        }
        
        return "";
    }
}