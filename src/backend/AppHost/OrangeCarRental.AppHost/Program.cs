using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Keycloak - Identity and Access Management
// Persistent lifetime ensures realm configuration survives container restarts
var keycloak = builder.AddContainer("keycloak", "quay.io/keycloak/keycloak", "26.0.7")
    .WithHttpEndpoint(port: 8080, targetPort: 8080, name: "http")
    .WithEnvironment("KEYCLOAK_ADMIN", "admin")
    .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", "admin")
    .WithEnvironment("KC_HEALTH_ENABLED", "true")
    .WithEnvironment("KC_METRICS_ENABLED", "true")
    .WithArgs("start-dev")
    .WithLifetime(ContainerLifetime.Persistent);

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

// Payments database - manages payment transactions
var paymentsDb = sqlServer.AddDatabase("payments", "OrangeCarRental_Payments");

// Notifications database - manages email and SMS notifications
var notificationsDb = sqlServer.AddDatabase("notifications", "OrangeCarRental_Notifications");

// Locations database - manages rental locations
var locationsDb = sqlServer.AddDatabase("locations", "OrangeCarRental_Locations");

// Database Migrator - Standalone console app for running all database migrations
// Can be triggered manually from the Aspire dashboard
var dbMigrator = builder.AddProject<Projects.OrangeCarRental_Database_Migrator>("db-migrator")
    .WithReference(fleetDb)
    .WithReference(reservationsDb)
    .WithReference(pricingDb)
    .WithReference(customersDb)
    .WithReference(paymentsDb)
    .WithReference(notificationsDb)
    .WithReference(locationsDb)
    .WaitFor(sqlServer);

// Fleet API - Vehicle inventory and availability management
// Also needs read-only access to Reservations database for date filtering
var fleetApi = builder
    .AddProject<OrangeCarRental_Fleet_Api>("fleet-api")
    .WithReference(fleetDb)
    .WithReference(reservationsDb)
    .WithEnvironment("Authentication__Keycloak__Authority", () => $"{keycloak.GetEndpoint("http")}/realms/orange-car-rental")
    .WithEnvironment("Authentication__Keycloak__Audience", "orange-car-rental-api")
    .WithEnvironment("Authentication__Keycloak__RequireHttpsMetadata", "false")
    .WithEnvironment("Authentication__Keycloak__ValidateIssuer", "true")
    .WithEnvironment("Authentication__Keycloak__ValidateAudience", "false")
    .WaitFor(keycloak)
    .WaitFor(sqlServer);

// Pricing API - Pricing policy and rental rate calculation
// Must be defined before Reservations API since Reservations depends on it
var pricingApi = builder
    .AddProject<OrangeCarRental_Pricing_Api>("pricing-api")
    .WithReference(pricingDb)
    .WithEnvironment("Authentication__Keycloak__Authority", () => $"{keycloak.GetEndpoint("http")}/realms/orange-car-rental")
    .WithEnvironment("Authentication__Keycloak__Audience", "orange-car-rental-api")
    .WithEnvironment("Authentication__Keycloak__RequireHttpsMetadata", "false")
    .WithEnvironment("Authentication__Keycloak__ValidateIssuer", "true")
    .WithEnvironment("Authentication__Keycloak__ValidateAudience", "false")
    .WaitFor(keycloak)
    .WaitFor(sqlServer);

// Customers API - Customer profile and driver's license management
// Must be defined before Reservations API since Reservations depends on it
var customersApi = builder
    .AddProject<OrangeCarRental_Customers_Api>("customers-api")
    .WithReference(customersDb)
    .WithEnvironment("Authentication__Keycloak__Authority", () => $"{keycloak.GetEndpoint("http")}/realms/orange-car-rental")
    .WithEnvironment("Authentication__Keycloak__Audience", "orange-car-rental-api")
    .WithEnvironment("Authentication__Keycloak__RequireHttpsMetadata", "false")
    .WithEnvironment("Authentication__Keycloak__ValidateIssuer", "true")
    .WithEnvironment("Authentication__Keycloak__ValidateAudience", "false")
    .WaitFor(keycloak)
    .WaitFor(sqlServer);

// Reservations API - Customer booking and rental management
// Uses Service Discovery to communicate with Pricing and Customers APIs
var reservationsApi = builder
    .AddProject<OrangeCarRental_Reservations_Api>("reservations-api")
    .WithReference(reservationsDb)
    .WithReference(pricingApi)
    .WithReference(customersApi)
    .WithEnvironment("Authentication__Keycloak__Authority", () => $"{keycloak.GetEndpoint("http")}/realms/orange-car-rental")
    .WithEnvironment("Authentication__Keycloak__Audience", "orange-car-rental-api")
    .WithEnvironment("Authentication__Keycloak__RequireHttpsMetadata", "false")
    .WithEnvironment("Authentication__Keycloak__ValidateIssuer", "true")
    .WithEnvironment("Authentication__Keycloak__ValidateAudience", "false")
    .WaitFor(keycloak)
    .WaitFor(sqlServer)
    .WaitFor(pricingApi)
    .WaitFor(customersApi);

// API Gateway - YARP reverse proxy
// Routes /api/vehicles/* to Fleet API, /api/reservations/* to Reservations API, etc.
// YARP requires explicit URLs for cluster configuration, so we use GetEndpoint() here
// Note: This is different from service-to-service communication which uses WithReference()
var apiGateway = builder.AddProject<OrangeCarRental_ApiGateway>("api-gateway")
    .WithEnvironment("FLEET_API_URL", fleetApi.GetEndpoint("http"))
    .WithEnvironment("RESERVATIONS_API_URL", reservationsApi.GetEndpoint("http"))
    .WithEnvironment("PRICING_API_URL", pricingApi.GetEndpoint("http"))
    .WithEnvironment("CUSTOMERS_API_URL", customersApi.GetEndpoint("http"))
    .WithEnvironment("Authentication__Keycloak__Authority", () => $"{keycloak.GetEndpoint("http")}/realms/orange-car-rental")
    .WithEnvironment("Authentication__Keycloak__Audience", "orange-car-rental-api")
    .WithEnvironment("Authentication__Keycloak__RequireHttpsMetadata", "false")
    .WithEnvironment("Authentication__Keycloak__ValidateIssuer", "true")
    .WithEnvironment("Authentication__Keycloak__ValidateAudience", "false")
    .WithExternalHttpEndpoints()
    .WaitFor(keycloak)
    .WaitFor(fleetApi)
    .WaitFor(reservationsApi)
    .WaitFor(pricingApi)
    .WaitFor(customersApi);

// Public Portal - Remote microfrontend for vehicle search and booking
// Accessible at http://localhost:4301
// Exposes remote entry point for Module Federation
// Using isProxied: false because Angular dev server binds directly to the port
// and Module Federation requires consistent URLs for remote entry points
var publicPortal = builder.AddJavaScriptApp("public-portal", "../../../frontend/apps/public-portal", "start")
    .WithYarn()
    .WithHttpEndpoint(4301, isProxied: false)
    .WithReference(apiGateway)
    .WithEnvironment("API_URL", apiGateway.GetEndpoint("http"))
    .WithExternalHttpEndpoints()
    .WaitFor(keycloak)
    .WaitFor(apiGateway);

// Call Center Portal - Remote microfrontend for reservation management
// Accessible at http://localhost:4302
// Exposes remote entry point for Module Federation
// Using isProxied: false because Angular dev server binds directly to the port
// and Module Federation requires consistent URLs for remote entry points
var callCenterPortal = builder.AddJavaScriptApp("call-center-portal", "../../../frontend/apps/call-center-portal", "start")
    .WithYarn()
    .WithHttpEndpoint(4302, isProxied: false)
    .WithReference(apiGateway)
    .WithEnvironment("API_URL", apiGateway.GetEndpoint("http"))
    .WithExternalHttpEndpoints()
    .WaitFor(keycloak)
    .WaitFor(apiGateway);

// Shell - Host microfrontend application that orchestrates remote microfrontends
// Main entry point accessible at http://localhost:4300
// Dynamically loads Public Portal and Call Center Portal via Module Federation
// Using isProxied: false because Angular dev server binds directly to the port
// and Module Federation requires consistent URLs for remote entry points
var shell = builder.AddJavaScriptApp("shell", "../../../frontend/apps/shell", "start")
    .WithYarn()
    .WithHttpEndpoint(4300, isProxied: false)
    .WithReference(apiGateway)
    .WithEnvironment("API_URL", apiGateway.GetEndpoint("http"))
    .WithEnvironment("PUBLIC_PORTAL_URL", publicPortal.GetEndpoint("http"))
    .WithEnvironment("CALLCENTER_PORTAL_URL", callCenterPortal.GetEndpoint("http"))
    .WithExternalHttpEndpoints()
    .WaitFor(keycloak)
    .WaitFor(apiGateway)
    .WaitFor(publicPortal)
    .WaitFor(callCenterPortal);

builder.Build().Run();
