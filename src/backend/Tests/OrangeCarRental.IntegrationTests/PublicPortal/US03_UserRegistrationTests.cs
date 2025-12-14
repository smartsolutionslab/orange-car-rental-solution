using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SmartSolutionsLab.OrangeCarRental.IntegrationTests.PublicPortal;

/// <summary>
///     US-3: User Registration and Authentication
///     As a customer, I want to create an account and log in
///     so that I can save my details and view my booking history.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public class US03_UserRegistrationTests(DistributedApplicationFixture fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    #region AC: Registration with required fields

    [Fact]
    public async Task RegisterCustomer_WithValidGermanData_CreatesCustomer()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var uniqueEmail = $"register.{Guid.NewGuid():N}@example.de";

        var request = new
        {
            customer = new
            {
                firstName = "Max",
                lastName = "Mustermann",
                email = uniqueEmail,
                phoneNumber = "+49 30 12345678",
                dateOfBirth = new DateOnly(1985, 6, 15)
            },
            address = new
            {
                street = "Alexanderplatz 1",
                city = "Berlin",
                postalCode = "10178",
                country = "Germany"
            },
            driversLicense = new
            {
                licenseNumber = $"B{Guid.NewGuid():N}".Substring(0, 10),
                licenseIssueCountry = "Germany",
                licenseIssueDate = new DateOnly(2010, 3, 20),
                licenseExpiryDate = new DateOnly(2030, 3, 19)
            }
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/customers", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<RegisterCustomerResult>(JsonOptions);
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.CustomerIdentifier);
        Assert.Equal(uniqueEmail, result.Email);

        // Verify location header
        Assert.NotNull(response.Headers.Location);
        Assert.Contains($"/api/customers/{result.CustomerIdentifier}", response.Headers.Location.ToString());
    }

    #endregion

    #region AC: Email must be unique

    [Fact]
    public async Task RegisterCustomer_DuplicateEmail_ReturnsConflict()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var sharedEmail = $"duplicate.{Guid.NewGuid():N}@example.de";

        var firstRequest = new
        {
            customer = new
            {
                firstName = "First",
                lastName = "User",
                email = sharedEmail,
                phoneNumber = "+49 89 1111111",
                dateOfBirth = new DateOnly(1990, 1, 1)
            },
            address = new
            {
                street = "Erste Straße 1",
                city = "München",
                postalCode = "80331",
                country = "Germany"
            },
            driversLicense = new
            {
                licenseNumber = $"F{Guid.NewGuid():N}".Substring(0, 10),
                licenseIssueCountry = "Germany",
                licenseIssueDate = new DateOnly(2015, 5, 10),
                licenseExpiryDate = new DateOnly(2035, 5, 9)
            }
        };

        // First registration
        var firstResponse = await httpClient.PostAsJsonAsync("/api/customers", firstRequest);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        // Second request with same email
        var secondRequest = new
        {
            customer = new
            {
                firstName = "Second",
                lastName = "User",
                email = sharedEmail, // Same email
                phoneNumber = "+49 40 2222222",
                dateOfBirth = new DateOnly(1988, 12, 25)
            },
            address = new
            {
                street = "Zweite Straße 2",
                city = "Hamburg",
                postalCode = "20359",
                country = "Germany"
            },
            driversLicense = new
            {
                licenseNumber = $"S{Guid.NewGuid():N}".Substring(0, 10),
                licenseIssueCountry = "Germany",
                licenseIssueDate = new DateOnly(2012, 8, 15),
                licenseExpiryDate = new DateOnly(2032, 8, 14)
            }
        };

        // Act
        var secondResponse = await httpClient.PostAsJsonAsync("/api/customers", secondRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
    }

    #endregion

    #region AC: Age validation (must be 18+)

    [Fact]
    public async Task RegisterCustomer_Under18YearsOld_ReturnsBadRequest()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var request = new
        {
            customer = new
            {
                firstName = "Jung",
                lastName = "Person",
                email = $"jung.{Guid.NewGuid():N}@example.de",
                phoneNumber = "+49 30 3333333",
                dateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-17)) // 17 years old
            },
            address = new
            {
                street = "Jugendstraße 1",
                city = "Berlin",
                postalCode = "10115",
                country = "Germany"
            },
            driversLicense = new
            {
                licenseNumber = $"J{Guid.NewGuid():N}".Substring(0, 10),
                licenseIssueCountry = "Germany",
                licenseIssueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-30)),
                licenseExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddYears(10))
            }
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/customers", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region AC: Driver's license validation

    [Fact]
    public async Task RegisterCustomer_ExpiredLicense_ReturnsBadRequest()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var request = new
        {
            customer = new
            {
                firstName = "Expired",
                lastName = "License",
                email = $"expired.{Guid.NewGuid():N}@example.de",
                phoneNumber = "+49 30 4444444",
                dateOfBirth = new DateOnly(1980, 5, 15)
            },
            address = new
            {
                street = "Abgelaufenweg 1",
                city = "Berlin",
                postalCode = "10117",
                country = "Germany"
            },
            driversLicense = new
            {
                licenseNumber = $"E{Guid.NewGuid():N}".Substring(0, 10),
                licenseIssueCountry = "Germany",
                licenseIssueDate = new DateOnly(2005, 1, 1),
                licenseExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)) // Expired
            }
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/customers", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region AC: Email format validation

    [Fact]
    public async Task RegisterCustomer_InvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var request = new
        {
            customer = new
            {
                firstName = "Invalid",
                lastName = "Email",
                email = "not-an-email", // Invalid format
                phoneNumber = "+49 30 5555555",
                dateOfBirth = new DateOnly(1990, 1, 1)
            },
            address = new
            {
                street = "Fehlerstraße 1",
                city = "Berlin",
                postalCode = "10119",
                country = "Germany"
            },
            driversLicense = new
            {
                licenseNumber = "I123456789",
                licenseIssueCountry = "Germany",
                licenseIssueDate = new DateOnly(2015, 5, 10),
                licenseExpiryDate = new DateOnly(2035, 5, 9)
            }
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/customers", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Health check

    [Fact]
    public async Task CustomersApi_HealthCheck_ReturnsHealthy()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("customers-api");

        // Act
        var response = await httpClient.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Healthy", content);
    }

    #endregion

    // Helper class
    private class RegisterCustomerResult
    {
        public Guid CustomerIdentifier { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}
