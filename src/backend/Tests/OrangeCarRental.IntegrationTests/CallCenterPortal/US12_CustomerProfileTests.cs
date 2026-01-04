using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SmartSolutionsLab.OrangeCarRental.IntegrationTests.CallCenterPortal;

/// <summary>
///     US-12: Customer Profile Management
///     As a call center agent, I want to view and update customer profiles
///     so that I can help customers maintain accurate information and manage their accounts.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
[Trait("Category", "Integration")]
[Trait("Portal", "CallCenter")]
public class US12_CustomerProfileTests(DistributedApplicationFixture fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    #region Setup: Create test customer

    private async Task<HttpClient> GetAuthenticatedClientAsync()
    {
        return await fixture.CreateCallCenterHttpClientAsync();
    }

    private async Task<(Guid customerId, string email)> CreateTestCustomerAsync(HttpClient httpClient)
    {
        var uniqueEmail = $"profile.{Guid.NewGuid():N}@example.de";
        var request = new
        {
            customer = new
            {
                firstName = "Profile",
                lastName = "TestKunde",
                email = uniqueEmail,
                phoneNumber = "+49 30 12345678",
                dateOfBirth = new DateOnly(1985, 6, 15)
            },
            address = new
            {
                street = "Profilstraße 123",
                city = "Berlin",
                postalCode = "10115",
                country = "Germany"
            },
            driversLicense = new
            {
                licenseNumber = $"P{Guid.NewGuid():N}"[..10],
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

    #region AC: Update customer profile

    [Fact]
    public async Task UpdateProfile_ValidData_ReturnsSuccess()
    {
        // Arrange
        var httpClient = await GetAuthenticatedClientAsync();
        var (customerId, _) = await CreateTestCustomerAsync(httpClient);

        var updateRequest = new
        {
            profile = new
            {
                firstName = "UpdatedFirst",
                lastName = "UpdatedLast",
                phoneNumber = "+49 89 98765432"
            },
            address = new
            {
                street = "Neue Straße 456",
                city = "Munich",
                postalCode = "80331",
                country = "Germany"
            }
        };

        // Act
        var response = await httpClient.PutAsJsonAsync($"/api/customers/{customerId}/profile", updateRequest);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task UpdateProfile_VerifyChangesApplied()
    {
        // Arrange
        var httpClient = await GetAuthenticatedClientAsync();
        var (customerId, _) = await CreateTestCustomerAsync(httpClient);

        var updateRequest = new
        {
            profile = new
            {
                firstName = "Verified",
                lastName = "Changes",
                phoneNumber = "+49 40 55555555"
            },
            address = new
            {
                street = "Verifikationsstraße 789",
                city = "Hamburg",
                postalCode = "20095",
                country = "Germany"
            }
        };

        // Act - Update profile
        await httpClient.PutAsJsonAsync($"/api/customers/{customerId}/profile", updateRequest);

        // Get updated profile
        var getResponse = await httpClient.GetAsync($"/api/customers/{customerId}");
        getResponse.EnsureSuccessStatusCode();
        var customer = await getResponse.Content.ReadFromJsonAsync<CustomerDetailDto>(JsonOptions);

        // Assert
        Assert.NotNull(customer);
        Assert.Equal("Verified", customer.FirstName);
        Assert.Equal("Changes", customer.LastName);
        Assert.Contains("Verifikationsstraße", customer.Street);
        Assert.Equal("Hamburg", customer.City);
    }

    [Fact]
    public async Task UpdateProfile_InvalidCustomerId_ReturnsNotFound()
    {
        // Arrange
        var httpClient = await GetAuthenticatedClientAsync();
        var invalidId = Guid.NewGuid();

        var updateRequest = new
        {
            profile = new
            {
                firstName = "Test",
                lastName = "Test",
                phoneNumber = "+49 30 1234567"
            },
            address = new
            {
                street = "Test 1",
                city = "Berlin",
                postalCode = "10115",
                country = "Germany"
            }
        };

        // Act
        var response = await httpClient.PutAsJsonAsync($"/api/customers/{invalidId}/profile", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region AC: Update driver's license

    [Fact]
    public async Task UpdateLicense_ValidData_ReturnsSuccess()
    {
        // Arrange
        var httpClient = await GetAuthenticatedClientAsync();
        var (customerId, _) = await CreateTestCustomerAsync(httpClient);

        var licenseUpdate = new
        {
            licenseNumber = $"N{Guid.NewGuid():N}"[..10],
            issueCountry = "Germany",
            issueDate = new DateOnly(2020, 6, 15),
            expiryDate = new DateOnly(2035, 6, 15)
        };

        // Act
        var response = await httpClient.PutAsJsonAsync($"/api/customers/{customerId}/license", licenseUpdate);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task UpdateLicense_ExpiringSoon_ReturnsBadRequest()
    {
        // Arrange
        var httpClient = await GetAuthenticatedClientAsync();
        var (customerId, _) = await CreateTestCustomerAsync(httpClient);

        // License expiring in less than 30 days should fail
        var licenseUpdate = new
        {
            licenseNumber = $"E{Guid.NewGuid():N}"[..10],
            issueCountry = "Germany",
            issueDate = new DateOnly(2015, 1, 1),
            expiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(15))
        };

        // Act
        var response = await httpClient.PutAsJsonAsync($"/api/customers/{customerId}/license", licenseUpdate);

        // Assert - License must be valid for at least 30 days
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateLicense_InvalidCustomerId_ReturnsNotFound()
    {
        // Arrange
        var httpClient = await GetAuthenticatedClientAsync();
        var invalidId = Guid.NewGuid();

        var licenseUpdate = new
        {
            licenseNumber = "TEST12345",
            issueCountry = "Germany",
            issueDate = new DateOnly(2020, 1, 1),
            expiryDate = new DateOnly(2035, 1, 1)
        };

        // Act
        var response = await httpClient.PutAsJsonAsync($"/api/customers/{invalidId}/license", licenseUpdate);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region AC: Change customer status

    [Fact]
    public async Task ChangeStatus_ToSuspended_ReturnsSuccess()
    {
        // Arrange
        var httpClient = await GetAuthenticatedClientAsync();
        var (customerId, _) = await CreateTestCustomerAsync(httpClient);

        var statusChange = new
        {
            newStatus = "Suspended",
            reason = "Payment issues - pending resolution"
        };

        // Act
        var response = await httpClient.PutAsJsonAsync($"/api/customers/{customerId}/status", statusChange);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task ChangeStatus_ToBlocked_ReturnsSuccess()
    {
        // Arrange
        var httpClient = await GetAuthenticatedClientAsync();
        var (customerId, _) = await CreateTestCustomerAsync(httpClient);

        var statusChange = new
        {
            newStatus = "Blocked",
            reason = "Fraudulent activity detected"
        };

        // Act
        var response = await httpClient.PutAsJsonAsync($"/api/customers/{customerId}/status", statusChange);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task ChangeStatus_ReactivateFromSuspended_ReturnsSuccess()
    {
        // Arrange
        var httpClient = await GetAuthenticatedClientAsync();
        var (customerId, _) = await CreateTestCustomerAsync(httpClient);

        // First suspend the customer
        var suspendRequest = new
        {
            newStatus = "Suspended",
            reason = "Temporary suspension for verification"
        };
        await httpClient.PutAsJsonAsync($"/api/customers/{customerId}/status", suspendRequest);

        // Then reactivate
        var reactivateRequest = new
        {
            newStatus = "Active",
            reason = "Verification complete - reactivating account"
        };

        // Act
        var response = await httpClient.PutAsJsonAsync($"/api/customers/{customerId}/status", reactivateRequest);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task ChangeStatus_InvalidCustomerId_ReturnsNotFound()
    {
        // Arrange
        var httpClient = await GetAuthenticatedClientAsync();
        var invalidId = Guid.NewGuid();

        var statusChange = new
        {
            newStatus = "Suspended",
            reason = "Test suspension"
        };

        // Act
        var response = await httpClient.PutAsJsonAsync($"/api/customers/{invalidId}/status", statusChange);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region AC: Customer profile view

    [Fact]
    public async Task GetProfile_ReturnsAllRequiredFields()
    {
        // Arrange
        var httpClient = await GetAuthenticatedClientAsync();
        var (customerId, email) = await CreateTestCustomerAsync(httpClient);

        // Act
        var response = await httpClient.GetAsync($"/api/customers/{customerId}");
        response.EnsureSuccessStatusCode();

        var customer = await response.Content.ReadFromJsonAsync<CustomerDetailDto>(JsonOptions);

        // Assert - All required profile fields present
        Assert.NotNull(customer);
        Assert.Equal(customerId, customer.Id);
        Assert.Equal(email, customer.Email);
        Assert.NotEmpty(customer.FirstName);
        Assert.NotEmpty(customer.LastName);
        Assert.NotEmpty(customer.PhoneNumber);
        Assert.NotEqual(default, customer.DateOfBirth);
        Assert.NotEmpty(customer.Street);
        Assert.NotEmpty(customer.City);
        Assert.NotEmpty(customer.PostalCode);
        Assert.NotEmpty(customer.Country);
        Assert.NotEmpty(customer.LicenseNumber);
        Assert.NotEmpty(customer.LicenseIssueCountry);
    }

    [Fact]
    public async Task GetProfile_ByEmail_ReturnsMatchingCustomer()
    {
        // Arrange
        var httpClient = await GetAuthenticatedClientAsync();
        var (_, email) = await CreateTestCustomerAsync(httpClient);

        // Act
        var response = await httpClient.GetAsync($"/api/customers/by-email/{Uri.EscapeDataString(email)}");
        response.EnsureSuccessStatusCode();

        var customer = await response.Content.ReadFromJsonAsync<CustomerDetailDto>(JsonOptions);

        // Assert
        Assert.NotNull(customer);
        Assert.Equal(email, customer.Email);
    }

    #endregion

    // Helper classes
    private class CustomerRegistrationResult
    {
        public Guid CustomerIdentifier { get; set; }
        public string Email { get; set; } = string.Empty;
    }

    private class CustomerDetailDto
    {
        public Guid Id { get; set; }
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
        public string Status { get; set; } = string.Empty;
    }
}
