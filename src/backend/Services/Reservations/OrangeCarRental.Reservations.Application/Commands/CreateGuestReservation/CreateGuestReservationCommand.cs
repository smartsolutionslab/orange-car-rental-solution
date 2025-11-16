using System.Diagnostics.CodeAnalysis;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CreateGuestReservation;

/// <summary>
///     Command to create a reservation for a guest (user without prior registration).
///     This command handles both customer registration and reservation creation in a single transaction.
///     Uses value objects for type safety and early validation.
/// </summary>
public sealed record CreateGuestReservationCommand(
    ReservationVehicleId VehicleId,
    ReservationVehicleCategory CategoryCode,
    BookingPeriod Period,
    LocationCode PickupLocationCode,
    LocationCode DropoffLocationCode,
    CustomerName Name,
    Email Email,
    PhoneNumber PhoneNumber,
    BirthDate DateOfBirth,
    Address Address,
    DriversLicense DriversLicense) : ICommand<CreateGuestReservationResult>;
