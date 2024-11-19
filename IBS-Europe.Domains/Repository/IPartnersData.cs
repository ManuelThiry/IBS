namespace IBS_Europe.Domains;

public interface IPartnersData
{
    public Task<List<Partners>> GetAllPartners();
    
    public Task SwitchPriority(int priority, string direction);
    
    public Task DeletePartner(int priority);
    
    public Task AddPartner(Partners partner);
    
    public Task<bool> PartnerExists(string name);
    
}