using AspireSample.ApiService;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddHttpClient<IWeatherClient, WeatherClient>(client =>
{
    var baseAddress = builder.Configuration["WeatherClient:BaseAddress"];
    client.BaseAddress = new Uri(baseAddress);
})
.AddHttpMessageHandler(() =>
{
    var apiKey = builder.Configuration["WeatherClient:ApiKey"];
    return new ApiKeyHandler(apiKey);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapGet("/weatherforecast", async (
    IWeatherClient weatherClient,
    CancellationToken cancellationToken) =>
{
    var forecast = await weatherClient.GetWeatherForecastAsync("new york", cancellationToken);
    return forecast;
});

app.MapDefaultEndpoints();

app.Run();

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}