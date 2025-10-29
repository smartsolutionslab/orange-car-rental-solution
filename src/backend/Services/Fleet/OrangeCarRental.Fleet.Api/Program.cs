using OrangeCarRental.Fleet.Application.Queries.SearchVehicles;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

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

// Register application services
builder.Services.AddScoped<SearchVehiclesQueryHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Orange Car Rental - Fleet API")
            .WithTheme(ScalarTheme.Mars)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseCors("AllowFrontend");

// Fleet API endpoints
var fleet = app.MapGroup("/api/vehicles")
    .WithTags("Fleet - Vehicles")
    .WithOpenApi();

// GET /api/vehicles - Search vehicles
fleet.MapGet("/", async (
    [AsParameters] SearchVehiclesQuery query,
    SearchVehiclesQueryHandler handler,
    CancellationToken cancellationToken) =>
{
    var result = await handler.HandleAsync(query, cancellationToken);
    return Results.Ok(result);
})
.WithName("SearchVehicles")
.WithSummary("Search available vehicles")
.WithDescription("Search and filter vehicles by location, category, dates, and other criteria. Returns vehicles with German VAT-inclusive pricing (19%).")
.Produces<SearchVehiclesResult>(StatusCodes.Status200OK)
.ProducesProblem(StatusCodes.Status400BadRequest);

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
    service = "Fleet API",
    status = "Healthy",
    timestamp = DateTime.UtcNow
}))
.WithTags("Health")
.WithName("HealthCheck");

app.Run();
