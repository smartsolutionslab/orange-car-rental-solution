using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Tests.Builders;

/// <summary>
/// Test data builder for Reservation aggregates.
/// Uses fluent API with sensible defaults.
/// </summary>
public class ReservationBuilder
{
    private VehicleIdentifier _vehicleId = TestDataHelpers.DefaultVehicleId;
    private CustomerIdentifier _customerId = TestDataHelpers.DefaultCustomerId;
    private BookingPeriod _period = TestDataHelpers.CreatePeriod();
    private LocationCode _pickupLocation = TestDataHelpers.BerlinHbf;
    private LocationCode _returnLocation = TestDataHelpers.BerlinHbf;
    private Money _totalPrice = TestDataHelpers.EuroFromGross(200.00m);

    /// <summary>
    /// Sets the vehicle identifier for the reservation.
    /// </summary>
    public ReservationBuilder WithVehicle(VehicleIdentifier vehicleId)
    {
        _vehicleId = vehicleId;
        return this;
    }

    /// <summary>
    /// Sets the customer identifier for the reservation.
    /// </summary>
    public ReservationBuilder WithCustomer(CustomerIdentifier customerId)
    {
        _customerId = customerId;
        return this;
    }

    /// <summary>
    /// Sets the booking period for the reservation.
    /// </summary>
    public ReservationBuilder WithPeriod(BookingPeriod period)
    {
        _period = period;
        return this;
    }

    /// <summary>
    /// Sets the booking period using start days from today and duration.
    /// </summary>
    public ReservationBuilder WithPeriod(int startDaysFromToday, int durationDays)
    {
        _period = TestDataHelpers.CreatePeriod(startDaysFromToday, durationDays);
        return this;
    }

    /// <summary>
    /// Sets the booking period to start today.
    /// </summary>
    public ReservationBuilder StartingToday(int durationDays = 3)
    {
        _period = TestDataHelpers.TodayPeriod(durationDays);
        return this;
    }

    /// <summary>
    /// Sets the pickup location.
    /// </summary>
    public ReservationBuilder WithPickupLocation(LocationCode location)
    {
        _pickupLocation = location;
        return this;
    }

    /// <summary>
    /// Sets the return location.
    /// </summary>
    public ReservationBuilder WithReturnLocation(LocationCode location)
    {
        _returnLocation = location;
        return this;
    }

    /// <summary>
    /// Sets both pickup and return locations to the same value.
    /// </summary>
    public ReservationBuilder AtLocation(LocationCode location)
    {
        _pickupLocation = location;
        _returnLocation = location;
        return this;
    }

    /// <summary>
    /// Sets the total price for the reservation.
    /// </summary>
    public ReservationBuilder WithPrice(Money totalPrice)
    {
        _totalPrice = totalPrice;
        return this;
    }

    /// <summary>
    /// Sets the total price using a gross amount in EUR.
    /// </summary>
    public ReservationBuilder WithGrossPrice(decimal grossAmount)
    {
        _totalPrice = TestDataHelpers.EuroFromGross(grossAmount);
        return this;
    }

    /// <summary>
    /// Builds a reservation in Pending status.
    /// </summary>
    public Reservation Build()
    {
        var reservation = new Reservation();
        reservation.Create(
            _vehicleId,
            _customerId,
            _period,
            _pickupLocation,
            _returnLocation,
            _totalPrice);
        return reservation;
    }

    /// <summary>
    /// Builds a reservation in Confirmed status.
    /// </summary>
    public Reservation BuildConfirmed()
    {
        var reservation = Build();
        reservation.Confirm();
        return reservation;
    }

    /// <summary>
    /// Builds a reservation in Active status (requires period starting today or in past).
    /// </summary>
    public Reservation BuildActive()
    {
        // Ensure period starts today
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (_period.PickupDate > today)
        {
            _period = TestDataHelpers.TodayPeriod(3);
        }

        var reservation = Build();
        reservation.Confirm();
        reservation.MarkAsActive();
        return reservation;
    }

    /// <summary>
    /// Builds a reservation in Completed status.
    /// </summary>
    public Reservation BuildCompleted()
    {
        var reservation = BuildActive();
        reservation.Complete();
        return reservation;
    }

    /// <summary>
    /// Builds a reservation in Cancelled status.
    /// </summary>
    public Reservation BuildCancelled(string? reason = "Test cancellation")
    {
        var reservation = Build();
        reservation.Cancel(reason);
        return reservation;
    }

    /// <summary>
    /// Creates a new ReservationBuilder with default values.
    /// </summary>
    public static ReservationBuilder Default() => new();

    /// <summary>
    /// Creates a reservation for a weekend trip (Fri-Sun).
    /// </summary>
    public static ReservationBuilder WeekendTrip()
    {
        return new ReservationBuilder()
            .WithPeriod(7, 3)
            .WithGrossPrice(250.00m);
    }

    /// <summary>
    /// Creates a reservation for a one-week trip.
    /// </summary>
    public static ReservationBuilder WeekLongTrip()
    {
        return new ReservationBuilder()
            .WithPeriod(7, 7)
            .WithGrossPrice(500.00m);
    }

    /// <summary>
    /// Creates a reservation with empty vehicle ID (for testing validation).
    /// </summary>
    public static ReservationBuilder WithEmptyVehicleId()
    {
        return new ReservationBuilder()
            .WithVehicle(VehicleIdentifier.From(Guid.Empty));
    }

    /// <summary>
    /// Creates a reservation with empty customer ID (for testing validation).
    /// </summary>
    public static ReservationBuilder WithEmptyCustomerId()
    {
        return new ReservationBuilder()
            .WithCustomer(CustomerIdentifier.From(Guid.Empty));
    }
}
