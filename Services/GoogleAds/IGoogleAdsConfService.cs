using Google.Ads.GoogleAds.Lib;

namespace Prompt2Ads.Services.GoogleAds;

public interface IGoogleAdsConfService
{
    GoogleAdsClient GetGoogleAdsClient(string refreshToken);
}
