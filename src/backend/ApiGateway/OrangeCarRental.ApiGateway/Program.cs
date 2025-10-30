using Serilog;

var builder = WebApplication.CreateBuilder(args);

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

Log.Information("Fleet API URL: {FleetApiUrl}", fleetApiUrl);
Log.Information("Reservations API URL: {ReservationsApiUrl}", reservationsApiUrl);

// Update configuration with actual URLs
builder.Configuration["ReverseProxy:Clusters:fleet-cluster:Destinations:destination1:Address"] = fleetApiUrl;
builder.Configuration["ReverseProxy:Clusters:reservations-cluster:Destinations:destination1:Address"] = reservationsApiUrl;

// Add YARP Reverse Proxy with configuration
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add CORS for frontend applications
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost:4201", "https://localhost:4200", "https://localhost:4201")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

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

app.UseCors();

// Map reverse proxy
app.MapReverseProxy();

// Health check
app.MapGet("/health", () => Results.Ok(new
{
    service = "API Gateway",
    status = "Healthy",
    timestamp = DateTime.UtcNow
})).WithName("HealthCheck");

app.Run();
