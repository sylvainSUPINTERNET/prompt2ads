using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Microsoft.AspNetCore.Mvc;

namespace Prompt2Ads.Controllers.GoogleAds;


[ApiController]
[Route("/api/googleads")]
public class DevOAuth2 : ControllerBase
{
    [HttpGet("/login")]
    public IActionResult GetGoogleAuthUrl()
    {
        var secrets = GoogleClientSecrets.FromFile("client_secret_web.json").Secrets;

        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = secrets,
            Scopes = new[] { "https://www.googleapis.com/auth/adwords" }
        });

        var redirectUri = "http://localhost:3000/callback";

        var authorizationUrl = flow.CreateAuthorizationCodeRequest(redirectUri).Build();

        return Ok(new { url = authorizationUrl });
    }

}

