using Moq;
using Shouldly;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CreateReservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Tests.Application.Commands;

public class CreateReservationCommandHandlerTests
{
    private readonly Mock<IReservationRepository> _reservationRepositoryMock;
    private readonly Mock<IPricingService> _pricingServiceMock;
    private readonly CreateReservationCommandHandler _handler;

    public CreateReservationCommandHandlerTests()
    {
        _reservationRepositoryMock = new Mock<IReservationRepository>();
        _pricingServiceMock = new Mock<IPricingService>();
        _handler = new CreateReservationCommandHandler(
            _reservationRepositoryMock.Object,
            _pricingServiceMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_ShouldCreateReservation()
    {
        // Arrange
        var command = CreateValidCommand();

        _pricingServiceMock
            .Setup(x => x.CalculatePriceAsync(
                It.IsAny<VehicleCategory>(),
                It.IsAny<BookingPeriod>(),
                It.IsAny<LocationCode>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PriceCalculationResult(250.00m, 47.50m, 297.50m));

        Reservation? addedReservation = null;
        _reservationRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Callback<Reservation, CancellationToken>((reservation, _) => addedReservation = reservation)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.ReservationId.ShouldNotBe(Guid.Empty);
        result.Status.ShouldBe("Pending");
        result.NetAmount.ShouldBe(250.00m);
        result.VatAmount.ShouldBe(47.50m);
        result.GrossAmount.ShouldBe(297.50m);

        addedReservation.ShouldNotBeNull();
        addedReservation.VehicleId.ShouldBe(command.VehicleId.Value);
        addedReservation.CustomerId.ShouldBe(command.CustomerId.Value);
        addedReservation.Period.ShouldBe(command.Period);
        addedReservation.PickupLocationCode.ShouldBe(command.PickupLocationCode);
        addedReservation.DropoffLocationCode.ShouldBe(command.DropoffLocationCode);
        addedReservation.Status.ShouldBe(ReservationStatus.Pending);

        _reservationRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _reservationRepositoryMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
        _pricingServiceMock.Verify(
            x => x.CalculatePriceAsync(
                It.IsAny<VehicleCategory>(),
                It.IsAny<BookingPeriod>(),
                It.IsAny<LocationCode>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithProvidedPrice_ShouldUseProvidedPrice()
    {
        // Arrange
        var providedPrice = Money.Of(350.00m, Currency.EUR);
        var command = CreateValidCommand() with { TotalPrice = providedPrice };

        Reservation? addedReservation = null;
        _reservationRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Callback<Reservation, CancellationToken>((reservation, _) => addedReservation = reservation)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.NetAmount.ShouldBe(providedPrice.NetAmount);
        result.VatAmount.ShouldBe(providedPrice.VatAmount);
        result.GrossAmount.ShouldBe(providedPrice.GrossAmount);

        addedReservation.ShouldNotBeNull();
        addedReservation.TotalPrice.ShouldBe(providedPrice);

        // Should NOT call pricing service when price is provided
        _pricingServiceMock.Verify(
            x => x.CalculatePriceAsync(
                It.IsAny<VehicleCategory>(),
                It.IsAny<BookingPeriod>(),
                It.IsAny<LocationCode>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WithoutProvidedPrice_ShouldCalculatePrice()
    {
        // Arrange
        var command = CreateValidCommand() with { TotalPrice = null };

        var calculatedPrice = new PriceCalculationResult(200.00m, 38.00m, 238.00m);
        _pricingServiceMock
            .Setup(x => x.CalculatePriceAsync(
                It.IsAny<VehicleCategory>(),
                It.IsAny<BookingPeriod>(),
                It.IsAny<LocationCode>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(calculatedPrice);

        Reservation? addedReservation = null;
        _reservationRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Callback<Reservation, CancellationToken>((reservation, _) => addedReservation = reservation)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.NetAmount.ShouldBe(200.00m);

        addedReservation.ShouldNotBeNull();
        addedReservation.TotalPrice.NetAmount.ShouldBe(200.00m);

        _pricingServiceMock.Verify(
            x => x.CalculatePriceAsync(
                command.CategoryCode,
                command.Period,
                command.PickupLocationCode,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldCallSaveChanges()
    {
        // Arrange
        var command = CreateValidCommand();

        _pricingServiceMock
            .Setup(x => x.CalculatePriceAsync(
                It.IsAny<VehicleCategory>(),
                It.IsAny<BookingPeriod>(),
                It.IsAny<LocationCode>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PriceCalculationResult(250.00m, 47.50m, 297.50m));

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _reservationRepositoryMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var command = CreateValidCommand();
        var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        _pricingServiceMock
            .Setup(x => x.CalculatePriceAsync(
                It.IsAny<VehicleCategory>(),
                It.IsAny<BookingPeriod>(),
                It.IsAny<LocationCode>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(() =>
            _handler.HandleAsync(command, cts.Token));
    }

    [Fact]
    public async Task HandleAsync_WithDifferentLocations_ShouldCreateCorrectly()
    {
        // Arrange
        var pickupLocation = LocationCode.Of("BER-HBF");
        var dropoffLocation = LocationCode.Of("MUC-FLG");

        var command = CreateValidCommand() with
        {
            PickupLocationCode = pickupLocation,
            DropoffLocationCode = dropoffLocation
        };

        _pricingServiceMock
            .Setup(x => x.CalculatePriceAsync(
                It.IsAny<VehicleCategory>(),
                It.IsAny<BookingPeriod>(),
                It.IsAny<LocationCode>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PriceCalculationResult(250.00m, 47.50m, 297.50m));

        Reservation? addedReservation = null;
        _reservationRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Callback<Reservation, CancellationToken>((reservation, _) => addedReservation = reservation)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        addedReservation.ShouldNotBeNull();
        addedReservation.PickupLocationCode.ShouldBe(pickupLocation);
        addedReservation.DropoffLocationCode.ShouldBe(dropoffLocation);
    }

    private static CreateReservationCommand CreateValidCommand()
    {
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        var returnDate = pickupDate.AddDays(3);

        return new CreateReservationCommand(
            VehicleIdentifier.New(),
            CustomerIdentifier.New(),
            VehicleCategory.SUV,
            BookingPeriod.Of(pickupDate, returnDate),
            LocationCode.Of("BER-HBF"),
            LocationCode.Of("BER-HBF"),
            null // TotalPrice will be calculated
        );
    }
}
