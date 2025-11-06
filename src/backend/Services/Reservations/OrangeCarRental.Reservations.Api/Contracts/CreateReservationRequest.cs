namespace SmartSolutionsLab.OrangeCarRental.Reservations.Api.Contracts;

/// <summary>
///     Request DTO for creating a new reservation.
///     Accepts primitives from HTTP requests and maps to CreateReservationCommand with value objects.
/// </summary>
public sealed record CreateReservationRequest
{
    public required Guid VehicleId { get; init; }
    public required Guid CustomerId { get; init; }
    public required string CategoryCode { get; init; }
    public required DateTime PickupDate { get; init; }
    public required DateTime ReturnDate { get; init; }
    public required string PickupLocationCode { get; init; }
    public required string DropoffLocationCode { get; init; }
    public decimal? TotalPriceNet { get; init; }
}
