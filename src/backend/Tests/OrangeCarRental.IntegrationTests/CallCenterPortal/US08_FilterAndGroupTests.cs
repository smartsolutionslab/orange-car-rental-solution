using System.Net.Http.Json;
using System.Text.Json;

namespace SmartSolutionsLab.OrangeCarRental.IntegrationTests.CallCenterPortal;

/// <summary>
///     US-08: Filter and Group Bookings
///     As a call center agent, I want to filter and group reservations by various criteria
///     so that I can quickly find specific bookings or analyze patterns.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public class US08_FilterAndGroupTests(DistributedApplicationFixture fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    #region AC: Status filter

    [Theory]
    [InlineData("Pending")]
    [InlineData("Confirmed")]
    [InlineData("Cancelled")]
    public async Task FilterByStatus_ReturnsMatchingReservations(string status)
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync($"/api/reservations/search?status={status}&pageSize=20");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ReservationSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.All(result.Items, r => Assert.Equal(status, r.Status));
    }

    #endregion

    #region AC: Date range filter

    [Fact]
    public async Task FilterByDateRange_ReturnsReservationsInRange()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var fromDate = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        var toDate = DateTime.UtcNow.Date.AddDays(30).ToString("yyyy-MM-dd");

        // Act
        var response = await httpClient.GetAsync(
            $"/api/reservations/search?pickupDateFrom={fromDate}&pickupDateTo={toDate}&pageSize=20");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ReservationSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        foreach (var reservation in result.Items)
        {
            Assert.True(reservation.PickupDate >= DateTime.UtcNow.Date);
            Assert.True(reservation.PickupDate <= DateTime.UtcNow.Date.AddDays(30));
        }
    }

    [Fact]
    public async Task FilterByPickupDateFrom_ReturnsOnlyFutureReservations()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var futureDate = DateTime.UtcNow.Date.AddDays(7).ToString("yyyy-MM-dd");

        // Act
        var response = await httpClient.GetAsync(
            $"/api/reservations/search?pickupDateFrom={futureDate}&pageSize=20");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ReservationSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.All(result.Items, r =>
            Assert.True(r.PickupDate >= DateTime.UtcNow.Date.AddDays(7)));
    }

    #endregion

    #region AC: Location filter

    [Fact]
    public async Task FilterByLocation_ReturnsReservationsAtLocation()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var locationCode = "MUC-FLG";

        // Act
        var response = await httpClient.GetAsync(
            $"/api/reservations/search?location={locationCode}&pageSize=20");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ReservationSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.All(result.Items, r =>
            Assert.True(r.PickupLocationCode == locationCode || r.DropoffLocationCode == locationCode));
    }

    #endregion

    #region AC: Price range filter

    [Fact]
    public async Task FilterByPriceRange_ReturnsReservationsInPriceRange()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var minPrice = 100m;
        var maxPrice = 500m;

        // Act
        var response = await httpClient.GetAsync(
            $"/api/reservations/search?minPrice={minPrice}&maxPrice={maxPrice}&pageSize=20");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ReservationSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.All(result.Items, r =>
        {
            Assert.True(r.TotalPriceGross >= minPrice);
            Assert.True(r.TotalPriceGross <= maxPrice);
        });
    }

    #endregion

    #region AC: Sorting

    [Fact]
    public async Task SortByPickupDateAscending_ReturnsOrderedResults()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync(
            "/api/reservations/search?sortBy=PickupDate&sortOrder=asc&pageSize=20");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ReservationSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        if (result.Items.Count > 1)
        {
            for (int i = 1; i < result.Items.Count; i++)
            {
                Assert.True(result.Items[i].PickupDate >= result.Items[i - 1].PickupDate);
            }
        }
    }

    [Fact]
    public async Task SortByPickupDateDescending_ReturnsOrderedResults()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync(
            "/api/reservations/search?sortBy=PickupDate&sortOrder=desc&pageSize=20");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ReservationSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        if (result.Items.Count > 1)
        {
            for (int i = 1; i < result.Items.Count; i++)
            {
                Assert.True(result.Items[i].PickupDate <= result.Items[i - 1].PickupDate);
            }
        }
    }

    [Fact]
    public async Task SortByPrice_ReturnsOrderedResults()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync(
            "/api/reservations/search?sortBy=Price&sortOrder=asc&pageSize=20");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ReservationSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        if (result.Items.Count > 1)
        {
            for (int i = 1; i < result.Items.Count; i++)
            {
                Assert.True(result.Items[i].TotalPriceGross >= result.Items[i - 1].TotalPriceGross);
            }
        }
    }

    #endregion

    #region AC: Multiple filters combined

    [Fact]
    public async Task CombineFilters_ReturnsMatchingResults()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var fromDate = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        var toDate = DateTime.UtcNow.Date.AddDays(60).ToString("yyyy-MM-dd");

        // Act
        var response = await httpClient.GetAsync(
            $"/api/reservations/search?status=Pending&pickupDateFrom={fromDate}&pickupDateTo={toDate}&sortBy=PickupDate&sortOrder=asc&pageSize=20");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ReservationSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.All(result.Items, r =>
        {
            Assert.Equal("Pending", r.Status);
            Assert.True(r.PickupDate >= DateTime.UtcNow.Date);
            Assert.True(r.PickupDate <= DateTime.UtcNow.Date.AddDays(60));
        });
    }

    #endregion

    #region AC: Result count display

    [Fact]
    public async Task Search_ReturnsTotalCount()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync("/api/reservations/search?pageSize=5");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ReservationSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.TotalCount >= 0);
        Assert.True(result.Items.Count <= result.TotalCount);
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
        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string PickupLocationCode { get; set; } = string.Empty;
        public string DropoffLocationCode { get; set; } = string.Empty;
        public decimal TotalPriceGross { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
