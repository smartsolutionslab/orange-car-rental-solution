var builder = DistributedApplication.CreateBuilder(args);

// SQL Server container for both databases
// Persistent lifetime ensures data survives container restarts
var sqlServer = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);

// Fleet database - manages vehicle inventory and availability
var fleetDb = sqlServer.AddDatabase("fleet", "OrangeCarRental_Fleet");

// Reservations database - manages customer bookings and rental history
var reservationsDb = sqlServer.AddDatabase("reservations", "OrangeCarRental_Reservations");

// Pricing database - manages pricing policies and rate calculations
var pricingDb = sqlServer.AddDatabase("pricing", "OrangeCarRental_Pricing");

// Check if we should run migrations as separate jobs (for Azure deployment simulation)
var runMigrationJobs = builder.Configuration["RunMigrationJobs"]?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;

// Fleet API - Vehicle inventory and availability management
// Also needs read-only access to Reservations database for date filtering
IResourceBuilder<ProjectResource> fleetApi = builder
    .AddProject<Projects.OrangeCarRental_Fleet_Api>("fleet-api")
    .WithReference(fleetDb)
    .WithReference(reservationsDb)
    .WaitFor(sqlServer);

// Pricing API - Pricing policy and rental rate calculation
// Must be defined before Reservations API since Reservations depends on it
IResourceBuilder<ProjectResource> pricingApi = builder
    .AddProject<Projects.OrangeCarRental_Pricing_Api>("pricing-api")
    .WithReference(pricingDb)
    .WaitFor(sqlServer);

// Reservations API - Customer booking and rental management
// Also needs access to Pricing API for automatic price calculation
IResourceBuilder<ProjectResource> reservationsApi = builder
    .AddProject<Projects.OrangeCarRental_Reservations_Api>("reservations-api")
    .WithReference(reservationsDb)
    .WithEnvironment("PRICING_API_URL", pricingApi.GetEndpoint("http"))
    .WaitFor(sqlServer)
    .WaitFor(pricingApi);

if (runMigrationJobs)
{
    // Migration jobs - run before APIs start (for Azure deployment pattern)
    var fleetMigration = builder.AddProject<Projects.OrangeCarRental_Fleet_Api>("fleet-migration")
        .WithReference(fleetDb)
        .WithArgs("--migrate-only");

    var reservationsMigration = builder.AddProject<Projects.OrangeCarRental_Reservations_Api>("reservations-migration")
        .WithReference(reservationsDb)
        .WithArgs("--migrate-only");

    var pricingMigration = builder.AddProject<Projects.OrangeCarRental_Pricing_Api>("pricing-migration")
        .WithReference(pricingDb)
        .WithArgs("--migrate-only");

    // Fleet API - wait for migrations to complete
    fleetApi = fleetApi
        .WaitFor(fleetMigration);

    // Reservations API - wait for migrations to complete
    reservationsApi = reservationsApi
        .WaitFor(reservationsMigration);

    // Pricing API - wait for migrations to complete
    pricingApi = pricingApi
        .WaitFor(pricingMigration);
}

// API Gateway - YARP reverse proxy with service discovery
// Routes /api/vehicles/* to Fleet API, /api/reservations/* to Reservations API, and /api/pricing/* to Pricing API
// Configured on port 5002 (see launchSettings.json)
var apiGateway = builder.AddProject<Projects.OrangeCarRental_ApiGateway>("api-gateway")
    .WithEnvironment("FLEET_API_URL", fleetApi.GetEndpoint("http"))
    .WithEnvironment("RESERVATIONS_API_URL", reservationsApi.GetEndpoint("http"))
    .WithEnvironment("PRICING_API_URL", pricingApi.GetEndpoint("http"))
    .WithExternalHttpEndpoints()
    .WaitFor(fleetApi)
    .WaitFor(reservationsApi)
    .WaitFor(pricingApi);

// Public Portal - Customer-facing Angular application for vehicle search and booking
// Accessible at http://localhost:4200
var publicPortal = builder.AddNpmApp("public-portal", "../../../frontend/apps/public-portal", "start")
    .WithHttpEndpoint(port: 4200, env: "PORT")
    .WithReference(apiGateway)
    .WithEnvironment("API_URL", apiGateway.GetEndpoint("http"))
    .WithExternalHttpEndpoints()
    .WaitFor(apiGateway);

// Call Center Portal - Agent-facing Angular application for reservation management
// Accessible at http://localhost:4201
var callCenterPortal = builder.AddNpmApp("call-center-portal", "../../../frontend/apps/call-center-portal", "start")
    .WithHttpEndpoint(port: 4201, env: "PORT")
    .WithReference(apiGateway)
    .WithEnvironment("API_URL", apiGateway.GetEndpoint("http"))
    .WithExternalHttpEndpoints()
    .WaitFor(apiGateway);

builder.Build().Run();
