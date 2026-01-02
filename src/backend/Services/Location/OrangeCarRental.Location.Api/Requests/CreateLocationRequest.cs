namespace SmartSolutionsLab.OrangeCarRental.Location.Api.Requests;

/// <summary>
///     Request to create a new rental location.
/// </summary>
/// <param name="Code">Location code (e.g., "BER-HBF").</param>
/// <param name="Name">Display name (e.g., "Berlin Hauptbahnhof").</param>
/// <param name="Street">Street address.</param>
/// <param name="City">City name.</param>
/// <param name="PostalCode">Postal code.</param>
/// <param name="Phone">Contact phone number.</param>
/// <param name="Email">Contact email address.</param>
/// <param name="OpeningHours">Opening hours description.</param>
/// <param name="Latitude">Optional latitude coordinate.</param>
/// <param name="Longitude">Optional longitude coordinate.</param>
public sealed record CreateLocationRequest(
    string Code,
    string Name,
    string Street,
    string City,
    string PostalCode,
    string Phone,
    string Email,
    string OpeningHours,
    decimal? Latitude = null,
    decimal? Longitude = null);