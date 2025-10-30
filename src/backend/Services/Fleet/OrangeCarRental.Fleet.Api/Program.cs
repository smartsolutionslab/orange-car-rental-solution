using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SmartSolutionsLab.OrangeCarRental.Fleet.Api.Extensions;
using SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.SearchVehicles;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Repositories;
using SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

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

// Register database context (connection string provided by Aspire)
builder.AddSqlServerDbContext<FleetDbContext>("fleet", configureDbContextOptions: options =>
{
    options.UseSqlServer(sqlOptions =>
        sqlOptions.MigrationsAssembly("OrangeCarRental.Fleet.Infrastructure"));
});

// Register repositories
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();

// Register application services
builder.Services.AddScoped<SearchVehiclesQueryHandler>();

var app = builder.Build();

// Check if running as migration job
if (args.Contains("--migrate-only"))
{
    var exitCode = await app.RunMigrationsAndExitAsync<FleetDbContext>();
    Environment.Exit(exitCode);
}

// Apply database migrations (auto in dev/Aspire, manual in production)
await app.MigrateDatabaseAsync<FleetDbContext>();

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

app.UseCors("AllowFrontend");

// Map API endpoints
app.MapFleetEndpoints();
app.MapHealthEndpoints();

app.Run();
