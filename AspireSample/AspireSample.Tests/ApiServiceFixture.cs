using AspireSample.ApiService;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace AspireSample.Tests;

public sealed class ApiServiceFixture : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly IHost _app;
    private readonly EndpointReference _weatherApiEndpoint;

    public ApiServiceFixture()
    {
        var options = new DistributedApplicationOptions
        {
            AssemblyName = typeof(ApiServiceFixture).Assembly.FullName,
            DisableDashboard = true
        };
        var appBuilder = DistributedApplication.CreateBuilder(options);

        var weatherApiService = appBuilder.AddContainer("weatherapiservice", "sheyenrath/wiremock.net-alpine", "latest")
            .WithServiceBinding(
                containerPort: 80,
                name: "weatherapiservice",
                scheme: "http");

        _weatherApiEndpoint = weatherApiService.GetEndpoint("weatherapiservice");

        _app = appBuilder.Build();
    }

    public async Task InitializeAsync()
    {
        await _app.StartAsync();

        // Wait until container start
        await Task.Delay(3_000);

        var weatherApiServer = new WeatherApiServer(_weatherApiEndpoint.UriString);
        await weatherApiServer.SetupAsync();
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _app.StopAsync();
        if (_app is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync().ConfigureAwait(false);
        }
        else
        {
           _app.Dispose();
        }
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            var settings = new Dictionary<string, string?>
            {
                { "WeatherClient:BaseAddress", _weatherApiEndpoint.UriString },
                { "WeatherClient:ApiKey", "valid_api_key" }
            };

            config.AddInMemoryCollection(settings);
        });
        return base.CreateHost(builder);
    }
}
