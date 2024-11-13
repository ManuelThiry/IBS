using System.ComponentModel.DataAnnotations;
using IBS_Europe.Domains;
using IBS_Europe.Domains.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IBS_Europe.App.Pages;

public class Translate : PageModel
{
    public Translator Translator { get; set; }
    private readonly UserManager<IdentityUser> _userManager;
    
    public int NbTranslations { get; set; }
    
    [BindProperty]
    public TranslatorModel TrTranslator { get; set; }
    
    private readonly ITranslatorData _data;
    
    public Translate(ITranslatorData data, UserManager<IdentityUser> userManager)
    {
        _data = data;
        _userManager = userManager;
    }
    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToPage("/Login");
        }

        Load();
        if (Translator != null)
        {
            TrTranslator = new TranslatorModel
            {
                Translation = Translator.TranslatedText
            };
        }
        
        return Page();
       
    }

    public void Load()
    {
        NbTranslations = _data.GetNumberOfTranslations();
        Translator = _data.GetTranslation();
    }
    
    public IActionResult OnPost()
    {
        Load();
        if (!ModelState.IsValid)
        {
            return Page();
        }
        
        _data.SetTranslation(TrTranslator.Translation, Translator.Id);
        return RedirectToPage();
    }

    public class TranslatorModel
    {
        [Required(ErrorMessage = "Le texte est requis.")]
        [StringLength(20000, ErrorMessage = "Le texte ne doit pas dépasser de 20 000 caractères")]
        public string Translation { get; set; }
    }
}