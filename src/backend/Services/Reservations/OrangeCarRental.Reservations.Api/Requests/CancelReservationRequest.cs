namespace SmartSolutionsLab.OrangeCarRental.Reservations.Api.Requests;

/// <summary>
///     Request DTO for cancelling a reservation.
///     Accepts primitives from HTTP requests and maps to CancelReservationCommand with value objects.
/// </summary>
public sealed record CancelReservationRequest(
    string? CancellationReason = null);
