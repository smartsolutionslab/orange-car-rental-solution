using Microsoft.AspNetCore.Mvc;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;
using SmartSolutionsLab.OrangeCarRental.Reservations.Api.Requests;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;
using CustomerIdentifier = SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared.CustomerIdentifier;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Api.Endpoints;

public static class ReservationEndpoints
{
    public static IEndpointRouteBuilder MapReservationEndpoints(this IEndpointRouteBuilder app)
    {
        var reservations = app.MapGroup("/api/reservations")
            .WithTags("Reservations");

        // POST /api/reservations - Create a new reservation
        reservations.MapPost("/", async (CreateReservationRequest request, ICommandHandler<CreateReservationCommand, CreateReservationResult> handler) =>
            {
                var (vehicleId, customerId, category, pickupDate, returnDate, pickupLocation, dropoffLocation, totalPriceNet) = request;

                var command = new CreateReservationCommand(
                    VehicleIdentifier.From(vehicleId),
                    CustomerIdentifier.From(customerId),
                    VehicleCategory.From(category),
                    BookingPeriod.Of(pickupDate, returnDate),
                    LocationCode.From(pickupLocation),
                    LocationCode.From(dropoffLocation),
                    Money.Euro(totalPriceNet)
                );

                var result = await handler.HandleAsync(command);
                return Results.Created($"/api/reservations/{result.ReservationId}", result);
            })
            .WithName("CreateReservation")
            .WithSummary("Create a new vehicle reservation")
            .WithDescription("""

                             Creates a new reservation for a vehicle rental. The reservation will be created in 'Pending' status
                             awaiting payment confirmation.

                             **Automatic Price Calculation:**
                             - Price is automatically calculated based on vehicle category, rental period, and location
                             - Pricing is retrieved from the Pricing Service API
                             - No need to provide TotalPriceNet - it will be calculated for you
                             - For backward compatibility, you can still provide TotalPriceNet to override automatic calculation

                             **German Market Pricing:**
                             - All prices include 19% German VAT (Mehrwertsteuer)
                             - Price breakdown shows net, VAT, and gross amounts
                             - Currency is EUR

                             **Date Requirements:**
                             - Pickup date must be today or in the future
                             - Return date must be after pickup date
                             - Maximum rental period is 90 days

                             """)
            .RequireAuthorization("CustomerOrCallCenterOrAdminPolicy");

        // POST /api/reservations/guest - Create a guest reservation (with inline customer registration)
        reservations.MapPost("/guest", async (CreateGuestReservationRequest request, ICommandHandler<CreateGuestReservationCommand, CreateGuestReservationResult> handler) =>
            {
                var (reservation, customer, address, driversLicense) = request;
                var (vehicleId, category, pickupDate, returnDate, pickupLocation, dropoffLocation) = reservation;
                var (firstName, lastName, email, phoneNumber, dateOfBirth) = customer;
                var (street, city, postalCode, country) = address;
                var (driversLicenseNumber, driversLicenseIssueCountry, driversLicenseIssueDate, driversLicenseExpiryDate) = driversLicense;

                var command = new CreateGuestReservationCommand(
                    VehicleIdentifier.From(vehicleId),
                    VehicleCategory.From(category),
                    BookingPeriod.Of(pickupDate, returnDate),
                    LocationCode.From(pickupLocation),
                    LocationCode.From(dropoffLocation),
                    CustomerName.Of(firstName, lastName, null),
                    Email.From(email),
                    PhoneNumber.From(phoneNumber),
                    BirthDate.Of(dateOfBirth),
                    Address.Of(
                        street,
                        city,
                        postalCode,
                        country),
                    DriversLicense.Of(
                        driversLicenseNumber,
                        driversLicenseIssueCountry,
                        driversLicenseIssueDate,
                        driversLicenseExpiryDate)
                );

                var result = await handler.HandleAsync(command);
                return Results.Created($"/api/reservations/{result.ReservationId}", result);
            })
            .WithName("CreateGuestReservation")
            .WithSummary("Create a reservation for a guest user (without pre-registration)")
            .WithDescription("""

                             Creates a reservation for a guest user by handling both customer registration and reservation creation in a single request.
                             This is the recommended endpoint for public portal bookings where users haven't registered yet.

                             **What This Endpoint Does:**
                             1. Registers the customer automatically via Customers API
                             2. Calculates the price automatically via Pricing API (19% German VAT included)
                             3. Creates the reservation with the new customer ID
                             4. Returns both customer ID and reservation ID

                             **Customer Data Requirements:**
                             - German market validation (postal code, phone number, driving license)
                             - Minimum age: 18 years (21 years for rental)
                             - Driver's license must be valid for at least 30 days

                             **Benefits Over Separate Registration:**
                             - Single API call instead of two
                             - Better UX for guest users
                             - Atomic operation - either both succeed or both fail

                             **Response:**
                             Returns the newly created customer ID, reservation ID, and pricing breakdown.

                             """)
            .AllowAnonymous();

        // GET /api/reservations/{id} - Get reservation by ID
        reservations.MapGet("/{id:guid}", async (Guid id, IQueryHandler<GetReservationQuery, ReservationDto> handler) =>
            {
                try
                {
                    var query = new GetReservationQuery(ReservationIdentifier.From(id));
                    var result = await handler.HandleAsync(query);
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
            })
            .WithName("GetReservation")
            .WithSummary("Get reservation by ID")
            .WithDescription("Retrieves detailed information about a specific reservation including pricing breakdown with German VAT.")
            .Produces<object>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization("CustomerOrCallCenterOrAdminPolicy");

        // GET /api/reservations/lookup - Guest lookup by reservation ID and email
        reservations.MapGet("/lookup", async (
                string reservationId,
                string email,
                IQueryHandler<LookupGuestReservationQuery, ReservationDto> handler) =>
            {
                try
                {
                    if (!Guid.TryParse(reservationId, out var id))
                    {
                        return Results.BadRequest(new { message = "Invalid reservation ID format." });
                    }

                    var query = new LookupGuestReservationQuery(
                        ReservationIdentifier.From(id),
                        email);
                    var result = await handler.HandleAsync(query);
                    return Results.Ok(result);
                }
                catch (EntityNotFoundException)
                {
                    return Results.NotFound(new { message = "Reservation not found or email does not match." });
                }
            })
            .WithName("LookupGuestReservation")
            .WithSummary("Lookup a guest reservation by ID and email")
            .WithDescription("""

                             Allows guests to look up their reservation without authentication.

                             **Security:**
                             - Requires both reservation ID and email to match
                             - Email is verified against the customer record
                             - Case-insensitive email matching

                             **Use Case:** Public portal guest users who made a booking without registering.
                             """)
            .AllowAnonymous();

        // GET /api/reservations/search - Search reservations with filters
        reservations.MapGet("/search", async (
                IQueryHandler<SearchReservationsQuery, PagedResult<ReservationDto>> handler,
                string? status = null,
                Guid? customerId = null,
                string? customerName = null,
                Guid? vehicleId = null,
                string? categoryCode = null,
                string? pickupLocationCode = null,
                DateOnly? pickupDateFrom = null,
                DateOnly? pickupDateTo = null,
                decimal? priceMin = null,
                decimal? priceMax = null,
                string? sortBy = null,
                bool sortDescending = false,
                int pageNumber = 1,
                int pageSize = 50) =>
            {
                var query = new SearchReservationsQuery(
                    status.TryParseReservationStatus(),
                     CustomerIdentifier.From(customerId),
                    SearchTerm.FromNullable(customerName),
                    VehicleIdentifier.From(vehicleId),
                    VehicleCategory.FromNullable(categoryCode),
                    LocationCode.FromNullable(pickupLocationCode),
                    DateRange.Create(pickupDateFrom, pickupDateTo),
                    PriceRange.Create(priceMin, priceMax),
                    sortBy,
                    sortDescending,
                    pageNumber,
                    pageSize);

                var result = await handler.HandleAsync(query);
                return Results.Ok(result);
            })
            .WithName("SearchReservations")
            .WithSummary("Search reservations with enhanced filters, sorting and pagination")
            .WithDescription("""

                             Search for reservations using various filters:

                             **Status & Customer Filters:**
                             - **status**: Filter by status (Pending, Confirmed, Active, Completed, Cancelled, NoShow)
                             - **customerId**: Filter by customer GUID
                             - **customerName**: Filter by customer name (requires denormalized data - not yet implemented)

                             **Vehicle Filters:**
                             - **vehicleId**: Filter by vehicle GUID
                             - **categoryCode**: Filter by vehicle category code (requires denormalized data - not yet implemented)

                             **Location & Date Filters:**
                             - **pickupLocationCode**: Filter by pickup location code (e.g., "BER-HBF")
                             - **pickupDateFrom**: Filter by pickup date from (inclusive)
                             - **pickupDateTo**: Filter by pickup date to (inclusive)

                             **Price Range Filters:**
                             - **priceMin**: Minimum total price (gross amount in EUR)
                             - **priceMax**: Maximum total price (gross amount in EUR)

                             **Sorting:**
                             - **sortBy**: Sort field (PickupDate, Price, Status, CreatedDate) - default: CreatedDate
                             - **sortDescending**: Sort in descending order (default: false)

                             **Pagination:**
                             - **pageNumber**: Page number (default: 1)
                             - **pageSize**: Items per page (default: 50, max: 100)

                             Returns paginated results with total count and pagination metadata.
                             """)
            .RequireAuthorization("CallCenterOrAdminPolicy");

        // PUT /api/reservations/{id}/confirm - Confirm a pending reservation
        reservations.MapPut("/{id:guid}/confirm", async (Guid id, ICommandHandler<ConfirmReservationCommand, ConfirmReservationResult> handler) =>
            {
                try
                {
                    var command = new ConfirmReservationCommand(ReservationIdentifier.From(id));
                    var result = await handler.HandleAsync(command);
                    return Results.Ok(result);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(new { ex.Message });
                }
            })
            .WithName("ConfirmReservation")
            .WithSummary("Confirm a pending reservation")
            .WithDescription("""

                             Confirms a pending reservation after payment has been received.

                             **Requirements:**
                             - Reservation must be in 'Pending' status
                             - Cannot confirm already confirmed, active, completed, or cancelled reservations

                             **What Happens:**
                             - Status changes from 'Pending' to 'Confirmed'
                             - ConfirmedAt timestamp is set
                             - ReservationConfirmed domain event is raised

                             **Use Case:** Call this endpoint after payment gateway confirms successful payment.
                             """)
            .RequireAuthorization("CallCenterOrAdminPolicy");

        // PUT /api/reservations/{id}/cancel - Cancel a reservation
        reservations.MapPut("/{id:guid}/cancel", async (
                Guid id,
                CancelReservationCommand command,
                ICommandHandler<CancelReservationCommand, CancelReservationResult> handler) =>
            {
                try
                {
                    // Override the ID from the URL
                    var commandWithId = command with { ReservationId = ReservationIdentifier.From(id) };
                    var result = await handler.HandleAsync(commandWithId);
                    return Results.Ok(result);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(new { ex.Message });
                }
            })
            .WithName("CancelReservation")
            .WithSummary("Cancel a reservation")
            .WithDescription("""

                             Cancels a reservation with an optional reason.

                             **Requirements:**
                             - Cannot cancel a completed reservation
                             - Cannot cancel an active rental (vehicle must be returned first)
                             - Already cancelled reservations are idempotent (no error)

                             **Request Body:**
                             ```json
                             {
                               "cancellationReason": "Customer requested cancellation"
                             }
                             ```

                             **What Happens:**
                             - Status changes to 'Cancelled'
                             - CancelledAt timestamp is set
                             - CancellationReason is stored
                             - ReservationCancelled domain event is raised

                             **Cancellation Policy (German Market):**
                             - Free cancellation up to 48h before pickup
                             - 50% refund for 24-48h before pickup
                             - No refund for < 24h before pickup
                             (Policy enforcement should be handled in payment service)
                             """)
            .RequireAuthorization("CustomerOrCallCenterOrAdminPolicy");

        // GET /api/reservations/availability - Get booked vehicle IDs for a period
        reservations.MapGet("/availability", async (
                DateOnly pickupDate,
                DateOnly returnDate,
                [FromServices] IQueryHandler<GetVehicleAvailabilityQuery, GetVehicleAvailabilityResult> handler,
                CancellationToken cancellationToken) =>
            {
                var query = new GetVehicleAvailabilityQuery(pickupDate, returnDate);
                var result = await handler.HandleAsync(query, cancellationToken);
                return Results.Ok(result);
            })
            .WithName("GetVehicleAvailability")
            .WithSummary("Get list of booked vehicle IDs for a period")
            .WithDescription("""
                             Returns a list of vehicle IDs that are unavailable (booked) during the specified period.
                             Used by the Fleet service to determine which vehicles are available for new reservations.

                             **Query Parameters:**
                             - **pickupDate**: Start date of the period (format: YYYY-MM-DD)
                             - **returnDate**: End date of the period (format: YYYY-MM-DD)

                             **Business Logic:**
                             - Only returns vehicles with Confirmed or Active reservations
                             - Pending, Cancelled, Completed, and NoShow reservations are ignored
                             - Period overlap check: reservation overlaps if reservation_pickup <= requested_return AND reservation_return >= requested_pickup

                             **Response:**
                             Returns a list of vehicle GUIDs that are booked during the period.
                             """)
            .AllowAnonymous();

        return app;
    }
}
