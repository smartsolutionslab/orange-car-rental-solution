using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Tests.Builders;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Tests.Domain;

public class ReservationTests
{
    #region Create Tests

    [Fact]
    public void Create_WithValidParameters_CreatesReservation()
    {
        // Arrange & Act
        var reservation = ReservationBuilder.Default().Build();

        // Assert
        reservation.ShouldNotBeNull();
        reservation.VehicleIdentifier.ShouldNotBe(default);
        reservation.CustomerIdentifier.ShouldNotBe(default);
        reservation.Status.ShouldBe(ReservationStatus.Pending);
        reservation.CreatedAt.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
        reservation.ConfirmedAt.ShouldBeNull();
        reservation.CancelledAt.ShouldBeNull();
        reservation.CompletedAt.ShouldBeNull();
        reservation.CancellationReason.ShouldBeNull();
    }

    [Fact]
    public void Create_WithEmptyVehicleId_ThrowsArgumentException()
    {
        // Arrange & Act
        var act = () => ReservationBuilder.WithEmptyVehicleId().Build();

        // Assert
        var ex = Should.Throw<ArgumentException>(act);
        ex.Message.ShouldContain("GUID cannot be empty");
    }

    [Fact]
    public void Create_WithEmptyCustomerId_ThrowsArgumentException()
    {
        // Arrange & Act
        var act = () => ReservationBuilder.WithEmptyCustomerId().Build();

        // Assert
        var ex = Should.Throw<ArgumentException>(act);
        ex.Message.ShouldContain("GUID cannot be empty");
    }

    #endregion

    #region Confirm Tests

    [Fact]
    public void Confirm_WhenPending_ConfirmsReservation()
    {
        // Arrange
        var reservation = ReservationBuilder.Default().Build();

        // Act - event-sourced aggregate mutates in place
        reservation.Confirm();

        // Assert
        reservation.Status.ShouldBe(ReservationStatus.Confirmed);
        reservation.ConfirmedAt.ShouldNotBeNull();
        reservation.ConfirmedAt.Value.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public void Confirm_WhenAlreadyConfirmed_ThrowsInvalidOperationException()
    {
        // Arrange
        var reservation = ReservationBuilder.Default().BuildConfirmed();

        // Act
        var act = () => reservation.Confirm();

        // Assert
        var ex = Should.Throw<InvalidOperationException>(act);
        ex.Message.ShouldContain("Cannot confirm reservation in status: Confirmed");
    }

    [Fact]
    public void Confirm_WhenCancelled_ThrowsInvalidOperationException()
    {
        // Arrange
        var reservation = ReservationBuilder.Default().BuildCancelled();

        // Act
        var act = () => reservation.Confirm();

        // Assert
        var ex = Should.Throw<InvalidOperationException>(act);
        ex.Message.ShouldContain("Cannot confirm reservation in status: Cancelled");
    }

    #endregion

    #region Cancel Tests

    [Fact]
    public void Cancel_WhenPending_CancelsReservation()
    {
        // Arrange
        var reservation = ReservationBuilder.Default().Build();

        // Act - event-sourced aggregate mutates in place
        reservation.Cancel("Changed plans");

        // Assert
        reservation.Status.ShouldBe(ReservationStatus.Cancelled);
        reservation.CancelledAt.ShouldNotBeNull();
        reservation.CancelledAt.Value.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
        reservation.CancellationReason.ShouldBe("Changed plans");
    }

    [Fact]
    public void Cancel_WhenConfirmed_CancelsReservation()
    {
        // Arrange
        var reservation = ReservationBuilder.Default().BuildConfirmed();

        // Act - event-sourced aggregate mutates in place
        reservation.Cancel("Emergency");

        // Assert
        reservation.Status.ShouldBe(ReservationStatus.Cancelled);
        reservation.CancellationReason.ShouldBe("Emergency");
    }

    [Fact]
    public void Cancel_WithoutReason_CancelsWithNullReason()
    {
        // Arrange
        var reservation = ReservationBuilder.Default().Build();

        // Act - event-sourced aggregate mutates in place
        reservation.Cancel();

        // Assert
        reservation.Status.ShouldBe(ReservationStatus.Cancelled);
        reservation.CancellationReason.ShouldBeNull();
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_DoesNotThrow()
    {
        // Arrange
        var reservation = ReservationBuilder.Default().BuildCancelled("First cancellation");

        // Act
        var act = () => reservation.Cancel("Second cancellation");

        // Assert
        Should.NotThrow(act);
        reservation.Status.ShouldBe(ReservationStatus.Cancelled);
        reservation.CancellationReason.ShouldBe("First cancellation"); // Reason doesn't change
    }

    [Fact]
    public void Cancel_WhenCompleted_ThrowsInvalidOperationException()
    {
        // Arrange
        var reservation = ReservationBuilder.Default().BuildCompleted();

        // Act
        var act = () => reservation.Cancel("Too late");

        // Assert
        var ex = Should.Throw<InvalidOperationException>(act);
        ex.Message.ShouldContain("Cannot cancel a completed reservation");
    }

    [Fact]
    public void Cancel_WhenActive_ThrowsInvalidOperationException()
    {
        // Arrange
        var reservation = ReservationBuilder.Default().BuildActive();

        // Act
        var act = () => reservation.Cancel("Cancel during rental");

        // Assert
        var ex = Should.Throw<InvalidOperationException>(act);
        ex.Message.ShouldContain("Cannot cancel an active rental");
    }

    #endregion

    #region MarkAsActive Tests

    [Fact]
    public void MarkAsActive_WhenConfirmedAndPickupDateIsToday_ActivatesReservation()
    {
        // Arrange
        var reservation = ReservationBuilder.Default()
            .StartingToday()
            .BuildConfirmed();

        // Act - event-sourced aggregate mutates in place
        reservation.MarkAsActive();

        // Assert
        reservation.Status.ShouldBe(ReservationStatus.Active);
    }

    [Fact]
    public void MarkAsActive_WhenPickupDateIsInFuture_ThrowsInvalidOperationException()
    {
        // Arrange
        var reservation = ReservationBuilder.Default()
            .WithPeriod(5, 3) // Starts in 5 days
            .BuildConfirmed();

        // Act
        var act = () => reservation.MarkAsActive();

        // Assert
        var ex = Should.Throw<InvalidOperationException>(act);
        ex.Message.ShouldContain("Cannot activate reservation before pickup date");
    }

    [Fact]
    public void MarkAsActive_WhenNotConfirmed_ThrowsInvalidOperationException()
    {
        // Arrange
        var reservation = ReservationBuilder.Default()
            .StartingToday()
            .Build(); // Not confirmed

        // Act
        var act = () => reservation.MarkAsActive();

        // Assert
        var ex = Should.Throw<InvalidOperationException>(act);
        ex.Message.ShouldContain("Cannot activate reservation in status: Pending");
    }

    [Fact]
    public void MarkAsActive_WhenCancelled_ThrowsInvalidOperationException()
    {
        // Arrange
        var reservation = ReservationBuilder.Default()
            .StartingToday()
            .BuildCancelled();

        // Act
        var act = () => reservation.MarkAsActive();

        // Assert
        var ex = Should.Throw<InvalidOperationException>(act);
        ex.Message.ShouldContain("Cannot activate reservation in status: Cancelled");
    }

    #endregion

    #region Complete Tests

    [Fact]
    public void Complete_WhenActive_CompletesReservation()
    {
        // Arrange
        var reservation = ReservationBuilder.Default().BuildActive();

        // Act - event-sourced aggregate mutates in place
        reservation.Complete();

        // Assert
        reservation.Status.ShouldBe(ReservationStatus.Completed);
        reservation.CompletedAt.ShouldNotBeNull();
        reservation.CompletedAt.Value.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public void Complete_WhenNotActive_ThrowsInvalidOperationException()
    {
        // Arrange
        var reservation = ReservationBuilder.Default().BuildConfirmed();

        // Act
        var act = () => reservation.Complete();

        // Assert
        var ex = Should.Throw<InvalidOperationException>(act);
        ex.Message.ShouldContain("Cannot complete reservation in status: Confirmed");
    }

    [Fact]
    public void Complete_WhenPending_ThrowsInvalidOperationException()
    {
        // Arrange
        var reservation = ReservationBuilder.Default().Build();

        // Act
        var act = () => reservation.Complete();

        // Assert
        var ex = Should.Throw<InvalidOperationException>(act);
        ex.Message.ShouldContain("Cannot complete reservation in status: Pending");
    }

    #endregion

    #region MarkAsNoShow Tests

    [Fact]
    public void MarkAsNoShow_WhenConfirmedButBeforePickupDate_ThrowsInvalidOperationException()
    {
        // Arrange
        var reservation = ReservationBuilder.Default()
            .WithPeriod(5, 3) // Future pickup
            .BuildConfirmed();

        // Act
        var act = () => reservation.MarkAsNoShow();

        // Assert
        var ex = Should.Throw<InvalidOperationException>(act);
        ex.Message.ShouldContain("Cannot mark as no-show before pickup date has passed");
    }

    [Fact]
    public void MarkAsNoShow_WhenNotConfirmed_ThrowsInvalidOperationException()
    {
        // Arrange
        var reservation = ReservationBuilder.Default().Build();

        // Act
        var act = () => reservation.MarkAsNoShow();

        // Assert
        var ex = Should.Throw<InvalidOperationException>(act);
        ex.Message.ShouldContain("Cannot mark as no-show in status: Pending");
    }

    [Fact]
    public void MarkAsNoShow_WhenActive_ThrowsInvalidOperationException()
    {
        // Arrange
        var reservation = ReservationBuilder.Default().BuildActive();

        // Act
        var act = () => reservation.MarkAsNoShow();

        // Assert
        var ex = Should.Throw<InvalidOperationException>(act);
        ex.Message.ShouldContain("Cannot mark as no-show in status: Active");
    }

    #endregion

    #region OverlapsWith Tests

    [Fact]
    public void OverlapsWith_WhenPeriodsOverlap_ReturnsTrue()
    {
        // Arrange
        var period1 = TestDataHelpers.CreatePeriod(7, 5);
        var reservation = ReservationBuilder.Default()
            .WithPeriod(period1)
            .Build();

        var period2 = TestDataHelpers.CreatePeriod(9, 3); // Starts during period1

        // Act
        var overlaps = reservation.OverlapsWith(period2);

        // Assert
        overlaps.ShouldBeTrue();
    }

    [Fact]
    public void OverlapsWith_WhenPeriodsDoNotOverlap_ReturnsFalse()
    {
        // Arrange
        var reservation = ReservationBuilder.Default()
            .WithPeriod(7, 3)
            .Build();

        var period2 = TestDataHelpers.CreatePeriod(15, 3); // After first period ends

        // Act
        var overlaps = reservation.OverlapsWith(period2);

        // Assert
        overlaps.ShouldBeFalse();
    }

    [Fact]
    public void OverlapsWith_WhenPeriodsAreIdentical_ReturnsTrue()
    {
        // Arrange
        var period = TestDataHelpers.CreatePeriod(7, 3);
        var reservation = ReservationBuilder.Default()
            .WithPeriod(period)
            .Build();

        // Act
        var overlaps = reservation.OverlapsWith(period);

        // Assert
        overlaps.ShouldBeTrue();
    }

    [Fact]
    public void OverlapsWith_WhenPeriodEndsOnPickupDate_ReturnsFalse()
    {
        // Arrange
        var reservation = ReservationBuilder.Default()
            .WithPeriod(7, 3)
            .Build();

        // Period starting the day after first period ends
        var period2 = TestDataHelpers.CreatePeriod(11, 3);

        // Act
        var overlaps = reservation.OverlapsWith(period2);

        // Assert
        overlaps.ShouldBeFalse();
    }

    #endregion

    #region Status Transition Tests

    [Fact]
    public void StatusTransition_PendingToConfirmedToActiveToCompleted_IsValid()
    {
        // Arrange & Act
        var reservation = ReservationBuilder.Default()
            .StartingToday()
            .Build();

        // Assert initial state
        reservation.Status.ShouldBe(ReservationStatus.Pending);

        // Confirm - event-sourced aggregate mutates in place
        reservation.Confirm();
        reservation.Status.ShouldBe(ReservationStatus.Confirmed);

        // Activate
        reservation.MarkAsActive();
        reservation.Status.ShouldBe(ReservationStatus.Active);

        // Complete
        reservation.Complete();
        reservation.Status.ShouldBe(ReservationStatus.Completed);
    }

    [Fact]
    public void StatusTransition_PendingToCancelled_IsValid()
    {
        // Arrange
        var reservation = ReservationBuilder.Default().Build();

        // Act - event-sourced aggregate mutates in place
        reservation.Cancel("Customer request");

        // Assert
        reservation.Status.ShouldBe(ReservationStatus.Cancelled);
    }

    [Fact]
    public void StatusTransition_ConfirmedToCancelled_IsValid()
    {
        // Arrange
        var reservation = ReservationBuilder.Default().BuildConfirmed();

        // Act - event-sourced aggregate mutates in place
        reservation.Cancel("Emergency");

        // Assert
        reservation.Status.ShouldBe(ReservationStatus.Cancelled);
    }

    [Fact]
    public void StatusTransition_ConfirmedToNoShow_ThrowsWhenBeforePickupDate()
    {
        // Arrange
        var reservation = ReservationBuilder.Default()
            .WithPeriod(1, 3) // Tomorrow
            .BuildConfirmed();

        // Act & Assert
        var act = () => reservation.MarkAsNoShow();
        Should.Throw<InvalidOperationException>(act);
    }

    #endregion

    #region Named Scenarios Tests

    [Fact]
    public void WeekendTrip_CreatesThreeDayReservation()
    {
        // Arrange & Act
        var reservation = ReservationBuilder.WeekendTrip().Build();

        // Assert - using .Value to access nullable struct properties
        reservation.Period!.Value.Days.ShouldBe(3);
        reservation.TotalPrice!.Value.GrossAmount.ShouldBe(250.00m);
    }

    [Fact]
    public void WeekLongTrip_CreatesSevenDayReservation()
    {
        // Arrange & Act
        var reservation = ReservationBuilder.WeekLongTrip().Build();

        // Assert - using .Value to access nullable struct properties
        reservation.Period!.Value.Days.ShouldBe(7);
        reservation.TotalPrice!.Value.GrossAmount.ShouldBe(500.00m);
    }

    #endregion
}
