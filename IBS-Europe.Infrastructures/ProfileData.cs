using IBS_Europe.Domains.Repository;
using Microsoft.EntityFrameworkCore;

namespace IBS_Europe.Infrastructures;

public class ProfileData : IProfileData
{
    private readonly IBSDbContext _context;

    public ProfileData(IBSDbContext context)
    {
        _context = context;
    }
    public async Task ChangeUsername(string id, string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => string.Equals(u.Id, id));
        user.UserName = username;
        await _context.SaveChangesAsync();
    }

    public async Task ChangePassword(string id, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => string.Equals(u.Id, id));
        user.PasswordHash = password;
        await _context.SaveChangesAsync();
    }
}