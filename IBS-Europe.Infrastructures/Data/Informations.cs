namespace IBS_Europe.Infrastructures.Data;

public class Informations
{
    public int Id { get; set; }
    
    public string Description { get; set; }
    
    public string Text { get; set; }
    
    public int Type { get; set; }
    
    public int Priority { get; set; }
    
    public int FirstTranslatorId { get; set; }
    public Translator FirstTranslator { get; set; }
    
    public int SecondTranslatorId { get; set; }
    public Translator SecondTranslator { get; set; }
}