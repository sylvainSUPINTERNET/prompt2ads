using Microsoft.Extensions.Primitives;
using Prompt2Ads.Repositories.OAuth2;

namespace Prompt2Ads.Middlewares;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<AuthMiddleware> _logger;

    private readonly IServiceScopeFactory _scopeFactory;

    private readonly HashSet<string> _allowedPath =
    [
        "/api/googleads/login",
        "/api/googleads/callback"
    ];

    public AuthMiddleware(
        RequestDelegate next,
        ILogger<AuthMiddleware> logger,
        IServiceScopeFactory scopeFactory)
    {
        _next = next;
        _logger = logger; 
        _scopeFactory = scopeFactory;
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

        if (context.Request.Headers.TryGetValue("X-Google-Session-Id", out StringValues value)
            && value.ToString().Trim().ToString() != "")
        {
            try
            {

                var scope = _scopeFactory.CreateScope();
                var userSessionRepository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();
                // TODO
                // Use something else ... like redis
                UserSession? userSession = await userSessionRepository.GetBySessionIdAsync(value.ToString().Trim());
                // TODO

                if (userSession == null)
                {
                    _ = ErrorResponseUnauthorized(context);
                    return;
                }

                context.Items["GoogleRefreshToken"] = userSession.RefreshToken;

                if (context.Request.Headers.TryGetValue("X-Google-Customer-Id", out var customerIdHeader)
                    && !StringValues.IsNullOrEmpty(customerIdHeader))
                {
                    string customerId = customerIdHeader.ToString();
                    if ( customerIdHeader.Count == 1 && customerId.Trim() != "")
                    {
                        context.Items["X-Google-Customer-Id"] = customerId.Trim();
                    }
                }
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