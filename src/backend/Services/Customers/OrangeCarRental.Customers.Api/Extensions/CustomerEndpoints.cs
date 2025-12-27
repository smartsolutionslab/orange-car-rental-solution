using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;
using SmartSolutionsLab.OrangeCarRental.Customers.Api.Contracts;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.ChangeCustomerStatus;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.RegisterCustomer;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.UpdateCustomerProfile;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.UpdateDriversLicense;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries.GetCustomer;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries.GetCustomerByEmail;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries.SearchCustomers;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Api.Extensions;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var customers = app.MapGroup("/api/customers")
            .WithTags("Customers");

        // POST /api/customers - Register new customer
        customers.MapPost("/", async (
                RegisterCustomerRequest request,
                RegisterCustomerCommandHandler handler,
                CancellationToken cancellationToken) =>
            {
                var (customer, address, driversLicense) = request;

                try
                {
                    // Map request DTO to command with value objects
                    var command = new RegisterCustomerCommand(
                        CustomerName.Of(customer.FirstName, customer.LastName),
                        Email.From(customer.Email),
                        PhoneNumber.From(customer.PhoneNumber),
                        BirthDate.Of(customer.DateOfBirth),
                        Address.Of(
                            address.Street,
                            address.City,
                            address.PostalCode,
                            address.Country),
                        DriversLicense.Of(
                            driversLicense.LicenseNumber,
                            driversLicense.LicenseIssueCountry,
                            driversLicense.LicenseIssueDate,
                            driversLicense.LicenseExpiryDate));

                    var result = await handler.HandleAsync(command, cancellationToken);
                    return Results.Created($"/api/customers/{result.CustomerIdentifier}", result);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError,
                        title: "An error occurred while registering the customer");
                }
            })
            .WithName("RegisterCustomer")
            .WithSummary("Register a new customer")
            .WithDescription(
                "Registers a new customer with personal details, address, and driver's license information. Email must be unique. Driver's license must be valid for at least 30 days. Customer must be at least 18 years old.")
            .Produces<RegisterCustomerResult>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .AllowAnonymous();

        // GET /api/customers/{id} - Get customer by ID
        customers.MapGet("/{id:guid}", async (
                Guid id,
                GetCustomerQueryHandler handler,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var result = await handler.HandleAsync(
                        new GetCustomerQuery(CustomerIdentifier.From(id)),
                        cancellationToken);
                    return Results.Ok(result);
                }
                catch (EntityNotFoundException ex)
                {
                    return Results.NotFound(new { message = ex.Message });
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError,
                        title: "An error occurred while retrieving the customer");
                }
            })
            .WithName("GetCustomerById")
            .WithSummary("Get customer by ID")
            .WithDescription("Retrieves a customer's complete profile by their unique identifier.")
            .Produces<CustomerDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization("CallCenterOrAdminPolicy");

        // GET /api/customers/by-email/{email} - Get customer by email
        customers.MapGet("/by-email/{email}", async (
                string email,
                GetCustomerByEmailQueryHandler handler,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var result = await handler.HandleAsync(
                        new GetCustomerByEmailQuery(Email.From(email)),
                        cancellationToken);
                    return Results.Ok(result);
                }
                catch (EntityNotFoundException ex)
                {
                    return Results.NotFound(new { message = ex.Message });
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError,
                        title: "An error occurred while retrieving the customer");
                }
            })
            .WithName("GetCustomerByEmail")
            .WithSummary("Get customer by email address")
            .WithDescription("Retrieves a customer's complete profile by their email address.")
            .Produces<CustomerDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization("CallCenterOrAdminPolicy");

        // GET /api/customers/search - Search customers with filtering and pagination
        customers.MapGet("/search", async (
                [AsParameters] SearchCustomersQuery query,
                SearchCustomersQueryHandler handler,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var result = await handler.HandleAsync(query, cancellationToken);
                    return Results.Ok(result);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError,
                        title: "An error occurred while searching customers");
                }
            })
            .WithName("SearchCustomers")
            .WithSummary("Search customers with filters")
            .WithDescription(
                "Search and filter customers by name, email, phone, status, city, postal code, age range, license expiry, and registration date. Supports sorting and pagination. Returns paged results with customer details.")
            .Produces<PagedResult<CustomerDto>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization("CallCenterOrAdminPolicy");

        // PUT /api/customers/{id}/profile - Update customer profile
        customers.MapPut("/{id:guid}/profile", async (
                Guid id,
                UpdateCustomerProfileRequest request,
                UpdateCustomerProfileCommandHandler handler,
                CancellationToken cancellationToken) =>
            {
                var (profile, address) = request;

                try
                {
                    // Map request DTO to command with value objects
                    var command = new UpdateCustomerProfileCommand(
                        CustomerIdentifier.From(id),
                        CustomerName.Of(profile.FirstName, profile.LastName),
                        PhoneNumber.From(profile.PhoneNumber),
                        Address.Of(
                            address.Street,
                            address.City,
                            address.PostalCode,
                            address.Country)
                        );

                    var result = await handler.HandleAsync(command, cancellationToken);
                    return Results.Ok(result);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.NotFound(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError,
                        title: "An error occurred while updating the customer profile");
                }
            })
            .WithName("UpdateCustomerProfile")
            .WithSummary("Update customer profile")
            .WithDescription(
                "Updates a customer's profile information including name, phone number, and address. Email and driver's license are updated via separate endpoints.")
            .Produces<UpdateCustomerProfileResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization("CallCenterOrAdminPolicy");

        // PUT /api/customers/{id}/license - Update driver's license
        customers.MapPut("/{id:guid}/license", async (
                Guid id,
                UpdateDriversLicenseRequest request,
                UpdateDriversLicenseCommandHandler handler,
                CancellationToken cancellationToken) =>
            {
                var (licenseNumber, issueCountry, issueDate, expiryDate) = request;

                try
                {
                    var command = new UpdateDriversLicenseCommand(
                        CustomerIdentifier.From(id),
                        DriversLicense.Of(licenseNumber, issueCountry, issueDate, expiryDate)
                    );

                    var result = await handler.HandleAsync(command, cancellationToken);
                    return Results.Ok(result);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.NotFound(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError,
                        title: "An error occurred while updating the driver's license");
                }
            })
            .WithName("UpdateDriversLicense")
            .WithSummary("Update driver's license information")
            .WithDescription(
                "Updates a customer's driver's license details. License must be valid for at least 30 days from the current date.")
            .Produces<UpdateDriversLicenseResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization("CallCenterOrAdminPolicy");

        // PUT /api/customers/{id}/status - Change customer status
        customers.MapPut("/{id:guid}/status", async (
                Guid id,
                ChangeCustomerStatusCommand command,
                ChangeCustomerStatusCommandHandler handler,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    // Ensure the ID in the route matches the command
                    // Override the command's customer ID with the route ID to ensure consistency
                    var commandWithId = command with { CustomerId = CustomerIdentifier.From(id) };

                    var result = await handler.HandleAsync(commandWithId, cancellationToken);
                    return Results.Ok(result);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.NotFound(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError,
                        title: "An error occurred while changing the customer status");
                }
            })
            .WithName("ChangeCustomerStatus")
            .WithSummary("Change customer status")
            .WithDescription(
                "Changes a customer's account status (Active, Suspended, Blocked). Requires a reason for audit trail. Suspended customers cannot make new reservations. Blocked customers have their account disabled.")
            .Produces<ChangeCustomerStatusResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization("CallCenterOrAdminPolicy");

        return app;
    }
}
