namespace SmartSolutionsLab.OrangeCarRental.Customers.Api.Contracts;

/// <summary>
///     Customer profile information for updates.
/// </summary>
public sealed record CustomerProfileDto(
    string FirstName,
    string LastName,
    string PhoneNumber);