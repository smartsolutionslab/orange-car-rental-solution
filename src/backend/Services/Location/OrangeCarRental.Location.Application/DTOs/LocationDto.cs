namespace SmartSolutionsLab.OrangeCarRental.Location.Application.DTOs;

public sealed record LocationDto
{
    public required string Code { get; init; }
    public required string Name { get; init; }

    // Split address fields for frontend compatibility
    public required string Street { get; init; }
    public required string City { get; init; }
    public required string PostalCode { get; init; }

    // Computed full address for display
    public required string FullAddress { get; init; }

    public required string OpeningHours { get; init; }
    public required string Phone { get; init; }
    public required string Email { get; init; }
    public required string Status { get; init; }
}
