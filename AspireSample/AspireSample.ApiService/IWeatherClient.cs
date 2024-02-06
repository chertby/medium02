using System.Web;

namespace AspireSample.ApiService;

public interface IWeatherClient
{
    Task<WeatherForecast[]> GetWeatherForecastAsync(
        string city,
        CancellationToken cancellationToken = default);
}

internal sealed class WeatherClient(HttpClient httpClient) : IWeatherClient
{
    public async Task<WeatherForecast[]> GetWeatherForecastAsync(
        string city,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(city);

        string escapedCityString = Uri.EscapeDataString(city);

        var requestUri = new Uri(
            $"v4/weather/forecast?location={escapedCityString}&timesteps=1d&units=metric",
            UriKind.Relative);

        var response = await httpClient.GetFromJsonAsync<TomorrowWeatherForecast>(
            requestUri,
            cancellationToken);

        return response is not null
            ? response.Timelines.Daily
                .Select(x => new WeatherForecast(
                    DateOnly.FromDateTime(x.Time),
                    (int)x.Values.TemperatureAvg,
                    "Mild"))
                .ToArray()
            : [];
    }
}

internal class ApiKeyHandler(string apiKey) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Add the API key as a query parameter to all requests
        var uriBuilder = new UriBuilder(request.RequestUri!);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["apikey"] = apiKey;
        uriBuilder.Query = query.ToString();

        request.RequestUri = uriBuilder.Uri;

        return base.SendAsync(request, cancellationToken);
    }
}

internal sealed record TomorrowWeatherForecast(Timelines Timelines);

internal sealed record Timelines(Timeline[] Daily);

internal sealed record Timeline(DateTime Time, Values Values);

internal sealed record Values(float TemperatureAvg);
