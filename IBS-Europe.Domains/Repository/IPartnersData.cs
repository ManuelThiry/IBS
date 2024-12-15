namespace IBS_Europe.Domains;

public interface IPartnersData
{
    public Task<List<Partners>> GetAllPartners(int selectedCategory);
    
    public Task SwitchPriority(int priority, string direction, int selectedCategory);
    
    public Task DeletePartner(int priority, int selectedCategory);
    
    public Task AddPartner(Partners partner);
    
    public Task<bool> PartnerExists(string name, int selectedCategory);

    public Task<Dictionary<string, string>> GetcatalogPaths();

}