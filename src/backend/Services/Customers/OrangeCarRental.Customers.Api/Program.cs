using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Extensions;
using SmartSolutionsLab.OrangeCarRental.Customers.Api.Endpoints;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Data;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Extensions;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Persistence;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);
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
        .Enrich.WithProperty("Application", "CustomersAPI")
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Application}] {Message:lj}{NewLine}{Exception}");

    // Add Seq sink if SEQ_URL is configured and valid
    var seqUrl = context.Configuration["SEQ_URL"];
    if (!string.IsNullOrEmpty(seqUrl) && Uri.TryCreate(seqUrl, UriKind.Absolute, out _))
    {
        configuration.WriteTo.Seq(seqUrl);
    }
});

// Add services to the container
builder.Services.AddOpenApi();
builder.Services.AddOrangeCarRentalJsonOptions();

// CORS for frontend applications (separated by portal)
builder.Services.AddOrangeCarRentalCors();

// Add JWT Authentication and Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddOrangeCarRentalAuthorization();

// Register database context (connection string provided by Aspire)
builder.AddSqlServerDbContext<CustomersDbContext>("customers", configureDbContextOptions: options =>
{
    options.UseSqlServer(sqlOptions =>
    {
        sqlOptions.MigrationsAssembly("OrangeCarRental.Customers.Infrastructure");
        // Retry on transient failures (e.g., database not yet created by migrator)
        sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
    });
});

// Register repository
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

// Register command handlers
builder.Services.AddScoped<ICommandHandler<RegisterCustomerCommand, RegisterCustomerResult>, RegisterCustomerCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateCustomerProfileCommand, UpdateCustomerProfileResult>, UpdateCustomerProfileCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateDriversLicenseCommand, UpdateDriversLicenseResult>, UpdateDriversLicenseCommandHandler>();
builder.Services.AddScoped<ICommandHandler<ChangeCustomerStatusCommand, ChangeCustomerStatusResult>, ChangeCustomerStatusCommandHandler>();

// Register query handlers
builder.Services.AddScoped<IQueryHandler<GetCustomerQuery, CustomerDto>, GetCustomerQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetCustomerByEmailQuery, CustomerDto>, GetCustomerByEmailQueryHandler>();
builder.Services.AddScoped<IQueryHandler<SearchCustomersQuery, PagedResult<CustomerDto>>, SearchCustomersQueryHandler>();

// Register data seeder
builder.Services.AddScoped<CustomerDataSeeder>();

var app = builder.Build();

// Seed database with sample data (development only)
await app.Services.SeedCustomersDataAsync();

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

// Centralized exception handling
app.UseExceptionHandling();

app.UseAllFrontendsCors();
app.UseHttpsRedirection();

// Add Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map API endpoints
app.MapCustomerEndpoints();
app.MapHealthEndpoints<CustomersDbContext>("Customers API");
app.MapDefaultEndpoints();

app.Run();
