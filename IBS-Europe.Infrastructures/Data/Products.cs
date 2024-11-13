namespace IBS_Europe.Infrastructures.Data;

public class Products
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Text { get; set; }
    
    public string Path { get; set; }
    
    public int TranslatorId { get; set; }
    public Translator Translator { get; set; }
}