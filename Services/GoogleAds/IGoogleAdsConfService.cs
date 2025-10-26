using Google.Ads.GoogleAds.Config;
using Google.Ads.GoogleAds.Lib;

namespace Prompt2Ads.Services.GoogleAds;

public interface IGoogleAdsConfService
{
    Dictionary<GoogleAdsClient, GoogleAdsConfig> GetGoogleAdsClient(string refreshToken);
}
