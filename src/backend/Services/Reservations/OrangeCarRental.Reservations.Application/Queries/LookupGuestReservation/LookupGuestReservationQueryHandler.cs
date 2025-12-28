using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.SearchReservations;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.LookupGuestReservation;

/// <summary>
///     Handler for LookupGuestReservationQuery.
///     Retrieves a reservation by ID and verifies the email matches the customer.
/// </summary>
public sealed class LookupGuestReservationQueryHandler(
    IReservationRepository reservations,
    ICustomersService customersService)
    : IQueryHandler<LookupGuestReservationQuery, ReservationDto>
{
    public async Task<ReservationDto> HandleAsync(
        LookupGuestReservationQuery query,
        CancellationToken cancellationToken = default)
    {
        // Get the reservation
        var reservation = await reservations.GetByIdAsync(query.ReservationId, cancellationToken);

        // Get the customer email from Customers service
        var customerId = reservation.CustomerIdentifier?.Value
            ?? throw new EntityNotFoundException(
                typeof(Reservation),
                query.ReservationId.Value,
                "Reservation not found or has no customer.");

        var customerEmail = await customersService.GetCustomerEmailAsync(customerId, cancellationToken);

        // Verify the email matches (case-insensitive)
        if (customerEmail is null ||
            !string.Equals(customerEmail, query.Email, StringComparison.OrdinalIgnoreCase))
        {
            throw new EntityNotFoundException(
                typeof(Reservation),
                query.ReservationId.Value,
                "Reservation not found or email does not match.");
        }

        return reservation.ToDto();
    }
}
