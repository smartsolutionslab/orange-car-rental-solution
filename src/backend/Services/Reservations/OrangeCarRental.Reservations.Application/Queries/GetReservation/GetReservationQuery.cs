namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.GetReservation;

/// <summary>
/// Query to retrieve a reservation by ID.
/// </summary>
public sealed record GetReservationQuery(Guid ReservationId);
