using System.ComponentModel.DataAnnotations;
using IBS_Europe.Domains.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IBS_Europe.App.Pages.Shared.Model;

public class ChangeUserName : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IProfileData _profile;
    
    [BindProperty] public Model UserName { get; set; } = new Model();
    
    public ChangeUserName(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IProfileData profile)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _profile = profile;
    }
    
    public class Model
    {
        public string CurrentUsername { get; set; }
            
        [Required(ErrorMessage = "Un nouveau nom d'utilisateur est requis.jhkghjenb")]
        [StringLength(50, ErrorMessage = "Le nom d'utilisateur ne doit pas dépasser 50 caractères.")]
        public string NewUsername { get; set; }
    }
    
    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        UserName.CurrentUsername = user.UserName;
        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            foreach (var modelStateEntry in ModelState)
            {
                var key = modelStateEntry.Key;
                var errors = modelStateEntry.Value.Errors;

                foreach (var error in errors)
                {
                    Console.WriteLine($"Erreur pour le champ '{key}': {error.ErrorMessage}");
                }
            }
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        string id = user.Id;

        _profile.ChangeUsername(id,UserName.NewUsername); 
            
        await _signInManager.RefreshSignInAsync(user);
        return RedirectToPage();
    }
}