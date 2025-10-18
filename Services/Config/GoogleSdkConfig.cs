using Google.Ads.GoogleAds.Config;
using Google.Ads.GoogleAds.Lib;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;

namespace Prompt2Ads.Services.Config;

public class GoogleSdkConfig: IGoogleSdkConfig
{
    private readonly ILogger<GoogleSdkConfig> _logger;

    public GoogleSdkConfig(ILogger<GoogleSdkConfig> logger)
    {
        _logger = logger;
    }

    public async Task<GoogleAdsClient> GetGoogleAdsClient()
    {
        return null;
    }


    /// <summary>
    /// Open OAuth2 modal to generate access token for desktop application ( local test only )
    /// Access token is created in folder at root level : GoogleAdsOAuthTokens
    /// You can delete this folder to re-generate a new access token / refresh token ...
    /// </summary>
    public async void DevDesktopOAuth2Modal()
    {
        _logger.LogInformation("GoogleSdkConfig dev init - DevDesktopOAuth2Modal");

        var secrets = GoogleClientSecrets.FromFile("client_secret_desktop_dev.json").Secrets;
        var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            secrets,
            new[] { "https://www.googleapis.com/auth/adwords" },
            "user",
            CancellationToken.None,
            new FileDataStore("GoogleAdsOAuthTokens", true)
        );

        var config = new GoogleAdsConfig()
        {
            // DeveloperToken = "DEV_TOKEN", ???
            OAuth2ClientId = secrets.ClientId,
            OAuth2ClientSecret = secrets.ClientSecret,
            OAuth2RefreshToken = credential.Token.RefreshToken
            //OAuth2AccessToken = accessToken, // TODO fix : https://chatgpt.com/c/68eecfdf-95bc-8333-9224-4cabdf3d834d
            // OAuth2RefreshToken = credential.Token.RefreshToken,
        };
        var client = new GoogleAdsClient(config);
    }
    
    public async Task<GoogleAdsClient> GetGoogleAdsClient(
        string accessToken,
        string refreshToken)
    {
        var secrets = GoogleClientSecrets.FromFile("client_secret_desktop_dev.json").Secrets;
        var config = new GoogleAdsConfig()
        {
            // DeveloperToken = "DEV_TOKEN", ???
            OAuth2ClientId = secrets.ClientId,
            OAuth2ClientSecret = secrets.ClientSecret,
            OAuth2RefreshToken = refreshToken
            //OAuth2AccessToken = accessToken, // TODO fix : https://chatgpt.com/c/68eecfdf-95bc-8333-9224-4cabdf3d834d
            // OAuth2RefreshToken = credential.Token.RefreshToken,
        };
        var client = new GoogleAdsClient(config);

        return client;
    }
}