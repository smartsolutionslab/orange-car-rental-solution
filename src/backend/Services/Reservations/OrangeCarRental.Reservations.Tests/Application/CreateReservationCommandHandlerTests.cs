using Moq;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CreateReservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Tests.Application;

public class CreateReservationCommandHandlerTests
{
    private readonly CreateReservationCommandHandler handler;
    private readonly Mock<IPricingService> pricingServiceMock = new();
    private readonly Mock<IReservationRepository> repositoryMock = new();
    private readonly Mock<IReservationsUnitOfWork> unitOfWorkMock = new();

    public CreateReservationCommandHandlerTests()
    {
        unitOfWorkMock.Setup(u => u.Reservations).Returns(repositoryMock.Object);
        handler = new(unitOfWorkMock.Object, pricingServiceMock.Object);
    }

    #region HandleAsync Tests

    [Fact]
    public async Task HandleAsync_WithValidCommand_CreatesReservation()
    {
        // Arrange
        var command = new CreateReservationCommand(
            VehicleIdentifier.New(),
            CustomerIdentifier.New(),
            VehicleCategory.From("KOMPAKT"),
            BookingPeriod.Of(
                DateTime.UtcNow.Date.AddDays(7),
                DateTime.UtcNow.Date.AddDays(10)
            ),
            LocationCode.From("BER-HBF"),
            LocationCode.From("BER-HBF"),
            Money.Euro(168.07m) // Net amount (200 gross with 19% VAT)
        );

        repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.ReservationId.ShouldNotBe(Guid.Empty);
        result.Status.ShouldBe(ReservationStatus.Pending.ToString());
        result.TotalPriceNet.ShouldBe(168.07m);
        result.TotalPriceGross.ShouldBe(200.00m, 0.01m);
        result.TotalPriceVat.ShouldBe(31.93m, 0.01m);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_CallsRepositoryAddAsync()
    {
        // Arrange
        var command = new CreateReservationCommand(
            VehicleIdentifier.New(),
            CustomerIdentifier.New(),
            VehicleCategory.From("KOMPAKT"),
            BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(8)),
            LocationCode.From("BER-HBF"),
            LocationCode.From("BER-HBF"),
            Money.Euro(150.00m)
        );

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        repositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_CallsRepositorySaveChangesAsync()
    {
        // Arrange
        var command = new CreateReservationCommand(
            VehicleIdentifier.New(),
            CustomerIdentifier.New(),
            VehicleCategory.From("KOMPAKT"),
            BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(8)),
            LocationCode.From("BER-HBF"),
            LocationCode.From("BER-HBF"),
            Money.Euro(150.00m)
        );

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_CallsRepositoryMethodsInCorrectOrder()
    {
        // Arrange
        var command = new CreateReservationCommand(
            VehicleIdentifier.New(),
            CustomerIdentifier.New(),
            VehicleCategory.From("KOMPAKT"),
            BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(8)),
            LocationCode.From("BER-HBF"),
            LocationCode.From("BER-HBF"),
            Money.Euro(150.00m)
        );

        var callOrder = new List<string>();

        repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("Add"))
            .Returns(Task.CompletedTask);

        unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("Save"))
            .ReturnsAsync(1);

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        callOrder.Count.ShouldBe(2);
        callOrder[0].ShouldBe("Add");
        callOrder[1].ShouldBe("Save");
    }

    [Fact]
    public void HandleAsync_WithEmptyVehicleId_ThrowsArgumentException()
    {
        // Arrange & Act - Exception now thrown during command construction
        var act = () => new CreateReservationCommand(
            VehicleIdentifier.From(Guid.Empty),
            CustomerIdentifier.New(),
            VehicleCategory.From("KOMPAKT"),
            BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(8)),
            LocationCode.From("BER-HBF"),
            LocationCode.From("BER-HBF"),
            Money.Euro(150.00m)
        );

        // Assert
        var ex = Should.Throw<ArgumentException>(act);
        ex.Message.ShouldContain("GUID cannot be empty");
    }

    [Fact]
    public void HandleAsync_WithEmptyCustomerId_ThrowsArgumentException()
    {
        // Arrange & Act - Exception now thrown during command construction
        var act = () => new CreateReservationCommand(
            VehicleIdentifier.New(),
            CustomerIdentifier.From(Guid.Empty),
            VehicleCategory.From("KOMPAKT"),
            BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(8)),
            LocationCode.From("BER-HBF"),
            LocationCode.From("BER-HBF"),
            Money.Euro(150.00m)
        );

        // Assert
        var ex = Should.Throw<ArgumentException>(act);
        ex.Message.ShouldContain("GUID cannot be empty");
    }

    [Fact]
    public void HandleAsync_WithPickupDateInPast_ThrowsArgumentException()
    {
        // Arrange & Act
        var act = () => new CreateReservationCommand(
            VehicleIdentifier.New(),
            CustomerIdentifier.New(),
            VehicleCategory.From("KOMPAKT"),
            BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(-1), DateTime.UtcNow.Date.AddDays(3)), // Yesterday
            LocationCode.From("BER-HBF"),
            LocationCode.From("BER-HBF"),
            Money.Euro(150.00m)
        );

        // Assert
        var ex = Should.Throw<ArgumentException>(act);
        ex.Message.ShouldContain("Pickup date cannot be in the past");
    }

    [Fact]
    public void HandleAsync_WithReturnDateBeforePickupDate_ThrowsArgumentException()
    {
        // Arrange & Act
        var act = () => new CreateReservationCommand(
            VehicleIdentifier.New(),
            CustomerIdentifier.New(),
            VehicleCategory.From("KOMPAKT"),
            BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(10), DateTime.UtcNow.Date.AddDays(5)), // Before pickup
            LocationCode.From("BER-HBF"),
            LocationCode.From("BER-HBF"),
            Money.Euro(150.00m)
        );

        // Assert
        var ex = Should.Throw<ArgumentException>(act);
        ex.Message.ShouldContain("Return date must be after pickup date");
    }

    [Fact]
    public void HandleAsync_WithReturnDateSameAsPickupDate_ThrowsArgumentException()
    {
        // Arrange
        var pickupDate = DateTime.UtcNow.Date.AddDays(5);

        // Act
        var act = () => new CreateReservationCommand(
            VehicleIdentifier.New(),
            CustomerIdentifier.New(),
            VehicleCategory.From("KOMPAKT"),
            BookingPeriod.Of(pickupDate, pickupDate), // Same as pickup
            LocationCode.From("BER-HBF"),
            LocationCode.From("BER-HBF"),
            Money.Euro(150.00m)
        );

        // Assert
        var ex = Should.Throw<ArgumentException>(act);
        ex.Message.ShouldContain("Return date must be after pickup date");
    }

    [Fact]
    public void HandleAsync_WithRentalPeriodOver90Days_ThrowsArgumentException()
    {
        // Arrange & Act
        var act = () => new CreateReservationCommand(
            VehicleIdentifier.New(),
            CustomerIdentifier.New(),
            VehicleCategory.From("KOMPAKT"),
            BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(100)), // 95 days
            LocationCode.From("BER-HBF"),
            LocationCode.From("BER-HBF"),
            Money.Euro(5000.00m)
        );

        // Assert
        var ex = Should.Throw<ArgumentException>(act);
        ex.Message.ShouldContain("Rental period cannot exceed 90 days");
    }

    [Fact]
    public async Task HandleAsync_WithMultipleDayRental_CalculatesCorrectPeriod()
    {
        // Arrange
        var pickupDate = DateTime.UtcNow.Date.AddDays(7);
        var returnDate = pickupDate.AddDays(6); // 7 days
        var command = new CreateReservationCommand(
            VehicleIdentifier.New(),
            CustomerIdentifier.New(),
            VehicleCategory.From("KOMPAKT"),
            BookingPeriod.Of(pickupDate, returnDate),
            LocationCode.From("BER-HBF"),
            LocationCode.From("BER-HBF"),
            Money.Euro(400.00m)
        );

        Reservation? capturedReservation = null;
        repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Callback<Reservation, CancellationToken>((res, ct) => capturedReservation = res)
            .Returns(Task.CompletedTask);

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        capturedReservation.ShouldNotBeNull();
        capturedReservation!.Period.Days.ShouldBe(7);
        capturedReservation.Period.PickupDate.ShouldBe(DateOnly.FromDateTime(pickupDate));
        capturedReservation.Period.ReturnDate.ShouldBe(DateOnly.FromDateTime(returnDate));
    }

    [Fact]
    public async Task HandleAsync_MapsResultCorrectly()
    {
        // Arrange
        var vehicleId = VehicleIdentifier.New();
        var customerId = CustomerIdentifier.New();
        var command = new CreateReservationCommand(
            vehicleId,
            customerId,
            VehicleCategory.From("KOMPAKT"),
            BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(8)),
            LocationCode.From("BER-HBF"),
            LocationCode.From("BER-HBF"),
            Money.Euro(250.00m)
        );

        Reservation? capturedReservation = null;
        repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Callback<Reservation, CancellationToken>((res, ct) => capturedReservation = res)
            .Returns(Task.CompletedTask);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        capturedReservation.ShouldNotBeNull();
        result.ReservationId.ShouldBe(capturedReservation!.Id.Value);
        result.Status.ShouldBe(capturedReservation.Status.ToString());
        result.TotalPriceNet.ShouldBe(capturedReservation.TotalPrice.NetAmount);
        result.TotalPriceVat.ShouldBe(capturedReservation.TotalPrice.VatAmount);
        result.TotalPriceGross.ShouldBe(capturedReservation.TotalPrice.GrossAmount);
    }

    [Fact]
    public async Task HandleAsync_CreatesReservationWithPendingStatus()
    {
        // Arrange
        var command = new CreateReservationCommand(
            VehicleIdentifier.New(),
            CustomerIdentifier.New(),
            VehicleCategory.From("KOMPAKT"),
            BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(8)),
            LocationCode.From("BER-HBF"),
            LocationCode.From("BER-HBF"),
            Money.Euro(150.00m)
        );

        Reservation? capturedReservation = null;
        repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Callback<Reservation, CancellationToken>((res, ct) => capturedReservation = res)
            .Returns(Task.CompletedTask);

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        capturedReservation.ShouldNotBeNull();
        capturedReservation!.Status.ShouldBe(ReservationStatus.Pending);
    }

    [Fact]
    public async Task HandleAsync_CreatesReservationWithCorrectVehicleAndCustomerId()
    {
        // Arrange
        var vehicleId = VehicleIdentifier.New();
        var customerId = CustomerIdentifier.New();
        var command = new CreateReservationCommand(
            vehicleId,
            customerId,
            VehicleCategory.From("KOMPAKT"),
            BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(8)),
            LocationCode.From("BER-HBF"),
            LocationCode.From("BER-HBF"),
            Money.Euro(150.00m)
        );

        Reservation? capturedReservation = null;
        repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Callback<Reservation, CancellationToken>((res, ct) => capturedReservation = res)
            .Returns(Task.CompletedTask);

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        capturedReservation.ShouldNotBeNull();
        capturedReservation!.VehicleIdentifier.Value.ShouldBe(vehicleId);
        capturedReservation.CustomerIdentifier.Value.ShouldBe(customerId);
    }

    #endregion
}
