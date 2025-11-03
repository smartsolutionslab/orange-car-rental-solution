using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CreateReservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CreateGuestReservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.GetReservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Api.Extensions;

public static class ReservationEndpoints
{
    public static IEndpointRouteBuilder MapReservationEndpoints(this IEndpointRouteBuilder app)
    {
        var reservations = app.MapGroup("/api/reservations")
            .WithTags("Reservations")
            .WithOpenApi();

        // POST /api/reservations - Create a new reservation
        reservations.MapPost("/", async (
            CreateReservationCommand command,
            CreateReservationCommandHandler handler) =>
        {
            var result = await handler.HandleAsync(command);
            return Results.Created($"/api/reservations/{result.ReservationId}", result);
        })
        .WithName("CreateReservation")
        .WithSummary("Create a new vehicle reservation")
        .WithDescription(@"
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
");

        // POST /api/reservations/guest - Create a guest reservation (with inline customer registration)
        reservations.MapPost("/guest", async (
            CreateGuestReservationCommand command,
            CreateGuestReservationCommandHandler handler) =>
        {
            var result = await handler.HandleAsync(command);
            return Results.Created($"/api/reservations/{result.ReservationId}", result);
        })
        .WithName("CreateGuestReservation")
        .WithSummary("Create a reservation for a guest user (without pre-registration)")
        .WithDescription(@"
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
");

        // GET /api/reservations/{id} - Get reservation by ID
        reservations.MapGet("/{id:guid}", async (
            Guid id,
            GetReservationQueryHandler handler) =>
        {
            var query = new GetReservationQuery(id);
            var result = await handler.HandleAsync(query);

            return result is not null
                ? Results.Ok(result)
                : Results.NotFound(new { Message = $"Reservation {id} not found" });
        })
        .WithName("GetReservation")
        .WithSummary("Get reservation by ID")
        .WithDescription("Retrieves detailed information about a specific reservation including pricing breakdown with German VAT.");

        return app;
    }
}
