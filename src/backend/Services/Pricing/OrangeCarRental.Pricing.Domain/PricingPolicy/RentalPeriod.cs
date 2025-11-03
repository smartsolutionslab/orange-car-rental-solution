namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

/// <summary>
/// Represents a rental period for price calculation.
/// </summary>
public readonly record struct RentalPeriod
{
    public DateTime PickupDate { get; }
    public DateTime ReturnDate { get; }
    public int TotalDays { get; }

    private RentalPeriod(DateTime pickupDate, DateTime returnDate)
    {
        PickupDate = pickupDate;
        ReturnDate = returnDate;
        TotalDays = (int)Math.Ceiling((returnDate - pickupDate).TotalDays);
    }

    public static RentalPeriod Of(DateTime pickupDate, DateTime returnDate)
    {
        if (pickupDate >= returnDate)
            throw new ArgumentException("Return date must be after pickup date");

        if (pickupDate < DateTime.UtcNow.Date)
            throw new ArgumentException("Pickup date cannot be in the past");

        return new RentalPeriod(pickupDate, returnDate);
    }

    public override string ToString() => $"{TotalDays} day(s) from {PickupDate:yyyy-MM-dd} to {ReturnDate:yyyy-MM-dd}";
}
