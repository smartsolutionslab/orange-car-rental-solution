using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Extensions;
using SmartSolutionsLab.OrangeCarRental.Notifications.Api.Endpoints;
using SmartSolutionsLab.OrangeCarRental.Notifications.Application.Commands;
using SmartSolutionsLab.OrangeCarRental.Notifications.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Notifications.Domain;
using SmartSolutionsLab.OrangeCarRental.Notifications.Domain.Notification;
using SmartSolutionsLab.OrangeCarRental.Notifications.Infrastructure.Persistence;
using SmartSolutionsLab.OrangeCarRental.Notifications.Infrastructure.Services;

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
        .Enrich.WithProperty("Application", "NotificationsAPI")
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Application}] {Message:lj}{NewLine}{Exception}");

    // Add Seq sink if SEQ_URL is configured
    var seqUrl = context.Configuration["SEQ_URL"];
    if (!string.IsNullOrEmpty(seqUrl))
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

// Register database context (connection string provided by Aspire)
builder.AddSqlServerDbContext<NotificationsDbContext>("notifications", configureDbContextOptions: options =>
{
    options.UseSqlServer(sqlOptions =>
        sqlOptions.MigrationsAssembly("OrangeCarRental.Notifications.Infrastructure"));
});

// Register Unit of Work and repositories
builder.Services.AddScoped<INotificationsUnitOfWork, NotificationsUnitOfWork>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

// Register infrastructure services (stub implementations for now)
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISmsService, SmsService>();

// Register command handlers
builder.Services.AddScoped<ICommandHandler<SendEmailCommand, SendEmailResult>, SendEmailCommandHandler>();
builder.Services.AddScoped<ICommandHandler<SendSmsCommand, SendSmsResult>, SendSmsCommandHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Orange Car Rental - Notifications API")
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
app.MapNotificationsEndpoints();
app.MapHealthEndpoints<NotificationsDbContext>("Notifications API");
app.MapDefaultEndpoints();

app.Run();
