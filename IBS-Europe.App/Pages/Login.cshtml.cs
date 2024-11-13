using System.ComponentModel.DataAnnotations;
using IBS_Europe.App.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IBS_Europe.App.Pages
{
    public class Login : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public Login(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [BindProperty]
        public LoginModel Input { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            // Afficher la page de connexion
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                
                var result = await _signInManager.PasswordSignInAsync(Input.Identifier, Input.Password, isPersistent: false, lockoutOnFailure: false);
                
                if (result.Succeeded)
                {
                    return RedirectToPage("/Index");
                }
                else
                {
                    ErrorMessage = SharedResource.C_WrongLogin;
                }
            }

            return Page();
        }

        public class LoginModel
        {
            [Required(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "C_RequiredUsername")]
            [StringLength(50,ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Pr_50U")]
            public string Identifier { get; set; }

            [Required(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "C_RequiredPassword")]
            [StringLength(20,ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Pr_20P")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
    }
}