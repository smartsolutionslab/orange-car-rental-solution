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
    private DistributedApplication? app;

    public DistributedApplication App => app ?? throw new InvalidOperationException("App not initialized. Call InitializeAsync first.");

    public async Task InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<OrangeCarRental_AppHost>();

        // Build and start the application
        app = await appHost.BuildAsync();
        await app.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (app != null) await app.DisposeAsync();
    }

    public HttpClient CreateHttpClient(string resourceName)
    {
        if (app == null) throw new InvalidOperationException("App not initialized");

        return app.CreateHttpClient(resourceName);
    }
}
