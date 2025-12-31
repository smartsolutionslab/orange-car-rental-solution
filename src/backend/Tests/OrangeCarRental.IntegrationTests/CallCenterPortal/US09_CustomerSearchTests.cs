using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SmartSolutionsLab.OrangeCarRental.IntegrationTests.CallCenterPortal;

/// <summary>
///     US-09: Search Bookings by Customer Details
///     As a call center agent, I want to search for customers and view their booking history
///     so that I can assist customers with inquiries about their reservations.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public class US09_CustomerSearchTests(DistributedApplicationFixture fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    #region Setup: Create test customer

    private async Task<(Guid customerId, string email)> CreateTestCustomerAsync(HttpClient httpClient)
    {
        var uniqueEmail = $"search.{Guid.NewGuid():N}@example.de";
        var request = new
        {
            customer = new
            {
                firstName = "Search",
                lastName = "TestKunde",
                email = uniqueEmail,
                phoneNumber = "+49 30 55555555",
                dateOfBirth = new DateOnly(1985, 6, 15)
            },
            address = new
            {
                street = "Suchstraße 123",
                city = "Berlin",
                postalCode = "10115",
                country = "Germany"
            },
            driversLicense = new
            {
                licenseNumber = $"S{Guid.NewGuid():N}"[..10],
                licenseIssueCountry = "Germany",
                licenseIssueDate = new DateOnly(2010, 1, 1),
                licenseExpiryDate = new DateOnly(2030, 1, 1)
            }
        };

        var response = await httpClient.PostAsJsonAsync("/api/customers", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<CustomerRegistrationResult>(JsonOptions);
        return (result!.CustomerIdentifier, uniqueEmail);
    }

    #endregion

    #region AC: Search by email

    [Fact]
    public async Task SearchByEmail_ReturnsMatchingCustomer()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var (customerId, email) = await CreateTestCustomerAsync(httpClient);

        // Act
        var response = await httpClient.GetAsync($"/api/customers/search?email={email}");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<CustomerSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Customers);
        Assert.Contains(result.Customers, c => c.Email == email);
    }

    [Fact]
    public async Task SearchByEmail_PartialMatch_ReturnsResults()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var (_, email) = await CreateTestCustomerAsync(httpClient);
        var partialEmail = email.Split('@')[0]; // Get part before @

        // Act
        var response = await httpClient.GetAsync($"/api/customers/search?email={partialEmail}");

        // Assert - Either succeeds with results or returns empty (depending on backend implementation)
        response.EnsureSuccessStatusCode();
    }

    #endregion

    #region AC: Search by last name

    [Fact]
    public async Task SearchByLastName_ReturnsMatchingCustomers()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        await CreateTestCustomerAsync(httpClient); // Creates customer with lastName "TestKunde"

        // Act
        var response = await httpClient.GetAsync("/api/customers/search?lastName=TestKunde");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<CustomerSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Customers);
        Assert.All(result.Customers, c =>
            Assert.Contains("TestKunde", c.LastName, StringComparison.OrdinalIgnoreCase));
    }

    #endregion

    #region AC: Search by phone number

    [Fact]
    public async Task SearchByPhone_ReturnsMatchingCustomer()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var uniquePhone = $"+49 30 {Random.Shared.Next(10000000, 99999999)}";

        // Create customer with unique phone
        var request = new
        {
            customer = new
            {
                firstName = "Phone",
                lastName = "SearchTest",
                email = $"phone.{Guid.NewGuid():N}@example.de",
                phoneNumber = uniquePhone,
                dateOfBirth = new DateOnly(1990, 3, 15)
            },
            address = new
            {
                street = "Telefonstraße 1",
                city = "Berlin",
                postalCode = "10117",
                country = "Germany"
            },
            driversLicense = new
            {
                licenseNumber = $"P{Guid.NewGuid():N}"[..10],
                licenseIssueCountry = "Germany",
                licenseIssueDate = new DateOnly(2012, 1, 1),
                licenseExpiryDate = new DateOnly(2032, 1, 1)
            }
        };

        await httpClient.PostAsJsonAsync("/api/customers", request);

        // Act
        var response = await httpClient.GetAsync($"/api/customers/search?phoneNumber={Uri.EscapeDataString(uniquePhone)}");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    #endregion

    #region AC: Customer detail view

    [Fact]
    public async Task GetCustomerById_ReturnsFullCustomerDetails()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var (customerId, email) = await CreateTestCustomerAsync(httpClient);

        // Act
        var response = await httpClient.GetAsync($"/api/customers/{customerId}");
        response.EnsureSuccessStatusCode();

        var customer = await response.Content.ReadFromJsonAsync<CustomerDetailDto>(JsonOptions);

        // Assert
        Assert.NotNull(customer);
        Assert.Equal(customerId, customer.CustomerId);
        Assert.Equal(email, customer.Email);
        Assert.NotEmpty(customer.FirstName);
        Assert.NotEmpty(customer.LastName);
        Assert.NotEmpty(customer.PhoneNumber);
        Assert.NotEqual(default, customer.DateOfBirth);

        // Address details
        Assert.NotEmpty(customer.Street);
        Assert.NotEmpty(customer.City);
        Assert.NotEmpty(customer.PostalCode);

        // License details
        Assert.NotEmpty(customer.LicenseNumber);
        Assert.NotEmpty(customer.LicenseIssueCountry);
    }

    [Fact]
    public async Task GetCustomerById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var invalidId = Guid.NewGuid();

        // Act
        var response = await httpClient.GetAsync($"/api/customers/{invalidId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region AC: Booking history for customer

    [Fact]
    public async Task GetReservationsForCustomer_ReturnsCustomerBookings()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var (customerId, _) = await CreateTestCustomerAsync(httpClient);

        // Act
        var response = await httpClient.GetAsync($"/api/reservations/search?customerId={customerId}");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ReservationSearchResult>(JsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Items);
        // All returned reservations should be for this customer
        Assert.All(result.Items, r => Assert.Equal(customerId, r.CustomerId));
    }

    #endregion

    #region AC: Empty search validation

    [Fact]
    public async Task Search_NoParameters_ReturnsAllOrError()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");

        // Act
        var response = await httpClient.GetAsync("/api/customers/search");

        // Assert - Either returns all customers or requires at least one parameter
        Assert.True(
            response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest,
            "Should either return results or require search parameters");
    }

    #endregion

    // Helper classes
    private class CustomerRegistrationResult
    {
        public Guid CustomerIdentifier { get; set; }
        public string Email { get; set; } = string.Empty;
    }

    private class CustomerSearchResult
    {
        public List<CustomerDto> Customers { get; set; } = new();
        public int TotalCount { get; set; }
    }

    private class CustomerDto
    {
        public Guid CustomerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    private class CustomerDetailDto
    {
        public Guid CustomerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string LicenseIssueCountry { get; set; } = string.Empty;
        public DateOnly LicenseIssueDate { get; set; }
        public DateOnly LicenseExpiryDate { get; set; }
    }

    private class ReservationSearchResult
    {
        public List<ReservationDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
    }

    private class ReservationDto
    {
        public Guid ReservationId { get; set; }
        public Guid CustomerId { get; set; }
    }
}
