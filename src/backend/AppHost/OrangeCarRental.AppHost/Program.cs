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
    .WithReference(fleetDb)
    .WithExternalHttpEndpoints();

// Reservations API
var reservationsApi = builder.AddProject<Projects.OrangeCarRental_Reservations_Api>("reservations-api")
    .WithReference(reservationsDb)
    .WithExternalHttpEndpoints();

builder.Build().Run();
