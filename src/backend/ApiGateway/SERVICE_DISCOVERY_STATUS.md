# API Gateway Service Discovery Status

## Current Configuration

### What We've Set Up

1. **Service Discovery Package**
   ```xml
   <PackageReference Include="Microsoft.Extensions.ServiceDiscovery" Version="9.5.2" />
   ```

2. **Service Discovery Registration** (`Program.cs`)
   ```csharp
   builder.Services.AddServiceDiscovery();
   builder.Services.ConfigureHttpClientDefaults(http =>
   {
       http.AddServiceDiscovery();
   });
   ```

3. **YARP Configuration** (`appsettings.json`)
   ```json
   {
     "Clusters": {
       "fleet-cluster": {
         "Destinations": {
           "destination1": {
             "Address": "http://_fleet-api"
           }
         }
       }
     }
   }
   ```

### Service Discovery in Aspire

When you use `.WithReference(fleetApi)` in the AppHost, Aspire provides service endpoints through:
- Environment variables
- Configuration providers
- Service discovery resolution

The `http://_fleet-api` format tells the service discovery system to resolve `fleet-api` to its actual HTTP endpoint.

## Is It Working?

**Short answer:** Yes, it should be working with the current configuration.

**How to verify:**

### 1. Check Aspire Dashboard

When you run the AppHost, open the Aspire Dashboard (usually at `http://localhost:15888` or shown in the console output):

- Look for the "Resources" tab
- Check that `fleet-api`, `reservations-api`, and `api-gateway` are all showing as "Running"
- Click on each service to see their actual endpoints
- Verify the gateway can see the other services

### 2. Check Gateway Logs

Start the Aspire app and look at the API Gateway logs:

```
Starting application...
Service discovery registered for: fleet-api
Service discovery registered for: reservations-api
```

You should see YARP logs showing it's resolving destinations.

### 3. Test API Calls

**Direct to Fleet API:**
```bash
# Get the Fleet API URL from Aspire Dashboard (e.g., http://localhost:5123)
curl http://localhost:5123/api/vehicles
```

**Through API Gateway:**
```bash
# Gateway should be on port 5002
curl http://localhost:5002/api/vehicles
```

If both return the same vehicle data, service discovery is working!

### 4. Check Integration Tests

```bash
# Stop Aspire first!
cd src/backend/Tests/OrangeCarRental.IntegrationTests
dotnet test --filter ClassName=ApiGatewayTests
```

The tests will spin up the entire Aspire app and verify:
- ✅ Gateway health check works
- ✅ Gateway routes to Fleet API
- ✅ Gateway routes to Reservations API
- ✅ Services can communicate

## Common Issues and Solutions

### Issue: 503 Service Unavailable

**Symptom:** API Gateway returns 503 for `/api/vehicles`

**Possible Causes:**
1. **Backend service not running** - Check Aspire Dashboard
2. **Service name mismatch** - Verify `_fleet-api` matches the resource name in AppHost
3. **Service discovery not resolving** - Check gateway logs for resolution failures

**Solution:**
```bash
# Check service names in AppHost match configuration
# AppHost: builder.AddProject<...>("fleet-api")
# Config: "Address": "http://_fleet-api"
```

### Issue: CORS Errors

**Symptom:** Browser blocks requests from frontend

**Solution:** Already configured for ports 4200/4201. If using different ports, update CORS in `Program.cs`.

### Issue: Service Discovery Not Resolving

**Symptom:** Gateway logs show "Failed to resolve destination"

**Possible Causes:**
1. Missing `.WithReference()` in AppHost
2. Service name doesn't match
3. Service discovery middleware not configured

**Current Status:** ✅ All configured correctly

## Configuration Summary

| Component | Status | Notes |
|-----------|--------|-------|
| Service Discovery Package | ✅ Installed | Microsoft.Extensions.ServiceDiscovery 9.5.2 |
| Service Discovery Registration | ✅ Configured | AddServiceDiscovery() in Program.cs |
| HttpClient Integration | ✅ Configured | ConfigureHttpClientDefaults with service discovery |
| YARP Configuration | ✅ Configured | Using `http://_fleet-api` format |
| Aspire References | ✅ Configured | .WithReference() in AppHost |
| Route Patterns | ✅ Configured | /api/vehicles/* and /api/reservations/* |

## Next Steps to Verify

1. **Stop the currently running Aspire instance** (if any)
2. **Start fresh from AppHost:**
   ```bash
   cd src/backend/AppHost/OrangeCarRental.AppHost
   dotnet run
   ```
3. **Check Aspire Dashboard** - All services should be "Running"
4. **Test gateway endpoint:**
   ```bash
   curl http://localhost:5002/health
   curl http://localhost:5002/api/vehicles
   ```
5. **Check logs** for service discovery resolution messages

## Expected Behavior

When working correctly, you should see:

1. **Aspire Dashboard:** All services showing as "Running" with green status
2. **Gateway Logs:** No "Failed to resolve" errors
3. **API Responses:** Gateway returns same data as direct service calls
4. **Integration Tests:** All ApiGatewayTests pass

## Conclusion

**Service discovery IS configured and SHOULD be working** with the current setup. The `http://_fleet-api` format combined with `AddServiceDiscovery()` and `ConfigureHttpClientDefaults()` enables automatic service resolution through Aspire's service discovery system.

To confirm it's working in your environment, follow the verification steps above.
