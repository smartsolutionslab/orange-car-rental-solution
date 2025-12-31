using System.Net.Http.Json;
using System.Text.Json;

namespace SmartSolutionsLab.OrangeCarRental.IntegrationTests.CallCenterPortal;

/// <summary>
///     US-7: List All Bookings
///     As a call center agent, I want to see a list of all reservations
///     so that I can manage bookings and assist customers.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public class US07_ListAllBookingsTests(DistributedApplicationFixture fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    #region AC: Dashboard statistics

    [Fact]
    public async Task SearchReservations_ReturnsReservationList()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync("/api/reservations/search?pageSize=10");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ReservationSearchResult>(JsonOptions);

        Assert.NotNull(result);
        Assert.NotNull(result.Items);
        Assert.True(result.TotalCount >= 0);
    }

    #endregion

    #region AC: Reservations table with all details

    [Fact]
    public async Task SearchReservations_ReturnsRequiredFields()
    {
        // First create a reservation to ensure we have data
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Search for vehicles first
        var vehicleResponse = await httpClient.GetAsync("/api/vehicles?pageSize=1");
        vehicleResponse.EnsureSuccessStatusCode();
        var vehicles = await vehicleResponse.Content.ReadFromJsonAsync<VehicleSearchResult>(JsonOptions);

        if (vehicles?.Items.Count > 0)
        {
            var vehicle = vehicles.Items[0];

            // Create a guest reservation
            var reservationRequest = new
            {
                vehicleId = Guid.Parse(vehicle.Id),
                categoryCode = vehicle.CategoryCode,
                pickupDate = DateTime.UtcNow.Date.AddDays(30),
                returnDate = DateTime.UtcNow.Date.AddDays(33),
                pickupLocationCode = vehicle.LocationCode,
                dropoffLocationCode = vehicle.LocationCode,
                firstName = "Test",
                lastName = "Agent",
                email = $"agent.{Guid.NewGuid():N}@test.de",
                phoneNumber = "+49 89 99999999",
                dateOfBirth = new DateOnly(1985, 1, 1),
                street = "Teststraße 1",
                city = "München",
                postalCode = "80331",
                country = "Germany",
                licenseNumber = $"T{Guid.NewGuid():N}"[..10],
                licenseIssueCountry = "Germany",
                licenseIssueDate = new DateOnly(2010, 1, 1),
                licenseExpiryDate = new DateOnly(2030, 1, 1)
            };

            await httpClient.PostAsJsonAsync("/api/reservations/guest", reservationRequest);
        }

        // Act - Search reservations
        var response = await httpClient.GetAsync("/api/reservations/search?pageSize=10");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ReservationSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        if (result.Items.Count > 0)
        {
            var reservation = result.Items[0];

            // Verify required fields per AC
            Assert.NotEqual(Guid.Empty, reservation.ReservationId);
            Assert.NotEqual(Guid.Empty, reservation.CustomerId);
            Assert.NotEqual(default, reservation.PickupDate);
            Assert.NotEqual(default, reservation.ReturnDate);
            Assert.NotEmpty(reservation.PickupLocationCode);
            Assert.NotEmpty(reservation.DropoffLocationCode);
            Assert.True(reservation.TotalPriceGross > 0);
            Assert.NotEmpty(reservation.Status);
        }
    }

    #endregion

    #region AC: Pagination

    [Fact]
    public async Task SearchReservations_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync("/api/reservations/search?pageNumber=1&pageSize=5");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ReservationSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.True(result.Items.Count <= 5);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(25)]
    [InlineData(50)]
    public async Task SearchReservations_DifferentPageSizes_Work(int pageSize)
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync($"/api/reservations/search?pageSize={pageSize}");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ReservationSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(pageSize, result.PageSize);
        Assert.True(result.Items.Count <= pageSize);
    }

    #endregion

    #region AC: Status filter

    [Theory]
    [InlineData("Pending")]
    [InlineData("Confirmed")]
    [InlineData("Cancelled")]
    public async Task SearchReservations_ByStatus_ReturnsMatchingReservations(string status)
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync($"/api/reservations/search?status={status}&pageSize=10");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ReservationSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        // All returned items should match the status filter
        Assert.All(result.Items, r => Assert.Equal(status, r.Status));
    }

    #endregion

    #region Health check

    [Fact]
    public async Task ReservationsApi_HealthCheck_ReturnsHealthy()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("reservations-api");

        // Act
        var response = await httpClient.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    #endregion

    // Helper classes
    private class ReservationSearchResult
    {
        public List<ReservationDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    private class ReservationDto
    {
        public Guid ReservationId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid VehicleId { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string PickupLocationCode { get; set; } = string.Empty;
        public string DropoffLocationCode { get; set; } = string.Empty;
        public decimal TotalPriceGross { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    private class VehicleSearchResult
    {
        public List<VehicleDto> Items { get; set; } = new();
    }

    private class VehicleDto
    {
        public string Id { get; set; } = string.Empty;
        public string CategoryCode { get; set; } = string.Empty;
        public string LocationCode { get; set; } = string.Empty;
    }
}
