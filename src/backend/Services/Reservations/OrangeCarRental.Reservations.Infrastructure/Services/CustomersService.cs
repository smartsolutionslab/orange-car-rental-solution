using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Http;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Services;

/// <summary>
///     HTTP client implementation for calling the Customers API.
/// </summary>
public sealed class CustomersService(HttpClient httpClient) : ICustomersService
{
    private const string ServiceName = "Customers API";

    public async Task<Guid> RegisterCustomerAsync(
        RegisterCustomerDto request,
        CancellationToken cancellationToken = default)
    {
        // The Customers API expects a nested structure with customer, address, and driversLicense objects
        var requestBody = new
        {
            customer = new
            {
                firstName = request.FirstName,
                lastName = request.LastName,
                email = request.Email,
                phoneNumber = request.PhoneNumber,
                dateOfBirth = request.DateOfBirth
            },
            address = new
            {
                street = request.Street,
                city = request.City,
                postalCode = request.PostalCode,
                country = request.Country
            },
            driversLicense = new
            {
                licenseNumber = request.LicenseNumber,
                licenseIssueCountry = request.LicenseIssueCountry,
                licenseIssueDate = request.LicenseIssueDate,
                licenseExpiryDate = request.LicenseExpiryDate
            }
        };

        var result = await httpClient.PostJsonAsync<object, RegisterCustomerResponse>(
            "/api/customers",
            requestBody,
            ServiceName,
            cancellationToken);

        return result.CustomerIdentifier;
    }

    public async Task<string?> GetCustomerEmailAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        var customer = await httpClient.GetJsonOrDefaultAsync<CustomerResponse>(
            $"/api/customers/{customerId}",
            ServiceName,
            cancellationToken);

        return customer?.Email;
    }

    // Property name must match the API response (CustomerIdentifier, not CustomerId)
    private sealed record RegisterCustomerResponse(Guid CustomerIdentifier);

    private sealed record CustomerResponse(Guid Id, string Email);
}
