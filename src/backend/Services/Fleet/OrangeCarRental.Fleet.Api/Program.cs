using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using SmartSolutionsLab.OrangeCarRental.Fleet.Api.Extensions;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.AddVehicleToFleet;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.UpdateVehicleStatus;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.UpdateVehicleLocation;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.UpdateVehicleDailyRate;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.GetLocations;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.SearchVehicles;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;
using SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence;
using SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

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

// Add CORS for frontend development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost:4201")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Register database contexts (connection strings provided by Aspire)
builder.AddSqlServerDbContext<FleetDbContext>("fleet", configureDbContextOptions: options =>
{
    options.UseSqlServer(sqlOptions =>
        sqlOptions.MigrationsAssembly("OrangeCarRental.Fleet.Infrastructure"));
});

// Register HTTP client for Reservations API (maintains bounded context boundaries)
builder.Services.AddHttpClient<IReservationService, ReservationService>(client =>
{
    // In production, use service discovery or configuration
    // For now, using Aspire-provided base address
    var reservationsApiUrl = builder.Configuration["Services:Reservations:Http:0"]
                             ?? "http://localhost:5002";
    client.BaseAddress = new Uri(reservationsApiUrl);
});

// Register repositories
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();

// Register application services - Query Handlers
builder.Services.AddScoped<SearchVehiclesQueryHandler>();
builder.Services.AddScoped<GetLocationsQueryHandler>();
builder.Services.AddScoped<GetLocationByCodeQueryHandler>();

// Register application services - Command Handlers
builder.Services.AddScoped<AddVehicleToFleetCommandHandler>();
builder.Services.AddScoped<UpdateVehicleStatusCommandHandler>();
builder.Services.AddScoped<UpdateVehicleLocationCommandHandler>();
builder.Services.AddScoped<UpdateVehicleDailyRateCommandHandler>();

var app = builder.Build();

// Seed database with sample data (development only)
await app.SeedFleetDataAsync();

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

app.UseCors("AllowFrontend");

// Map API endpoints
app.MapFleetEndpoints();
app.MapHealthEndpoints();

app.Run();
