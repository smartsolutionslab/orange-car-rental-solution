using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CreateReservation;

/// <summary>
///     Command to create a new reservation.
///     Uses value objects for type safety and early validation.
/// </summary>
public sealed record CreateReservationCommand(
    VehicleIdentifier VehicleId,
    Guid CustomerId,
    VehicleCategory CategoryCode,
    BookingPeriod Period,
    LocationCode PickupLocationCode,
    LocationCode DropoffLocationCode,
    Money? TotalPrice = null
) : ICommand<CreateReservationResult>;
