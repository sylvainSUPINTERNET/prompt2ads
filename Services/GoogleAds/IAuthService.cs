using Microsoft.AspNetCore.Mvc;

namespace Prompt2Ads.Services.GoogleAds;

public interface IAuthService
{
    Dictionary<string, string>? CheckRefreshTokenGoogleApi(HttpContext context);

}