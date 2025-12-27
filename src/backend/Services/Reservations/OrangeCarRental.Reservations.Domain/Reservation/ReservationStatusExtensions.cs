namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

/// <summary>
///     Extension methods for parsing ReservationStatus from string.
///     Uses C# 14 Extension Members syntax.
/// </summary>
public static class ReservationStatusExtensions
{
    /// <summary>
    ///     C# 14 Extension Members for nullable string to ReservationStatus parsing.
    /// </summary>
    extension(string? value)
    {
        /// <summary>
        ///     Tries to parse the string to a ReservationStatus enum value.
        /// </summary>
        /// <returns>The parsed ReservationStatus or null if parsing fails.</returns>
        public ReservationStatus? TryParseReservationStatus()
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            if (Enum.TryParse<ReservationStatus>(value, ignoreCase: true, out var parsedStatus))
            {
                return parsedStatus;
            }

            return null;
        }
    }
}
