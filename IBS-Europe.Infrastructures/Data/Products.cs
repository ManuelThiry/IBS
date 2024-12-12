namespace IBS_Europe.Infrastructures.Data;

public class Products
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Text { get; set; }
    
    public string Path { get; set; }
    
    public string SmallDescription { get; set; }
    
    public int Priority { get; set; }
    
    public int FirstTranslatorId { get; set; }
    public Translator FirstTranslator { get; set; }
    
    public int SecondTranslatorId { get; set; }
    public Translator SecondTranslator { get; set; }
}