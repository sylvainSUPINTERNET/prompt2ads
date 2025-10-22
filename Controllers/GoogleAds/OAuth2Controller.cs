using Google.Ads.GoogleAds;
using Google.Ads.GoogleAds.Config;
using Google.Ads.GoogleAds.Lib;
using Google.Ads.GoogleAds.V21.Resources;
using Google.Ads.GoogleAds.V21.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Microsoft.AspNetCore.Mvc;
using Prompt2Ads.Repositories.OAuth2;

namespace Prompt2Ads.Controllers.GoogleAds;


[ApiController]
[Route("/api/googleads")]
public class OAuth2Controller : ControllerBase
{
    private readonly string redirectUri;
    private readonly ILogger<OAuth2Controller> _logger;
    private readonly IConfiguration _configuration;

    private readonly IUserSessionRepository _userSessionRepository;

    private readonly string _provider = "google";

    public OAuth2Controller(
        ILogger<OAuth2Controller> logger,
        IConfiguration configuration,
        IUserSessionRepository userSessionRepository
        )
    {
        _logger = logger;
        _configuration = configuration;
        redirectUri = _configuration["Google:redirectUri"] ?? "http://localhost:3000/api/googleads/callback";
        _userSessionRepository = userSessionRepository;
    }

    [HttpGet("login")]
    public IActionResult GetGoogleAuthUrl()
    {
        var secrets = GoogleClientSecrets.FromFile("client_secret_web.json").Secrets;

        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            Prompt = "consent",
            ClientSecrets = secrets,
            Scopes = new[] { "https://www.googleapis.com/auth/adwords" }
        });

        Uri? authorizationUrl = flow.CreateAuthorizationCodeRequest(redirectUri).Build();
        if (authorizationUrl == null)
        {
            return BadRequest("Failed to create authorization URL.");
        }
        return Redirect(authorizationUrl.ToString());
    }

    [HttpGet("callback")]
    public async Task<IActionResult> GoogleAdsCallback(string code)
    {
        var secrets = GoogleClientSecrets.FromFile("client_secret_web.json").Secrets;

        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = secrets,
            Scopes = new[] { "https://www.googleapis.com/auth/adwords" }
        });

        var token = await flow.ExchangeCodeForTokenAsync(
            userId: "user",
            code: code,
            redirectUri: redirectUri,
            taskCancellationToken: CancellationToken.None
        );


        // TODO: Save the refresh token securely for future use KV database not MONGODB !!!!
        await _userSessionRepository.CreateAsync(new UserSession
        {
            SessionId = Guid.NewGuid().ToString(),
            RefreshToken = token.RefreshToken,
            Provider = _provider,
            Scopes = [.. flow.Scopes]
        });
        // TODO


        // create google client with token
        GoogleAdsConfig googleAdsConfig = new GoogleAdsConfig()
        {
            DeveloperToken = _configuration["Google:developerToken"] ?? "",
            OAuth2ClientId = secrets.ClientId,
            OAuth2ClientSecret = secrets.ClientSecret,
            OAuth2RefreshToken = token.RefreshToken,
        };

        GoogleAdsClient googleAdsClient = new GoogleAdsClient(googleAdsConfig);

        CampaignServiceClient campaignService = googleAdsClient.GetService(Services.V21.CampaignService);
        CustomerServiceClient customerService =
            googleAdsClient.GetService(Services.V21.CustomerService);
        var googleAdsService = googleAdsClient.GetService(Services.V21.GoogleAdsService);

        string[] accessibleCustomers = customerService.ListAccessibleCustomers();

        string query = @"SELECT
            customer.id,
            customer.descriptive_name,
            customer.currency_code,
            customer.time_zone,
            customer.account_status
        FROM customer";

        foreach (string customer in accessibleCustomers)
        {
            try
            {
                string customerId = customer.Split('/')[1];

                Console.WriteLine($"Customer ID: {customerId}");

                var searchRequest = new SearchGoogleAdsStreamRequest
                {
                    CustomerId = customerId,
                    Query = query
                };

                googleAdsService.SearchStream(searchRequest, response =>
                {
                    foreach (var row in response.Results)
                    {
                        Customer customer = row.Customer;
                        Console.WriteLine($"Nom : {customer.DescriptiveName}");
                        Console.WriteLine($"Devise : {customer.CurrencyCode}");
                        Console.WriteLine($"Fuseau horaire : {customer.TimeZone}");
                        Console.WriteLine("-------------------------------------");
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer data");
                continue;
            }
    }
        
        //googleAdsConfig.LoginCustomerId = long.Parse(accessibleCustomers[0].Replace("customers/", ""));

        return Ok(new Dictionary<string, string>
        {
            { "access_token", token.AccessToken },
            { "refresh_token", token.RefreshToken }
        });

    }

}

