namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

/// <summary>
///     Constants for reservation sort field names.
///     Use these instead of magic strings when specifying sort fields.
/// </summary>
public static class ReservationSortFields
{
    // Date fields
    public const string PickupDate = "pickupdate";
    public const string PickupDateAlt = "pickup_date";

    // Price fields
    public const string Price = "price";
    public const string TotalPrice = "totalprice";
    public const string TotalPriceAlt = "total_price";

    // Status
    public const string Status = "status";

    // Created date fields
    public const string CreatedDate = "createddate";
    public const string CreatedDateAlt = "created_date";
    public const string CreatedAt = "createdat";
    public const string CreatedAtAlt = "created_at";

    /// <summary>
    ///     Default sort field for reservation queries.
    /// </summary>
    public const string Default = CreatedAt;

    /// <summary>
    ///     All valid sort field names.
    /// </summary>
    public static readonly IReadOnlyList<string> All =
    [
        PickupDate, PickupDateAlt,
        Price, TotalPrice, TotalPriceAlt,
        Status,
        CreatedDate, CreatedDateAlt, CreatedAt, CreatedAtAlt
    ];
}
