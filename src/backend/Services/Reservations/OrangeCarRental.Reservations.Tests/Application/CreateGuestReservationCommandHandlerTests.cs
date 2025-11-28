using Moq;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CreateGuestReservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Tests.Application;

public class CreateGuestReservationCommandHandlerTests
{
    private readonly Mock<ICustomersService> customersServiceMock = new();
    private readonly CreateGuestReservationCommandHandler handler;
    private readonly Mock<IPricingService> pricingServiceMock = new();
    private readonly Mock<IReservationRepository> repositoryMock = new();

    public CreateGuestReservationCommandHandlerTests()
    {
        handler = new CreateGuestReservationCommandHandler(
            customersServiceMock.Object,
            repositoryMock.Object,
            pricingServiceMock.Object);
    }

    #region Happy Path Tests

    [Fact]
    public async Task HandleAsync_WithValidCommand_CreatesGuestReservation()
    {
        // Arrange
        var command = CreateValidCommand();
        var customerId = Guid.NewGuid();

        customersServiceMock
            .Setup(s => s.RegisterCustomerAsync(It.IsAny<RegisterCustomerDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(customerId);

        pricingServiceMock
            .Setup(s => s.CalculatePriceAsync(
                It.IsAny<VehicleCategory>(),
                It.IsAny<BookingPeriod>(),
                It.IsAny<LocationCode>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PriceCalculationDto(
                "KOMPAKT",
                3,
                56.02m,
                66.67m,
                168.07m,
                200.00m,
                31.93m,
                0.19m,
                "EUR"));

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
        result.CustomerId.ShouldBe(customerId);
        result.ReservationId.ShouldNotBe(Guid.Empty);
        result.TotalPriceNet.ShouldBe(168.07m);
        result.TotalPriceVat.ShouldBe(31.93m, 0.01m);
        result.TotalPriceGross.ShouldBe(200.00m, 0.01m);
        result.Currency.ShouldBe("EUR");
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_CallsCustomersServiceRegisterAsync()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupDefaultMocks();

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        customersServiceMock.Verify(
            s => s.RegisterCustomerAsync(
                It.Is<RegisterCustomerDto>(dto =>
                    dto.FirstName == command.Name.FirstName.Value &&
                    dto.LastName == command.Name.LastName.Value &&
                    dto.Email == command.Email.Value &&
                    dto.PhoneNumber == command.PhoneNumber.Value &&
                    dto.DateOfBirth == command.DateOfBirth.Value &&
                    dto.Street == command.Address.Street &&
                    dto.City == command.Address.City.Value &&
                    dto.PostalCode == command.Address.PostalCode.Value &&
                    dto.Country == command.Address.Country &&
                    dto.LicenseNumber == command.DriversLicense.LicenseNumber &&
                    dto.LicenseIssueCountry == command.DriversLicense.IssueCountry &&
                    dto.LicenseIssueDate == command.DriversLicense.IssueDate &&
                    dto.LicenseExpiryDate == command.DriversLicense.ExpiryDate),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_CallsPricingServiceCalculatePriceAsync()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupDefaultMocks();

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        pricingServiceMock.Verify(
            s => s.CalculatePriceAsync(
                command.CategoryCode,
                command.Period,
                command.PickupLocationCode,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_CallsRepositoryAddAsync()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupDefaultMocks();

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
        var command = CreateValidCommand();
        SetupDefaultMocks();

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        repositoryMock.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_CallsServicesInCorrectOrder()
    {
        // Arrange
        var command = CreateValidCommand();
        var callOrder = new List<string>();

        customersServiceMock
            .Setup(s => s.RegisterCustomerAsync(It.IsAny<RegisterCustomerDto>(), It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("RegisterCustomer"))
            .ReturnsAsync(Guid.NewGuid());

        pricingServiceMock
            .Setup(s => s.CalculatePriceAsync(
                It.IsAny<VehicleCategory>(),
                It.IsAny<BookingPeriod>(),
                It.IsAny<LocationCode>(),
                It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("CalculatePrice"))
            .ReturnsAsync(new PriceCalculationDto(
                "KOMPAKT",
                3,
                50.00m,
                59.50m,
                150.00m,
                178.50m,
                28.50m,
                0.19m,
                "EUR"));

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
        callOrder.Count.ShouldBe(4);
        callOrder[0].ShouldBe("RegisterCustomer");
        callOrder[1].ShouldBe("CalculatePrice");
        callOrder[2].ShouldBe("Add");
        callOrder[3].ShouldBe("Save");
    }

    [Fact]
    public async Task HandleAsync_CreatesReservationWithCorrectCustomerId()
    {
        // Arrange
        var command = CreateValidCommand();
        var customerId = Guid.NewGuid();

        customersServiceMock
            .Setup(s => s.RegisterCustomerAsync(It.IsAny<RegisterCustomerDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(customerId);

        pricingServiceMock
            .Setup(s => s.CalculatePriceAsync(
                It.IsAny<VehicleCategory>(),
                It.IsAny<BookingPeriod>(),
                It.IsAny<LocationCode>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PriceCalculationDto(
                "KOMPAKT",
                3,
                50.00m,
                59.50m,
                150.00m,
                178.50m,
                28.50m,
                0.19m,
                "EUR"));

        Reservation? capturedReservation = null;
        repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Callback<Reservation, CancellationToken>((res, ct) => capturedReservation = res)
            .Returns(Task.CompletedTask);

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        capturedReservation.ShouldNotBeNull();
        capturedReservation!.CustomerIdentifier.Value.ShouldBe(customerId);
    }

    [Fact]
    public async Task HandleAsync_CreatesReservationWithCorrectVehicleId()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupDefaultMocks();

        Reservation? capturedReservation = null;
        repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Callback<Reservation, CancellationToken>((res, ct) => capturedReservation = res)
            .Returns(Task.CompletedTask);

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        capturedReservation.ShouldNotBeNull();
        capturedReservation!.VehicleIdentifier.ShouldBe(command.VehicleIdentifier);
    }

    [Fact]
    public async Task HandleAsync_CreatesReservationWithPendingStatus()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupDefaultMocks();

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
    public async Task HandleAsync_CreatesReservationWithCorrectDates()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupDefaultMocks();

        Reservation? capturedReservation = null;
        repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Callback<Reservation, CancellationToken>((res, ct) => capturedReservation = res)
            .Returns(Task.CompletedTask);

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        capturedReservation.ShouldNotBeNull();
        capturedReservation!.Period.PickupDate.ShouldBe(command.Period.PickupDate);
        capturedReservation.Period.ReturnDate.ShouldBe(command.Period.ReturnDate);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task HandleAsync_WhenCustomersServiceFails_ThrowsException()
    {
        // Arrange
        var command = CreateValidCommand();

        customersServiceMock
            .Setup(s => s.RegisterCustomerAsync(It.IsAny<RegisterCustomerDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Failed to register customer via Customers API"));

        // Act & Assert
        var ex = await Should.ThrowAsync<InvalidOperationException>(async () => await handler.HandleAsync(command, CancellationToken.None));
        ex.Message.ShouldContain("Failed to register customer");
    }

    [Fact]
    public async Task HandleAsync_WhenPricingServiceFails_ThrowsException()
    {
        // Arrange
        var command = CreateValidCommand();

        customersServiceMock
            .Setup(s => s.RegisterCustomerAsync(It.IsAny<RegisterCustomerDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        pricingServiceMock
            .Setup(s => s.CalculatePriceAsync(
                It.IsAny<VehicleCategory>(),
                It.IsAny<BookingPeriod>(),
                It.IsAny<LocationCode>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Failed to calculate price via Pricing API"));

        // Act & Assert
        var ex = await Should.ThrowAsync<InvalidOperationException>(async () => await handler.HandleAsync(command, CancellationToken.None));
        ex.Message.ShouldContain("Failed to calculate price");
    }

    [Fact]
    public async Task HandleAsync_WhenCustomersServiceFails_DoesNotCallPricingService()
    {
        // Arrange
        var command = CreateValidCommand();

        customersServiceMock
            .Setup(s => s.RegisterCustomerAsync(It.IsAny<RegisterCustomerDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Failed to register customer"));

        // Act
        try
        {
            await handler.HandleAsync(command, CancellationToken.None);
        }
        catch
        {
            // Expected exception
        }

        // Assert
        pricingServiceMock.Verify(
            s => s.CalculatePriceAsync(
                It.IsAny<VehicleCategory>(),
                It.IsAny<BookingPeriod>(),
                It.IsAny<LocationCode>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WhenPricingServiceFails_DoesNotCallRepository()
    {
        // Arrange
        var command = CreateValidCommand();

        customersServiceMock
            .Setup(s => s.RegisterCustomerAsync(It.IsAny<RegisterCustomerDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        pricingServiceMock
            .Setup(s => s.CalculatePriceAsync(
                It.IsAny<VehicleCategory>(),
                It.IsAny<BookingPeriod>(),
                It.IsAny<LocationCode>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Failed to calculate price"));

        // Act
        try
        {
            await handler.HandleAsync(command, CancellationToken.None);
        }
        catch
        {
            // Expected exception
        }

        // Assert
        repositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()),
            Times.Never);

        repositoryMock.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public void HandleAsync_WithEmptyVehicleId_ThrowsArgumentException()
    {
        // Arrange & Act - Exception now thrown during value object construction
        var act = () => CreateValidCommand() with { VehicleIdentifier = VehicleIdentifier.From(Guid.Empty) };

        // Assert
        var ex = Should.Throw<ArgumentException>(act);
        ex.Message.ShouldContain("Vehicle ID cannot be empty");
    }

    [Fact]
    public void HandleAsync_WithPickupDateInPast_ThrowsArgumentException()
    {
        // Arrange & Act
        var act = () => CreateValidCommand() with { Period = BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(-1), DateTime.UtcNow.Date.AddDays(3)) };

        // Assert
        var ex = Should.Throw<ArgumentException>(act);
        ex.Message.ShouldContain("Pickup date cannot be in the past");
    }

    [Fact]
    public void HandleAsync_WithReturnDateBeforePickupDate_ThrowsArgumentException()
    {
        // Arrange & Act
        var act = () => CreateValidCommand() with
        {
            Period = BookingPeriod.Of(
                DateTime.UtcNow.Date.AddDays(10),
                DateTime.UtcNow.Date.AddDays(5)
            )
        };

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
        var act = () => CreateValidCommand() with { Period = BookingPeriod.Of(pickupDate, pickupDate) };

        // Assert
        var ex = Should.Throw<ArgumentException>(act);
        ex.Message.ShouldContain("Return date must be after pickup date");
    }

    [Fact]
    public void HandleAsync_WithRentalPeriodOver90Days_ThrowsArgumentException()
    {
        // Arrange & Act
        var act = () => CreateValidCommand() with
        {
            Period = BookingPeriod.Of(
                DateTime.UtcNow.Date.AddDays(5),
                DateTime.UtcNow.Date.AddDays(100)
            ) // 95 days
        };

        // Assert
        var ex = Should.Throw<ArgumentException>(act);
        ex.Message.ShouldContain("Rental period cannot exceed 90 days");
    }

    #endregion

    #region Helper Methods

    private CreateGuestReservationCommand CreateValidCommand()
    {
        return new CreateGuestReservationCommand(
            VehicleIdentifier.New(),
            VehicleCategory.From("KOMPAKT"),
            BookingPeriod.Of(
                DateTime.UtcNow.Date.AddDays(7),
                DateTime.UtcNow.Date.AddDays(10)
            ),
            LocationCode.From("BER-HBF"),
            LocationCode.From("BER-HBF"),
            CustomerName.Of("Max", "Mustermann"),
            Email.From("max.mustermann@example.com"),
            PhoneNumber.From("+49 30 12345678"),
            BirthDate.Of(1990, 1, 15),
            Address.Of("Musterstraße 123", "Berlin", "10115", "Germany"),
            DriversLicense.Of(
                "B123456789",
                "Germany",
                new DateOnly(2010, 6, 1),
                new DateOnly(2030, 6, 1)
            )
        );
    }

    private void SetupDefaultMocks()
    {
        customersServiceMock
            .Setup(s => s.RegisterCustomerAsync(It.IsAny<RegisterCustomerDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        pricingServiceMock
            .Setup(s => s.CalculatePriceAsync(
                It.IsAny<VehicleCategory>(),
                It.IsAny<BookingPeriod>(),
                It.IsAny<LocationCode>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PriceCalculationDto(
                "KOMPAKT",
                3,
                50.00m,
                59.50m,
                150.00m,
                178.50m,
                28.50m,
                0.19m,
                "EUR"));

        repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        repositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    #endregion
}
