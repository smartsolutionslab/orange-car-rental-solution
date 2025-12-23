using Moq;
using Shouldly;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.CommandServices;
using SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.EventSourcing;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Tests.Infrastructure.CommandServices;

public class ReservationCommandServiceTests
{
    private readonly Mock<IEventSourcedReservationRepository> _repositoryMock = new();
    private readonly ReservationCommandService _service;

    // Common test data
    private readonly VehicleIdentifier _vehicleId = VehicleIdentifier.New();
    private readonly CustomerIdentifier _customerId = CustomerIdentifier.New();
    private readonly LocationCode _pickupLocation = LocationCode.From("BER-HBF");
    private readonly LocationCode _dropoffLocation = LocationCode.From("MUC-FLG");
    private readonly Money _totalPrice = Money.Euro(299.99m);

    public ReservationCommandServiceTests()
    {
        _service = new ReservationCommandService(_repositoryMock.Object);
    }

    private BookingPeriod CreateFutureBookingPeriod(int daysFromNow = 7, int duration = 3) =>
        BookingPeriod.Of(
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(daysFromNow)),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(daysFromNow + duration)));

    private BookingPeriod CreateTodayBookingPeriod(int duration = 3) =>
        BookingPeriod.Of(
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(duration)));

    private Reservation CreatePendingReservation()
    {
        var reservation = new Reservation();
        reservation.Create(_vehicleId, _customerId, CreateFutureBookingPeriod(), _pickupLocation, _dropoffLocation, _totalPrice);
        return reservation;
    }

    private Reservation CreatePendingReservationWithTodayPickup()
    {
        var reservation = new Reservation();
        reservation.Create(_vehicleId, _customerId, CreateTodayBookingPeriod(), _pickupLocation, _dropoffLocation, _totalPrice);
        return reservation;
    }

    private Reservation CreateConfirmedReservation()
    {
        var reservation = CreatePendingReservation();
        reservation.Confirm();
        return reservation;
    }

    private Reservation CreateConfirmedReservationWithTodayPickup()
    {
        var reservation = CreatePendingReservationWithTodayPickup();
        reservation.Confirm();
        return reservation;
    }

    private Reservation CreateActiveReservation()
    {
        var reservation = CreateConfirmedReservationWithTodayPickup();
        reservation.MarkAsActive();
        return reservation;
    }

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateAndSaveReservation()
    {
        // Arrange
        var period = CreateFutureBookingPeriod();

        // Act
        var result = await _service.CreateAsync(
            _vehicleId, _customerId, period, _pickupLocation, _dropoffLocation, _totalPrice);

        // Assert
        result.ShouldNotBeNull();
        result.VehicleIdentifier.ShouldBe(_vehicleId);
        result.CustomerIdentifier.ShouldBe(_customerId);
        result.Period.ShouldBe(period);
        result.PickupLocationCode.ShouldBe(_pickupLocation);
        result.DropoffLocationCode.ShouldBe(_dropoffLocation);
        result.TotalPrice.ShouldBe(_totalPrice);
        result.Status.ShouldBe(ReservationStatus.Pending);

        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallSaveAsyncWithNewReservation()
    {
        // Arrange
        var period = CreateFutureBookingPeriod();

        Reservation? savedReservation = null;
        _repositoryMock
            .Setup(r => r.SaveAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Callback<Reservation, CancellationToken>((r, _) => savedReservation = r)
            .Returns(Task.CompletedTask);

        // Act
        await _service.CreateAsync(_vehicleId, _customerId, period, _pickupLocation, _dropoffLocation, _totalPrice);

        // Assert
        savedReservation.ShouldNotBeNull();
        savedReservation.VehicleIdentifier.ShouldBe(_vehicleId);
        savedReservation.CustomerIdentifier.ShouldBe(_customerId);
    }

    #endregion

    #region ConfirmAsync Tests

    [Fact]
    public async Task ConfirmAsync_WithPendingReservation_ShouldConfirmAndSave()
    {
        // Arrange
        var existingReservation = CreatePendingReservation();
        var reservationId = existingReservation.Id;

        _repositoryMock
            .Setup(r => r.LoadAsync(reservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingReservation);

        // Act
        var result = await _service.ConfirmAsync(reservationId);

        // Assert
        result.Status.ShouldBe(ReservationStatus.Confirmed);
        result.ConfirmedAt.ShouldNotBeNull();
        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ConfirmAsync_WithNonExistingReservation_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var reservationId = ReservationIdentifier.New();
        var emptyReservation = new Reservation();

        _repositoryMock
            .Setup(r => r.LoadAsync(reservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyReservation);

        // Act & Assert
        var ex = await Should.ThrowAsync<InvalidOperationException>(
            () => _service.ConfirmAsync(reservationId));

        ex.Message.ShouldContain(reservationId.Value.ToString());
        ex.Message.ShouldContain("not found");
    }

    #endregion

    #region CancelAsync Tests

    [Fact]
    public async Task CancelAsync_WithPendingReservation_ShouldCancelAndSave()
    {
        // Arrange
        var existingReservation = CreatePendingReservation();
        var reservationId = existingReservation.Id;
        var reason = "Customer requested cancellation";

        _repositoryMock
            .Setup(r => r.LoadAsync(reservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingReservation);

        // Act
        var result = await _service.CancelAsync(reservationId, reason);

        // Assert
        result.Status.ShouldBe(ReservationStatus.Cancelled);
        result.CancellationReason.ShouldBe(reason);
        result.CancelledAt.ShouldNotBeNull();
        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CancelAsync_WithConfirmedReservation_ShouldCancelAndSave()
    {
        // Arrange
        var existingReservation = CreateConfirmedReservation();
        var reservationId = existingReservation.Id;

        _repositoryMock
            .Setup(r => r.LoadAsync(reservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingReservation);

        // Act
        var result = await _service.CancelAsync(reservationId, "Changed plans");

        // Assert
        result.Status.ShouldBe(ReservationStatus.Cancelled);
    }

    [Fact]
    public async Task CancelAsync_WithoutReason_ShouldWork()
    {
        // Arrange
        var existingReservation = CreatePendingReservation();
        var reservationId = existingReservation.Id;

        _repositoryMock
            .Setup(r => r.LoadAsync(reservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingReservation);

        // Act
        var result = await _service.CancelAsync(reservationId);

        // Assert
        result.Status.ShouldBe(ReservationStatus.Cancelled);
        result.CancellationReason.ShouldBeNull();
    }

    [Fact]
    public async Task CancelAsync_WithNonExistingReservation_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var reservationId = ReservationIdentifier.New();
        var emptyReservation = new Reservation();

        _repositoryMock
            .Setup(r => r.LoadAsync(reservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyReservation);

        // Act & Assert
        var ex = await Should.ThrowAsync<InvalidOperationException>(
            () => _service.CancelAsync(reservationId));

        ex.Message.ShouldContain("not found");
    }

    #endregion

    #region ActivateAsync Tests

    [Fact]
    public async Task ActivateAsync_WithConfirmedReservationOnPickupDate_ShouldActivateAndSave()
    {
        // Arrange
        var existingReservation = CreateConfirmedReservationWithTodayPickup();
        var reservationId = existingReservation.Id;

        _repositoryMock
            .Setup(r => r.LoadAsync(reservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingReservation);

        // Act
        var result = await _service.ActivateAsync(reservationId);

        // Assert
        result.Status.ShouldBe(ReservationStatus.Active);
        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ActivateAsync_WithNonExistingReservation_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var reservationId = ReservationIdentifier.New();
        var emptyReservation = new Reservation();

        _repositoryMock
            .Setup(r => r.LoadAsync(reservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyReservation);

        // Act & Assert
        var ex = await Should.ThrowAsync<InvalidOperationException>(
            () => _service.ActivateAsync(reservationId));

        ex.Message.ShouldContain("not found");
    }

    #endregion

    #region CompleteAsync Tests

    [Fact]
    public async Task CompleteAsync_WithActiveReservation_ShouldCompleteAndSave()
    {
        // Arrange
        var existingReservation = CreateActiveReservation();
        var reservationId = existingReservation.Id;

        _repositoryMock
            .Setup(r => r.LoadAsync(reservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingReservation);

        // Act
        var result = await _service.CompleteAsync(reservationId);

        // Assert
        result.Status.ShouldBe(ReservationStatus.Completed);
        result.CompletedAt.ShouldNotBeNull();
        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CompleteAsync_WithNonExistingReservation_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var reservationId = ReservationIdentifier.New();
        var emptyReservation = new Reservation();

        _repositoryMock
            .Setup(r => r.LoadAsync(reservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyReservation);

        // Act & Assert
        var ex = await Should.ThrowAsync<InvalidOperationException>(
            () => _service.CompleteAsync(reservationId));

        ex.Message.ShouldContain("not found");
    }

    #endregion

    #region MarkAsNoShowAsync Tests

    [Fact]
    public async Task MarkAsNoShowAsync_WithNonExistingReservation_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var reservationId = ReservationIdentifier.New();
        var emptyReservation = new Reservation();

        _repositoryMock
            .Setup(r => r.LoadAsync(reservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyReservation);

        // Act & Assert
        var ex = await Should.ThrowAsync<InvalidOperationException>(
            () => _service.MarkAsNoShowAsync(reservationId));

        ex.Message.ShouldContain("not found");
    }

    #endregion

    #region CancellationToken Tests

    [Fact]
    public async Task CreateAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var period = CreateFutureBookingPeriod();
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        // Act
        await _service.CreateAsync(_vehicleId, _customerId, period, _pickupLocation, _dropoffLocation, _totalPrice, token);

        // Assert
        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<Reservation>(), token), Times.Once);
    }

    [Fact]
    public async Task ConfirmAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var existingReservation = CreatePendingReservation();
        var reservationId = existingReservation.Id;

        _repositoryMock
            .Setup(r => r.LoadAsync(reservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingReservation);

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        // Act
        await _service.ConfirmAsync(reservationId, token);

        // Assert
        _repositoryMock.Verify(r => r.LoadAsync(reservationId, token), Times.Once);
        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<Reservation>(), token), Times.Once);
    }

    [Fact]
    public async Task CancelAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var existingReservation = CreatePendingReservation();
        var reservationId = existingReservation.Id;

        _repositoryMock
            .Setup(r => r.LoadAsync(reservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingReservation);

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        // Act
        await _service.CancelAsync(reservationId, "Test", token);

        // Assert
        _repositoryMock.Verify(r => r.LoadAsync(reservationId, token), Times.Once);
        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<Reservation>(), token), Times.Once);
    }

    #endregion
}
