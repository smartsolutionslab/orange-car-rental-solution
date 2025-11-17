using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using SmartSolutionsLab.OrangeCarRental.Payments.Api.Extensions;
using SmartSolutionsLab.OrangeCarRental.Payments.Application.Commands;
using SmartSolutionsLab.OrangeCarRental.Payments.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;
using SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Persistence;
using SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProperty("Application", "PaymentsAPI")
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

// Register database context
builder.AddSqlServerDbContext<PaymentsDbContext>("payments", configureDbContextOptions: options =>
{
    options.UseSqlServer(sqlOptions =>
        sqlOptions.MigrationsAssembly("OrangeCarRental.Payments.Infrastructure"));
});

// Register repositories
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

// Register infrastructure services
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Register command handlers
builder.Services.AddScoped<ProcessPaymentCommandHandler>();
builder.Services.AddScoped<RefundPaymentCommandHandler>();

var app = builder.Build();

// Check if running as migration job
if (args.Contains("--migrate-only"))
{
    var exitCode = await app.RunMigrationsAndExitAsync<PaymentsDbContext>();
    Environment.Exit(exitCode);
}

// Apply database migrations
await app.MigrateDatabaseAsync<PaymentsDbContext>();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Orange Car Rental - Payments API")
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
app.MapPaymentEndpoints();
app.MapHealthEndpoints();

app.Run();
