using Microsoft.AspNetCore.Mvc;

namespace Prompt2Ads.Services.GoogleAds;

public class AuthService : IAuthService
{
    Dictionary<string, string> IAuthService.CheckRefreshTokenGoogleApi(HttpContext context)
    {
        context.Items.TryGetValue("GoogleRefreshToken", out var refreshToken);

        if (refreshToken == null)
        {
            return new Dictionary<string, string>
            {
                { "error", "No valid google refresh token found in context." }
            };
        } else
        {
            return new Dictionary<string, string>{
                {
                    "refreshToken", $"{refreshToken}"
                }
            };
        }
    }
}