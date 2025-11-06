using Moq;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CreateGuestReservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Tests.Application;

public class CreateGuestReservationCommandHandlerTests
{
    private readonly Mock<ICustomersService> _customersServiceMock = new();
    private readonly CreateGuestReservationCommandHandler _handler;
    private readonly Mock<IPricingService> _pricingServiceMock = new();
    private readonly Mock<IReservationRepository> _repositoryMock = new();

    public CreateGuestReservationCommandHandlerTests()
    {
        _handler = new CreateGuestReservationCommandHandler(
            _customersServiceMock.Object,
            _repositoryMock.Object,
            _pricingServiceMock.Object);
    }

    #region Happy Path Tests

    [Fact]
    public async Task HandleAsync_WithValidCommand_CreatesGuestReservation()
    {
        // Arrange
        var command = CreateValidCommand();
        var customerId = Guid.NewGuid();

        _customersServiceMock
            .Setup(s => s.RegisterCustomerAsync(It.IsAny<RegisterCustomerDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(customerId);

        _pricingServiceMock
            .Setup(s => s.CalculatePriceAsync(
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PriceCalculationDto
            {
                CategoryCode = "KOMPAKT",
                TotalDays = 3,
                DailyRateNet = 56.02m,
                DailyRateGross = 66.67m,
                TotalPriceNet = 168.07m,
                TotalPriceGross = 200.00m,
                VatAmount = 31.93m,
                VatRate = 0.19m,
                Currency = "EUR"
            });

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
        result.CustomerId.Should().Be(customerId);
        result.ReservationId.Should().NotBeEmpty();
        result.TotalPriceNet.Should().Be(168.07m);
        result.TotalPriceVat.Should().BeApproximately(31.93m, 0.01m);
        result.TotalPriceGross.Should().BeApproximately(200.00m, 0.01m);
        result.Currency.Should().Be("EUR");
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_CallsCustomersServiceRegisterAsync()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupDefaultMocks();

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _customersServiceMock.Verify(
            s => s.RegisterCustomerAsync(
                It.Is<RegisterCustomerDto>(dto =>
                    dto.FirstName == command.FirstName &&
                    dto.LastName == command.LastName &&
                    dto.Email == command.Email &&
                    dto.PhoneNumber == command.PhoneNumber &&
                    dto.DateOfBirth == command.DateOfBirth &&
                    dto.Street == command.Street &&
                    dto.City == command.City &&
                    dto.PostalCode == command.PostalCode &&
                    dto.Country == command.Country &&
                    dto.LicenseNumber == command.LicenseNumber &&
                    dto.LicenseIssueCountry == command.LicenseIssueCountry &&
                    dto.LicenseIssueDate == command.LicenseIssueDate &&
                    dto.LicenseExpiryDate == command.LicenseExpiryDate),
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
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _pricingServiceMock.Verify(
            s => s.CalculatePriceAsync(
                command.CategoryCode,
                command.Period.PickupDate.ToDateTime(TimeOnly.MinValue),
                command.Period.ReturnDate.ToDateTime(TimeOnly.MinValue),
                command.PickupLocationCode.Value,
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
        var command = CreateValidCommand();
        SetupDefaultMocks();

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_CallsServicesInCorrectOrder()
    {
        // Arrange
        var command = CreateValidCommand();
        var callOrder = new List<string>();

        _customersServiceMock
            .Setup(s => s.RegisterCustomerAsync(It.IsAny<RegisterCustomerDto>(), It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("RegisterCustomer"))
            .ReturnsAsync(Guid.NewGuid());

        _pricingServiceMock
            .Setup(s => s.CalculatePriceAsync(
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("CalculatePrice"))
            .ReturnsAsync(new PriceCalculationDto
            {
                CategoryCode = "KOMPAKT",
                TotalDays = 3,
                DailyRateNet = 50.00m,
                DailyRateGross = 59.50m,
                TotalPriceNet = 150.00m,
                TotalPriceGross = 178.50m,
                VatAmount = 28.50m,
                VatRate = 0.19m,
                Currency = "EUR"
            });

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
        callOrder.Should().HaveCount(4);
        callOrder[0].Should().Be("RegisterCustomer");
        callOrder[1].Should().Be("CalculatePrice");
        callOrder[2].Should().Be("Add");
        callOrder[3].Should().Be("Save");
    }

    [Fact]
    public async Task HandleAsync_CreatesReservationWithCorrectCustomerId()
    {
        // Arrange
        var command = CreateValidCommand();
        var customerId = Guid.NewGuid();

        _customersServiceMock
            .Setup(s => s.RegisterCustomerAsync(It.IsAny<RegisterCustomerDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(customerId);

        _pricingServiceMock
            .Setup(s => s.CalculatePriceAsync(
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PriceCalculationDto
            {
                CategoryCode = "KOMPAKT",
                TotalDays = 3,
                DailyRateNet = 50.00m,
                DailyRateGross = 59.50m,
                TotalPriceNet = 150.00m,
                TotalPriceGross = 178.50m,
                VatAmount = 28.50m,
                VatRate = 0.19m,
                Currency = "EUR"
            });

        Reservation? capturedReservation = null;
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Callback<Reservation, CancellationToken>((res, ct) => capturedReservation = res)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        capturedReservation.Should().NotBeNull();
        capturedReservation!.CustomerId.Should().Be(customerId);
    }

    [Fact]
    public async Task HandleAsync_CreatesReservationWithCorrectVehicleId()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupDefaultMocks();

        Reservation? capturedReservation = null;
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Callback<Reservation, CancellationToken>((res, ct) => capturedReservation = res)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        capturedReservation.Should().NotBeNull();
        capturedReservation!.VehicleId.Should().Be(command.VehicleId);
    }

    [Fact]
    public async Task HandleAsync_CreatesReservationWithPendingStatus()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupDefaultMocks();

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
    public async Task HandleAsync_CreatesReservationWithCorrectDates()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupDefaultMocks();

        Reservation? capturedReservation = null;
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Callback<Reservation, CancellationToken>((res, ct) => capturedReservation = res)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        capturedReservation.Should().NotBeNull();
        capturedReservation!.Period.PickupDate.Should().Be(command.Period.PickupDate);
        capturedReservation.Period.ReturnDate.Should().Be(command.Period.ReturnDate);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task HandleAsync_WhenCustomersServiceFails_ThrowsException()
    {
        // Arrange
        var command = CreateValidCommand();

        _customersServiceMock
            .Setup(s => s.RegisterCustomerAsync(It.IsAny<RegisterCustomerDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Failed to register customer via Customers API"));

        // Act
        var act = async () => await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Failed to register customer*");
    }

    [Fact]
    public async Task HandleAsync_WhenPricingServiceFails_ThrowsException()
    {
        // Arrange
        var command = CreateValidCommand();

        _customersServiceMock
            .Setup(s => s.RegisterCustomerAsync(It.IsAny<RegisterCustomerDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        _pricingServiceMock
            .Setup(s => s.CalculatePriceAsync(
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Failed to calculate price via Pricing API"));

        // Act
        var act = async () => await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Failed to calculate price*");
    }

    [Fact]
    public async Task HandleAsync_WhenCustomersServiceFails_DoesNotCallPricingService()
    {
        // Arrange
        var command = CreateValidCommand();

        _customersServiceMock
            .Setup(s => s.RegisterCustomerAsync(It.IsAny<RegisterCustomerDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Failed to register customer"));

        // Act
        try
        {
            await _handler.HandleAsync(command, CancellationToken.None);
        }
        catch
        {
            // Expected exception
        }

        // Assert
        _pricingServiceMock.Verify(
            s => s.CalculatePriceAsync(
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WhenPricingServiceFails_DoesNotCallRepository()
    {
        // Arrange
        var command = CreateValidCommand();

        _customersServiceMock
            .Setup(s => s.RegisterCustomerAsync(It.IsAny<RegisterCustomerDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        _pricingServiceMock
            .Setup(s => s.CalculatePriceAsync(
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Failed to calculate price"));

        // Act
        try
        {
            await _handler.HandleAsync(command, CancellationToken.None);
        }
        catch
        {
            // Expected exception
        }

        // Assert
        _repositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _repositoryMock.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WithEmptyVehicleId_ThrowsArgumentException()
    {
        // Arrange
        var command = CreateValidCommand() with { VehicleId = Guid.Empty };
        SetupDefaultMocks();

        // Act
        var act = async () => await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*GUID cannot be empty*");
    }

    [Fact]
    public void HandleAsync_WithPickupDateInPast_ThrowsArgumentException()
    {
        // Arrange & Act
        var act = () => CreateValidCommand() with { Period = BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(-1), DateTime.UtcNow.Date.AddDays(3)) };

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Pickup date cannot be in the past*");
    }

    [Fact]
    public void HandleAsync_WithReturnDateBeforePickupDate_ThrowsArgumentException()
    {
        // Arrange & Act
        var act = () => CreateValidCommand() with
        {
            Period = BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(10), DateTime.UtcNow.Date.AddDays(5))
        };

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Return date must be after pickup date*");
    }

    [Fact]
    public void HandleAsync_WithReturnDateSameAsPickupDate_ThrowsArgumentException()
    {
        // Arrange
        var pickupDate = DateTime.UtcNow.Date.AddDays(5);

        // Act
        var act = () => CreateValidCommand() with { Period = BookingPeriod.Of(pickupDate, pickupDate) };

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Return date must be after pickup date*");
    }

    [Fact]
    public void HandleAsync_WithRentalPeriodOver90Days_ThrowsArgumentException()
    {
        // Arrange & Act
        var act = () => CreateValidCommand() with
        {
            Period = BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(100)) // 95 days
        };

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Rental period cannot exceed 90 days*");
    }

    #endregion

    #region Helper Methods

    private CreateGuestReservationCommand CreateValidCommand()
    {
        return new CreateGuestReservationCommand
        {
            // Vehicle details
            VehicleId = Guid.NewGuid(),
            CategoryCode = "KOMPAKT",
            Period = BookingPeriod.Of(DateTime.UtcNow.Date.AddDays(7), DateTime.UtcNow.Date.AddDays(10)),
            PickupLocationCode = LocationCode.Of("BER-HBF"),
            DropoffLocationCode = LocationCode.Of("BER-HBF"),

            // Customer details
            FirstName = "Max",
            LastName = "Mustermann",
            Email = "max.mustermann@example.com",
            PhoneNumber = "+49 30 12345678",
            DateOfBirth = new DateOnly(1990, 1, 15),

            // Address details
            Street = "Musterstraße 123",
            City = "Berlin",
            PostalCode = "10115",
            Country = "Germany",

            // License details
            LicenseNumber = "B123456789",
            LicenseIssueCountry = "Germany",
            LicenseIssueDate = new DateOnly(2010, 6, 1),
            LicenseExpiryDate = new DateOnly(2030, 6, 1)
        };
    }

    private void SetupDefaultMocks()
    {
        _customersServiceMock
            .Setup(s => s.RegisterCustomerAsync(It.IsAny<RegisterCustomerDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        _pricingServiceMock
            .Setup(s => s.CalculatePriceAsync(
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PriceCalculationDto
            {
                CategoryCode = "KOMPAKT",
                TotalDays = 3,
                DailyRateNet = 50.00m,
                DailyRateGross = 59.50m,
                TotalPriceNet = 150.00m,
                TotalPriceGross = 178.50m,
                VatAmount = 28.50m,
                VatRate = 0.19m,
                Currency = "EUR"
            });

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _repositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    #endregion
}
