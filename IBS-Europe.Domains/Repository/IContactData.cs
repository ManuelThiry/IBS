namespace IBS_Europe.Domains;

public interface IContactData
{
    public Task<List<Email>> GetEmails();
    
    public Task<List<Informations>> GetInformations();
    
    public Task SwitchPriority(int priority, string direction);

    public Task<Email> GetTypeOfMessage(string name);
    
    public Task UpdateInformation(Informations information);
    
    public Task UpdateEmail(Email email);
    
    public Task AddInformation(Informations information);
    
    public Task AddEmail(Email email);
    
    public Task DeleteInformation(int id);
    
    public Task DeleteEmail(int id);

}