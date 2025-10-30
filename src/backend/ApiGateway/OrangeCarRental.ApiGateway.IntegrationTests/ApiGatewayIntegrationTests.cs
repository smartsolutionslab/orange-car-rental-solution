using Aspire.Hosting;
using Aspire.Hosting.Testing;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace OrangeCarRental.ApiGateway.IntegrationTests;

/// <summary>
/// Integration tests for the API Gateway using Aspire Hosting Testing
/// These tests verify the complete distributed application stack
/// </summary>
public class ApiGatewayIntegrationTests : IAsyncLifetime
{
    private DistributedApplication? _app;
    private HttpClient? _gatewayClient;

    public async Task InitializeAsync()
    {
        // Create distributed application from AppHost for integration testing
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.OrangeCarRental_AppHost>();

        _app = await appHost.BuildAsync();
        await _app.StartAsync();

        // Get HTTP client for API Gateway
        _gatewayClient = _app.CreateHttpClient("api-gateway");
    }

    public async Task DisposeAsync()
    {
        if (_app is not null)
        {
            await _app.DisposeAsync();
        }

        _gatewayClient?.Dispose();
    }

    [Fact]
    public async Task Gateway_HealthEndpoint_ReturnsHealthy()
    {
        // Arrange & Act
        var response = await _gatewayClient!.GetAsync("/health");
        var health = await response.Content.ReadFromJsonAsync<HealthResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        health.Should().NotBeNull();
        health!.Service.Should().Be("API Gateway");
        health.Status.Should().Be("Healthy");
        health.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task Gateway_RoutesToFleetApi_Successfully()
    {
        // Arrange & Act
        var response = await _gatewayClient!.GetAsync("/api/vehicles?pageSize=5");
        var result = await response.Content.ReadFromJsonAsync<VehicleListResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Vehicles.Should().NotBeEmpty();
        result.Vehicles.Should().HaveCountLessOrEqualTo(5);
        result.TotalCount.Should().BeGreaterThan(0);
        result.PageSize.Should().Be(5);
    }

    [Fact]
    public async Task Gateway_FleetApiHealth_ReturnsHealthy()
    {
        // Arrange & Act
        var response = await _gatewayClient!.GetAsync("/api/vehicles/health");
        var health = await response.Content.ReadFromJsonAsync<HealthResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        health.Should().NotBeNull();
        health!.Service.Should().Be("Fleet API");
        health.Status.Should().Be("Healthy");
    }

    [Fact]
    public async Task Gateway_ReservationsApiHealth_ReturnsHealthy()
    {
        // Arrange & Act
        var response = await _gatewayClient!.GetAsync("/api/reservations/health");
        var health = await response.Content.ReadFromJsonAsync<HealthResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        health.Should().NotBeNull();
        health!.Service.Should().Be("Reservations API");
        health.Status.Should().Be("Healthy");
    }

    [Fact]
    public async Task Gateway_VehicleSearch_WithFilters_ReturnsFilteredResults()
    {
        // Arrange & Act
        var response = await _gatewayClient!.GetAsync("/api/vehicles?categoryCode=SUV&pageSize=10");
        var result = await response.Content.ReadFromJsonAsync<VehicleListResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Vehicles.Should().NotBeEmpty();
        result.Vehicles.Should().OnlyContain(v => v.CategoryCode == "SUV");
    }

    [Fact]
    public async Task Gateway_InvalidRoute_Returns404()
    {
        // Arrange & Act
        var response = await _gatewayClient!.GetAsync("/api/invalid-route");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Gateway_Pagination_ReturnsCorrectMetadata()
    {
        // Arrange & Act
        var response = await _gatewayClient!.GetAsync("/api/vehicles?pageNumber=1&pageSize=3");
        var result = await response.Content.ReadFromJsonAsync<VehicleListResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(3);
        result.Vehicles.Should().HaveCount(3);
        result.HasNextPage.Should().BeTrue();
        result.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public async Task Gateway_CreateReservation_ThroughGateway_Succeeds()
    {
        // Arrange
        var createRequest = new
        {
            vehicleId = Guid.NewGuid(),
            customerId = Guid.NewGuid(),
            pickupDate = DateTime.UtcNow.Date.AddDays(7),
            returnDate = DateTime.UtcNow.Date.AddDays(10),
            totalPriceNet = 250.50m
        };

        // Act
        var response = await _gatewayClient!.PostAsJsonAsync("/api/reservations", createRequest);
        var result = await response.Content.ReadFromJsonAsync<ReservationResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
        result.Status.Should().Be("Pending");
        result.TotalPriceNet.Should().Be(createRequest.totalPriceNet);
    }

    [Fact]
    public async Task Gateway_MultipleServices_AreAccessibleSimultaneously()
    {
        // Arrange & Act - Make parallel requests to both services
        var fleetTask = _gatewayClient!.GetAsync("/api/vehicles/health");
        var reservationsTask = _gatewayClient!.GetAsync("/api/reservations/health");

        await Task.WhenAll(fleetTask, reservationsTask);

        var fleetResponse = await fleetTask;
        var reservationsResponse = await reservationsTask;

        // Assert
        fleetResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        reservationsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // Response DTOs for deserialization
    private record HealthResponse(string Service, string Status, DateTime Timestamp);

    private record VehicleListResponse(
        List<VehicleDto> Vehicles,
        int TotalCount,
        int PageNumber,
        int PageSize,
        int TotalPages,
        bool HasPreviousPage,
        bool HasNextPage);

    private record VehicleDto(
        Guid Id,
        string Name,
        string CategoryCode,
        string CategoryName,
        string LocationCode,
        string City,
        int Seats,
        string FuelType,
        string TransmissionType,
        decimal DailyRateNet,
        decimal DailyRateVat,
        decimal DailyRateGross,
        string Currency,
        string Status);

    private record ReservationResponse(
        Guid Id,
        Guid VehicleId,
        Guid CustomerId,
        DateTime PickupDate,
        DateTime ReturnDate,
        decimal TotalPriceNet,
        string Status,
        DateTime CreatedAt);
}
