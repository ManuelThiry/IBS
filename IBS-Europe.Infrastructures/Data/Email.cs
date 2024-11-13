namespace IBS_Europe.Infrastructures.Data;

public class Email
{
    public int Id { get; set; }
    
    public string EmailAddress { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public int FirstTranslatorId { get; set; }
    public Translator FirstTranslator { get; set; }
    
    public int SecondTranslatorId { get; set; }
    public Translator SecondTranslator { get; set; }
}