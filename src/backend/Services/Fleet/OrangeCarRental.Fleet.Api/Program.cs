using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Extensions;
using SmartSolutionsLab.OrangeCarRental.Fleet.Api.Endpoints;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;
using SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence;
using SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults (OpenTelemetry, health checks, service discovery, resilience)
builder.AddServiceDefaults();

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProperty("Application", "FleetAPI")
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Application}] {Message:lj}{NewLine}{Exception}"));

// Add services to the container
builder.Services.AddOpenApi();

// CORS for frontend applications (separated by portal)
builder.Services.AddOrangeCarRentalCors();

// Add JWT Authentication and Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddOrangeCarRentalAuthorization();

// Register database contexts (connection strings provided by Aspire)
builder.AddSqlServerDbContext<FleetDbContext>("fleet", configureDbContextOptions: options =>
{
    options.UseSqlServer(sqlOptions =>
        sqlOptions.MigrationsAssembly("OrangeCarRental.Fleet.Infrastructure"));
});

// Register HTTP client for Reservations API with service discovery
builder.Services.AddHttpClient<IReservationService, ReservationService>(client =>
{
    client.BaseAddress = new Uri("http://reservations-api");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Register Unit of Work and repositories
builder.Services.AddScoped<IFleetUnitOfWork, FleetUnitOfWork>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();

// Register application services - Query Handlers
builder.Services.AddScoped<IQueryHandler<SearchVehiclesQuery, PagedResult<VehicleDto>>, SearchVehiclesQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetLocationsQuery, IReadOnlyList<LocationDto>>, GetLocationsQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetLocationByCodeQuery, LocationDto>, GetLocationByCodeQueryHandler>();

// Register application services - Command Handlers (Vehicle)
builder.Services.AddScoped<ICommandHandler<AddVehicleToFleetCommand, AddVehicleToFleetResult>, AddVehicleToFleetCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateVehicleStatusCommand, UpdateVehicleStatusResult>, UpdateVehicleStatusCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateVehicleLocationCommand, UpdateVehicleLocationResult>, UpdateVehicleLocationCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateVehicleDailyRateCommand, UpdateVehicleDailyRateResult>, UpdateVehicleDailyRateCommandHandler>();

// Register application services - Command Handlers (Location)
builder.Services.AddScoped<ICommandHandler<AddLocationCommand, AddLocationResult>, AddLocationCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateLocationCommand, UpdateLocationResult>, UpdateLocationCommandHandler>();
builder.Services.AddScoped<ICommandHandler<ChangeLocationStatusCommand, ChangeLocationStatusResult>, ChangeLocationStatusCommandHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Orange Car Rental - Fleet API")
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

app.UseAllFrontendsCors();

// Add Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map API endpoints
app.MapFleetEndpoints();
app.MapHealthEndpoints<FleetDbContext>("Fleet API");
app.MapDefaultEndpoints();

app.Run();
