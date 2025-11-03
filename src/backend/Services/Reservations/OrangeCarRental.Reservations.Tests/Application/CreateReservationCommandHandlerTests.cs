using Moq;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CreateReservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Tests.Application;

public class CreateReservationCommandHandlerTests
{
    private readonly Mock<IReservationRepository> _repositoryMock = new();
    private readonly Mock<IPricingService> _pricingServiceMock = new();
    private readonly CreateReservationCommandHandler _handler;

    public CreateReservationCommandHandlerTests()
    {
        _handler = new CreateReservationCommandHandler(_repositoryMock.Object, _pricingServiceMock.Object);
    }

    #region HandleAsync Tests

    [Fact]
    public async Task HandleAsync_WithValidCommand_CreatesReservation()
    {
        // Arrange
        var command = new CreateReservationCommand(
            VehicleId: Guid.NewGuid(),
            CustomerId: Guid.NewGuid(),
            CategoryCode: "KOMPAKT",
            PickupDate: DateTime.UtcNow.Date.AddDays(7),
            ReturnDate: DateTime.UtcNow.Date.AddDays(10),
            PickupLocationCode: "BER-HBF",
            DropoffLocationCode: "BER-HBF",
            TotalPriceNet: 168.07m // Net amount (200 gross with 19% VAT)
        );

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _repositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ReservationId.Should().NotBeEmpty();
        result.Status.Should().Be(ReservationStatus.Pending.ToString());
        result.TotalPriceNet.Should().Be(168.07m);
        result.TotalPriceGross.Should().BeApproximately(200.00m, 0.01m);
        result.TotalPriceVat.Should().BeApproximately(31.93m, 0.01m);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_CallsRepositoryAddAsync()
    {
        // Arrange
        var command = new CreateReservationCommand(
            VehicleId: Guid.NewGuid(),
            CustomerId: Guid.NewGuid(),
            CategoryCode: "KOMPAKT",
            PickupDate: DateTime.UtcNow.Date.AddDays(5),
            ReturnDate: DateTime.UtcNow.Date.AddDays(8),
            PickupLocationCode: "BER-HBF",
            DropoffLocationCode: "BER-HBF",
            TotalPriceNet: 150.00m
        );

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_CallsRepositorySaveChangesAsync()
    {
        // Arrange
        var command = new CreateReservationCommand(
            VehicleId: Guid.NewGuid(),
            CustomerId: Guid.NewGuid(),
            CategoryCode: "KOMPAKT",
            PickupDate: DateTime.UtcNow.Date.AddDays(5),
            ReturnDate: DateTime.UtcNow.Date.AddDays(8),
            PickupLocationCode: "BER-HBF",
            DropoffLocationCode: "BER-HBF",
            TotalPriceNet: 150.00m
        );

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_CallsRepositoryMethodsInCorrectOrder()
    {
        // Arrange
        var command = new CreateReservationCommand(
            VehicleId: Guid.NewGuid(),
            CustomerId: Guid.NewGuid(),
            CategoryCode: "KOMPAKT",
            PickupDate: DateTime.UtcNow.Date.AddDays(5),
            ReturnDate: DateTime.UtcNow.Date.AddDays(8),
            PickupLocationCode: "BER-HBF",
            DropoffLocationCode: "BER-HBF",
            TotalPriceNet: 150.00m
        );

        var callOrder = new List<string>();

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("Add"))
            .Returns(Task.CompletedTask);

        _repositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("Save"))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        callOrder.Should().HaveCount(2);
        callOrder[0].Should().Be("Add");
        callOrder[1].Should().Be("Save");
    }

    [Fact]
    public async Task HandleAsync_WithEmptyVehicleId_ThrowsArgumentException()
    {
        // Arrange
        var command = new CreateReservationCommand(
            VehicleId: Guid.Empty,
            CustomerId: Guid.NewGuid(),
            CategoryCode: "KOMPAKT",
            PickupDate: DateTime.UtcNow.Date.AddDays(5),
            ReturnDate: DateTime.UtcNow.Date.AddDays(8),
            PickupLocationCode: "BER-HBF",
            DropoffLocationCode: "BER-HBF",
            TotalPriceNet: 150.00m
        );

        // Act
        var act = async () => await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Vehicle ID cannot be empty*");
    }

    [Fact]
    public async Task HandleAsync_WithEmptyCustomerId_ThrowsArgumentException()
    {
        // Arrange
        var command = new CreateReservationCommand(
            VehicleId: Guid.NewGuid(),
            CustomerId: Guid.Empty,
            CategoryCode: "KOMPAKT",
            PickupDate: DateTime.UtcNow.Date.AddDays(5),
            ReturnDate: DateTime.UtcNow.Date.AddDays(8),
            PickupLocationCode: "BER-HBF",
            DropoffLocationCode: "BER-HBF",
            TotalPriceNet: 150.00m
        );

        // Act
        var act = async () => await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Customer ID cannot be empty*");
    }

    [Fact]
    public async Task HandleAsync_WithPickupDateInPast_ThrowsArgumentException()
    {
        // Arrange
        var command = new CreateReservationCommand(
            VehicleId: Guid.NewGuid(),
            CustomerId: Guid.NewGuid(),
            CategoryCode: "KOMPAKT",
            PickupDate: DateTime.UtcNow.Date.AddDays(-1), // Yesterday
            ReturnDate: DateTime.UtcNow.Date.AddDays(3),
            PickupLocationCode: "BER-HBF",
            DropoffLocationCode: "BER-HBF",
            TotalPriceNet: 150.00m
        );

        // Act
        var act = async () => await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Pickup date cannot be in the past*");
    }

    [Fact]
    public async Task HandleAsync_WithReturnDateBeforePickupDate_ThrowsArgumentException()
    {
        // Arrange
        var command = new CreateReservationCommand(
            VehicleId: Guid.NewGuid(),
            CustomerId: Guid.NewGuid(),
            CategoryCode: "KOMPAKT",
            PickupDate: DateTime.UtcNow.Date.AddDays(10),
            ReturnDate: DateTime.UtcNow.Date.AddDays(5), // Before pickup
            PickupLocationCode: "BER-HBF",
            DropoffLocationCode: "BER-HBF",
            TotalPriceNet: 150.00m
        );

        // Act
        var act = async () => await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Return date must be after pickup date*");
    }

    [Fact]
    public async Task HandleAsync_WithReturnDateSameAsPickupDate_ThrowsArgumentException()
    {
        // Arrange
        var pickupDate = DateTime.UtcNow.Date.AddDays(5);
        var command = new CreateReservationCommand(
            VehicleId: Guid.NewGuid(),
            CustomerId: Guid.NewGuid(),
            CategoryCode: "KOMPAKT",
            PickupDate: pickupDate,
            ReturnDate: pickupDate, // Same as pickup
            PickupLocationCode: "BER-HBF",
            DropoffLocationCode: "BER-HBF",
            TotalPriceNet: 150.00m
        );

        // Act
        var act = async () => await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Return date must be after pickup date*");
    }

    [Fact]
    public async Task HandleAsync_WithRentalPeriodOver90Days_ThrowsArgumentException()
    {
        // Arrange
        var command = new CreateReservationCommand(
            VehicleId: Guid.NewGuid(),
            CustomerId: Guid.NewGuid(),
            CategoryCode: "KOMPAKT",
            PickupDate: DateTime.UtcNow.Date.AddDays(5),
            ReturnDate: DateTime.UtcNow.Date.AddDays(100), // 95 days
            PickupLocationCode: "BER-HBF",
            DropoffLocationCode: "BER-HBF",
            TotalPriceNet: 5000.00m
        );

        // Act
        var act = async () => await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Rental period cannot exceed 90 days*");
    }

    [Fact]
    public async Task HandleAsync_WithMultipleDayRental_CalculatesCorrectPeriod()
    {
        // Arrange
        var pickupDate = DateTime.UtcNow.Date.AddDays(7);
        var returnDate = pickupDate.AddDays(6); // 7 days
        var command = new CreateReservationCommand(
            VehicleId: Guid.NewGuid(),
            CustomerId: Guid.NewGuid(),
            CategoryCode: "KOMPAKT",
            PickupDate: pickupDate,
            ReturnDate: returnDate,
            PickupLocationCode: "BER-HBF",
            DropoffLocationCode: "BER-HBF",
            TotalPriceNet: 400.00m
        );

        Reservation? capturedReservation = null;
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Callback<Reservation, CancellationToken>((res, ct) => capturedReservation = res)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        capturedReservation.Should().NotBeNull();
        capturedReservation!.Period.Days.Should().Be(7);
        capturedReservation.Period.PickupDate.Should().Be(pickupDate);
        capturedReservation.Period.ReturnDate.Should().Be(returnDate);
    }

    [Fact]
    public async Task HandleAsync_MapsResultCorrectly()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var command = new CreateReservationCommand(
            VehicleId: vehicleId,
            CustomerId: customerId,
            CategoryCode: "KOMPAKT",
            PickupDate: DateTime.UtcNow.Date.AddDays(5),
            ReturnDate: DateTime.UtcNow.Date.AddDays(8),
            PickupLocationCode: "BER-HBF",
            DropoffLocationCode: "BER-HBF",
            TotalPriceNet: 250.00m
        );

        Reservation? capturedReservation = null;
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Callback<Reservation, CancellationToken>((res, ct) => capturedReservation = res)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        capturedReservation.Should().NotBeNull();
        result.ReservationId.Should().Be(capturedReservation!.Id.Value);
        result.Status.Should().Be(capturedReservation.Status.ToString());
        result.TotalPriceNet.Should().Be(capturedReservation.TotalPrice.NetAmount);
        result.TotalPriceVat.Should().Be(capturedReservation.TotalPrice.VatAmount);
        result.TotalPriceGross.Should().Be(capturedReservation.TotalPrice.GrossAmount);
    }

    [Fact]
    public async Task HandleAsync_CreatesReservationWithPendingStatus()
    {
        // Arrange
        var command = new CreateReservationCommand(
            VehicleId: Guid.NewGuid(),
            CustomerId: Guid.NewGuid(),
            CategoryCode: "KOMPAKT",
            PickupDate: DateTime.UtcNow.Date.AddDays(5),
            ReturnDate: DateTime.UtcNow.Date.AddDays(8),
            PickupLocationCode: "BER-HBF",
            DropoffLocationCode: "BER-HBF",
            TotalPriceNet: 150.00m
        );

        Reservation? capturedReservation = null;
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Callback<Reservation, CancellationToken>((res, ct) => capturedReservation = res)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        capturedReservation.Should().NotBeNull();
        capturedReservation!.Status.Should().Be(ReservationStatus.Pending);
    }

    [Fact]
    public async Task HandleAsync_CreatesReservationWithCorrectVehicleAndCustomerId()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var command = new CreateReservationCommand(
            VehicleId: vehicleId,
            CustomerId: customerId,
            CategoryCode: "KOMPAKT",
            PickupDate: DateTime.UtcNow.Date.AddDays(5),
            ReturnDate: DateTime.UtcNow.Date.AddDays(8),
            PickupLocationCode: "BER-HBF",
            DropoffLocationCode: "BER-HBF",
            TotalPriceNet: 150.00m
        );

        Reservation? capturedReservation = null;
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Callback<Reservation, CancellationToken>((res, ct) => capturedReservation = res)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        capturedReservation.Should().NotBeNull();
        capturedReservation!.VehicleId.Should().Be(vehicleId);
        capturedReservation.CustomerId.Should().Be(customerId);
    }

    #endregion
}
