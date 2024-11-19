
using IBS_Europe.Domains;
using IBS_Europe.Domains.Translation;
using IBS_Europe.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;
using Email = IBS_Europe.Infrastructures.Data.Email;
using Informations = IBS_Europe.Domains.Informations;
using Translator = IBS_Europe.Infrastructures.Data.Translator;
using Type = IBS_Europe.Domains.Type;

namespace IBS_Europe.Infrastructures;

public class ContactData : IContactData
{
    private readonly IBSDbContext _context;
    
    public ContactData(IBSDbContext context)
    {
        _context = context;
    }
    

    public async Task<List<Domains.Email>>GetEmails()
    {
        List <Domains.Email> myList = new List<Domains.Email>();
        var items = await _context.Email.ToListAsync();
        foreach (var item in items)
        {
            myList.Add(new ()
            {
                Name = item.Name,
                EmailAddress = item.EmailAddress,
                Description = item.Description,
                Id = item.Id
            });
        }
        
        return myList;
    }

    public async Task<List<Informations>> GetInformations()
    {
        List <Domains.Informations> myList = new List<Domains.Informations>();
        var items = await _context.Informations.OrderBy(x=> x.Priority).ToListAsync();
        foreach (var item in items)
        {
            myList.Add(new ()
            {
                Text = item.Text,
                Description = item.Description,
                Type = item.Type,
                Priority = item.Priority,
                Id = item.Id
            });
        }
        
        return myList;
    }
    
    public async Task SwitchPriority(int priority, string direction)
    {
        var item1 = await _context.Informations.Where(p=> p.Priority == priority).FirstOrDefaultAsync();
        Data.Informations? item2;

        if (direction.Equals("bottom"))
        {
            item2 = await _context.Informations.Where(p=> p.Priority == item1.Priority +1).FirstOrDefaultAsync();
        }
        else
        {
            item2 = await _context.Informations.Where(p=> p.Priority == item1.Priority -1).FirstOrDefaultAsync();
        }
        if (item1 != null && item2 != null)
        {
            int temp = item1.Priority;
            item1.Priority = item2.Priority;
            item2.Priority = temp;
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<Domains.Email> GetTypeOfMessage(string name)
    {
        var item = await _context.Email.FirstOrDefaultAsync(p => p.Name == name);
        
        return new Domains.Email()
        {
            Name = item.Name,
            EmailAddress = item.EmailAddress,
            Description = item.Description,
            Id = item.Id
        };
    }

    public async Task UpdateInformation(Informations information)
    {
        var item = await _context.Informations.Include(x => x.FirstTranslator).Include(x => x.SecondTranslator).FirstOrDefaultAsync(p => p.Id == information.Id);
        if ( information.Description != item.Description )
        {
            var translate1 = await DeeplTranslate.TranslateTextWithDeeplAsync(information.Description, "EN");
            item.FirstTranslator.Text = translate1;
            item.FirstTranslator.IsChecked = false;
        }
        if ( information.Text != item.Text )
        {
            var translate2 = await DeeplTranslate.TranslateTextWithDeeplAsync(information.Text, "EN");
            item.SecondTranslator.Text = translate2;
            item.SecondTranslator.IsChecked = false;
        }
        item.Description = information.Description;
        item.Text = information.Text;
        item.Type = information.Type;
        await _context.SaveChangesAsync();
    }

    public async Task UpdateEmail(Domains.Email email)
    {
        var item = await _context.Email.Include(x => x.FirstTranslator).Include(x => x.SecondTranslator).FirstOrDefaultAsync(p => p.Id == email.Id);
        if ( email.Description != item.Description )
        {
            var translate1 = await DeeplTranslate.TranslateTextWithDeeplAsync(email.Description, "EN");
            item.FirstTranslator.Text = translate1;
            item.FirstTranslator.IsChecked = false;
        }
        if ( email.Name != item.Name )
        {
            var translate2 = await DeeplTranslate.TranslateTextWithDeeplAsync(email.Name, "EN");
            item.SecondTranslator.Text = translate2;
            item.SecondTranslator.IsChecked = false;
        }
        item.Description = email.Description;
        item.Name = email.Name;
        item.EmailAddress = email.EmailAddress;
        await _context.SaveChangesAsync();
    }

    public async Task AddInformation(Informations information)
    {
        var translate1 = await DeeplTranslate.TranslateTextWithDeeplAsync(information.Description, "EN");
        var translate2 = await DeeplTranslate.TranslateTextWithDeeplAsync(information.Text, "EN");
        Translator translator1 = new Translator{
            Text = translate1
        };
        Translator translator2 = new Translator{
            Text = translate2
        };
        var result = _context.Informations.Add(new Data.Informations
        {
            Description = information.Description,
            Text = information.Text,
            Type = information.Type,
            Priority = _context.Informations.Count() + 1,
            FirstTranslator = translator1,
            SecondTranslator = translator2
        });
        await _context.SaveChangesAsync();
    }

    public async Task AddEmail(Domains.Email email)
    {
        var translate1 = await DeeplTranslate.TranslateTextWithDeeplAsync(email.Name, "EN");
        var translate2 = await DeeplTranslate.TranslateTextWithDeeplAsync(email.Description, "EN");
        Translator translator1 = new Translator{
            Text = translate1
        };
        Translator translator2 = new Translator{
            Text = translate2
        };
        var result = _context.Email.Add(new Data.Email()
        {
            Description = email.Description,
            Name = email.Name,
            EmailAddress = email.EmailAddress,
            FirstTranslator = translator1,
            SecondTranslator = translator2
        });
        await _context.SaveChangesAsync();
    }

    public async Task DeleteInformation(int id)
    {
        var item = await _context.Informations.Include(b => b.FirstTranslator).Include(b=> b.SecondTranslator).FirstOrDefaultAsync(a => a.Id == id);
        
        if (item != null)
        {
            var priority = item.Priority;
            _context.Translator.Remove(item.FirstTranslator);
            _context.Translator.Remove(item.SecondTranslator);
            _context.Informations.Remove(item);
            
            var itemsAbove = _context.Informations.Where(p=> p.Priority > priority).ToList();

            foreach (var i in itemsAbove ) 
            {
                i.Priority -= 1;
            }
        
            await _context.SaveChangesAsync();
            
        }
    }

    public async Task DeleteEmail(int id)
    {
        var item = await _context.Email.Include(b => b.FirstTranslator).Include(b=> b.SecondTranslator).FirstOrDefaultAsync(a => a.Id == id);

        _context.Translator.Remove(item.FirstTranslator);
        _context.Translator.Remove(item.SecondTranslator);
        _context.Email.Remove(item);
        await _context.SaveChangesAsync();
    }
}