using Microsoft.AspNetCore.Identity;

namespace IBS_Europe.Infrastructures;

public static class IBSDbContextSeed
{
    public static async Task SeedDefaultUserAsync(UserManager<IdentityUser> userManager)
    {
        // Créez un utilisateur par défaut
        var defaultUser = new IdentityUser
        {
            UserName = "IBSAdmin",
            Email = "IBS.Admin@hotmail.com",
            EmailConfirmed = true,
        };

        // Vérifiez si l'utilisateur existe déjà
        if (userManager.Users.All(u => u.Email != defaultUser.Email))
        {
            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                // Créez l'utilisateur par défaut avec un mot de passe
                await userManager.CreateAsync(defaultUser, "IBS_Admin123!");
            }
        }
    }
}
