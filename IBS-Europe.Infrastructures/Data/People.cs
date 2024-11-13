namespace IBS_Europe.Infrastructures.Data;

public class People
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    
    public string Phone { get; set; }
    
    public string LastName { get; set; }
    public string Role { get; set; }
    public string Email { get; set; }
    
    public string Path { get; set; }
    
    public int Priority { get; set; }
    
    public int TranslatorId { get; set; }
    public Translator Translator { get; set; }
}