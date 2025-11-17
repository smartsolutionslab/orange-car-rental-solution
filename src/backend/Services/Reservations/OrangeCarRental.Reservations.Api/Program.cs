using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
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

// Register database context (connection string provided by Aspire)
builder.AddSqlServerDbContext<ReservationsDbContext>("reservations", configureDbContextOptions: options =>
{
    options.UseSqlServer(sqlOptions =>
        sqlOptions.MigrationsAssembly("OrangeCarRental.Reservations.Infrastructure"));
});

// Register HTTP client for Pricing API
var pricingApiUrl = builder.Configuration["PRICING_API_URL"] ?? "http://localhost:5002";
Log.Information("Pricing API URL: {PricingApiUrl}", pricingApiUrl);

builder.Services.AddHttpClient<IPricingService, PricingService>(client =>
{
    client.BaseAddress = new Uri(pricingApiUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Register HTTP client for Customers API
var customersApiUrl = builder.Configuration["CUSTOMERS_API_URL"] ?? "http://localhost:5001";
Log.Information("Customers API URL: {CustomersApiUrl}", customersApiUrl);

builder.Services.AddHttpClient<ICustomersService, CustomersService>(client =>
{
    client.BaseAddress = new Uri(customersApiUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

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

// Map API endpoints
app.MapReservationEndpoints();
app.MapHealthEndpoints();

app.Run();
