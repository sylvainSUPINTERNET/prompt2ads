using Google.Ads.GoogleAds.Lib;

namespace Prompt2Ads.Services.Config;

public interface IGoogleSdkConfig
{
    void DevDesktopOAuth2Modal();

    Task<GoogleAdsClient> GetGoogleAdsClient();
}