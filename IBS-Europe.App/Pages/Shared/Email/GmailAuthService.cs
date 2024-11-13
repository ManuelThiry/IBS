using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using System.Threading;

public class GmailOAuthService
{
    private readonly string[] Scopes = { "https://mail.google.com/" }; // Portée pour Gmail
    private readonly string ApplicationName = "IBS Email"; // Nom de votre application
    private readonly string clientId = "101422464832-2shlm9jqhe388vp87errski13798u6hu.apps.googleusercontent.com";
    private readonly string clientSecret = "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";

    public async Task<UserCredential> GetUserCredential()
    {
        var clientSecrets = new ClientSecrets
        {
            ClientId = clientId,
            ClientSecret = clientSecret
        };

        // Stockage des informations utilisateur (accès + jeton de rafraîchissement) dans token.json
        string credPath = "token.json";  // Stockage des jetons

        var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            clientSecrets,
            Scopes,
            "user",
            CancellationToken.None,
            new FileDataStore(credPath, true)
        );

        return credential;
    }
}