using Google.Ads.GoogleAds.Lib;
using Microsoft.AspNetCore.Mvc;
using Prompt2Ads.Services.GoogleAds;

namespace Prompt2Ads.Controllers.GoogleAds;

[ApiController]
[Route("customers")]
public class CustomerController(
    ILogger<CustomerController> logger,
    IConfiguration configuration,
    IAuthService authService,
    IGoogleAdsConfService googleAdsClientService,
    ICustomerService customerService) : ControllerBase
{
    private readonly ILogger<CustomerController> _logger = logger;

    private readonly IConfiguration _configuration = configuration;

    private readonly IAuthService _authService = authService;

    private readonly IGoogleAdsConfService _googleAdsClientService = googleAdsClientService;

    private readonly ICustomerService _customerService = customerService;

    [HttpGet(Name = "GetCustomerIds")]
    public IActionResult Get()
    {   
        Dictionary<string, string> resDict = _authService.CheckRefreshTokenGoogleApi(HttpContext);
        if ( resDict.ContainsKey("error") || resDict.GetValueOrDefault("refreshToken") == null )
        {
            return Unauthorized(resDict);
        } else
        {
            try
            {
                string[] customerIds = _customerService.GetAccessibleCustomers(_googleAdsClientService.GetGoogleAdsClient(resDict.GetValueOrDefault("refreshToken")!));
                return Ok(new { customerIds });
            } catch ( Exception ex )
            {
                _logger.LogError("Error in GetCustomerIds: {}", ex.Message);
                return StatusCode(500, new { errMessage = "Internal server error" });
            }

        }
    }

}