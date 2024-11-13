namespace IBS_Europe.Domains;

public interface IPartnersData
{
    public List<Partners> GetAllPartners();
    
    public void SwitchPriority(int priority, string direction);
    
    public void DeletePartner(int priority);
    
    public void AddPartner(Partners partner);
    
    public bool PartnerExists(string name);
    
}