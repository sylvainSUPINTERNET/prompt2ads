using Google.Ads.GoogleAds.Config;
using Google.Ads.GoogleAds.Lib;
using Google.Apis.Auth.OAuth2;

namespace Prompt2Ads.Services.GoogleAds;

public class GoogleAdsConfService : IGoogleAdsConfService
{
    private readonly IConfiguration _configuration;
    public GoogleAdsConfService(
        IConfiguration configuration
    )
    {
        _configuration = configuration;
    }

    public Dictionary<GoogleAdsClient, GoogleAdsConfig> GetGoogleAdsClient(string refreshToken)
    {
        var secrets = GoogleClientSecrets.FromFile("client_secret_web.json").Secrets;

        GoogleAdsConfig googleAdsConfig = new()
        {
            DeveloperToken = _configuration["Google:developerToken"] ?? "",
            OAuth2ClientId = secrets.ClientId,
            OAuth2ClientSecret = secrets.ClientSecret,
            OAuth2RefreshToken = refreshToken?.ToString() ?? "",
        };

        GoogleAdsClient googleAdsClient = new(googleAdsConfig);

        return new Dictionary<GoogleAdsClient, GoogleAdsConfig>
        {
            { googleAdsClient, googleAdsConfig }
        };
    }
}

