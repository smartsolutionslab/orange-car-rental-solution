namespace SmartSolutionsLab.OrangeCarRental.Reservations.Api.Contracts;

/// <summary>
///     Customer details for inline registration.
/// </summary>
public sealed record CustomerDetailsDto(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateOnly DateOfBirth);
