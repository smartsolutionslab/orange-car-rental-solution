using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SmartSolutionsLab.OrangeCarRental.IntegrationTests.Infrastructure;

/// <summary>
///     Infrastructure tests for API Gateway routing and service health.
///     These tests verify the cross-cutting infrastructure that supports all user stories.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
[Trait("Category", "Integration")]
[Trait("Portal", "Infrastructure")]
public class GatewayAndServicesTests(DistributedApplicationFixture fixture)
{
    private static readonly JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };

    #region Gateway Health

    [Fact]
    public async Task ApiGateway_HealthEndpoint_ReturnsHealthy()
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

    #endregion

    #region Service Routing through Gateway

    [Fact]
    public async Task Gateway_RoutesToFleetApi_Successfully()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync("/api/vehicles?pageSize=1");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Gateway_RoutesToLocationsApi_Successfully()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync("/api/locations");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Gateway_RoutesToPricingApi_Successfully()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var request = new
        {
            categoryCode = "KOMPAKT",
            pickupDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(7),
            returnDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10)
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/pricing/calculate", request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Gateway_RoutesToReservationsApi_Successfully()
    {
        // Arrange - Reservation search requires call center or admin authentication
        var httpClient = await fixture.CreateCallCenterHttpClientAsync();

        // Act
        var response = await httpClient.GetAsync("/api/reservations/search?pageSize=1");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Gateway_RoutesToCustomersApi_Successfully()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var request = new
        {
            customer = new
            {
                firstName = "Gateway",
                lastName = "Test",
                email = $"gateway.{Guid.NewGuid():N}@test.de",
                phoneNumber = "+49 30 1234567",
                dateOfBirth = new DateOnly(1990, 1, 1)
            },
            address = new
            {
                street = "Teststraße 1",
                city = "Berlin",
                postalCode = "10115",
                country = "Germany"
            },
            driversLicense = new
            {
                licenseNumber = $"G{Guid.NewGuid():N}"[..10],
                licenseIssueCountry = "Germany",
                licenseIssueDate = new DateOnly(2015, 1, 1),
                licenseExpiryDate = new DateOnly(2035, 1, 1)
            }
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/customers", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Gateway_InvalidRoute_Returns404()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync("/api/nonexistent/endpoint");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Individual Service Health

    [Fact]
    public async Task FleetApi_DirectHealthCheck_ReturnsHealthy()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("fleet-api");

        // Act
        var response = await httpClient.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task ReservationsApi_DirectHealthCheck_ReturnsHealthy()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("reservations-api");

        // Act
        var response = await httpClient.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task PricingApi_DirectHealthCheck_ReturnsHealthy()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("pricing-api");

        // Act
        var response = await httpClient.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task CustomersApi_DirectHealthCheck_ReturnsHealthy()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("customers-api");

        // Act
        var response = await httpClient.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    #endregion

    #region All Services Simultaneously

    [Fact]
    public async Task AllServices_AccessibleSimultaneously()
    {
        // Arrange - Use authenticated client for reservation search
        var httpClient = await fixture.CreateCallCenterHttpClientAsync();

        // Act - Call multiple services in parallel
        var vehiclesTask = httpClient.GetAsync("/api/vehicles?pageSize=1");
        var locationsTask = httpClient.GetAsync("/api/locations");
        var reservationsTask = httpClient.GetAsync("/api/reservations/search?pageSize=1");

        var results = await Task.WhenAll(vehiclesTask, locationsTask, reservationsTask);

        // Assert
        Assert.True(results[0].IsSuccessStatusCode);
        Assert.True(results[1].IsSuccessStatusCode);
        Assert.True(results[2].IsSuccessStatusCode);
    }

    #endregion
}
