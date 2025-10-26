using Google.Ads.GoogleAds.Lib;

namespace Prompt2Ads.Services.GoogleAds;

public interface ICustomerService
{
    string[] GetAccessibleCustomers(GoogleAdsClient googleAdsClient);
}