var builder = DistributedApplication.CreateBuilder(args);

// SQL Server container for both databases
var sqlServer = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);

// Fleet database
var fleetDb = sqlServer.AddDatabase("fleet", "OrangeCarRental_Fleet");

// Reservations database
var reservationsDb = sqlServer.AddDatabase("reservations", "OrangeCarRental_Reservations");

// Check if we should run migrations as separate jobs (for Azure deployment simulation)
var runMigrationJobs = builder.Configuration["RunMigrationJobs"]?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;

IResourceBuilder<ProjectResource> fleetApi;
IResourceBuilder<ProjectResource> reservationsApi;

if (runMigrationJobs)
{
    // Migration jobs - run before APIs start (for Azure deployment pattern)
    var fleetMigration = builder.AddProject<Projects.OrangeCarRental_Fleet_Api>("fleet-migration")
        .WithReference(fleetDb)
        .WithArgs("--migrate-only");

    var reservationsMigration = builder.AddProject<Projects.OrangeCarRental_Reservations_Api>("reservations-migration")
        .WithReference(reservationsDb)
        .WithArgs("--migrate-only");

    // Fleet API - wait for migrations to complete
    fleetApi = builder.AddProject<Projects.OrangeCarRental_Fleet_Api>("fleet-api")
        .WithReference(fleetDb)
        .WaitFor(fleetMigration);

    // Reservations API - wait for migrations to complete
    reservationsApi = builder.AddProject<Projects.OrangeCarRental_Reservations_Api>("reservations-api")
        .WithReference(reservationsDb)
        .WaitFor(reservationsMigration);
}
else
{
    // Standard mode - APIs auto-migrate on startup (default for local development)
    // Fleet API
    fleetApi = builder.AddProject<Projects.OrangeCarRental_Fleet_Api>("fleet-api")
        .WithReference(fleetDb);

    // Reservations API
    reservationsApi = builder.AddProject<Projects.OrangeCarRental_Reservations_Api>("reservations-api")
        .WithReference(reservationsDb);
}

// API Gateway (port 5002 configured in launchSettings.json)
var apiGateway = builder.AddProject<Projects.OrangeCarRental_ApiGateway>("api-gateway")
    .WithEnvironment("FLEET_API_URL", fleetApi.GetEndpoint("http"))
    .WithEnvironment("RESERVATIONS_API_URL", reservationsApi.GetEndpoint("http"))
    .WithExternalHttpEndpoints();

// Frontend Applications (Aspire passes port via PORT environment variable)
var publicPortal = builder.AddNpmApp("public-portal", "../../../frontend/apps/public-portal", "start")
    .WithHttpEndpoint(port: 4200, env: "PORT")
    .WithReference(apiGateway)
    .WithEnvironment("API_URL", apiGateway.GetEndpoint("http"))
    .WithExternalHttpEndpoints();

var callCenterPortal = builder.AddNpmApp("call-center-portal", "../../../frontend/apps/call-center-portal", "start")
    .WithHttpEndpoint(port: 4201, env: "PORT")
    .WithReference(apiGateway)
    .WithEnvironment("API_URL", apiGateway.GetEndpoint("http"))
    .WithExternalHttpEndpoints();

builder.Build().Run();
