using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Testing;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation.Events;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Tests.Domain.Entities;

public class ReservationTests
{
    private readonly VehicleIdentifier validVehicleIdentifier = VehicleIdentifier.From(TestIds.Vehicle1);
    private readonly CustomerIdentifier validCustomerIdentifier = CustomerIdentifier.From(TestIds.Customer1);
    private readonly BookingPeriod validPeriod = BookingPeriod.Of(TestDates.DefaultPickup, TestDates.DefaultReturn);
    private readonly LocationCode validPickupLocation = LocationCode.From(TestLocations.BerlinHbf);
    private readonly LocationCode validDropoffLocation = LocationCode.From(TestLocations.MunichAirport);
    private readonly Money validTotalPrice = TestMoney.EuroNet(299.99m);

    [Fact]
    public void Create_WithValidData_ShouldCreateReservation()
    {
        // Act
        var reservation = new Reservation();
        reservation.Create(
            validVehicleIdentifier,
            validCustomerIdentifier,
            validPeriod,
            validPickupLocation,
            validDropoffLocation,
            validTotalPrice);

        // Assert
        reservation.ShouldNotBeNull();
        reservation.Id.Value.ShouldNotBe(Guid.Empty);
        reservation.VehicleIdentifier.ShouldBe(validVehicleIdentifier);
        reservation.CustomerIdentifier.ShouldBe(validCustomerIdentifier);
        reservation.Period.ShouldBe(validPeriod);
        reservation.PickupLocationCode.ShouldBe(validPickupLocation);
        reservation.DropoffLocationCode.ShouldBe(validDropoffLocation);
        reservation.TotalPrice.ShouldBe(validTotalPrice);
        reservation.Status.ShouldBe(ReservationStatus.Pending);
        reservation.CreatedAt.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public void Create_WithEmptyVehicleId_ShouldThrowArgumentException()
    {
        // Arrange
        var reservation = new Reservation();

        // Act & Assert
        Should.Throw<ArgumentException>(() => reservation.Create(
            VehicleIdentifier.From(Guid.Empty),
            validCustomerIdentifier,
            validPeriod,
            validPickupLocation,
            validDropoffLocation,
            validTotalPrice));
    }

    [Fact]
    public void Create_WithEmptyCustomerId_ShouldThrowArgumentException()
    {
        // Arrange
        var reservation = new Reservation();

        // Act & Assert
        Should.Throw<ArgumentException>(() => reservation.Create(
            validVehicleIdentifier,
            CustomerIdentifier.From(Guid.Empty),
            validPeriod,
            validPickupLocation,
            validDropoffLocation,
            validTotalPrice));
    }

    [Fact]
    public void Create_ShouldRaiseReservationCreatedEvent()
    {
        // Arrange & Act
        var reservation = new Reservation();
        reservation.Create(
            validVehicleIdentifier,
            validCustomerIdentifier,
            validPeriod,
            validPickupLocation,
            validDropoffLocation,
            validTotalPrice);

        // Assert - Changes contains uncommitted events from Eventuous Aggregate
        var events = reservation.Changes;
        events.ShouldNotBeEmpty();
        var createdEvent = events.ShouldHaveSingleItem();
        createdEvent.ShouldBeOfType<ReservationCreated>();

        var evt = (ReservationCreated)createdEvent;
        evt.ReservationId.ShouldBe(reservation.Id);
        evt.VehicleIdentifier.ShouldBe(validVehicleIdentifier);
        evt.CustomerIdentifier.ShouldBe(validCustomerIdentifier);
        evt.Period.ShouldBe(validPeriod);
        evt.TotalPrice.ShouldBe(validTotalPrice);
    }

    [Fact]
    public void Confirm_WhenPending_ShouldChangeToConfirmed()
    {
        // Arrange
        var reservation = CreateTestReservation();

        // Act
        reservation.Confirm();

        // Assert - event-sourced aggregate mutates in place
        reservation.Status.ShouldBe(ReservationStatus.Confirmed);
        reservation.ConfirmedAt.ShouldNotBeNull();
        reservation.ConfirmedAt.Value.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public void Confirm_ShouldRaiseReservationConfirmedEvent()
    {
        // Arrange
        var reservation = CreateTestReservation();
        var initialEventCount = reservation.Changes.Count;

        // Act
        reservation.Confirm();

        // Assert
        reservation.Changes.Count.ShouldBe(initialEventCount + 1);
        var confirmedEvent = reservation.Changes.Last();
        confirmedEvent.ShouldBeOfType<ReservationConfirmed>();

        var evt = (ReservationConfirmed)confirmedEvent;
        evt.ReservationId.ShouldBe(reservation.Id);
    }

    [Theory]
    [InlineData(ReservationStatus.Confirmed)]
    [InlineData(ReservationStatus.Cancelled)]
    public void Confirm_WhenNotPending_ShouldThrowInvalidOperationException(ReservationStatus status)
    {
        // Arrange
        var reservation = CreateTestReservation();
        if (status == ReservationStatus.Confirmed)
            reservation.Confirm();
        else if (status == ReservationStatus.Cancelled)
            reservation.Cancel();

        // Act & Assert
        var ex = Should.Throw<InvalidOperationException>(() => reservation.Confirm());
        ex.Message.ShouldContain("Cannot confirm reservation in status");
    }

    [Fact]
    public void Cancel_WhenPending_ShouldChangeToCancelled()
    {
        // Arrange
        var reservation = CreateTestReservation();
        var reason = "Customer requested cancellation";

        // Act
        reservation.Cancel(reason);

        // Assert - event-sourced aggregate mutates in place
        reservation.Status.ShouldBe(ReservationStatus.Cancelled);
        reservation.CancellationReason.ShouldBe(reason);
        reservation.CancelledAt.ShouldNotBeNull();
        reservation.CancelledAt.Value.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_ShouldBeIdempotent()
    {
        // Arrange
        var reservation = CreateTestReservation();
        reservation.Cancel("First cancellation");
        var eventCountAfterFirstCancel = reservation.Changes.Count;

        // Act
        reservation.Cancel("Second attempt");

        // Assert - no new event raised (idempotent)
        reservation.Changes.Count.ShouldBe(eventCountAfterFirstCancel);
    }

    [Fact]
    public void Cancel_ShouldRaiseReservationCancelledEvent()
    {
        // Arrange
        var reservation = CreateTestReservation();
        var initialEventCount = reservation.Changes.Count;
        var reason = "Customer requested cancellation";

        // Act
        reservation.Cancel(reason);

        // Assert
        reservation.Changes.Count.ShouldBe(initialEventCount + 1);
        var cancelledEvent = reservation.Changes.Last();
        cancelledEvent.ShouldBeOfType<ReservationCancelled>();

        var evt = (ReservationCancelled)cancelledEvent;
        evt.ReservationId.ShouldBe(reservation.Id);
        evt.CancellationReason.ShouldBe(reason);
    }

    [Fact]
    public void MarkAsActive_WhenConfirmedAndPickupDateIsTodayOrPast_ShouldChangeToActive()
    {
        // Arrange - Create reservation with pickup date today
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var returnDate = pickupDate.AddDays(3);
        var period = BookingPeriod.Of(pickupDate, returnDate);

        var reservation = new Reservation();
        reservation.Create(
            validVehicleIdentifier,
            validCustomerIdentifier,
            period,
            validPickupLocation,
            validDropoffLocation,
            validTotalPrice);
        reservation.Confirm();

        // Act
        reservation.MarkAsActive();

        // Assert
        reservation.Status.ShouldBe(ReservationStatus.Active);
    }

    [Fact]
    public void MarkAsActive_WhenNotConfirmed_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var returnDate = pickupDate.AddDays(3);
        var period = BookingPeriod.Of(pickupDate, returnDate);

        var reservation = new Reservation();
        reservation.Create(
            validVehicleIdentifier,
            validCustomerIdentifier,
            period,
            validPickupLocation,
            validDropoffLocation,
            validTotalPrice);

        // Act & Assert
        var ex = Should.Throw<InvalidOperationException>(() => reservation.MarkAsActive());
        ex.Message.ShouldContain("Cannot activate reservation in status");
    }

    [Fact]
    public void MarkAsActive_WhenPickupDateInFuture_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var reservation = CreateTestReservation();
        reservation.Confirm();

        // Act & Assert (pickup date is 7 days in future)
        var ex = Should.Throw<InvalidOperationException>(() => reservation.MarkAsActive());
        ex.Message.ShouldContain("Cannot activate reservation before pickup date");
    }

    [Fact]
    public void Complete_WhenActive_ShouldChangeToCompleted()
    {
        // Arrange
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var returnDate = pickupDate.AddDays(3);
        var period = BookingPeriod.Of(pickupDate, returnDate);

        var reservation = new Reservation();
        reservation.Create(
            validVehicleIdentifier,
            validCustomerIdentifier,
            period,
            validPickupLocation,
            validDropoffLocation,
            validTotalPrice);
        reservation.Confirm();
        reservation.MarkAsActive();

        // Act
        reservation.Complete();

        // Assert
        reservation.Status.ShouldBe(ReservationStatus.Completed);
        reservation.CompletedAt.ShouldNotBeNull();
        reservation.CompletedAt.Value.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public void Complete_ShouldRaiseReservationCompletedEvent()
    {
        // Arrange
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var returnDate = pickupDate.AddDays(3);
        var period = BookingPeriod.Of(pickupDate, returnDate);

        var reservation = new Reservation();
        reservation.Create(
            validVehicleIdentifier,
            validCustomerIdentifier,
            period,
            validPickupLocation,
            validDropoffLocation,
            validTotalPrice);
        reservation.Confirm();
        reservation.MarkAsActive();
        var eventCountBeforeComplete = reservation.Changes.Count;

        // Act
        reservation.Complete();

        // Assert
        reservation.Changes.Count.ShouldBe(eventCountBeforeComplete + 1);
        var completedEvent = reservation.Changes.Last();
        completedEvent.ShouldBeOfType<ReservationCompleted>();

        var evt = (ReservationCompleted)completedEvent;
        evt.ReservationId.ShouldBe(reservation.Id);
    }

    [Fact]
    public void Complete_WhenNotActive_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var reservation = CreateTestReservation();

        // Act & Assert
        var ex = Should.Throw<InvalidOperationException>(() => reservation.Complete());
        ex.Message.ShouldContain("Cannot complete reservation in status");
    }

    [Fact]
    public void MarkAsNoShow_WhenPickupDateInFuture_ShouldThrowInvalidOperationException()
    {
        // Arrange - Create reservation with future dates
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var returnDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(4));
        var period = BookingPeriod.Of(pickupDate, returnDate);

        var reservation = new Reservation();
        reservation.Create(
            validVehicleIdentifier,
            validCustomerIdentifier,
            period,
            validPickupLocation,
            validDropoffLocation,
            validTotalPrice);
        reservation.Confirm();

        // Act & Assert
        var ex = Should.Throw<InvalidOperationException>(() => reservation.MarkAsNoShow());
        ex.Message.ShouldContain("Cannot mark as no-show before pickup date has passed");
    }

    [Fact]
    public void MarkAsNoShow_WhenNotConfirmed_ShouldThrowInvalidOperationException()
    {
        // Arrange - Create reservation with valid future dates
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var returnDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(4));
        var period = BookingPeriod.Of(pickupDate, returnDate);

        var reservation = new Reservation();
        reservation.Create(
            validVehicleIdentifier,
            validCustomerIdentifier,
            period,
            validPickupLocation,
            validDropoffLocation,
            validTotalPrice);

        // Act & Assert
        var ex = Should.Throw<InvalidOperationException>(() => reservation.MarkAsNoShow());
        ex.Message.ShouldContain("Cannot mark as no-show in status");
    }

    [Fact]
    public void MarkAsNoShow_WhenPickupDateNotPassed_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var reservation = CreateTestReservation();
        reservation.Confirm();

        // Act & Assert (pickup date is in future)
        var ex = Should.Throw<InvalidOperationException>(() => reservation.MarkAsNoShow());
        ex.Message.ShouldContain("Cannot mark as no-show before pickup date has passed");
    }

    [Fact]
    public void OverlapsWith_ShouldDelegateToBookingPeriod()
    {
        // Arrange
        var reservation = CreateTestReservation();

        var overlappingPickup = validPeriod.PickupDate.AddDays(1);
        var overlappingReturn = validPeriod.ReturnDate.AddDays(1);
        var overlappingPeriod = BookingPeriod.Of(overlappingPickup, overlappingReturn);

        // Act
        var overlaps = reservation.OverlapsWith(overlappingPeriod);

        // Assert
        overlaps.ShouldBeTrue();
    }

    private Reservation CreateTestReservation()
    {
        var reservation = new Reservation();
        reservation.Create(
            validVehicleIdentifier,
            validCustomerIdentifier,
            validPeriod,
            validPickupLocation,
            validDropoffLocation,
            validTotalPrice);
        return reservation;
    }
}
