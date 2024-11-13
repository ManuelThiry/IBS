using IBS_Europe.Domains;
using IBS_Europe.Domains.Repository;
using IBS_Europe.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;
using Translator = IBS_Europe.Domains.Translator;

namespace IBS_Europe.Infrastructures;

public class TranslatorData : ITranslatorData
{
    private readonly int _numberOfTranslations = 7;
    private readonly IBSDbContext _context;

    public TranslatorData(IBSDbContext context)
    {
        _context = context;
    }
    public Domains.Translator GetTranslation()
    {
        for (int i = 0; i <= _numberOfTranslations; i++)
        {
            var item = GetTranslation(i);

            if (item != null)
            {
                return item;  
            }
                  
        }

        return null;
    }

   private Domains.Translator GetTranslation(int number)
{
    switch (number)
    {
        case 1:
            var broker = _context.Brokers.Include(t => t.Translator)
                .FirstOrDefault(t => t.Translator.IsChecked == false);
            if (broker == null || broker.Translator == null)
                return null;
            return new Translator
            {
                OriginalText = broker.Name,
                TranslatedText = broker.Translator.Text,
                Id = broker.Translator.Id
            };

        case 2:
            var email1 = _context.Email.Include(t => t.FirstTranslator)
                .FirstOrDefault(t => t.FirstTranslator.IsChecked == false);
            if (email1 == null || email1.FirstTranslator == null)
                return null;
            return new Translator
            {
                OriginalText = email1.Name,
                TranslatedText = email1.FirstTranslator.Text,
                Id = email1.FirstTranslator.Id
            };

        case 3:
            var email2 = _context.Email.Include(t => t.SecondTranslator)
                .FirstOrDefault(t => t.SecondTranslator.IsChecked == false);
            if (email2 == null || email2.SecondTranslator == null)
                return null;
            return new Translator
            {
                OriginalText = email2.Description,
                TranslatedText = email2.SecondTranslator.Text,
                Id = email2.SecondTranslator.Id
            };

        case 4:
            var info1 = _context.Informations.Include(t => t.FirstTranslator)
                .FirstOrDefault(t => t.FirstTranslator.IsChecked == false);
            if (info1 == null || info1.FirstTranslator == null)
                return null;
            return new Translator
            {
                OriginalText = info1.Description,
                TranslatedText = info1.FirstTranslator.Text,
                Id = info1.FirstTranslator.Id
            };

        case 5:
            var info2 = _context.Informations.Include(t => t.SecondTranslator)
                .FirstOrDefault(t => t.SecondTranslator.IsChecked == false);
            if (info2 == null || info2.SecondTranslator == null)
                return null;
            return new Translator
            {
                OriginalText = info2.Text,
                TranslatedText = info2.SecondTranslator.Text,
                Id = info2.SecondTranslator.Id
            };

        case 6:
            var people = _context.People.Include(t => t.Translator)
                .FirstOrDefault(t => t.Translator.IsChecked == false);
            if (people == null || people.Translator == null)
                return null;
            return new Translator
            {
                OriginalText = people.Role,
                TranslatedText = people.Translator.Text,
                Id = people.Translator.Id
            };

        case 7:
            var product = _context.Products.Include(t => t.Translator)
                .FirstOrDefault(t => t.Translator.IsChecked == false);
            if (product == null || product.Translator == null)
                return null;
            return new Translator
            {
                OriginalText = product.Text,
                TranslatedText = product.Translator.Text,
                Id = product.Translator.Id
            };

        default:
            return null;
    }
}



    public int GetNumberOfTranslations()
    {
        return _context.Translator.Where(t=>t.IsChecked == false).Count();
    }

    public void SetTranslation(string translation, int id)
    {
        var item = _context.Translator.FirstOrDefault(t=> t.Id == id);
        item.Text = translation;
        item.IsChecked = true;
        _context.SaveChanges();
    }
}