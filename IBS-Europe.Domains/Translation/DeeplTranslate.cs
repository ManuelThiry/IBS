using System.Text.Json;

namespace IBS_Europe.Domains.Translation;

public class DeeplSettings
{
    public string ApiKey { get; set; }
    public string Url { get; set; }
}


public class DeeplTranslate
{
    private static string _apiKey;
    private static string _url;

    // Méthode pour initialiser les valeurs statiques
    public static void Init(DeeplSettings deeplSettings)
    {
        _apiKey = deeplSettings.ApiKey;
        _url = deeplSettings.Url;
    }
    public static async Task<string> TranslateTextWithDeeplAsync(string text, string targetLanguage)
    {
        if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_url))
        {
            throw new InvalidOperationException("DeeplTranslate has not been initialized with API key and URL.");
        }
    
        using (HttpClient client = new HttpClient())
        {
            var parameters = new Dictionary<string, string>
            {
                { "auth_key", _apiKey },
                { "text", text },
                { "target_lang", targetLanguage }
            };
        
            var encodedContent = new FormUrlEncodedContent(parameters);
            var response = await client.PostAsync(_url, encodedContent);
            response.EnsureSuccessStatusCode();
        
            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonDocument.Parse(responseBody);
            var translatedText = jsonResponse.RootElement
                .GetProperty("translations")[0]
                .GetProperty("text")
                .GetString();
        
            return translatedText;
        }
    }

}