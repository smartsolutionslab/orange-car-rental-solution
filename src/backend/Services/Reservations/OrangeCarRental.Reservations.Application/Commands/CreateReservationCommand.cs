using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands;

/// <summary>
///     Command to create a new reservation.
///     Uses value objects for type safety and early validation.
/// </summary>
public sealed record CreateReservationCommand(
    VehicleIdentifier VehicleIdentifier,
    CustomerIdentifier CustomerIdentifier,
    VehicleCategory CategoryCode,
    BookingPeriod Period,
    LocationCode PickupLocationCode,
    LocationCode DropoffLocationCode,
    Money? TotalPrice = null
) : ICommand<CreateReservationResult>;
