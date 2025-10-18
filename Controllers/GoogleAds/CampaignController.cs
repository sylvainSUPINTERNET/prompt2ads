using Microsoft.AspNetCore.Mvc;

namespace MyApp.Controllers.GoogleAds;

[ApiController]
[Route("[controller]")]
public class CampaignController : ControllerBase
{
    private readonly ILogger<CampaignController> _logger;

    public CampaignController(ILogger<CampaignController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "CreateCampaignGoogleAds")]
    public object Get()
    {
        string? accessToken = Request.Headers["Authorization"];

        _logger.LogInformation("CreateCampaignGoogleAds called with AccessToken: {}", accessToken);

        return new { message = "CreateCampaignGoogleAds endpoint called" };
    }
}