namespace IBS_Europe.Domains.Repository;

public interface ITranslatorData
{
    public Translator GetTranslation();
    
    public int GetNumberOfTranslations();
    
    public void SetTranslation(string translation, int id);
}