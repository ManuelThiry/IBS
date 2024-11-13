using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using IBS_Europe.App.Resources;
using IBS_Europe.Domains.Repository;

namespace IBS_Europe.App.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IProfileData _profile;

        [BindProperty] public UserNameModel UserName { get; set; } = new UserNameModel();

        [BindProperty] public PasswordModel Password { get; set; } = new PasswordModel();

        public ProfileModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IProfileData profile)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _profile = profile;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }
            
            UserName.CurrentUsername = user.UserName;
            
            return Page();
        }

        public async Task<IActionResult> OnPostChangeUsernameAsync()
        {
            UserName.CurrentUsername = (await _userManager.GetUserAsync(User)).UserName;
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (UsernameErrors())
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            string id = user.Id;
            
            _profile.ChangeUsername(id,UserName.NewUsername); 
            
            await _signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = SharedResource.Pr_SuccessU;
            ModelState.Clear();
            UserName = new UserNameModel();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            UserName.CurrentUsername = (await _userManager.GetUserAsync(User)).UserName;
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if ( PasswordErrors())
            {
                return Page();
            }
            var user = await _userManager.GetUserAsync(User);
            
            if ( !await _userManager.CheckPasswordAsync(user, Password.CurrentPassword))
            {
                ModelState.AddModelError("Password.CurrentPassword", SharedResource.Pr_IncorrectP);
                return Page();
            }
            
            string id = user.Id;

            string hashedPassword = _userManager.PasswordHasher.HashPassword(user, Password.NewPassword);
            _profile.ChangePassword(id, hashedPassword); 
            
            await _signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = SharedResource.Pr_SuccessP;
            ModelState.Clear();
            Password = new PasswordModel();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Login");
        }
        
        public class UserNameModel
        {
            public string CurrentUsername { get; set; }
            
            [MaxLength(50,ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Pr_50U")]
            public string NewUsername { get; set; }
        }

        public class PasswordModel
        {
            [MaxLength(20,ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Pr_20P")]
            [DataType(DataType.Password)]
            public string CurrentPassword { get; set; }
            
            [MaxLength(20,ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Pr_20P")]
            [MinLength(8, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Pr_8P")]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{4,20}$", 
                ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Pr_Expression")]
            [DataType(DataType.Password)]
            public string NewPassword { get; set; }
            
            [MaxLength(20,ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Pr_20P")]
            [DataType(DataType.Password)]
            [Compare("NewPassword", ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Pr_WrongP")]
            public string ConfirmPassword { get; set; }
        }
        public bool UsernameErrors()
        {
            if (string.IsNullOrEmpty(UserName.NewUsername))
            {
                ModelState.AddModelError("UserName.NewUsername", SharedResource.Pr_RequiredU);
                return true;
            }

            return false;
        }


        public bool PasswordErrors()
        {
            bool value = false;
            if (string.IsNullOrEmpty(Password.CurrentPassword))
            {
                ModelState.AddModelError("Password.CurrentPassword", SharedResource.Pr_RequiredP);
                value = true;
            }
            
            if ( string.IsNullOrEmpty(Password.NewPassword))
            {
                ModelState.AddModelError("Password.NewPassword", SharedResource.Pr_RequiredNewP);
                value = true;
            }
            
            if ( string.IsNullOrEmpty(Password.ConfirmPassword))
            {
                ModelState.AddModelError("Password.ConfirmPassword", SharedResource.Pr_RequiredConfirmP);
                value = true;
            }

            return value;
        }
    }
    
}
