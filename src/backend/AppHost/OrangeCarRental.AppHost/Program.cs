var builder = DistributedApplication.CreateBuilder(args);

// SQL Server container for both databases
var sqlServer = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);

// Fleet database
var fleetDb = sqlServer.AddDatabase("fleet", "OrangeCarRental_Fleet");

// Reservations database
var reservationsDb = sqlServer.AddDatabase("reservations", "OrangeCarRental_Reservations");

// Fleet API
var fleetApi = builder.AddProject<Projects.OrangeCarRental_Fleet_Api>("fleet-api")
    .WithReference(fleetDb);

// Reservations API
var reservationsApi = builder.AddProject<Projects.OrangeCarRental_Reservations_Api>("reservations-api")
    .WithReference(reservationsDb);

// API Gateway
var apiGateway = builder.AddProject<Projects.OrangeCarRental_ApiGateway>("api-gateway")
    .WithReference(fleetApi)
    .WithReference(reservationsApi)
    .WithExternalHttpEndpoints();

// Frontend Applications
var publicPortal = builder.AddNpmApp("public-portal", "../../../../frontend/apps/public-portal", "start")
    .WithHttpEndpoint(port: 4200, env: "PORT")
    .WithReference(apiGateway)
    .WithExternalHttpEndpoints();

var callCenterPortal = builder.AddNpmApp("call-center-portal", "../../../../frontend/apps/call-center-portal", "start")
    .WithHttpEndpoint(port: 4201, env: "PORT")
    .WithReference(apiGateway)
    .WithExternalHttpEndpoints();

builder.Build().Run();
