using Google.Ads.GoogleAds;
using Google.Ads.GoogleAds.Config;
using Google.Ads.GoogleAds.Lib;
using Google.Ads.GoogleAds.V22.Resources;
using Google.Ads.GoogleAds.V22.Services;
using Google.Apis.Auth.OAuth2;
using static Google.Ads.GoogleAds.Services;

namespace Prompt2Ads.Services.GoogleAds;

public class CustomerService : ICustomerService
{

    public CustomerService()
    {

    }

    public string[] GetAccessibleCustomers(GoogleAdsClient googleAdsClient)
    {
        CustomerServiceClient customerService = googleAdsClient.GetService(V22.CustomerService);
        return customerService.ListAccessibleCustomers();
    }


}
