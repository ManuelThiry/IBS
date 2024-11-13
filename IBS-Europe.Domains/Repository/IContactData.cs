namespace IBS_Europe.Domains;

public interface IContactData
{
    public List<Email> GetEmails();
    
    public List<Informations> GetInformations();
    
    public void SwitchPriority(int priority, string direction);

    public Email GetTypeOfMessage(string name);
    
    public Task UpdateInformation(Informations information);
    
    public Task UpdateEmail(Email email);
    
    public Task AddInformation(Informations information);
    
    public Task AddEmail(Email email);
    
    public void DeleteInformation(int id);
    
    public void DeleteEmail(int id);

}