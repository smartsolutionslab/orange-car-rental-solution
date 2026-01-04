using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Shouldly;
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
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        health.ShouldNotBeNull();
        health!.Service.ShouldBe("API Gateway");
        health.Status.ShouldBe("Healthy");
        health.Timestamp.ShouldBeInRange(DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow.AddMinutes(1));
    }

    [Fact]
    public async Task Gateway_RoutesToFleetApi_Successfully()
    {
        // Arrange & Act
        var response = await _gatewayClient!.GetAsync("/api/vehicles?pageSize=5");
        var result = await response.Content.ReadFromJsonAsync<VehicleListResponse>();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();
        result!.Vehicles.ShouldNotBeEmpty();
        result.Vehicles.Count.ShouldBeLessThanOrEqualTo(5);
        result.TotalCount.ShouldBeGreaterThan(0);
        result.PageSize.ShouldBe(5);
    }

    [Fact]
    public async Task Gateway_FleetApiHealth_ReturnsHealthy()
    {
        // Arrange & Act
        var response = await _gatewayClient!.GetAsync("/api/vehicles/health");
        var health = await response.Content.ReadFromJsonAsync<HealthResponse>();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        health.ShouldNotBeNull();
        health!.Service.ShouldBe("Fleet API");
        health.Status.ShouldBe("Healthy");
    }

    [Fact]
    public async Task Gateway_ReservationsApiHealth_ReturnsHealthy()
    {
        // Arrange & Act
        var response = await _gatewayClient!.GetAsync("/api/reservations/health");
        var health = await response.Content.ReadFromJsonAsync<HealthResponse>();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        health.ShouldNotBeNull();
        health!.Service.ShouldBe("Reservations API");
        health.Status.ShouldBe("Healthy");
    }

    [Fact]
    public async Task Gateway_VehicleSearch_WithFilters_ReturnsFilteredResults()
    {
        // Arrange & Act
        var response = await _gatewayClient!.GetAsync("/api/vehicles?categoryCode=SUV&pageSize=10");
        var result = await response.Content.ReadFromJsonAsync<VehicleListResponse>();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();
        result!.Vehicles.ShouldNotBeEmpty();
        result.Vehicles.ShouldAllBe(v => v.CategoryCode == "SUV");
    }

    [Fact]
    public async Task Gateway_InvalidRoute_Returns404()
    {
        // Arrange & Act
        var response = await _gatewayClient!.GetAsync("/api/invalid-route");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Gateway_Pagination_ReturnsCorrectMetadata()
    {
        // Arrange & Act
        var response = await _gatewayClient!.GetAsync("/api/vehicles?pageNumber=1&pageSize=3");
        var result = await response.Content.ReadFromJsonAsync<VehicleListResponse>();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();
        result!.PageNumber.ShouldBe(1);
        result.PageSize.ShouldBe(3);
        result.Vehicles.Count.ShouldBe(3);
        result.HasNextPage.ShouldBeTrue();
        result.HasPreviousPage.ShouldBeFalse();
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
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();
        result!.Id.ShouldNotBe(Guid.Empty);
        result.Status.ShouldBe("Pending");
        result.TotalPriceNet.ShouldBe(createRequest.totalPriceNet);
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
        fleetResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        reservationsResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Gateway_CreateGuestReservation_ThroughGateway_Succeeds()
    {
        // Arrange - Create a realistic guest booking request with German market data (nested structure)
        var createGuestRequest = new
        {
            reservation = new
            {
                vehicleId = Guid.NewGuid(),
                categoryCode = "ECAR",
                pickupDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(7),
                returnDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10),
                pickupLocationCode = "MUC",
                dropoffLocationCode = "MUC"
            },
            customer = new
            {
                firstName = "Max",
                lastName = "Mustermann",
                email = $"max.mustermann.{Guid.NewGuid():N}@example.de",
                phoneNumber = "+49 89 12345678",
                dateOfBirth = new DateOnly(1990, 1, 15)
            },
            address = new
            {
                street = "Musterstraße 123",
                city = "München",
                postalCode = "80331",
                country = "Germany"
            },
            driversLicense = new
            {
                licenseNumber = "B12345678",
                licenseIssueCountry = "Germany",
                licenseIssueDate = new DateOnly(2015, 6, 1),
                licenseExpiryDate = new DateOnly(2035, 6, 1)
            }
        };

        // Act - Send request through API Gateway to /api/reservations/guest endpoint
        var response = await _gatewayClient!.PostAsJsonAsync("/api/reservations/guest", createGuestRequest);
        var result = await response.Content.ReadFromJsonAsync<GuestReservationResponse>();

        // Assert - Verify the request was successfully routed and processed
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.ShouldNotBeNull();

        // Verify customer was created
        result!.CustomerId.ShouldNotBe(Guid.Empty);

        // Verify reservation was created
        result.ReservationId.ShouldNotBe(Guid.Empty);

        // Verify pricing was calculated (German VAT: 19%)
        result.TotalPriceNet.ShouldBeGreaterThan(0);
        result.TotalPriceVat.ShouldBeGreaterThan(0);
        result.TotalPriceGross.ShouldBe(result.TotalPriceNet + result.TotalPriceVat);
        result.Currency.ShouldBe("EUR");

        // Verify response location header points to the new reservation
        response.Headers.Location.ShouldNotBeNull();
        response.Headers.Location!.ToString().ShouldContain($"/api/reservations/{result.ReservationId}");
    }

    [Fact]
    public async Task Gateway_CreateGuestReservation_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange - Create a guest booking request with invalid pickup date (in the past, nested structure)
        var invalidGuestRequest = new
        {
            reservation = new
            {
                vehicleId = Guid.NewGuid(),
                categoryCode = "ECAR",
                pickupDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1), // Invalid: past date
                returnDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(3),
                pickupLocationCode = "MUC",
                dropoffLocationCode = "MUC"
            },
            customer = new
            {
                firstName = "Max",
                lastName = "Mustermann",
                email = "max.mustermann@example.de",
                phoneNumber = "+49 89 12345678",
                dateOfBirth = new DateOnly(1990, 1, 15)
            },
            address = new
            {
                street = "Musterstraße 123",
                city = "München",
                postalCode = "80331",
                country = "Germany"
            },
            driversLicense = new
            {
                licenseNumber = "B12345678",
                licenseIssueCountry = "Germany",
                licenseIssueDate = new DateOnly(2015, 6, 1),
                licenseExpiryDate = new DateOnly(2035, 6, 1)
            }
        };

        // Act
        var response = await _gatewayClient!.PostAsJsonAsync("/api/reservations/guest", invalidGuestRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
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
        string Status,
        string? LicensePlate,
        string? Manufacturer,
        string? Model,
        int? Year,
        string? ImageUrl);

    private record ReservationResponse(
        Guid Id,
        Guid VehicleId,
        Guid CustomerId,
        DateTime PickupDate,
        DateTime ReturnDate,
        decimal TotalPriceNet,
        string Status,
        DateTime CreatedAt);

    private record GuestReservationResponse(
        Guid CustomerId,
        Guid ReservationId,
        decimal TotalPriceNet,
        decimal TotalPriceVat,
        decimal TotalPriceGross,
        string Currency);
}
