using IBS_Europe.Domains;
using IBS_Europe.Domains.Repository;
using IBS_Europe.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;
using Translator = IBS_Europe.Domains.Translator;

namespace IBS_Europe.Infrastructures;

public class TranslatorData : ITranslatorData
{
    private readonly int _numberOfTranslations = 8;
    private readonly IBSDbContext _context;

    public TranslatorData(IBSDbContext context)
    {
        _context = context;
    }
    public async Task<Domains.Translator> GetTranslation()
    {
        for (int i = 0; i <= _numberOfTranslations; i++)
        {
            var item = await GetTranslation(i);

            if (item != null)
            {
                return item;  
            }
                  
        }

        return null;
    }

   private async Task<Domains.Translator> GetTranslation(int number)
{
    switch (number)
    {
        case 1:
            var broker = await _context.Brokers.Include(t => t.Translator)
                .FirstOrDefaultAsync(t => t.Translator.IsChecked == false);
            if (broker == null || broker.Translator == null)
                return null;
            return new Translator
            {
                OriginalText = broker.Name,
                TranslatedText = broker.Translator.Text,
                Id = broker.Translator.Id
            };

        case 2:
            var email1 = await _context.Email.Include(t => t.FirstTranslator)
                .FirstOrDefaultAsync(t => t.FirstTranslator.IsChecked == false);
            if (email1 == null || email1.FirstTranslator == null)
                return null;
            return new Translator
            {
                OriginalText = email1.Name,
                TranslatedText = email1.FirstTranslator.Text,
                Id = email1.FirstTranslator.Id
            };

        case 3:
            var email2 = await _context.Email.Include(t => t.SecondTranslator)
                .FirstOrDefaultAsync(t => t.SecondTranslator.IsChecked == false);
            if (email2 == null || email2.SecondTranslator == null)
                return null;
            return new Translator
            {
                OriginalText = email2.Description,
                TranslatedText = email2.SecondTranslator.Text,
                Id = email2.SecondTranslator.Id
            };

        case 4:
            var info1 = await _context.Informations.Include(t => t.FirstTranslator)
                .FirstOrDefaultAsync(t => t.FirstTranslator.IsChecked == false);
            if (info1 == null || info1.FirstTranslator == null)
                return null;
            return new Translator
            {
                OriginalText = info1.Description,
                TranslatedText = info1.FirstTranslator.Text,
                Id = info1.FirstTranslator.Id
            };

        case 5:
            var info2 = await _context.Informations.Include(t => t.SecondTranslator)
                .FirstOrDefaultAsync(t => t.SecondTranslator.IsChecked == false);
            if (info2 == null || info2.SecondTranslator == null)
                return null;
            return new Translator
            {
                OriginalText = info2.Text,
                TranslatedText = info2.SecondTranslator.Text,
                Id = info2.SecondTranslator.Id
            };

        case 6:
            var people = await _context.People.Include(t => t.Translator)
                .FirstOrDefaultAsync(t => t.Translator.IsChecked == false);
            if (people == null || people.Translator == null)
                return null;
            return new Translator
            {
                OriginalText = people.Role,
                TranslatedText = people.Translator.Text,
                Id = people.Translator.Id
            };

        case 7:
            var product = await _context.Products.Include(t => t.FirstTranslator)
                .FirstOrDefaultAsync(t => t.FirstTranslator.IsChecked == false);
            if (product == null || product.FirstTranslator == null)
                return null;
            return new Translator
            {
                OriginalText = product.Text,
                TranslatedText = product.FirstTranslator.Text,
                Id = product.FirstTranslator.Id
            };
        case 8:
            var product2 = await _context.Products.Include(t => t.SecondTranslator)
                .FirstOrDefaultAsync(t => t.SecondTranslator.IsChecked == false);
            if (product2 == null || product2.SecondTranslator == null)
                return null;
            return new Translator
            {
                OriginalText = product2.Text,
                TranslatedText = product2.SecondTranslator.Text,
                Id = product2.SecondTranslator.Id
            };

        default:
            return null;
    }
}



    public async Task<int> GetNumberOfTranslations()
    {
        return await _context.Translator.Where(t=>t.IsChecked == false).CountAsync();
    }

    public async Task SetTranslation(string translation, int id)
    {
        var item = await _context.Translator.FirstOrDefaultAsync(t=> t.Id == id);
        item.Text = translation;
        item.IsChecked = true;
        await _context.SaveChangesAsync();
    }
}