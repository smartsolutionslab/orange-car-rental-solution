namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

public static class ReservationStatusExtensions
{
    public static ReservationStatus? TryParseReservationStatus(this string? value)
    {
        ReservationStatus? status = null;
        if (!string.IsNullOrWhiteSpace(value) &&
            Enum.TryParse<ReservationStatus>(value, ignoreCase: true, out var parsedStatus))
        {
            status = parsedStatus;
        }

        return status;
    }
}
