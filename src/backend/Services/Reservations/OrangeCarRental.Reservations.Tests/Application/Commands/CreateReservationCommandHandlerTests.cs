using Moq;
using Shouldly;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CreateReservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Tests.Application.Commands;

public class CreateReservationCommandHandlerTests
{
    private readonly Mock<IReservationCommandService> commandServiceMock = new();
    private readonly Mock<IPricingService> pricingServiceMock = new();
    private readonly CreateReservationCommandHandler handler;

    public CreateReservationCommandHandlerTests()
    {
        handler = new CreateReservationCommandHandler(commandServiceMock.Object, pricingServiceMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_ShouldCreateReservation()
    {
        // Arrange
        var command = CreateValidCommand();

        pricingServiceMock
            .Setup(x => x.CalculatePriceAsync(
                It.IsAny<VehicleCategory>(),
                It.IsAny<BookingPeriod>(),
                It.IsAny<LocationCode>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PriceCalculationDto(
                "SUV",
                3,
                83.33m,
                99.16m,
                250.00m,
                297.50m,
                47.50m,
                0.19m,
                "EUR"));

        var createdReservation = CreateReservationFromCommand(command, Money.Euro(250.00m));
        commandServiceMock
            .Setup(x => x.CreateAsync(
                It.IsAny<VehicleIdentifier>(),
                It.IsAny<CustomerIdentifier>(),
                It.IsAny<BookingPeriod>(),
                It.IsAny<LocationCode>(),
                It.IsAny<LocationCode>(),
                It.IsAny<Money>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdReservation);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.ReservationId.ShouldNotBe(Guid.Empty);
        result.Status.ShouldBe("Pending");
        result.TotalPriceNet.ShouldBe(250.00m);

        commandServiceMock.Verify(
            x => x.CreateAsync(
                It.IsAny<VehicleIdentifier>(),
                It.IsAny<CustomerIdentifier>(),
                It.IsAny<BookingPeriod>(),
                It.IsAny<LocationCode>(),
                It.IsAny<LocationCode>(),
                It.IsAny<Money>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
        pricingServiceMock.Verify(
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
        var providedPrice = Money.Euro(350.00m);
        var command = CreateValidCommand() with { TotalPrice = providedPrice };

        var createdReservation = CreateReservationFromCommand(command, providedPrice);
        commandServiceMock
            .Setup(x => x.CreateAsync(
                It.IsAny<VehicleIdentifier>(),
                It.IsAny<CustomerIdentifier>(),
                It.IsAny<BookingPeriod>(),
                It.IsAny<LocationCode>(),
                It.IsAny<LocationCode>(),
                providedPrice,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdReservation);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.TotalPriceNet.ShouldBe(providedPrice.NetAmount);

        // Should NOT call pricing service when price is provided
        pricingServiceMock.Verify(
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

        var calculatedPrice = new PriceCalculationDto(
            "SUV",
            3,
            66.67m,
            79.33m,
            200.00m,
            238.00m,
            38.00m,
            0.19m,
            "EUR");

        pricingServiceMock
            .Setup(x => x.CalculatePriceAsync(
                It.IsAny<VehicleCategory>(),
                It.IsAny<BookingPeriod>(),
                It.IsAny<LocationCode>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(calculatedPrice);

        var createdReservation = CreateReservationFromCommand(command, Money.Euro(200.00m));
        commandServiceMock
            .Setup(x => x.CreateAsync(
                It.IsAny<VehicleIdentifier>(),
                It.IsAny<CustomerIdentifier>(),
                It.IsAny<BookingPeriod>(),
                It.IsAny<LocationCode>(),
                It.IsAny<LocationCode>(),
                It.IsAny<Money>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdReservation);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.TotalPriceNet.ShouldBe(200.00m);

        pricingServiceMock.Verify(
            x => x.CalculatePriceAsync(
                command.CategoryCode,
                command.Period,
                command.PickupLocationCode,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldCallCommandService()
    {
        // Arrange
        var command = CreateValidCommand();

        pricingServiceMock
            .Setup(x => x.CalculatePriceAsync(
                It.IsAny<VehicleCategory>(),
                It.IsAny<BookingPeriod>(),
                It.IsAny<LocationCode>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PriceCalculationDto(
                "SUV",
                3,
                83.33m,
                99.16m,
                250.00m,
                297.50m,
                47.50m,
                0.19m,
                "EUR"));

        var createdReservation = CreateReservationFromCommand(command, Money.Euro(250.00m));
        commandServiceMock
            .Setup(x => x.CreateAsync(
                It.IsAny<VehicleIdentifier>(),
                It.IsAny<CustomerIdentifier>(),
                It.IsAny<BookingPeriod>(),
                It.IsAny<LocationCode>(),
                It.IsAny<LocationCode>(),
                It.IsAny<Money>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdReservation);

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        commandServiceMock.Verify(
            x => x.CreateAsync(
                command.VehicleIdentifier,
                command.CustomerIdentifier,
                command.Period,
                command.PickupLocationCode,
                command.DropoffLocationCode,
                It.IsAny<Money>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var command = CreateValidCommand();
        var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        pricingServiceMock
            .Setup(x => x.CalculatePriceAsync(
                It.IsAny<VehicleCategory>(),
                It.IsAny<BookingPeriod>(),
                It.IsAny<LocationCode>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(() =>
            handler.HandleAsync(command, cts.Token));
    }

    [Fact]
    public async Task HandleAsync_WithDifferentLocations_ShouldCreateCorrectly()
    {
        // Arrange
        var pickupLocation = LocationCode.From("BER-HBF");
        var dropoffLocation = LocationCode.From("MUC-FLG");

        var command = CreateValidCommand() with
        {
            PickupLocationCode = pickupLocation,
            DropoffLocationCode = dropoffLocation
        };

        pricingServiceMock
            .Setup(x => x.CalculatePriceAsync(
                It.IsAny<VehicleCategory>(),
                It.IsAny<BookingPeriod>(),
                It.IsAny<LocationCode>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PriceCalculationDto(
                "SUV",
                3,
                83.33m,
                99.16m,
                250.00m,
                297.50m,
                47.50m,
                0.19m,
                "EUR"));

        var createdReservation = CreateReservationFromCommand(command, Money.Euro(250.00m));
        commandServiceMock
            .Setup(x => x.CreateAsync(
                It.IsAny<VehicleIdentifier>(),
                It.IsAny<CustomerIdentifier>(),
                It.IsAny<BookingPeriod>(),
                pickupLocation,
                dropoffLocation,
                It.IsAny<Money>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdReservation);

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        commandServiceMock.Verify(
            x => x.CreateAsync(
                It.IsAny<VehicleIdentifier>(),
                It.IsAny<CustomerIdentifier>(),
                It.IsAny<BookingPeriod>(),
                pickupLocation,
                dropoffLocation,
                It.IsAny<Money>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
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
            LocationCode.From("BER-HBF"),
            LocationCode.From("BER-HBF"),
            null // TotalPrice will be calculated
        );
    }

    private static Reservation CreateReservationFromCommand(CreateReservationCommand command, Money totalPrice)
    {
        var reservation = new Reservation();
        reservation.Create(
            command.VehicleIdentifier,
            command.CustomerIdentifier,
            command.Period,
            command.PickupLocationCode,
            command.DropoffLocationCode,
            totalPrice);
        return reservation;
    }
}
