using Google.Ads.GoogleAds.Lib;

namespace Prompt2Ads.Middlewares;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<AuthMiddleware> _logger;

    private readonly IGoogleAdsClient _googleAdsClient;

    public AuthMiddleware(
        RequestDelegate next,
        ILogger<AuthMiddleware> logger,
        IGoogleAdsClient googleAdsClient)
    {
        _next = next;
        _logger = logger;
        _googleAdsClient = googleAdsClient;
    }

    public async Task InvokeAsync(HttpContext context)
    {

        if (context.Request.Headers.ContainsKey("Authorization")
            && context.Request.Headers.Authorization.ToString() != ""
            && context.Request.Headers.Authorization.ToString().StartsWith("Google "))
        {

            string accessToken = context.Request.Headers.Authorization.ToString()["Google ".Length..].Trim();

            context.Items["RequestId"] = "data for ctrl";
            await _next(context);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;

        Dictionary<string, object> responseNotAuthorized = new Dictionary<string, object>
        {
            ["error"] = "Unauthorized"
        };
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(
            System.Text.Json.JsonSerializer.Serialize(responseNotAuthorized)
        );
        return;
    }
    
}