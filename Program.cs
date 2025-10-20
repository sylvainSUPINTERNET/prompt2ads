using MongoDB.Driver;
using Prompt2Ads.Middlewares;
using Prompt2Ads.Repositories.OAuth2;
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


var mongoDbName = builder.Configuration["MongoDb:DatabaseName"] ?? "prompt2ads";

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(builder.Configuration.GetConnectionString("Prompt2AdsMongoDBMainCluster")));
builder.Services.AddSingleton<IMongoDatabase>(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase(mongoDbName));

builder.Services.AddScoped<ICampaign, Campaign>();
builder.Services.AddScoped<IUserSessionRepository, UserSessionRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // using (var scope = app.Services.CreateScope())
    // {
    //     var sdkConfig = scope.ServiceProvider.GetRequiredService<IGoogleSdkConfig>();
    //     sdkConfig.DevDesktopOAuth2Modal();
    // }

}


app.UseHttpsRedirection();
app.UseCors();
app.UseMiddleware<AuthMiddleware>();
app.MapControllers();
app.Run();
