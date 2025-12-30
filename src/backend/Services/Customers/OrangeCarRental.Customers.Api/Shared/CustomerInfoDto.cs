namespace SmartSolutionsLab.OrangeCarRental.Customers.Api.Shared;

/// <summary>
///     Customer personal information.
/// </summary>
public sealed record CustomerInfoDto(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateOnly DateOfBirth);
