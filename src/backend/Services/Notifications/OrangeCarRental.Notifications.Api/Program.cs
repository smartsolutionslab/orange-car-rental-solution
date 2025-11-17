using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using SmartSolutionsLab.OrangeCarRental.Notifications.Api.Extensions;
using SmartSolutionsLab.OrangeCarRental.Notifications.Application.Commands.SendEmail;
using SmartSolutionsLab.OrangeCarRental.Notifications.Application.Commands.SendSms;
using SmartSolutionsLab.OrangeCarRental.Notifications.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Notifications.Domain.Notification;
using SmartSolutionsLab.OrangeCarRental.Notifications.Infrastructure.Persistence;
using SmartSolutionsLab.OrangeCarRental.Notifications.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProperty("Application", "NotificationsAPI")
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
builder.AddSqlServerDbContext<NotificationsDbContext>("notifications", configureDbContextOptions: options =>
{
    options.UseSqlServer(sqlOptions =>
        sqlOptions.MigrationsAssembly("OrangeCarRental.Notifications.Infrastructure"));
});

// Register repositories
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

// Register infrastructure services (stub implementations for now)
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISmsService, SmsService>();

// Register command handlers
builder.Services.AddScoped<SendEmailCommandHandler>();
builder.Services.AddScoped<SendSmsCommandHandler>();

var app = builder.Build();

// Check if running as migration job
if (args.Contains("--migrate-only"))
{
    var exitCode = await app.RunMigrationsAndExitAsync<NotificationsDbContext>();
    Environment.Exit(exitCode);
}

// Apply database migrations (auto in dev/Aspire, manual in production)
await app.MigrateDatabaseAsync<NotificationsDbContext>();

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

app.UseCors("AllowFrontend");

// Map API endpoints
app.MapNotificationsEndpoints();
app.MapHealthEndpoints();

app.Run();
