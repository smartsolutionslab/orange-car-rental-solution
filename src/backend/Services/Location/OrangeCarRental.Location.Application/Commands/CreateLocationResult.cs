namespace SmartSolutionsLab.OrangeCarRental.Location.Application.Commands;

public sealed record CreateLocationResult
{
    public required string Code { get; init; }
    public required string Name { get; init; }
}
