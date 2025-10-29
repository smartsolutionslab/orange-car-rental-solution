var builder = WebApplication.CreateBuilder(args);

// Get backend service URLs from Aspire environment variables
var fleetApiUrl = builder.Configuration["FLEET_API_URL"] ?? "http://localhost:5000";
var reservationsApiUrl = builder.Configuration["RESERVATIONS_API_URL"] ?? "http://localhost:5001";

Console.WriteLine($"Fleet API URL: {fleetApiUrl}");
Console.WriteLine($"Reservations API URL: {reservationsApiUrl}");

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
