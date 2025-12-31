namespace SmartSolutionsLab.OrangeCarRental.Customers.Api.Shared;

/// <summary>
///     Customer profile information for updates.
/// </summary>
public sealed record CustomerProfileDto(
    string FirstName,
    string LastName,
    string PhoneNumber);
