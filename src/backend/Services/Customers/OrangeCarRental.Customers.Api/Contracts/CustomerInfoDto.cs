namespace SmartSolutionsLab.OrangeCarRental.Customers.Api.Contracts;

/// <summary>
///     Customer personal information.
/// </summary>
public sealed record CustomerInfoDto(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateOnly DateOfBirth);
