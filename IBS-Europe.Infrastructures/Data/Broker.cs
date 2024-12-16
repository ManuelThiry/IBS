namespace IBS_Europe.Infrastructures.Data;

public class Broker
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Path { get; set; }
    
    public int Category { get; set; }
    
    public int Priority { get; set; }
    
    public int TranslatorId { get; set; }
    public Translator Translator { get; set; }
    
    public int? ProductsId { get; set; }
    public Products? Products { get; set; }
}