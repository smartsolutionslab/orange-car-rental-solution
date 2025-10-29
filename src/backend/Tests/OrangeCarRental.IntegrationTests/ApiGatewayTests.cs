using Aspire.Hosting.Testing;

namespace OrangeCarRental.IntegrationTests;

/// <summary>
/// Integration tests for the API Gateway
/// Tests the entire Aspire setup including SQL Server, Fleet API, Reservations API and the Gateway
/// </summary>
public class ApiGatewayTests : IClassFixture<DistributedApplicationFixture>
{
    private readonly DistributedApplicationFixture _fixture;

    public ApiGatewayTests(DistributedApplicationFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ApiGateway_HealthCheck_ReturnsHealthy()
    {
        // Arrange
        var httpClient = _fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Healthy", content);
    }

    [Fact]
    public async Task ApiGateway_RoutesToFleetApi_SearchVehicles()
    {
        // Arrange
        var httpClient = _fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync("/api/vehicles");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        // Should return vehicle search results
        Assert.Contains("vehicles", content);
        Assert.Contains("totalCount", content);
    }

    [Fact]
    public async Task ApiGateway_RoutesToReservationsApi_HealthCheck()
    {
        // Arrange
        var httpClient = _fixture.CreateHttpClient("api-gateway");

        // Act - The gateway should route /api/reservations to the Reservations API
        // For now, we'll test that the route exists by calling a non-existent reservation
        var response = await httpClient.GetAsync("/api/reservations/00000000-0000-0000-0000-000000000001");

        // Assert - Should get 404 (not found) not 502 (bad gateway) or 404 with gateway error
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("not found", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task FleetApi_DirectAccess_SearchVehicles()
    {
        // Arrange - Access Fleet API directly (not through gateway)
        var httpClient = _fixture.CreateHttpClient("fleet-api");

        // Act
        var response = await httpClient.GetAsync("/api/vehicles");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("vehicles", content);
        Assert.Contains("totalCount", content);
        Assert.Contains("pageNumber", content);
    }

    [Fact]
    public async Task ReservationsApi_DirectAccess_GetNonExistentReservation_Returns404()
    {
        // Arrange - Access Reservations API directly
        var httpClient = _fixture.CreateHttpClient("reservations-api");

        // Act
        var response = await httpClient.GetAsync("/api/reservations/00000000-0000-0000-0000-000000000001");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
