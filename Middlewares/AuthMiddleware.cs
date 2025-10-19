using Microsoft.Extensions.Primitives;

namespace Prompt2Ads.Middlewares;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<AuthMiddleware> _logger;

    private readonly HashSet<string> _allowedPath =
    [
        "/api/googleads/login",
        "/api/googleads/callback"
    ];

    public AuthMiddleware(
        RequestDelegate next,
        ILogger<AuthMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    static async Task<Dictionary<string, object>> ErrorResponseUnauthorized(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        Dictionary<string, object> responseNotAuthorized = new Dictionary<string, object>
        {
            ["error"] = "Unauthorized"
        };
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(
            System.Text.Json.JsonSerializer.Serialize(responseNotAuthorized)
        );
        return responseNotAuthorized;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if ( context.Request.Path.Value != null &&
             _allowedPath.Contains( context.Request.Path.Value.ToLower() ) )
        {
            await _next(context);
            return;
        }

        if (context.Request.Headers.TryGetValue("X-Google-Token", out StringValues value)
            && value.ToString().Trim().ToString() != "")
        {
            try
            {
                // var googleSdkConfigSvc = context.RequestServices.GetRequiredService<IGoogleSdkConfig>();
                // string accessToken = value.ToString().Trim();
                context.Items["RequestId"] = "data for ctrl";
                await _next(context);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AuthMiddleware");
                _ = ErrorResponseUnauthorized(context);
                return;
            }
        }

        _ = ErrorResponseUnauthorized(context);
        return; 
    }
    
}