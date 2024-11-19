namespace IBS_Europe.Domains.Repository;

public interface IProfileData
{
    public Task ChangeUsername(string id,string username);
    
    public Task ChangePassword(string id,string password);
}