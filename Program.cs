using Google.Ads.Gax.Config;
using Google.Ads.GoogleAds;
using Google.Ads.GoogleAds.Config;
using Google.Ads.GoogleAds.Lib;
using Google.Ads.GoogleAds.V21.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// ( oauth2 desktop app client !)
var secrets = GoogleClientSecrets.FromFile("client_secret_261652279569-bbe9mt3624vqqop5ob8lmd28l6vgitd5.apps.googleusercontent.com.json").Secrets;
var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
    secrets,
    new[] { "https://www.googleapis.com/auth/adwords" },
    "user",
    CancellationToken.None,
    new FileDataStore("GoogleAdsOAuthTokens", true)
);

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
