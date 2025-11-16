using Google.Ads.GoogleAds;
using Google.Ads.GoogleAds.Config;
using Google.Ads.GoogleAds.Lib;
using Google.Ads.GoogleAds.V22.Resources;
using Google.Ads.GoogleAds.V22.Services;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/google/campaigns")]
public class CampaignController : ControllerBase
{
    private readonly ILogger<CampaignController> _logger;

    private readonly IConfiguration _configuration;

    public CampaignController(ILogger<CampaignController> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpPost(Name = "CreateCampaignGoogleAds")]
    public object Get()
    {

        return new { message = "CreateCampaignGoogleAds endpoint called" };
    }

    // [HttpGet("test")]
    // public IActionResult Test()
    // {
    //     HttpContext.Items.TryGetValue("GoogleRefreshToken", out var refreshToken);

    //     if (refreshToken == null)
    //     {
    //         return Unauthorized("No refresh token found in context.");
    //     }

    //     var secrets = GoogleClientSecrets.FromFile("client_secret_web.json").Secrets;

    //     GoogleAdsConfig googleAdsConfig = new GoogleAdsConfig()
    //     {
    //         DeveloperToken = _configuration["Google:developerToken"] ?? "",
    //         OAuth2ClientId = secrets.ClientId,
    //         OAuth2ClientSecret = secrets.ClientSecret,
    //         OAuth2RefreshToken = refreshToken?.ToString() ?? "",
    //     };

    //     GoogleAdsClient googleAdsClient = new GoogleAdsClient(googleAdsConfig);

    //     CampaignServiceClient campaignService = googleAdsClient.GetService(Services.V22.CampaignService);
    //     CustomerServiceClient customerService =
    //         googleAdsClient.GetService(Services.V22.CustomerService);
    //     var googleAdsService = googleAdsClient.GetService(Services.V22.GoogleAdsService);

    //     string[] accessibleCustomers = customerService.ListAccessibleCustomers();

    //     string query = @"SELECT
    //         customer.id,
    //         customer.descriptive_name,
    //         customer.currency_code,
    //         customer.time_zone
    //     FROM customer";

    //     foreach (string customer in accessibleCustomers)
    //     {
    //         try
    //         {
    //             string customerId = customer.Split('/')[1];

    //             Console.WriteLine($"Customer ID: {customerId}");

    //             var searchRequest = new SearchGoogleAdsStreamRequest
    //             {
    //                 CustomerId = customerId,
    //                 Query = query
    //             };

    //             googleAdsConfig.LoginCustomerId = customerId;

    //             googleAdsService.SearchStream(searchRequest, response =>
    //             {
    //                 foreach (var row in response.Results)
    //                 {
    //                     Customer customer = row.Customer;
    //                     Console.WriteLine($"Nom : {customer.DescriptiveName}");
    //                     Console.WriteLine($"Devise : {customer.CurrencyCode}");
    //                     Console.WriteLine($"Fuseau horaire : {customer.TimeZone}");
    //                     Console.WriteLine("----------------------w---------------");
    //                 }
    //             });


    //             // Create budget
    //             // https://developers.google.com/google-ads/api/docs/campaigns/create-campaigns#c
    //         }
    //         catch (Exception ex)
    //         {
    //             _logger.LogError(ex, "Error fetching customer data");
    //             continue;
    //         }

    //     }
    //     return Ok("Test completed");
    // }
}