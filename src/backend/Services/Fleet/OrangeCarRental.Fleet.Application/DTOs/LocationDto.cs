namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.DTOs;

/// <summary>
///     Location data transfer object
/// </summary>
public sealed record LocationDto(
    string Code,
    string Name,
    string Street,
    string City,
    string PostalCode,
    string FullAddress);
