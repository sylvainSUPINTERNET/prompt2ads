namespace Prompt2Ads.Services.GoogleAds;

public class Campaign : ICampaign
{
    private readonly ILogger<Campaign> _logger;

    public Campaign(ILogger<Campaign> logger)
    {
        _logger = logger;
    }


    public void SomeMethod()
    {
        _logger.LogInformation("Campaign method executed.");
    }
    

}