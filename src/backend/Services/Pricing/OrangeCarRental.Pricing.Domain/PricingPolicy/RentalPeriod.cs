using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

/// <summary>
///     Represents a rental period for price calculation.
/// </summary>
public readonly record struct RentalPeriod : IValueObject
{
    private RentalPeriod(DateOnly pickupDate, DateOnly returnDate)
    {
        PickupDate = pickupDate;
        ReturnDate = returnDate;
        TotalDays = returnDate.DayNumber - pickupDate.DayNumber + 1;
    }

    public DateOnly PickupDate { get; }
    public DateOnly ReturnDate { get; }
    public int TotalDays { get; }

    public static RentalPeriod Of(DateOnly pickupDate, DateOnly returnDate)
    {
        Ensure.That(returnDate, nameof(returnDate))
            .ThrowIf(pickupDate >= returnDate, "Return date must be after pickup date");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        Ensure.That(pickupDate, nameof(pickupDate))
            .ThrowIf(pickupDate < today, "Pickup date cannot be in the past");

        return new RentalPeriod(pickupDate, returnDate);
    }

    /// <summary>
    ///     Factory method that accepts DateTime and converts to DateOnly.
    ///     Useful for API/DTO conversions.
    /// </summary>
    public static RentalPeriod Of(DateTime pickupDate, DateTime returnDate) =>
        Of(DateOnly.FromDateTime(pickupDate), DateOnly.FromDateTime(returnDate));

    public override string ToString() =>
        $"{TotalDays} day(s) from {PickupDate:yyyy-MM-dd} to {ReturnDate:yyyy-MM-dd}";
}
