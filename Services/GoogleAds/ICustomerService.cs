using Google.Ads.GoogleAds.Config;
using Google.Ads.GoogleAds.Lib;
using static Prompt2Ads.Services.GoogleAds.CustomerService;

namespace Prompt2Ads.Services.GoogleAds;

public interface ICustomerService
{
    string[] GetAccessibleCustomers(GoogleAdsClient googleAdsClient);

    public Dictionary<string, CustomerDto?> ValidateCustomerId(Dictionary<GoogleAdsClient, GoogleAdsConfig> googleAdsClientInfoDictionary, string customerId);
 
}