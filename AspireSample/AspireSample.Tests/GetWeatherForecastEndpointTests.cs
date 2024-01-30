using System.Net;

namespace AspireSample.Tests;

public sealed class GetWeatherForecastEndpointTests(ApiServiceFixture fixture) : IClassFixture<ApiServiceFixture>
{
    private readonly HttpClient _httpClient = fixture.CreateClient();

    [Fact]
    public async Task GetWeatherForecast_When()
    {
        // Arrange
        var requestUri = new Uri("weatherforecast", UriKind.Relative);

        // Act
        var response = await _httpClient.GetAsync(requestUri);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}