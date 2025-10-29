var builder = WebApplication.CreateBuilder(args);

// Add YARP Reverse Proxy
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
}));

app.Run();
