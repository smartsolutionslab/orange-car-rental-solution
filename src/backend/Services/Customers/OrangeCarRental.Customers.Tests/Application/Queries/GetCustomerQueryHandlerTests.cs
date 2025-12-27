using Moq;
using Shouldly;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries.GetCustomer;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;
using SmartSolutionsLab.OrangeCarRental.Customers.Tests.Builders;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Tests.Application.Queries;

public class GetCustomerQueryHandlerTests
{
    private readonly Mock<ICustomerRepository> customerRepositoryMock = new();
    private readonly GetCustomerQueryHandler handler;

    public GetCustomerQueryHandlerTests()
    {
        handler = new GetCustomerQueryHandler(customerRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithExistingCustomer_ShouldReturnCustomerDto()
    {
        // Arrange
        var customer = CreateTestCustomer();
        var query = new GetCustomerQuery(customer.Id);

        customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert - using .Value to access nullable struct properties
        result.ShouldNotBeNull();
        result.Id.ShouldBe(customer.Id.Value);
        result.FirstName.ShouldBe(customer.Name!.Value.FirstName.Value);
        result.LastName.ShouldBe(customer.Name!.Value.LastName.Value);
        result.Email.ShouldBe(customer.Email!.Value.Value);
        result.PhoneNumber.ShouldBe(customer.PhoneNumber!.Value.Value);
        result.PhoneNumberFormatted.ShouldBe(customer.PhoneNumber!.Value.FormattedValue);
        result.Status.ShouldBe(customer.Status.ToString());

        customerRepositoryMock.Verify(x => x.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentCustomer_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var customerId = CustomerIdentifier.New();
        var query = new GetCustomerQuery(customerId);

        customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException(typeof(Customer), customerId.Value));

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
        var query = new GetCustomerQuery(customerId);
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
        return CustomerBuilder.MaxMustermann().Build();
    }
}
