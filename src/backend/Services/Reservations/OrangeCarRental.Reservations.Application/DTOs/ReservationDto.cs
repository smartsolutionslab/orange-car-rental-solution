namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;

/// <summary>
/// Data transfer object for reservation details.
/// </summary>
public sealed record ReservationDto
{
    public Guid ReservationId { get; init; }
    public Guid VehicleId { get; init; }
    public Guid CustomerId { get; init; }
    public DateTime PickupDate { get; init; }
    public DateTime ReturnDate { get; init; }
    public string PickupLocationCode { get; init; } = string.Empty;
    public string DropoffLocationCode { get; init; } = string.Empty;
    public int RentalDays { get; init; }
    public decimal TotalPriceNet { get; init; }
    public decimal TotalPriceVat { get; init; }
    public decimal TotalPriceGross { get; init; }
    public string Currency { get; init; } = "EUR";
    public string Status { get; init; } = string.Empty;
    public string? CancellationReason { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ConfirmedAt { get; init; }
    public DateTime? CancelledAt { get; init; }
    public DateTime? CompletedAt { get; init; }
}
