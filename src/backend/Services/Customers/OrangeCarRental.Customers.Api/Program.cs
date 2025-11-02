using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using SmartSolutionsLab.OrangeCarRental.Customers.Api.Extensions;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.RegisterCustomer;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.UpdateCustomerProfile;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.UpdateDriversLicense;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.ChangeCustomerStatus;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries.GetCustomer;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries.GetCustomerByEmail;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries.SearchCustomers;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Repositories;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Data;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Persistence;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProperty("Application", "CustomersAPI")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Application}] {Message:lj}{NewLine}{Exception}"));

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
builder.AddSqlServerDbContext<CustomersDbContext>("customers", configureDbContextOptions: options =>
{
    options.UseSqlServer(sqlOptions =>
        sqlOptions.MigrationsAssembly("OrangeCarRental.Customers.Infrastructure"));
});

// Register repositories
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

// Register command handlers
builder.Services.AddScoped<RegisterCustomerCommandHandler>();
builder.Services.AddScoped<UpdateCustomerProfileCommandHandler>();
builder.Services.AddScoped<UpdateDriversLicenseCommandHandler>();
builder.Services.AddScoped<ChangeCustomerStatusCommandHandler>();

// Register query handlers
builder.Services.AddScoped<GetCustomerQueryHandler>();
builder.Services.AddScoped<GetCustomerByEmailQueryHandler>();
builder.Services.AddScoped<SearchCustomersQueryHandler>();

// Register data seeder
builder.Services.AddScoped<CustomerDataSeeder>();

var app = builder.Build();

// Check if running as migration job
if (args.Contains("--migrate-only"))
{
    var exitCode = await app.RunMigrationsAndExitAsync<CustomersDbContext>();
    Environment.Exit(exitCode);
}

// Apply database migrations (auto in dev/Aspire, manual in production)
await app.MigrateDatabaseAsync<CustomersDbContext>();

// Seed database with sample data (development only)
await app.SeedCustomersDataAsync();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Orange Car Rental - Customers API")
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
app.MapCustomerEndpoints();
app.MapHealthEndpoints();

app.Run();
