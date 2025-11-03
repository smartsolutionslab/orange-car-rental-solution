using System.Text;
using System.Text.Json;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Services;

/// <summary>
/// HTTP client implementation for calling the Customers API.
/// </summary>
public sealed class CustomersService : ICustomersService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public CustomersService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<Guid> RegisterCustomerAsync(
        RegisterCustomerDto request,
        CancellationToken cancellationToken = default)
    {
        // Prepare request payload
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

        var json = JsonSerializer.Serialize(requestBody, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Call Customers API
        var response = await _httpClient.PostAsync("/api/customers", content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                $"Failed to register customer via Customers API. Status: {response.StatusCode}, Error: {errorContent}");
        }

        // Deserialize response to get the customer ID
        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<RegisterCustomerResponse>(responseJson, _jsonOptions);

        if (result is null)
        {
            throw new InvalidOperationException("Failed to deserialize customer registration response from Customers API");
        }

        return result.CustomerId;
    }

    /// <summary>
    /// Response model from Customers API register endpoint.
    /// </summary>
    private sealed record RegisterCustomerResponse(Guid CustomerId);
}
