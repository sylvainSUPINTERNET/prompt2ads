using System.Text.RegularExpressions;
using Google.Ads.GoogleAds.Config;
using Google.Ads.GoogleAds.Lib;
using Google.Ads.GoogleAds.V22.Resources;
using Google.Ads.GoogleAds.V22.Services;
using static Google.Ads.GoogleAds.Services;

namespace Prompt2Ads.Services.GoogleAds;

public class CustomerService : ICustomerService
{

    public record CustomerDto(string? DescriptiveName, string? CurrencyCode, string? TimeZone, string? InvalidReason) {}

    private readonly ILogger<CustomerService> _logger;
    public CustomerService(ILogger<CustomerService> logger)
    {
        _logger = logger;
    }

    public string[] GetAccessibleCustomers(GoogleAdsClient googleAdsClient)
    {
        CustomerServiceClient customerService = googleAdsClient.GetService(V22.CustomerService);
        return [.. customerService.ListAccessibleCustomers().Select(id => id.Split("/")[1])];
    }

    public Dictionary<string, CustomerDto?> ValidateCustomerId(Dictionary<GoogleAdsClient, GoogleAdsConfig> googleAdsClientInfoDictionary, string customerId)
    {

        GoogleAdsClient googleAdsClient = googleAdsClientInfoDictionary.Keys.First();
        GoogleAdsConfig googleAdsConfig = googleAdsClientInfoDictionary.Values.First();

        googleAdsConfig.LoginCustomerId = customerId;

        var googleAdsService = googleAdsClient.GetService(V22.GoogleAdsService);

        // GAQL query ( will use customerId properly without 'SQL injection' risk)
        string query = @"SELECT
            customer.id,
            customer.descriptive_name,
            customer.currency_code,
            customer.time_zone
        FROM customer";

        var searchRequest = new SearchGoogleAdsStreamRequest
        {
            CustomerId = customerId,
            Query = query
        };


        Dictionary<string, CustomerDto?> customers = [];
        try
        {
            googleAdsService.SearchStream(searchRequest, response =>
            {
                CustomerDto? customerDto = null;
                if ( response.Results.Count == 0 )
                {
                    customers.Add(customerId, new CustomerDto(
                        null,
                        null,
                        null,
                        "Invalid customer ID, no customer found."
                    ));
                    return;
                } else
                {
                    foreach (var row in response.Results)
                    {
                        Customer customer = row.Customer;
                        customerDto = new(
                            customer.DescriptiveName,
                            customer.CurrencyCode,
                            customer.TimeZone,
                            null
                        );
                        break;
                    }
                    customers.Add(customerId, customerDto);
                }

            });
        } catch ( Grpc.Core.RpcException ex )
        {
            _logger.LogError("Error validating customerId {}: {}", customerId, ex.Message);
            
            customers.Add(customerId, new CustomerDto(
                null,
                null,
                null,
                ex.Status.Detail
            ));
        }

        
        return customers;
    }


}
