namespace IBS_Europe.Domains.Repository;

public interface ITranslatorData
{
    public Task<Translator> GetTranslation();
    
    public Task<int> GetNumberOfTranslations();
    
    public Task SetTranslation(string translation, int id);
}