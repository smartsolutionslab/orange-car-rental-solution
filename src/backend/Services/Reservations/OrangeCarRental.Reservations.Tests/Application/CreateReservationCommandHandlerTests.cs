using Moq;
using Shouldly;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CreateReservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Tests.Application;

public class CreateReservationCommandHandlerTests
{
    private readonly CreateReservationCommandHandler handler;
    private readonly Mock<IPricingService> pricingServiceMock = new();
    private readonly Mock<IReservationRepository> repositoryMock = new();

    public CreateReservationCommandHandlerTests()
    {
        handler = new(repositoryMock.Object, pricingServiceMock.Object);
    }

    #region HandleAsync Tests

    [Fact]
    public async Task HandleAsync_WithValidCommand_CreatesReservation()
    {
        // Arrange
        var command = new CreateReservationCommand(
            VehicleIdentifier.New(),
            CustomerIdentifier.New(),
            VehicleCategory.FromCode("KOMPAKT"),
            BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(7), DateTime.UtcNow.Date.AddDays(10)),
            LocationCode.Of("BER-HBF"),
            LocationCode.Of("BER-HBF"),
            Money.Euro(168.07m) // Net amount (200 gross with 19% VAT)
        );

        repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        repositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

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
            VehicleCategory.FromCode("KOMPAKT"),
            BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(8)),
            LocationCode.Of("BER-HBF"),
            LocationCode.Of("BER-HBF"),
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
            VehicleCategory.FromCode("KOMPAKT"),
            BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(8)),
            LocationCode.Of("BER-HBF"),
            LocationCode.Of("BER-HBF"),
            Money.Euro(150.00m)
        );

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        repositoryMock.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_CallsRepositoryMethodsInCorrectOrder()
    {
        // Arrange
        var command = new CreateReservationCommand(
            VehicleIdentifier.New(),
            CustomerIdentifier.New(),
            VehicleCategory.FromCode("KOMPAKT"),
            BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(8)),
            LocationCode.Of("BER-HBF"),
            LocationCode.Of("BER-HBF"),
            Money.Euro(150.00m)
        );

        var callOrder = new List<string>();

        repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("Add"))
            .Returns(Task.CompletedTask);

        repositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("Save"))
            .Returns(Task.CompletedTask);

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
            VehicleIdentifier.New(),
            CustomerIdentifier.New(),
            VehicleCategory.FromCode("KOMPAKT"),
            BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(8)),
            LocationCode.Of("BER-HBF"),
            LocationCode.Of("BER-HBF"),
            Money.Euro(150.00m)
        );

        // Assert
        var ex = Should.Throw<ArgumentException>(act);
        ex.Message.ShouldContain("must not be equal to");
        ex.Message.ShouldContain("00000000-0000-0000-0000-000000000000");
    }

    [Fact]
    public async Task HandleAsync_WithEmptyCustomerId_ThrowsArgumentException()
    {
        // Arrange
        var command = new CreateReservationCommand(
            VehicleIdentifier.New(),
            CustomerIdentifier.From(Guid.Empty),
            VehicleCategory.FromCode("KOMPAKT"),
            BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(8)),
            LocationCode.Of("BER-HBF"),
            LocationCode.Of("BER-HBF"),
            Money.Euro(150.00m)
        );

        // Act & Assert
        var ex = await Should.ThrowAsync<ArgumentException>(async () => await handler.HandleAsync(command, CancellationToken.None));
        ex.Message.ShouldContain("GUID cannot be empty");
    }

    [Fact]
    public void HandleAsync_WithPickupDateInPast_ThrowsArgumentException()
    {
        // Arrange & Act
        var act = () => new CreateReservationCommand(
            VehicleIdentifier.New(),
            CustomerIdentifier.New(),
            VehicleCategory.FromCode("KOMPAKT"),
            BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(-1), DateTime.UtcNow.Date.AddDays(3)), // Yesterday
            LocationCode.Of("BER-HBF"),
            LocationCode.Of("BER-HBF"),
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
            VehicleCategory.FromCode("KOMPAKT"),
            BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(10), DateTime.UtcNow.Date.AddDays(5)), // Before pickup
            LocationCode.Of("BER-HBF"),
            LocationCode.Of("BER-HBF"),
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
            VehicleCategory.FromCode("KOMPAKT"),
            BookingPeriod.Of(pickupDate, pickupDate), // Same as pickup
            LocationCode.Of("BER-HBF"),
            LocationCode.Of("BER-HBF"),
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
            VehicleCategory.FromCode("KOMPAKT"),
            BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(100)), // 95 days
            LocationCode.Of("BER-HBF"),
            LocationCode.Of("BER-HBF"),
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
            VehicleCategory.FromCode("KOMPAKT"),
            BookingPeriod.Of(pickupDate, returnDate),
            LocationCode.Of("BER-HBF"),
            LocationCode.Of("BER-HBF"),
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
        var vehicleId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var command = new CreateReservationCommand(
            VehicleIdentifier.From(vehicleId),
            CustomerIdentifier.From(customerId),
            VehicleCategory.FromCode("KOMPAKT"),
            BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(8)),
            LocationCode.Of("BER-HBF"),
            LocationCode.Of("BER-HBF"),
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
            VehicleCategory.FromCode("KOMPAKT"),
            BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(8)),
            LocationCode.Of("BER-HBF"),
            LocationCode.Of("BER-HBF"),
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
        var vehicleId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var command = new CreateReservationCommand(
            VehicleIdentifier.From(vehicleId),
            CustomerIdentifier.From(customerId),
            VehicleCategory.FromCode("KOMPAKT"),
            BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(8)),
            LocationCode.Of("BER-HBF"),
            LocationCode.Of("BER-HBF"),
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
        capturedReservation!.VehicleId.ShouldBe(vehicleId);
        capturedReservation.CustomerId.ShouldBe(customerId);
    }

    #endregion
}
