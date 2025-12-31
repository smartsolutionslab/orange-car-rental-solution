using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;

namespace SmartSolutionsLab.OrangeCarRental.Location.Application.Queries;

public sealed record GetAllLocationsQuery : IQuery<GetAllLocationsResult>;
