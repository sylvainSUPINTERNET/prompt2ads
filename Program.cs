using Prompt2Ads.Services.Config;
using Prompt2Ads.Services.GoogleAds;

var builder = WebApplication.CreateBuilder(args);

// TODO : move to development only later and configure with the appsettings.json for the production URL
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<GoogleSdkConfig>();
builder.Services.AddScoped<ICampaign, Campaign>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.Services.GetService<GoogleSdkConfig>()?.DevDesktopClientAccessTokenGenerator();
}


app.UseHttpsRedirection();
app.UseCors();
app.MapControllers();
app.Run();
