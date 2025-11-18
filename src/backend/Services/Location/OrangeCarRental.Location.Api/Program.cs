using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Extensions;
using SmartSolutionsLab.OrangeCarRental.Location.Api.Extensions;
using SmartSolutionsLab.OrangeCarRental.Location.Application.Commands.CreateLocation;
using SmartSolutionsLab.OrangeCarRental.Location.Application.Queries.GetAllLocations;
using SmartSolutionsLab.OrangeCarRental.Location.Domain.Location;
using SmartSolutionsLab.OrangeCarRental.Location.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProperty("Application", "LocationAPI")
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

// Add JWT Authentication and Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddOrangeCarRentalAuthorization();

// Register database context (connection string provided by Aspire)
builder.AddSqlServerDbContext<LocationsDbContext>("locations", configureDbContextOptions: options =>
{
    options.UseSqlServer(sqlOptions =>
        sqlOptions.MigrationsAssembly("OrangeCarRental.Location.Infrastructure"));
});

// Register repositories
builder.Services.AddScoped<ILocationRepository, LocationRepository>();

// Register command handlers
builder.Services.AddScoped<CreateLocationCommandHandler>();

// Register query handlers
builder.Services.AddScoped<GetAllLocationsQueryHandler>();

var app = builder.Build();

// Check if running as migration job
if (args.Contains("--migrate-only"))
{
    var exitCode = await app.RunMigrationsAndExitAsync<LocationsDbContext>();
    Environment.Exit(exitCode);
}

// Apply database migrations (auto in dev/Aspire, manual in production)
await app.MigrateDatabaseAsync<LocationsDbContext>();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Orange Car Rental - Location API")
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

// Add Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map API endpoints
app.MapLocationEndpoints();
app.MapHealthEndpoints();

app.Run();
