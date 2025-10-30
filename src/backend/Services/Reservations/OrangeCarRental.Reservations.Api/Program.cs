using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SmartSolutionsLab.OrangeCarRental.Reservations.Api.Extensions;
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

// Check if running as migration job
if (args.Contains("--migrate-only"))
{
    var exitCode = await app.RunMigrationsAndExitAsync<ReservationsDbContext>();
    Environment.Exit(exitCode);
}

// Apply database migrations (auto in dev/Aspire, manual in production)
await app.MigrateDatabaseAsync<ReservationsDbContext>();

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

app.UseCors();
app.UseHttpsRedirection();

// Map API endpoints
app.MapReservationEndpoints();
app.MapHealthEndpoints();

app.Run();
