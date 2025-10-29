using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CreateReservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.GetReservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Repositories;
using SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();

// CORS for frontend applications
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost:4201")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register database context (connection string provided by Aspire)
builder.AddSqlServerDbContext<ReservationsDbContext>("reservations", configureDbContextOptions: options =>
{
    options.UseSqlServer(sqlOptions =>
        sqlOptions.MigrationsAssembly("OrangeCarRental.Reservations.Infrastructure"));
});

// Register repositories
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

// Register application handlers
builder.Services.AddScoped<CreateReservationCommandHandler>();
builder.Services.AddScoped<GetReservationQueryHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseHttpsRedirection();

// Reservation endpoints
var reservations = app.MapGroup("/api/reservations")
    .WithTags("Reservations")
    .WithOpenApi();

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

**German Market Pricing:**
- All prices include 19% German VAT (Mehrwertsteuer)
- Provide the net amount, VAT will be calculated automatically
- Currency is EUR

**Date Requirements:**
- Pickup date must be today or in the future
- Return date must be after pickup date
- Maximum rental period is 90 days
");

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

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Service = "Reservations" }))
    .WithName("HealthCheck")
    .WithTags("Health");

app.Run();
