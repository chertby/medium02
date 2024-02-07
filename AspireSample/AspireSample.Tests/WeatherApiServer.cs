using RestEase;
using WireMock.Admin.Mappings;
using WireMock.Client;
using WireMock.Client.Extensions;

namespace AspireSample.Tests;

public sealed class WeatherApiServer(string baseUrl)
{
    private readonly IWireMockAdminApi _wireMockAdminApi =
        RestClient.For<IWireMockAdminApi>(new Uri(baseUrl, UriKind.Absolute));

    public Task SetupAsync(CancellationToken cancellationToken = default)
    {
        var mappingBuilder = _wireMockAdminApi.GetMappingBuilder();

        mappingBuilder.Given(builder => builder
            .WithRequest(request => request
                .UsingGet()
                .WithPath("/v4/weather/forecast")
                .WithParams(p => p
                    .Add(GetLocationParameter)
                    .Add(GetDaysTimeStepsParameter)
                    .Add(GetDaysUnitsParameter)
                    .Add(GetApiKeyParameter))
            )
            .WithResponse(response => response
                .WithBody("""
                {
                    "timelines": {
                        "daily": [
                            {
                                "time": "2024-01-30T11:00:00Z",
                                "values": {
                                    "temperatureAvg": 2.01
                                }
                            },
                            {
                                "time": "2024-01-31T11:00:00Z",
                                "values": {
                                    "temperatureAvg": 3.02
                                }
                            },
                            {
                                "time": "2024-02-01T11:00:00Z",
                                "values": {
                                    "temperatureAvg": 4.03
                                }
                            }
                        ]
                    }
                }
                """)
            )
        );

        return mappingBuilder.BuildAndPostAsync(cancellationToken);

        static ParamModel GetLocationParameter() =>
            new()
            {
                Name = "location",
                Matchers =
                [
                    new() { Name = "ExactMatcher", Pattern = "new york" }
                ]
            };

        static ParamModel GetDaysTimeStepsParameter() =>
            new()
            {
                Name = "timesteps",
                Matchers =
                [
                    new() { Name = "ExactMatcher", Pattern = "1d" }
                ]
            };

        static ParamModel GetDaysUnitsParameter() =>
            new()
            {
                Name = "units",
                Matchers =
                [
                    new() { Name = "ExactMatcher", Pattern = "metric" }
                ]
            };

        static ParamModel GetApiKeyParameter() =>
            new()
            {
                Name = "apikey",
                Matchers =
                [
                    new() { Name = "ExactMatcher", Pattern = "valid_api_key" }
                ]
            };
    }
}
