using Serilog;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults (OpenTelemetry, health checks, service discovery, resilience)
builder.AddServiceDefaults();

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProperty("Application", "APIGateway")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Application}] {Message:lj}{NewLine}{Exception}"));

// Get backend service URLs from Aspire environment variables
var fleetApiUrl = builder.Configuration["FLEET_API_URL"] ?? "http://localhost:5000";
var reservationsApiUrl = builder.Configuration["RESERVATIONS_API_URL"] ?? "http://localhost:5001";
var pricingApiUrl = builder.Configuration["PRICING_API_URL"] ?? "http://localhost:5002";
var customersApiUrl = builder.Configuration["CUSTOMERS_API_URL"] ?? "http://localhost:5003";
var paymentsApiUrl = builder.Configuration["PAYMENTS_API_URL"] ?? "http://localhost:5004";
var notificationsApiUrl = builder.Configuration["NOTIFICATIONS_API_URL"] ?? "http://localhost:5005";
var locationsApiUrl = builder.Configuration["LOCATIONS_API_URL"] ?? "http://localhost:5006";

Log.Information("Fleet API URL: {FleetApiUrl}", fleetApiUrl);
Log.Information("Reservations API URL: {ReservationsApiUrl}", reservationsApiUrl);
Log.Information("Pricing API URL: {PricingApiUrl}", pricingApiUrl);
Log.Information("Customers API URL: {CustomersApiUrl}", customersApiUrl);
Log.Information("Payments API URL: {PaymentsApiUrl}", paymentsApiUrl);
Log.Information("Notifications API URL: {NotificationsApiUrl}", notificationsApiUrl);
Log.Information("Locations API URL: {LocationsApiUrl}", locationsApiUrl);

// Update configuration with actual URLs
builder.Configuration["ReverseProxy:Clusters:fleet-cluster:Destinations:destination1:Address"] = fleetApiUrl;
builder.Configuration["ReverseProxy:Clusters:reservations-cluster:Destinations:destination1:Address"] = reservationsApiUrl;
builder.Configuration["ReverseProxy:Clusters:pricing-cluster:Destinations:destination1:Address"] = pricingApiUrl;
builder.Configuration["ReverseProxy:Clusters:customers-cluster:Destinations:destination1:Address"] = customersApiUrl;
builder.Configuration["ReverseProxy:Clusters:payments-cluster:Destinations:destination1:Address"] = paymentsApiUrl;
builder.Configuration["ReverseProxy:Clusters:notifications-cluster:Destinations:destination1:Address"] = notificationsApiUrl;
builder.Configuration["ReverseProxy:Clusters:locations-cluster:Destinations:destination1:Address"] = locationsApiUrl;

// Add YARP Reverse Proxy with configuration
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add JWT Authentication (validates tokens at the gateway)
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add CORS for frontend applications (separated by portal)
builder.Services.AddOrangeCarRentalCors();

var app = builder.Build();

// Add Serilog request logging
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent);
        diagnosticContext.Set("ForwardedFor", httpContext.Request.Headers["X-Forwarded-For"]);
    };
});

app.UseAllFrontendsCors();
app.UseHttpsRedirection();

// Add Authentication middleware (validates JWT tokens)
app.UseAuthentication();

// Map reverse proxy (will forward Authorization header to backend services)
app.MapReverseProxy();

// Map default health check endpoints
app.MapDefaultEndpoints();

app.Run();
