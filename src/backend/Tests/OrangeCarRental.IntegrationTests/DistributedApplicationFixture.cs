using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Projects;

namespace SmartSolutionsLab.OrangeCarRental.IntegrationTests;

/// <summary>
///     Fixture that starts the Aspire AppHost for integration testing
///     This spins up the entire application including SQL Server, all APIs, and the gateway
/// </summary>
public class DistributedApplicationFixture : IAsyncLifetime
{
    private DistributedApplication? _app;

    public DistributedApplication App => _app
                                         ?? throw new InvalidOperationException(
                                             "App not initialized. Call InitializeAsync first.");

    public async Task InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<OrangeCarRental_AppHost>();

        // Build and start the application
        _app = await appHost.BuildAsync();
        await _app.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (_app != null) await _app.DisposeAsync();
    }

    public HttpClient CreateHttpClient(string resourceName)
    {
        if (_app == null)
            throw new InvalidOperationException("App not initialized");

        return _app.CreateHttpClient(resourceName);
    }
}
