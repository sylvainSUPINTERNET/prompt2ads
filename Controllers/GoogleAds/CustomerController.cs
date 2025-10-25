using Microsoft.AspNetCore.Mvc;
using Prompt2Ads.Services.GoogleAds;

namespace Prompt2Ads.Controllers.GoogleAds;

[ApiController]
[Route("[controller]")]
public class CampaignController : ControllerBase
{
    private readonly ILogger<CampaignController> _logger;

    private readonly IConfiguration _configuration;

    private readonly IAuthService _authService;

    public CampaignController(
        ILogger<CampaignController> logger,
        IConfiguration configuration,
        IAuthService authService
        )
    {
        _logger = logger;
        _configuration = configuration;
        _authService = authService;
    }

    [HttpGet(Name = "GetCustomerIds")]
    public IActionResult Get()
    {
        var errorDict = _authService.CheckRefreshTokenGoogleApi(HttpContext);
        if (errorDict != null)
        {
            return Unauthorized(errorDict);
        }


        return Ok("Test completed");
    }

}