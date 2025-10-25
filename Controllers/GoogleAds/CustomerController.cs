using Google.Ads.GoogleAds.Lib;
using Microsoft.AspNetCore.Mvc;
using Prompt2Ads.Services.GoogleAds;

namespace Prompt2Ads.Controllers.GoogleAds;

[ApiController]
[Route("customers")]
public class CustomerController : ControllerBase
{
    private readonly ILogger<CustomerController> _logger;

    private readonly IConfiguration _configuration;

    private readonly IAuthService _authService;

    private readonly IGoogleAdsConfService _googleAdsClientService;
    
    public CustomerController(
        ILogger<CustomerController> logger,
        IConfiguration configuration,
        IAuthService authService,
        IGoogleAdsConfService googleAdsClientService
        )
    {
        _logger = logger;
        _configuration = configuration;
        _authService = authService;
        _googleAdsClientService = googleAdsClientService;
    }

    [HttpGet(Name = "GetCustomerIds")]
    public IActionResult Get()
    {   
        Dictionary<string, string> resDict = _authService.CheckRefreshTokenGoogleApi(HttpContext);
        if ( resDict.ContainsKey("error") || resDict.GetValueOrDefault("refreshToken") == null )
        {
            return Unauthorized(resDict);
        } else
        {
            GoogleAdsClient googleAdsClient = _googleAdsClientService.GetGoogleAdsClient(resDict.GetValueOrDefault("refreshToken")!);
            
            return Ok("Test completed");
        }
    }

}