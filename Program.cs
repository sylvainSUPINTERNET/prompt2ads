using Prompt2Ads.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<GoogleSdkConfig>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.Services.GetService<GoogleSdkConfig>()?.DevDesktopClientAccessTokenGenerator();
}


app.UseHttpsRedirection();
app.MapControllers();
app.Run();
