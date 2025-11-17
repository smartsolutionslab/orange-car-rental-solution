using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using SmartSolutionsLab.OrangeCarRental.Pricing.Api.Extensions;
using SmartSolutionsLab.OrangeCarRental.Pricing.Application.Queries.CalculatePrice;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;
using SmartSolutionsLab.OrangeCarRental.Pricing.Infrastructure.Data;
using SmartSolutionsLab.OrangeCarRental.Pricing.Infrastructure.Persistence;
using SmartSolutionsLab.OrangeCarRental.Pricing.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProperty("Application", "PricingAPI")
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

// Register database context (connection string provided by Aspire)
builder.AddSqlServerDbContext<PricingDbContext>("pricing", configureDbContextOptions: options =>
{
    options.UseSqlServer(sqlOptions =>
        sqlOptions.MigrationsAssembly("OrangeCarRental.Pricing.Infrastructure"));
});

// Register repositories
builder.Services.AddScoped<IPricingPolicyRepository, PricingPolicyRepository>();

// Register application services
builder.Services.AddScoped<CalculatePriceQueryHandler>();

// Register data seeder
builder.Services.AddScoped<PricingDataSeeder>();

var app = builder.Build();

// Seed database with sample data (development only)
await app.SeedPricingDataAsync();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Orange Car Rental - Pricing API")
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
app.MapPricingEndpoints();
app.MapHealthEndpoints();

app.Run();
