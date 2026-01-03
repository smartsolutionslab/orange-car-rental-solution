using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Extensions;
using SmartSolutionsLab.OrangeCarRental.Reservations.Api.Endpoints;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Data;
using SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Persistence;
using SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults (OpenTelemetry, health checks, service discovery, resilience)
builder.AddServiceDefaults();

// Configure Serilog with Seq for structured logging
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .Enrich.WithProperty("Application", "ReservationsAPI")
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Application}] {Message:lj}{NewLine}{Exception}");

    // Add Seq sink if SEQ_URL is configured
    var seqUrl = context.Configuration["SEQ_URL"];
    if (!string.IsNullOrEmpty(seqUrl))
    {
        configuration.WriteTo.Seq(seqUrl);
    }
});

// Add services to the container
builder.Services.AddOpenApi();

// CORS for frontend applications (separated by portal)
builder.Services.AddOrangeCarRentalCors();

// Add JWT Authentication and Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddOrangeCarRentalAuthorization();

// Register database context (connection string provided by Aspire)
builder.AddSqlServerDbContext<ReservationsDbContext>("reservations", configureDbContextOptions: options =>
{
    options.UseSqlServer(sqlOptions =>
        sqlOptions.MigrationsAssembly("OrangeCarRental.Reservations.Infrastructure"));
});

// Register HTTP client for Pricing API (service discovery configured via AddServiceDefaults)
builder.Services.AddHttpClient<IPricingService, PricingService>(client =>
{
    client.BaseAddress = new Uri("http://pricing-api");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Register HTTP client for Customers API (service discovery configured via AddServiceDefaults)
builder.Services.AddHttpClient<ICustomersService, CustomersService>(client =>
{
    client.BaseAddress = new Uri("http://customers-api");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Register repository
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

// Register data seeder
builder.Services.AddScoped<ReservationsDataSeeder>();

// Register command handlers
builder.Services.AddScoped<ICommandHandler<CreateReservationCommand, CreateReservationResult>, CreateReservationCommandHandler>();
builder.Services.AddScoped<ICommandHandler<CreateGuestReservationCommand, CreateGuestReservationResult>, CreateGuestReservationCommandHandler>();
builder.Services.AddScoped<ICommandHandler<ConfirmReservationCommand, ConfirmReservationResult>, ConfirmReservationCommandHandler>();
builder.Services.AddScoped<ICommandHandler<CancelReservationCommand, CancelReservationResult>, CancelReservationCommandHandler>();

// Register query handlers
builder.Services.AddScoped<IQueryHandler<GetReservationQuery, ReservationDto>, GetReservationQueryHandler>();
builder.Services.AddScoped<IQueryHandler<LookupGuestReservationQuery, ReservationDto>, LookupGuestReservationQueryHandler>();
builder.Services.AddScoped<IQueryHandler<SearchReservationsQuery, PagedResult<ReservationDto>>, SearchReservationsQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetVehicleAvailabilityQuery, GetVehicleAvailabilityResult>, GetVehicleAvailabilityQueryHandler>();

var app = builder.Build();

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

// Centralized exception handling
app.UseExceptionHandling();

app.UseAllFrontendsCors();
app.UseHttpsRedirection();

// Add Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map API endpoints
app.MapReservationEndpoints();
app.MapHealthEndpoints<ReservationsDbContext>("Reservations API");
app.MapDefaultEndpoints();

app.Run();
