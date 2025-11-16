using Moq;
using Shouldly;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries.GetCustomer;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Tests.Application.Queries;

public class GetCustomerQueryHandlerTests
{
    private readonly Mock<ICustomerRepository> customerRepositoryMock;
    private readonly GetCustomerQueryHandler handler;

    public GetCustomerQueryHandlerTests()
    {
        customerRepositoryMock = new Mock<ICustomerRepository>();
        handler = new GetCustomerQueryHandler(customerRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithExistingCustomer_ShouldReturnCustomerDto()
    {
        // Arrange
        var customer = CreateTestCustomer();
        var query = new GetCustomerQuery { CustomerIdentifier = customer.Id };

        customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(customer.Id.Value);
        result.FirstName.ShouldBe(customer.Name.FirstName.Value);
        result.LastName.ShouldBe(customer.Name.LastName.Value);
        result.Email.ShouldBe(customer.Email.Value);
        result.PhoneNumber.ShouldBe(customer.PhoneNumber.FormattedValue);
        result.Status.ShouldBe(customer.Status.ToString());

        customerRepositoryMock.Verify(x => x.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentCustomer_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var customerId = CustomerIdentifier.New();
        var query = new GetCustomerQuery { CustomerIdentifier = customerId };

        customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        // Act & Assert
        await Should.ThrowAsync<EntityNotFoundException>(() =>
            handler.HandleAsync(query, CancellationToken.None));

        customerRepositoryMock.Verify(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var customerId = CustomerIdentifier.New();
        var query = new GetCustomerQuery { CustomerIdentifier = customerId };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(() =>
            handler.HandleAsync(query, cts.Token));
    }

    private static Customer CreateTestCustomer()
    {
        var name = CustomerName.Of("Max", "Mustermann", Salutation.Herr);
        var email = Email.Of("max.mustermann@example.com");
        var phone = PhoneNumber.Of("0151 12345678");
        var birthDate = BirthDate.Of(new DateOnly(1990, 1, 1));
        var address = Address.Of("Hauptstraße", "123", "Berlin", PostalCode.Of("10115"), City.Of("Berlin"), "Deutschland");

        var issueDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-5));
        var expiryDate = DateOnly.FromDateTime(DateTime.Now.AddYears(5));
        var license = DriversLicense.Of("DE123456789", "Deutschland", issueDate, expiryDate);

        return Customer.Register(name, email, phone, birthDate, address, license);
    }
}
