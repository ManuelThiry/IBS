using System.ComponentModel.DataAnnotations;
using IBS_Europe.App.Resources;
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

        await Load();
        if (Translator != null)
        {
            TrTranslator = new TranslatorModel
            {
                Translation = Translator.TranslatedText
            };
        }
        
        return Page();
       
    }

    public async Task Load()
    {
        NbTranslations = await _data.GetNumberOfTranslations();
        Translator = await _data.GetTranslation();
    }
    
    public async Task<IActionResult> OnPost()
    {
        await Load();
        if (!ModelState.IsValid)
        {
            return Page();
        }
        
        await _data.SetTranslation(TrTranslator.Translation, Translator.Id);
        return RedirectToPage();
    }

    public class TranslatorModel
    {
        [Required(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "T_TR" )]
        [StringLength(20000, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "T_T200")]
        public string Translation { get; set; }
    }
}