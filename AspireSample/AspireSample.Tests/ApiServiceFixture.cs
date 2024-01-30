using AspireSample.ApiService;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace AspireSample.Tests;

public sealed class ApiServiceFixture : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly IHost _app;

    public ApiServiceFixture()
    {
        var options = new DistributedApplicationOptions
        {
            AssemblyName = typeof(ApiServiceFixture).Assembly.FullName,
            DisableDashboard = true
        };
        var appBuilder = DistributedApplication.CreateBuilder(options);
         _app = appBuilder.Build();
    }

    public async Task InitializeAsync()
    {
        await _app.StartAsync();
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
}