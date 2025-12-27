using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;

namespace SmartSolutionsLab.OrangeCarRental.Location.Application.Commands.CreateLocation;

public sealed record CreateLocationCommand(
    string Code,
    string Name,
    string Street,
    string City,
    string PostalCode,
    string Phone,
    string Email,
    string OpeningHours,
    decimal? Latitude = null,
    decimal? Longitude = null) : ICommand<CreateLocationResult>;
