using Prompt2Ads.Middlewares;
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
builder.Services.AddScoped<IGoogleSdkConfig, GoogleSdkConfig>();
builder.Services.AddScoped<ICampaign, Campaign>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    using (var scope = app.Services.CreateScope())
    {
        var sdkConfig = scope.ServiceProvider.GetRequiredService<IGoogleSdkConfig>();
        sdkConfig.DevDesktopOAuth2Modal();
    }

}


app.UseHttpsRedirection();
app.UseCors();
app.UseMiddleware<AuthMiddleware>();
app.MapControllers();
app.Run();
