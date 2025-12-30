using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Extensions;
using SmartSolutionsLab.OrangeCarRental.Customers.Api.Endpoints;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Data;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Extensions;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Persistence;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProperty("Application", "CustomersAPI")
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Application}] {Message:lj}{NewLine}{Exception}"));

// Add services to the container
builder.Services.AddOpenApi();

// CORS for frontend applications
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4300", "http://localhost:4301", "http://localhost:4302")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add JWT Authentication and Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddOrangeCarRentalAuthorization();

// Register database context (connection string provided by Aspire)
builder.AddSqlServerDbContext<CustomersDbContext>("customers", configureDbContextOptions: options =>
{
    options.UseSqlServer(sqlOptions =>
        sqlOptions.MigrationsAssembly("OrangeCarRental.Customers.Infrastructure"));
});

// Register repository
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

app.UseCors();
app.UseHttpsRedirection();

// Add Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map API endpoints
app.MapCustomerEndpoints();
app.MapDefaultEndpoints();

app.Run();
