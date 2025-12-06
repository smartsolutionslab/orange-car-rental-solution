using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;
using Projects;

namespace SmartSolutionsLab.OrangeCarRental.IntegrationTests;

/// <summary>
///     Collection definition for sharing the Aspire distributed application across all integration tests.
///     This ensures only one instance of the AppHost is created and shared across all test classes.
/// </summary>
[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<DistributedApplicationFixture>
{
    public const string Name = "IntegrationTests";
}

/// <summary>
///     Fixture that starts the Aspire AppHost for integration testing
///     This spins up the entire application including SQL Server, all APIs, and the gateway
/// </summary>
public class DistributedApplicationFixture : IAsyncLifetime
{
    private DistributedApplication? app;
    private readonly TimeSpan resourceStartTimeout = TimeSpan.FromMinutes(5);

    public DistributedApplication App => app ?? throw new InvalidOperationException("App not initialized. Call InitializeAsync first.");

    public async Task InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<OrangeCarRental_AppHost>();

        // Build and start the application
        app = await appHost.BuildAsync();

        // Use a CancellationToken with timeout for startup
        using var cts = new CancellationTokenSource(resourceStartTimeout);
        await app.StartAsync(cts.Token);

        // Wait for critical resources to be ready using Aspire's ResourceNotificationService
        await WaitForResourcesAsync(cts.Token);
    }

    private async Task WaitForResourcesAsync(CancellationToken cancellationToken)
    {
        if (app == null) return;

        // Use the Aspire-recommended way to wait for resources
        var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();

        // Wait for the API resources to be in Running state
        var resourcesToWait = new[] { "api-gateway", "fleet-api", "reservations-api", "pricing-api", "customers-api" };

        foreach (var resourceName in resourcesToWait)
        {
            Console.WriteLine($"Waiting for resource '{resourceName}' to enter Running state...");
            await resourceNotificationService
                .WaitForResourceAsync(resourceName, KnownResourceStates.Running)
                .WaitAsync(resourceStartTimeout, cancellationToken);
            Console.WriteLine($"Resource '{resourceName}' is now Running");
        }

        // Additional health check to ensure APIs are fully ready
        await WaitForHealthyAsync(resourcesToWait, cancellationToken);
    }

    private async Task WaitForHealthyAsync(string[] resourceNames, CancellationToken cancellationToken)
    {
        if (app == null) return;

        var maxRetries = 30;
        var delayBetweenRetries = TimeSpan.FromSeconds(2);

        foreach (var resourceName in resourceNames)
        {
            var healthy = false;
            for (var i = 0; i < maxRetries && !healthy; i++)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    using var httpClient = app.CreateHttpClient(resourceName);
                    httpClient.Timeout = TimeSpan.FromSeconds(30);

                    var response = await httpClient.GetAsync("/health", cancellationToken);
                    if (response.IsSuccessStatusCode)
                    {
                        healthy = true;
                        Console.WriteLine($"Resource '{resourceName}' health check passed after {i + 1} attempts");
                    }
                    else
                    {
                        Console.WriteLine($"Resource '{resourceName}' health check returned {response.StatusCode} (attempt {i + 1}/{maxRetries})");
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    Console.WriteLine($"Waiting for '{resourceName}' health (attempt {i + 1}/{maxRetries}): {ex.Message}");
                }

                if (!healthy)
                {
                    await Task.Delay(delayBetweenRetries, cancellationToken);
                }
            }

            if (!healthy)
            {
                throw new TimeoutException($"Resource '{resourceName}' did not become healthy within the timeout period");
            }
        }
    }

    public async Task DisposeAsync()
    {
        if (app != null) await app.DisposeAsync();
    }

    public HttpClient CreateHttpClient(string resourceName)
    {
        if (app == null) throw new InvalidOperationException("App not initialized");

        var client = app.CreateHttpClient(resourceName);
        client.Timeout = TimeSpan.FromMinutes(2);
        return client;
    }
}
