namespace IBS_Europe.Domains;

public interface IPeopleData
{
    public Task<List<People>> GetAllPeople();
    
    public Task<bool> PeopleExists(string Firstname, string LastName, int id);
    
    public Task AddPeople(People people);
    
    public Task<string> DeletePeople(int id);
    
    public Task SwitchPriority(int id, string direction);
    
    public Task UpdatePeople(People people);
    
    public Task<string> GetPath(int id);
}