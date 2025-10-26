using Google.Ads.GoogleAds.Config;
using Google.Ads.GoogleAds.Lib;
using Microsoft.AspNetCore.Mvc;
using Prompt2Ads.Services.GoogleAds;

namespace Prompt2Ads.Controllers.GoogleAds;

[ApiController]
[Route("customers")]
public class CustomerController(
    ILogger<CustomerController> logger,
    IAuthService authService,
    IGoogleAdsConfService googleAdsClientService,
    ICustomerService customerService) : ControllerBase
{
    private readonly ILogger<CustomerController> _logger = logger;

    private readonly IAuthService _authService = authService;

    private readonly IGoogleAdsConfService _googleAdsClientService = googleAdsClientService;

    private readonly ICustomerService _customerService = customerService;

    [HttpGet(Name = "GetCustomerIds")]
    public IActionResult Get()
    {
        Dictionary<string, string> resDict = _authService.CheckRefreshTokenGoogleApi(HttpContext);
        if (resDict.ContainsKey("error") || resDict.GetValueOrDefault("refreshToken") == null)
        {
            return Unauthorized(resDict);
        }
        else
        {
            try
            {
                Dictionary<GoogleAdsClient, GoogleAdsConfig> googleAdsClients = _googleAdsClientService.GetGoogleAdsClient(resDict.GetValueOrDefault("refreshToken")!);
                string[] customerIds = _customerService.GetAccessibleCustomers(googleAdsClients.Keys.First());
                return Ok(new { customerIds });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetCustomerIds: {}", ex.Message);
                return StatusCode(500, new { errMessage = "Internal server error" });
            }

        }
    }
    
    [HttpGet("validate", Name = "ValidateCustomerId")]
    public IActionResult Validate()
    {
        HttpContext.Items.TryGetValue("X-Google-Customer-Id", out var customerIdHeader);
        if (customerIdHeader == null)
        {
            return BadRequest(new { error = "No customerId header provided." });
        }
        
        Dictionary<string, string> resDict = _authService.CheckRefreshTokenGoogleApi(HttpContext);
        if (resDict.ContainsKey("error") || resDict.GetValueOrDefault("refreshToken") == null)
        {
            return Unauthorized(resDict);
        }
        else
        {
            try
            {
                Dictionary<GoogleAdsClient, GoogleAdsConfig> googleAdsClients = _googleAdsClientService.GetGoogleAdsClient(resDict.GetValueOrDefault("refreshToken")!);

                Dictionary<string, CustomerService.CustomerDto?> resp = _customerService.ValidateCustomerId(googleAdsClients, customerIdHeader.ToString()!);
                return Ok(resp); // If customerDto is null, the customerId is InvalidReason != null means there was an error
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in ValidateCustomerId: {}", ex.Message);
                return StatusCode(500, new { errMessage = "Internal server error" });
            }

        }
    }

}