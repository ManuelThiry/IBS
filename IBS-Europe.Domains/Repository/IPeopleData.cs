namespace IBS_Europe.Domains;

public interface IPeopleData
{
    public Task<List<People>> GetAllPeople();
    
    public Task<bool> PeopleExists(string Firstname, string LastName, int id);
    
    public Task AddPeople(People people);
    
    public Task DeletePeople(int id);
    
    public Task SwitchPriority(int id, string direction);
    
    public Task<string> GetName(int id);
    
    public Task UpdateImage(int id, string path);
    
    public Task UpdatePeople(People people);
}