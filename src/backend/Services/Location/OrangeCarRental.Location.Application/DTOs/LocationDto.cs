namespace SmartSolutionsLab.OrangeCarRental.Location.Application.DTOs;

public sealed record LocationDto
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string Address { get; init; }
    public required string OpeningHours { get; init; }
    public required string Phone { get; init; }
    public required string Email { get; init; }
    public required string Status { get; init; }
}
