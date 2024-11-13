namespace IBS_Europe.Domains;

public interface IBrokerData
{
    public List<Broker> GetBrokers(int category, string culture);
    public List<Broker> GetGeneralBrokers(string culture);
    
    public void DeleteBroker(int id);
    
    public bool BrokerExists(int id, string name, int category);
    
    public Task UpdateBroker(int id, string newName);
    
    public Task SwitchPriority(int id, string direction);
    
    public int GetCategory(int id);
    
    public Task AddBroker(Broker broker);
    
    public string GetBrokerName(int id);
}