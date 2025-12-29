using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Http;
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
        var requestBody = new
        {
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
            request.DateOfBirth,
            request.Street,
            request.City,
            request.PostalCode,
            request.Country,
            request.LicenseNumber,
            request.LicenseIssueCountry,
            request.LicenseIssueDate,
            request.LicenseExpiryDate
        };

        var result = await httpClient.PostJsonAsync<object, RegisterCustomerResponse>(
            "/api/customers",
            requestBody,
            ServiceName,
            cancellationToken);

        return result.CustomerId;
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

    private sealed record RegisterCustomerResponse(Guid CustomerId);

    private sealed record CustomerResponse(Guid Id, string Email);
}
