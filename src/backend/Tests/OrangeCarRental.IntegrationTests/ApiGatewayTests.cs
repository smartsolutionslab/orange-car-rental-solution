using System.Net;

namespace SmartSolutionsLab.OrangeCarRental.IntegrationTests;

/// <summary>
///     Integration tests for the API Gateway
///     Tests the entire Aspire setup including SQL Server, Fleet API, Reservations API and the Gateway
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public class ApiGatewayTests(DistributedApplicationFixture fixture)
{
    [Fact]
    public async Task ApiGateway_HealthCheck_ReturnsHealthy()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

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
        var httpClient = fixture.CreateHttpClient("api-gateway");

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
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act - The gateway should route /api/reservations to the Reservations API
        // Since this endpoint requires authentication, we expect 401 Unauthorized
        var response = await httpClient.GetAsync("/api/reservations/00000000-0000-0000-0000-000000000001");

        // Assert - Should get 401 (unauthorized) because the endpoint requires authentication
        // This proves the route exists and is properly forwarded (not 502 bad gateway)
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task FleetApi_DirectAccess_SearchVehicles()
    {
        // Arrange - Access Fleet API directly (not through gateway)
        var httpClient = fixture.CreateHttpClient("fleet-api");

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
    public async Task ReservationsApi_DirectAccess_GetReservation_Returns401_WhenNotAuthenticated()
    {
        // Arrange - Access Reservations API directly
        var httpClient = fixture.CreateHttpClient("reservations-api");

        // Act - This endpoint requires authentication
        var response = await httpClient.GetAsync("/api/reservations/00000000-0000-0000-0000-000000000001");

        // Assert - Should get 401 (unauthorized) because the endpoint requires authentication
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
