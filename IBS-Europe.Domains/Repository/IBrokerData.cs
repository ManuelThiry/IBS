﻿namespace IBS_Europe.Domains;

public interface IBrokerData
{
    public Task<List<Broker>> GetBrokers(int category, string culture);
    public Task<List<Broker>> GetGeneralBrokers(string culture);
    
    public Task<string> DeleteBroker(int id);
    
    public Task<bool> BrokerExists(int id, string name, int category);
    
    public Task UpdateBroker(int id, string newName, string product);
    
    public Task SwitchPriority(int id, string direction);
    
    public Task<int> GetCategory(int id);
    
    public Task AddBroker(Broker broker);
    
    public Task<string> GetBrokerName(int id);

    public Task<List<string>> GetProductsList();
    
    public Task<string> GetProduct(int id);
}