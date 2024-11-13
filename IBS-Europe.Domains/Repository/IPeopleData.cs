namespace IBS_Europe.Domains;

public interface IPeopleData
{
    public List<People> GetAllPeople();
    
    public bool PeopleExists(string Firstname, string LastName, int id);
    
    public Task AddPeople(People people);
    
    public void DeletePeople(int id);
    
    public void SwitchPriority(int id, string direction);
    
    public string GetName(int id);
    
    public void UpdateImage(int id, string path);
    
    public Task UpdatePeople(People people);
}