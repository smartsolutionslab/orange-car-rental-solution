using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Extensions;
using SmartSolutionsLab.OrangeCarRental.Reservations.Api.Extensions;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CancelReservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.ConfirmReservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CreateGuestReservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CreateReservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.GetReservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.SearchReservations;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Persistence;
using SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProperty("Application", "ReservationsAPI")
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Application}] {Message:lj}{NewLine}{Exception}"));

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

// Add JWT Authentication and Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddOrangeCarRentalAuthorization();

// Register database context (connection string provided by Aspire)
builder.AddSqlServerDbContext<ReservationsDbContext>("reservations", configureDbContextOptions: options =>
{
    options.UseSqlServer(sqlOptions =>
        sqlOptions.MigrationsAssembly("OrangeCarRental.Reservations.Infrastructure"));
});

// Register Aspire Service Discovery for inter-service communication
builder.Services.AddServiceDiscovery();

// Register HTTP client for Pricing API with Service Discovery
// The service name "http://pricing-api" is resolved by Aspire's Service Discovery
builder.Services.AddHttpClient<IPricingService, PricingService>(client =>
{
    client.BaseAddress = new Uri("http://pricing-api");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddServiceDiscovery();

Log.Information("Pricing API: Using Aspire Service Discovery (http://pricing-api)");

// Register HTTP client for Customers API with Service Discovery
// The service name "http://customers-api" is resolved by Aspire's Service Discovery
builder.Services.AddHttpClient<ICustomersService, CustomersService>(client =>
{
    client.BaseAddress = new Uri("http://customers-api");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddServiceDiscovery();

Log.Information("Customers API: Using Aspire Service Discovery (http://customers-api)");

// Register repositories
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

// Register application handlers
builder.Services.AddScoped<CreateReservationCommandHandler>();
builder.Services.AddScoped<CreateGuestReservationCommandHandler>();
builder.Services.AddScoped<GetReservationQueryHandler>();
builder.Services.AddScoped<SearchReservationsQueryHandler>();
builder.Services.AddScoped<ConfirmReservationCommandHandler>();
builder.Services.AddScoped<CancelReservationCommandHandler>();

var app = builder.Build();

// Seed database with sample data (development only)
await app.SeedReservationsDataAsync();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Orange Car Rental - Reservations API")
            .WithTheme(ScalarTheme.Mars)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

// Add Serilog request logging
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent);
    };
});

app.UseCors();
app.UseHttpsRedirection();

// Add Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map API endpoints
app.MapReservationEndpoints();
app.MapHealthEndpoints();

app.Run();
