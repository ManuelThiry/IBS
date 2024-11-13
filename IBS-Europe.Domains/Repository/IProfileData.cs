namespace IBS_Europe.Domains.Repository;

public interface IProfileData
{
    public void ChangeUsername(string id,string username);
    
    public void ChangePassword(string id,string password);
}