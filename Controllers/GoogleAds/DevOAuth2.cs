using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Microsoft.AspNetCore.Mvc;

namespace Prompt2Ads.Controllers.GoogleAds;


[ApiController]
[Route("/api/googleads")]
public class DevOAuth2 : ControllerBase
{

    private readonly string redirectUri = "http://localhost:3000/api/googleads/callback";
    private readonly ILogger<DevOAuth2> _logger;
    public DevOAuth2(ILogger<DevOAuth2> logger)
    {
        _logger = logger;
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

        // TODO: Save the refresh token securely for future use
        // await SaveRefreshToken("user", token.RefreshToken);

        return Ok(new Dictionary<string, string>
        {
            { "access_token", token.AccessToken },
            { "refresh_token", token.RefreshToken }
        });

    }

}

