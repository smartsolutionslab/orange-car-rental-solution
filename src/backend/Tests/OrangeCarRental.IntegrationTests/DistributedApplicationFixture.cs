using System.Net.Http.Headers;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Projects;
using SmartSolutionsLab.OrangeCarRental.IntegrationTests.Infrastructure;

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
///     Fixture that starts the Aspire AppHost for integration testing.
///     This spins up the entire application including SQL Server, all APIs, and the gateway.
///     Databases are cleaned up after each test run to ensure test isolation.
/// </summary>
public class DistributedApplicationFixture : IAsyncLifetime
{
    private DistributedApplication? app;
    private KeycloakTokenProvider? tokenProvider;
    private readonly TimeSpan resourceStartTimeout = TimeSpan.FromMinutes(8);
    private string? sqlConnectionString;

    /// <summary>
    ///     Database names used by the application that should be dropped after tests.
    /// </summary>
    private static readonly string[] DatabaseNames =
    [
        "OrangeCarRental_Fleet",
        "OrangeCarRental_Reservations",
        "OrangeCarRental_Pricing",
        "OrangeCarRental_Customers",
        "OrangeCarRental_Payments",
        "OrangeCarRental_Notifications",
        "OrangeCarRental_Locations"
    ];

    public DistributedApplication App => app ?? throw new InvalidOperationException("App not initialized. Call InitializeAsync first.");

    /// <summary>
    ///     Gets the Keycloak token provider for acquiring test user tokens.
    /// </summary>
    public KeycloakTokenProvider TokenProvider => tokenProvider ?? throw new InvalidOperationException("TokenProvider not initialized. Call InitializeAsync first.");

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

        // Initialize Keycloak token provider
        await InitializeKeycloakAsync(cts.Token);

        // Store SQL connection string for cleanup
        await CaptureSqlConnectionStringAsync(cts.Token);
    }

    private async Task InitializeKeycloakAsync(CancellationToken cancellationToken)
    {
        if (app == null) return;

        // Get Keycloak endpoint
        var keycloakEndpoint = app.GetEndpoint("keycloak", "http");
        var keycloakUrl = keycloakEndpoint.ToString().TrimEnd('/');

        Console.WriteLine($"Keycloak URL: {keycloakUrl}");

        // Wait for Keycloak to be ready and realm to be imported
        await WaitForKeycloakRealmAsync(keycloakUrl, cancellationToken);

        // Initialize token provider
        tokenProvider = new KeycloakTokenProvider(keycloakUrl);
    }

    private async Task WaitForKeycloakRealmAsync(string keycloakUrl, CancellationToken cancellationToken)
    {
        var maxRetries = 90; // Increased for CI environments
        var delayBetweenRetries = TimeSpan.FromSeconds(2);
        var realmUrl = $"{keycloakUrl}/realms/orange-car-rental/.well-known/openid-configuration";

        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(10);

        for (var i = 0; i < maxRetries; i++)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var response = await httpClient.GetAsync(realmUrl, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Keycloak realm 'orange-car-rental' is ready after {i + 1} attempts");
                    return;
                }

                Console.WriteLine($"Waiting for Keycloak realm (attempt {i + 1}/{maxRetries}): {response.StatusCode}");
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Console.WriteLine($"Waiting for Keycloak realm (attempt {i + 1}/{maxRetries}): {ex.Message}");
            }

            await Task.Delay(delayBetweenRetries, cancellationToken);
        }

        throw new TimeoutException("Keycloak realm 'orange-car-rental' did not become available within the timeout period");
    }

    private async Task WaitForResourcesAsync(CancellationToken cancellationToken)
    {
        if (app == null) return;

        // Use the Aspire-recommended way to wait for resources
        var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();

        // First, wait for db-migrator to complete (it's a one-shot executable)
        Console.WriteLine("Waiting for 'db-migrator' to complete...");
        await resourceNotificationService
            .WaitForResourceAsync("db-migrator", KnownResourceStates.Finished)
            .WaitAsync(resourceStartTimeout, cancellationToken);
        Console.WriteLine("'db-migrator' completed successfully");

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

        var maxRetries = 45;
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
        // Clean up databases before disposing to ensure fresh state on next run
        await CleanupDatabasesAsync();

        if (app != null) await app.DisposeAsync();
    }

    public HttpClient CreateHttpClient(string resourceName)
    {
        if (app == null) throw new InvalidOperationException("App not initialized");

        var client = app.CreateHttpClient(resourceName);
        client.Timeout = TimeSpan.FromMinutes(2);
        return client;
    }

    /// <summary>
    ///     Creates an HTTP client authenticated as a customer.
    /// </summary>
    public async Task<HttpClient> CreateCustomerHttpClientAsync(string resourceName = "api-gateway")
    {
        var token = await TokenProvider.GetCustomerTokenAsync();
        return CreateAuthenticatedClient(resourceName, token);
    }

    /// <summary>
    ///     Creates an HTTP client authenticated as a call center agent.
    /// </summary>
    public async Task<HttpClient> CreateCallCenterHttpClientAsync(string resourceName = "api-gateway")
    {
        var token = await TokenProvider.GetCallCenterTokenAsync();
        return CreateAuthenticatedClient(resourceName, token);
    }

    /// <summary>
    ///     Creates an HTTP client authenticated as a fleet manager.
    /// </summary>
    public async Task<HttpClient> CreateFleetManagerHttpClientAsync(string resourceName = "api-gateway")
    {
        var token = await TokenProvider.GetFleetManagerTokenAsync();
        return CreateAuthenticatedClient(resourceName, token);
    }

    /// <summary>
    ///     Creates an HTTP client authenticated as an admin.
    /// </summary>
    public async Task<HttpClient> CreateAdminHttpClientAsync(string resourceName = "api-gateway")
    {
        var token = await TokenProvider.GetAdminTokenAsync();
        return CreateAuthenticatedClient(resourceName, token);
    }

    private HttpClient CreateAuthenticatedClient(string resourceName, string token)
    {
        if (app == null) throw new InvalidOperationException("App not initialized");

        // Create a custom HttpClientHandler that preserves Authorization headers on redirects
        // This is needed because HTTP→HTTPS redirects (307) strip auth headers by default
        var handler = new AuthPreservingRedirectHandler();

        // Get the base address from Aspire (prefer HTTPS to avoid redirect issues)
        var baseAddress = app.GetEndpoint(resourceName, "https") ?? app.GetEndpoint(resourceName, "http");

        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseAddress.ToString()),
            Timeout = TimeSpan.FromMinutes(2)
        };
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    /// <summary>
    /// Custom handler that preserves Authorization headers across redirects.
    /// By default, HttpClient strips auth headers on cross-origin redirects for security.
    /// In integration tests, we need to preserve them across HTTP→HTTPS redirects.
    /// </summary>
    private class AuthPreservingRedirectHandler : HttpClientHandler
    {
        public AuthPreservingRedirectHandler()
        {
            // Don't automatically follow redirects - we'll handle them manually
            AllowAutoRedirect = false;
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true; // Accept self-signed certs in tests
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // For POST/PUT/PATCH requests, we need to buffer the content so we can resend it on redirect
            byte[]? contentBytes = null;
            string? contentType = null;
            if (request.Content != null)
            {
                contentBytes = await request.Content.ReadAsByteArrayAsync(cancellationToken);
                contentType = request.Content.Headers.ContentType?.ToString();
            }

            var response = await base.SendAsync(request, cancellationToken);

            // Handle redirects manually while preserving the Authorization header and body
            var redirectCount = 0;
            while (IsRedirect(response.StatusCode) && redirectCount < 5)
            {
                var location = response.Headers.Location;
                if (location == null) break;

                // Make the location absolute if it's relative
                if (!location.IsAbsoluteUri)
                {
                    location = new Uri(request.RequestUri!, location);
                }

                // For 307/308, preserve the HTTP method; for others, convert to GET
                var method = response.StatusCode == System.Net.HttpStatusCode.TemporaryRedirect ||
                             response.StatusCode == System.Net.HttpStatusCode.PermanentRedirect
                    ? request.Method
                    : HttpMethod.Get;

                // Create new request preserving the Authorization header
                var newRequest = new HttpRequestMessage(method, location);

                // Copy the Authorization header
                if (request.Headers.Authorization != null)
                {
                    newRequest.Headers.Authorization = request.Headers.Authorization;
                }

                // Copy other headers (excluding headers that shouldn't be forwarded on redirects)
                var excludedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "Authorization", // Already copied above
                    "Transfer-Encoding", // Will be set automatically if needed
                    "Content-Length", // Will be set automatically if needed
                    "Content-Type", // Will be set with content if needed
                    "Host" // Will be set automatically for new URI
                };

                foreach (var header in request.Headers)
                {
                    if (!excludedHeaders.Contains(header.Key))
                    {
                        newRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }

                // For 307/308 redirects, preserve the request body
                if (contentBytes != null && contentBytes.Length > 0 &&
                    (response.StatusCode == System.Net.HttpStatusCode.TemporaryRedirect ||
                     response.StatusCode == System.Net.HttpStatusCode.PermanentRedirect))
                {
                    newRequest.Content = new ByteArrayContent(contentBytes);
                    if (!string.IsNullOrEmpty(contentType))
                    {
                        newRequest.Content.Headers.TryAddWithoutValidation("Content-Type", contentType);
                    }
                }

                response.Dispose();
                response = await base.SendAsync(newRequest, cancellationToken);
                redirectCount++;
            }

            return response;
        }

        private static bool IsRedirect(System.Net.HttpStatusCode statusCode) =>
            statusCode == System.Net.HttpStatusCode.Moved ||
            statusCode == System.Net.HttpStatusCode.Found ||
            statusCode == System.Net.HttpStatusCode.SeeOther ||
            statusCode == System.Net.HttpStatusCode.TemporaryRedirect ||
            statusCode == System.Net.HttpStatusCode.PermanentRedirect;
    }

    /// <summary>
    ///     Captures the SQL Server connection string from the running Aspire app.
    ///     This is used later for database cleanup.
    /// </summary>
    private async Task CaptureSqlConnectionStringAsync(CancellationToken cancellationToken)
    {
        if (app == null) return;

        try
        {
            // Get connection string from fleet database resource (any database would work)
            var connectionString = await app.GetConnectionStringAsync("fleet", cancellationToken);
            if (!string.IsNullOrEmpty(connectionString))
            {
                // Modify connection string to connect to master database for DROP commands
                var builder = new SqlConnectionStringBuilder(connectionString)
                {
                    InitialCatalog = "master",
                    TrustServerCertificate = true
                };
                sqlConnectionString = builder.ConnectionString;
                Console.WriteLine("Captured SQL Server connection string for cleanup.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not capture SQL connection string: {ex.Message}");
            // Non-fatal - tests can still run, just won't have cleanup
        }
    }

    /// <summary>
    ///     Drops all test databases to ensure a fresh state for the next test run.
    ///     This provides test isolation between runs.
    /// </summary>
    private async Task CleanupDatabasesAsync()
    {
        if (string.IsNullOrEmpty(sqlConnectionString))
        {
            Console.WriteLine("No SQL connection string available. Skipping database cleanup.");
            return;
        }

        Console.WriteLine("Cleaning up test databases...");

        try
        {
            await using var connection = new SqlConnection(sqlConnectionString);
            await connection.OpenAsync();

            foreach (var dbName in DatabaseNames)
            {
                try
                {
                    // First, kill all connections to the database
                    var killConnectionsSql = $@"
                        IF EXISTS (SELECT name FROM sys.databases WHERE name = '{dbName}')
                        BEGIN
                            ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                        END";

                    await using (var killCmd = new SqlCommand(killConnectionsSql, connection))
                    {
                        killCmd.CommandTimeout = 30;
                        await killCmd.ExecuteNonQueryAsync();
                    }

                    // Then drop the database
                    var dropSql = $@"
                        IF EXISTS (SELECT name FROM sys.databases WHERE name = '{dbName}')
                        BEGIN
                            DROP DATABASE [{dbName}];
                        END";

                    await using (var dropCmd = new SqlCommand(dropSql, connection))
                    {
                        dropCmd.CommandTimeout = 30;
                        await dropCmd.ExecuteNonQueryAsync();
                    }

                    Console.WriteLine($"  Dropped database: {dbName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  Warning: Could not drop database {dbName}: {ex.Message}");
                    // Continue with other databases
                }
            }

            Console.WriteLine("Database cleanup completed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Database cleanup failed: {ex.Message}");
            // Non-fatal - don't fail the test run because of cleanup issues
        }
    }
}
