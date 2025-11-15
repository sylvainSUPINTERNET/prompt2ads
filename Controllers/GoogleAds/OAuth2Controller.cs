using System.Net.Http.Headers;
using Google.Ads.GoogleAds;
using Google.Ads.GoogleAds.Config;
using Google.Ads.GoogleAds.Lib;
using Google.Ads.GoogleAds.V22.Resources;
using Google.Ads.GoogleAds.V22.Services;
using Google.Api;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Prompt2Ads.Repositories.OAuth2;
using Prompt2Ads.Services.GoogleAds;

namespace Prompt2Ads.Controllers.GoogleAds;


[ApiController]
[Route("/api/googleads")]
public class OAuth2Controller : ControllerBase
{
    private readonly string redirectUri;
    private readonly ILogger<OAuth2Controller> _logger;
    private readonly IConfiguration _configuration;
    private readonly IAuthService _authService;

    private readonly IUserSessionRepository _userSessionRepository;

    private readonly string _provider = "google";

    private readonly string[] _scopes = new[]
    {
        "https://www.googleapis.com/auth/adwords",
        "openid",
        "email",
        "profile"
    };

    public OAuth2Controller(
        ILogger<OAuth2Controller> logger,
        IConfiguration configuration,
        IUserSessionRepository userSessionRepository,
        IAuthService authService
        )
    {
        _logger = logger;
        _configuration = configuration;
        redirectUri = _configuration["Google:redirectUri"] ?? "http://localhost:3000/api/googleads/callback";
        _userSessionRepository = userSessionRepository;
        _authService = authService;
    }

    [HttpGet("login")]
    public IActionResult GetGoogleAuthUrl()
    {
        var secrets = GoogleClientSecrets.FromFile("client_secret_web.json").Secrets;

        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            Prompt = "consent",
            ClientSecrets = secrets,
            Scopes = _scopes
        });

        Uri? authorizationUrl = flow.CreateAuthorizationCodeRequest(redirectUri).Build();
        if (authorizationUrl == null)
        {
            return BadRequest("Failed to create authorization URL.");
        }
        return Redirect(authorizationUrl.ToString());
    }

    [HttpGet("user-info")]
    public async Task<IActionResult> GetUserInfo()
    {
        try
        {
            Dictionary<string, string> resDict = _authService.CheckRefreshTokenGoogleApi(HttpContext);
            if (resDict.ContainsKey("error") || resDict.GetValueOrDefault("refreshToken") == null)
            {
                return Unauthorized(resDict);
            }

            return Ok( new 
            { 
                email = HttpContext.Items.TryGetValue("GoogleEmail", out var email) ? email : null,
                name = HttpContext.Items.TryGetValue("GoogleName", out var name) ? name : null,
                picture = HttpContext.Items.TryGetValue("GooglePicture", out var picture) ? picture : null,
                emailVerified = HttpContext.Items.TryGetValue("GoogleEmailVerified", out var emailVerified) ? emailVerified : null,
                givenName = HttpContext.Items.TryGetValue("GoogleGivenName", out var givenName) ? givenName : null,
                familyName = HttpContext.Items.TryGetValue("GoogleFamilyName", out var familyName) ? familyName : null,
                sub = HttpContext.Items.TryGetValue("GoogleSub", out var sub) ? sub : null,
                scopes = HttpContext.Items.TryGetValue("GoogleScopes", out var scopes) ? scopes : null
            } );
        }
         catch ( Exception ex)
        {
            _logger.LogError(ex, "Error in GetUserInfo");
            return StatusCode(500, new { error = "Internal server error", details = ex.Message }); 
        }
    }

    [HttpGet("callback")]
    public async Task<IActionResult> GoogleAdsCallback(string code)
    {

        try
        {
            
            var secrets = GoogleClientSecrets.FromFile("client_secret_web.json").Secrets;

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = secrets,
                Scopes = _scopes
            });

            var token = await flow.ExchangeCodeForTokenAsync(
                userId: "user",
                code: code,
                redirectUri: redirectUri,
                taskCancellationToken: CancellationToken.None
            );
        
            var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var response = await http.GetStringAsync("https://openidconnect.googleapis.com/v1/userinfo");
            var user = JObject.Parse(response);
            
            // TODO: Save the refresh token securely for future use KV database not MONGODB !!!!

            // TODO => use TTL based on token expiry
            // await _userSessionRepository.CreateAsync(new UserSession
            // {
            //     SessionId = Guid.NewGuid().ToString(),
            //     RefreshToken = token.RefreshToken,
            //     Provider = _provider,
            //     Scopes = [.. flow.Scopes],
            //     Email = user["email"]?.ToString() ?? "",
            //     Name = user["name"]?.ToString() ?? "",
            //     Picture = user["picture"]?.ToString() ?? "",
            //     EmailVerified = user["email_verified"]?.ToObject<bool>() ?? false,
            //     GivenName = user["given_name"]?.ToString(),
            //     FamilyName = user["family_name"]?.ToString(),
            //     Sub = user["sub"]!.ToString()
            // });
            // TODO

            // // TODO => for testing 
            //     // create google client with token
            //     GoogleAdsConfig googleAdsConfig = new GoogleAdsConfig()
            //     {
            //         DeveloperToken = _configuration["Google:developerToken"] ?? "",
            //         OAuth2ClientId = secrets.ClientId,
            //         OAuth2ClientSecret = secrets.ClientSecret,
            //         OAuth2RefreshToken = token.RefreshToken,
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
            //                     Console.WriteLine("-------------------------------------");
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
            //  }
                

            return Ok(new Dictionary<string, string>
            {
                { "access_token", token.AccessToken },
                { "refresh_token", token.RefreshToken }
            });
        }
            catch ( Exception ex)
        {
            _logger.LogError(ex, "Error in GoogleAdsCallback");
            return StatusCode(500, new { error = "Internal server error", details = ex.Message }); 
        }
    }

}

