using Moq;
using Shouldly;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Testing;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Tests.Application.Commands;

public class CreateReservationCommandHandlerTests
{
    private readonly Mock<IReservationRepository> repositoryMock = new();
    private readonly Mock<IPricingService> pricingServiceMock = new();
    private readonly Mock<IUnitOfWork> unitOfWorkMock = new();
    private readonly CreateReservationCommandHandler handler;

    public CreateReservationCommandHandlerTests()
    {
        handler = new CreateReservationCommandHandler(
            repositoryMock.Object,
            pricingServiceMock.Object,
            unitOfWorkMock.Object);
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

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.ReservationId.ShouldNotBe(Guid.Empty);
        result.Status.ShouldBe("Pending");
        result.TotalPriceNet.ShouldBe(250.00m);

        repositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<Reservation>(),
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
    public async Task HandleAsync_ShouldCallRepository()
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

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        repositoryMock.Verify(
            x => x.AddAsync(
                It.Is<Reservation>(r =>
                    r.VehicleIdentifier == command.VehicleIdentifier &&
                    r.CustomerIdentifier == command.CustomerIdentifier &&
                    r.PickupLocationCode == command.PickupLocationCode &&
                    r.DropoffLocationCode == command.DropoffLocationCode),
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
        var pickupLocation = LocationCode.From(TestLocations.BerlinHbf);
        var dropoffLocation = LocationCode.From(TestLocations.MunichAirport);

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

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        repositoryMock.Verify(
            x => x.AddAsync(
                It.Is<Reservation>(r =>
                    r.PickupLocationCode == pickupLocation &&
                    r.DropoffLocationCode == dropoffLocation),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static CreateReservationCommand CreateValidCommand()
    {
        var (pickup, returnDate) = TestDates.RentalPeriod();

        return new CreateReservationCommand(
            VehicleIdentifier.From(TestIds.Vehicle1),
            CustomerIdentifier.From(TestIds.Customer1),
            VehicleCategory.SUV,
            BookingPeriod.Of(pickup, returnDate),
            LocationCode.From(TestLocations.BerlinHbf),
            LocationCode.From(TestLocations.BerlinHbf),
            null // TotalPrice will be calculated
        );
    }
}
