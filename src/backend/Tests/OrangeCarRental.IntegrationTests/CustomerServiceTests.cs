using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SmartSolutionsLab.OrangeCarRental.IntegrationTests;

/// <summary>
///     Integration tests for Customer API endpoints
///     Tests customer registration (public) and customer service health
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public class CustomerServiceTests(DistributedApplicationFixture fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

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

    [Fact]
    public async Task RegisterCustomer_ValidGermanCustomer_CreatesCustomer()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var uniqueEmail = $"test.{Guid.NewGuid():N}@example.de";
        var request = new
        {
            customer = new
            {
                firstName = "Hans",
                lastName = "Müller",
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

    [Fact]
    public async Task RegisterCustomer_DuplicateEmail_ReturnsConflict()
    {
        // Arrange
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var sharedEmail = $"duplicate.{Guid.NewGuid():N}@example.de";

        var request = new
        {
            customer = new
            {
                firstName = "Max",
                lastName = "Mustermann",
                email = sharedEmail,
                phoneNumber = "+49 89 9876543",
                dateOfBirth = new DateOnly(1990, 1, 1)
            },
            address = new
            {
                street = "Marienplatz 5",
                city = "München",
                postalCode = "80331",
                country = "Germany"
            },
            driversLicense = new
            {
                licenseNumber = $"M{Guid.NewGuid():N}".Substring(0, 10),
                licenseIssueCountry = "Germany",
                licenseIssueDate = new DateOnly(2015, 5, 10),
                licenseExpiryDate = new DateOnly(2035, 5, 9)
            }
        };

        // First registration should succeed
        var firstResponse = await httpClient.PostAsJsonAsync("/api/customers", request);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        // Create second request with same email but different license
        var secondRequest = new
        {
            customer = new
            {
                firstName = "Maria",
                lastName = "Schmidt",
                email = sharedEmail, // Same email
                phoneNumber = "+49 40 5555555",
                dateOfBirth = new DateOnly(1988, 12, 25)
            },
            address = new
            {
                street = "Reeperbahn 1",
                city = "Hamburg",
                postalCode = "20359",
                country = "Germany"
            },
            driversLicense = new
            {
                licenseNumber = $"H{Guid.NewGuid():N}".Substring(0, 10),
                licenseIssueCountry = "Germany",
                licenseIssueDate = new DateOnly(2012, 8, 15),
                licenseExpiryDate = new DateOnly(2032, 8, 14)
            }
        };

        // Act - Second registration with same email
        var secondResponse = await httpClient.PostAsJsonAsync("/api/customers", secondRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
    }

    [Fact]
    public async Task RegisterCustomer_UnderageCustomer_ReturnsBadRequest()
    {
        // Arrange - Customer under 18 years old
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var request = new
        {
            customer = new
            {
                firstName = "Jung",
                lastName = "Person",
                email = $"jung.{Guid.NewGuid():N}@example.de",
                phoneNumber = "+49 30 1111111",
                dateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-17)) // 17 years old
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

    [Fact]
    public async Task RegisterCustomer_ExpiredLicense_ReturnsBadRequest()
    {
        // Arrange - Customer with expired driver's license
        var httpClient = fixture.CreateHttpClient("api-gateway");
        var request = new
        {
            customer = new
            {
                firstName = "Expired",
                lastName = "License",
                email = $"expired.{Guid.NewGuid():N}@example.de",
                phoneNumber = "+49 30 2222222",
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
                licenseExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)) // Expired yesterday
            }
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/customers", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

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
                email = "not-an-email",
                phoneNumber = "+49 30 3333333",
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

    [Fact]
    public async Task CustomersApi_DirectAccess_HealthCheck()
    {
        // Arrange - Access Customers API directly (not through gateway)
        var httpClient = fixture.CreateHttpClient("customers-api");

        // Act
        var response = await httpClient.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    // Helper class for deserialization
    private class RegisterCustomerResult
    {
        public Guid CustomerIdentifier { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}
