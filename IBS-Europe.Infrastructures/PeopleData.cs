
using IBS_Europe.Domains;
using IBS_Europe.Domains.Translation;
using IBS_Europe.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;
using People = IBS_Europe.Domains.People;
using Translator = IBS_Europe.Infrastructures.Data.Translator;

namespace IBS_Europe.Infrastructures;

public class PeopleData : IPeopleData
{
    private readonly IBSDbContext _context;
    
    public PeopleData(IBSDbContext context)
    {
        _context = context;
    }
    
    public List<People> GetAllPeople()
    {
        List <People> myList = new List<People>();
        var items = _context.People.Include(p=> p.Translator).OrderBy(p => p.Priority).ToList();
        foreach (var item in items)
        {
            var role = item.Role;
            if ( Thread.CurrentThread.CurrentCulture.Name == "en-US")
            {
                role = item.Translator.Text;
            }
            myList.Add(new People
            {
                Id = item.Id,
                FirstName = item.FirstName,
                LastName = item.LastName,
                Role = role,
                Email = item.Email,
                Path = item.Path,
                Phone = item.Phone,
                Priority = item.Priority
            });
        }
        
        return myList;
    }
    
    public bool PeopleExists(string Firstname, string LastName, int id)
    {
        LastName = LastName ?? "";
        Firstname = Firstname.ToLower().Trim();
        LastName = LastName.ToLower().Trim();
        return _context.People.Any(e => e.FirstName.ToLower().Trim() == Firstname && e.LastName.ToLower().Trim() == LastName && e.Id != id);
    }
    
    public async Task AddPeople(People people)
    {
        people.Email = people.Email ?? "";
        people.Phone = people.Phone ?? "";
        people.LastName = people.LastName ?? "";
        
        int maxPriority = _context.People.Any() 
            ? _context.People.Max(p => p.Priority) 
            : 0;
    
        int newPriority = maxPriority + 1;

        Translator translator = new Translator
        {
            Text = await DeeplTranslate.TranslateTextWithDeeplAsync(people.Role, "EN")
        };

        Data.People p = new Data.People
        {
            FirstName = people.FirstName,
            LastName = people.LastName,
            Role = people.Role,
            Email = people.Email,
            Path = people.Path,
            Phone = people.Phone,
            Priority = newPriority,
            Translator = translator
        };
        _context.People.Add(p);
        _context.SaveChanges();
    }
    
    public void DeletePeople(int id)
    {
        var item = _context.People.Include(p=> p.Translator).Where(p=> p.Id == id).FirstOrDefault();
        int priority = item.Priority;
        if (item != null)
        {
            _context.People.Remove(item);
            _context.Translator.Remove(item.Translator);
            
            var itemsAbove = _context.People.Where(p=> p.Priority > priority).ToList();

            foreach (var i in itemsAbove ) 
            {
                i.Priority -= 1;
            }
            
            _context.SaveChanges();
        }
    }
    
    public void SwitchPriority(int priority, string direction)
    {
        var item1 = _context.People.Where(p=> p.Priority == priority).FirstOrDefault();
        Data.People? item2;

        if (direction.Equals("right"))
        {
            item2 = _context.People.Where(p=> p.Priority == item1.Priority +1).FirstOrDefault();
        }
        else
        {
            item2 = _context.People.Where(p=> p.Priority == item1.Priority -1).FirstOrDefault();
        }
        if (item1 != null && item2 != null)
        {
            int temp = item1.Priority;
            item1.Priority = item2.Priority;
            item2.Priority = temp;
            _context.SaveChanges();
        }
    }
    
    public string GetName(int id)
    {
        var item = _context.People.FirstOrDefault(p => p.Id == id);
    
        if (item != null)
        {
            string cleanName = item.FirstName+item.LastName;
            
            cleanName = System.Text.RegularExpressions.Regex.Replace(cleanName, @"[^a-zA-Z0-9\-_]", "");
        
            return cleanName;
        }

        return string.Empty;
    }
    
    public void UpdateImage(int id, string path)
    {
        var item = _context.People.FirstOrDefault(p => p.Id == id);
        if (item != null)
        {
            item.Path = path;
            _context.SaveChanges();
        }
    }
    
    public async Task UpdatePeople(People people)
    {
        people.Email = people.Email ?? "";
        people.Phone = people.Phone ?? "";
        people.LastName = people.LastName ?? "";
        var item = _context.People.Include(p=> p.Translator).FirstOrDefault(p => p.Id == people.Id);
        var translator = item.Translator;
        if (item != null)
        {
            item.FirstName = people.FirstName;
            item.LastName = people.LastName;
            item.Email = people.Email;
            item.Phone = people.Phone;
            if (people.Role != item.Role)
            {
                translator.Text = await DeeplTranslate.TranslateTextWithDeeplAsync(people.Role, "EN");
                translator.IsChecked = false;
            }
            item.Role = people.Role;
            
            _context.SaveChanges();
        }
    }
}