using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Http;

namespace OrangeCarRental.Shared.HealthChecks;

/// <summary>
/// Health check for Keycloak authentication service
/// </summary>
public class KeycloakHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;
    private readonly string _keycloakUrl;

    public KeycloakHealthCheck(IHttpClientFactory httpClientFactory, string keycloakUrl)
    {
        _httpClient = httpClientFactory.CreateClient();
        _keycloakUrl = keycloakUrl;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var healthUrl = $"{_keycloakUrl}/health/ready";
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var response = await _httpClient.GetAsync(healthUrl, cancellationToken);
            stopwatch.Stop();

            var responseTime = stopwatch.ElapsedMilliseconds;

            if (response.IsSuccessStatusCode)
            {
                var status = responseTime switch
                {
                    < 200 => HealthStatus.Healthy,
                    < 1000 => HealthStatus.Degraded,
                    _ => HealthStatus.Unhealthy
                };

                return new HealthCheckResult(
                    status,
                    description: $"Keycloak is reachable (response: {responseTime}ms)",
                    data: new Dictionary<string, object>
                    {
                        ["responseTimeMs"] = responseTime,
                        ["statusCode"] = (int)response.StatusCode,
                        ["keycloakUrl"] = _keycloakUrl
                    });
            }

            return HealthCheckResult.Unhealthy(
                $"Keycloak returned status code {response.StatusCode}",
                data: new Dictionary<string, object>
                {
                    ["statusCode"] = (int)response.StatusCode,
                    ["responseTimeMs"] = responseTime,
                    ["keycloakUrl"] = _keycloakUrl
                });
        }
        catch (HttpRequestException ex)
        {
            return HealthCheckResult.Unhealthy(
                "Keycloak is not reachable",
                exception: ex,
                data: new Dictionary<string, object>
                {
                    ["error"] = ex.Message,
                    ["keycloakUrl"] = _keycloakUrl
                });
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Keycloak health check failed",
                exception: ex,
                data: new Dictionary<string, object>
                {
                    ["error"] = ex.Message,
                    ["keycloakUrl"] = _keycloakUrl
                });
        }
    }
}
