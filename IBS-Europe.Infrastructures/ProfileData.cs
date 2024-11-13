using IBS_Europe.Domains.Repository;

namespace IBS_Europe.Infrastructures;

public class ProfileData : IProfileData
{
    private readonly IBSDbContext _context;

    public ProfileData(IBSDbContext context)
    {
        _context = context;
    }
    public void ChangeUsername(string id, string username)
    {
        var user = _context.Users.FirstOrDefault(u => string.Equals(u.Id, id));
        user.UserName = username;
        _context.SaveChanges();
    }

    public void ChangePassword(string id, string password)
    {
        var user = _context.Users.FirstOrDefault(u => string.Equals(u.Id, id));
        user.PasswordHash = password;
        _context.SaveChanges();
    }
}