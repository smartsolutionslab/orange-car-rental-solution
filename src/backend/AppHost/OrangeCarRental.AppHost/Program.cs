using Projects;

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

// Customers database - manages customer profiles and driver's license information
var customersDb = sqlServer.AddDatabase("customers", "OrangeCarRental_Customers");

// Check if we should run migrations as separate jobs (for Azure deployment simulation)
var runMigrationJobs = builder.Configuration["RunMigrationJobs"]?.Equals("true", StringComparison.OrdinalIgnoreCase) ??
                       false;

// Fleet API - Vehicle inventory and availability management
// Also needs read-only access to Reservations database for date filtering
var fleetApi = builder
    .AddProject<OrangeCarRental_Fleet_Api>("fleet-api")
    .WithReference(fleetDb)
    .WithReference(reservationsDb)
    .WaitFor(sqlServer);

// Pricing API - Pricing policy and rental rate calculation
// Must be defined before Reservations API since Reservations depends on it
var pricingApi = builder
    .AddProject<OrangeCarRental_Pricing_Api>("pricing-api")
    .WithReference(pricingDb)
    .WaitFor(sqlServer);

// Reservations API - Customer booking and rental management
// Also needs access to Pricing API for automatic price calculation
var reservationsApi = builder
    .AddProject<OrangeCarRental_Reservations_Api>("reservations-api")
    .WithReference(reservationsDb)
    .WithEnvironment("PRICING_API_URL", pricingApi.GetEndpoint("http"))
    .WaitFor(sqlServer)
    .WaitFor(pricingApi);

// Customers API - Customer profile and driver's license management
var customersApi = builder
    .AddProject<OrangeCarRental_Customers_Api>("customers-api")
    .WithReference(customersDb)
    .WaitFor(sqlServer);

if (runMigrationJobs)
{
    // Migration jobs - run before APIs start (for Azure deployment pattern)
    var fleetMigration = builder.AddProject<OrangeCarRental_Fleet_Api>("fleet-migration")
        .WithReference(fleetDb)
        .WithArgs("--migrate-only");

    var reservationsMigration = builder.AddProject<OrangeCarRental_Reservations_Api>("reservations-migration")
        .WithReference(reservationsDb)
        .WithArgs("--migrate-only");

    var pricingMigration = builder.AddProject<OrangeCarRental_Pricing_Api>("pricing-migration")
        .WithReference(pricingDb)
        .WithArgs("--migrate-only");

    var customersMigration = builder.AddProject<OrangeCarRental_Customers_Api>("customers-migration")
        .WithReference(customersDb)
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

    // Customers API - wait for migrations to complete
    customersApi = customersApi
        .WaitFor(customersMigration);
}

// API Gateway - YARP reverse proxy with service discovery
// Routes /api/vehicles/* to Fleet API, /api/reservations/* to Reservations API, /api/pricing/* to Pricing API, and /api/customers/* to Customers API
// Configured on port 5002 (see launchSettings.json)
var apiGateway = builder.AddProject<OrangeCarRental_ApiGateway>("api-gateway")
    .WithEnvironment("FLEET_API_URL", fleetApi.GetEndpoint("http"))
    .WithEnvironment("RESERVATIONS_API_URL", reservationsApi.GetEndpoint("http"))
    .WithEnvironment("PRICING_API_URL", pricingApi.GetEndpoint("http"))
    .WithEnvironment("CUSTOMERS_API_URL", customersApi.GetEndpoint("http"))
    .WithExternalHttpEndpoints()
    .WaitFor(fleetApi)
    .WaitFor(reservationsApi)
    .WaitFor(pricingApi)
    .WaitFor(customersApi);

// Public Portal - Customer-facing Angular application for vehicle search and booking
// Accessible at http://localhost:4200
var publicPortal = builder.AddNpmApp("public-portal", "../../../frontend/apps/public-portal")
    .WithHttpEndpoint(4200, env: "PORT")
    .WithReference(apiGateway)
    .WithEnvironment("API_URL", apiGateway.GetEndpoint("http"))
    .WithExternalHttpEndpoints()
    .WaitFor(apiGateway);

// Call Center Portal - Agent-facing Angular application for reservation management
// Accessible at http://localhost:4201
var callCenterPortal = builder.AddNpmApp("call-center-portal", "../../../frontend/apps/call-center-portal")
    .WithHttpEndpoint(4201, env: "PORT")
    .WithReference(apiGateway)
    .WithEnvironment("API_URL", apiGateway.GetEndpoint("http"))
    .WithExternalHttpEndpoints()
    .WaitFor(apiGateway);

builder.Build().Run();
