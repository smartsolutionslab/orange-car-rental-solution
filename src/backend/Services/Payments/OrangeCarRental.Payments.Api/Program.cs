using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Extensions;
using SmartSolutionsLab.OrangeCarRental.Payments.Api.Endpoints;
using SmartSolutionsLab.OrangeCarRental.Payments.Application.Commands;
using SmartSolutionsLab.OrangeCarRental.Payments.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Sepa;
using SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Configuration;
using SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Persistence;
using SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Services;

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
        .Enrich.WithProperty("Application", "PaymentsAPI")
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

// CORS for Call Center Portal only (internal staff operations)
builder.Services.AddCallCenterPortalCors();

// Add JWT Authentication and Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddOrangeCarRentalAuthorization();

// Register database context
builder.AddSqlServerDbContext<PaymentsDbContext>("payments", configureDbContextOptions: options =>
{
    options.UseSqlServer(sqlOptions =>
    {
        sqlOptions.MigrationsAssembly("OrangeCarRental.Payments.Infrastructure");
        // Retry on transient failures (e.g., database not yet created by migrator)
        sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
    });
});

// Register configuration
builder.Services.Configure<SepaConfiguration>(
    builder.Configuration.GetSection(SepaConfiguration.SectionName));

// Register Unit of Work and repositories
builder.Services.AddScoped<IPaymentsUnitOfWork, PaymentsUnitOfWork>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<ISepaMandateRepository, SepaMandateRepository>();

// Register infrastructure services
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IInvoiceGenerator, InvoiceGenerator>();
builder.Services.AddScoped<IInvoiceEmailSender, InvoiceEmailSender>();

// Register command handlers
builder.Services.AddScoped<ICommandHandler<ProcessPaymentCommand, ProcessPaymentResult>, ProcessPaymentCommandHandler>();
builder.Services.AddScoped<ICommandHandler<RefundPaymentCommand, RefundPaymentResult>, RefundPaymentCommandHandler>();
builder.Services.AddScoped<ICommandHandler<GenerateInvoiceCommand, GenerateInvoiceResult>, GenerateInvoiceCommandHandler>();

var app = builder.Build();

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

// Centralized exception handling
app.UseExceptionHandling();

app.UseCallCenterPortalCors();
app.UseHttpsRedirection();

// Add Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map API endpoints
app.MapPaymentEndpoints();
app.MapInvoiceEndpoints();
app.MapHealthEndpoints<PaymentsDbContext>("Payments API");
app.MapDefaultEndpoints();

app.Run();
