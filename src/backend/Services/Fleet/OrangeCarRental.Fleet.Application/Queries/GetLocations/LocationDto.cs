namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.GetLocations;

/// <summary>
///     Location data transfer object
/// </summary>
public sealed record LocationDto
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string Street { get; init; }
    public required string City { get; init; }
    public required string PostalCode { get; init; }
    public required string FullAddress { get; init; }
}
