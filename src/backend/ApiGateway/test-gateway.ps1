# API Gateway Service Discovery Test Script
# Run this script to verify the API Gateway is working correctly

Write-Host "`n=== Orange Car Rental - API Gateway Test ===" -ForegroundColor Cyan
Write-Host "This script will test if the API Gateway can route requests to backend services`n" -ForegroundColor Gray

# Test 1: Gateway Health Check
Write-Host "[Test 1] Testing API Gateway Health..." -ForegroundColor Yellow
try {
    $healthResponse = Invoke-RestMethod -Uri "http://localhost:5002/health" -Method Get -TimeoutSec 5
    Write-Host "✓ Gateway is healthy!" -ForegroundColor Green
    Write-Host "  Status: $($healthResponse.status)" -ForegroundColor Gray
    Write-Host "  Service: $($healthResponse.service)`n" -ForegroundColor Gray
} catch {
    Write-Host "✗ Gateway health check failed!" -ForegroundColor Red
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "  Make sure Aspire AppHost is running!`n" -ForegroundColor Yellow
    exit 1
}

# Test 2: Gateway → Fleet API (Vehicle Search)
Write-Host "[Test 2] Testing Gateway routing to Fleet API..." -ForegroundColor Yellow
try {
    $vehiclesResponse = Invoke-RestMethod -Uri "http://localhost:5002/api/vehicles?pageSize=1" -Method Get -TimeoutSec 10
    Write-Host "✓ Gateway successfully routed to Fleet API!" -ForegroundColor Green
    Write-Host "  Total vehicles: $($vehiclesResponse.totalCount)" -ForegroundColor Gray
    Write-Host "  Page size: $($vehiclesResponse.pageSize)" -ForegroundColor Gray
    if ($vehiclesResponse.vehicles.Count -gt 0) {
        $vehicle = $vehiclesResponse.vehicles[0]
        Write-Host "  Sample vehicle: $($vehicle.name) - $($vehicle.dailyRateGross) EUR`n" -ForegroundColor Gray
    }
} catch {
    Write-Host "✗ Failed to route to Fleet API!" -ForegroundColor Red
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "  Status Code: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red

    if ($_.Exception.Response.StatusCode.value__ -eq 503) {
        Write-Host "`n  → Service discovery might not be resolving 'fleet-api'" -ForegroundColor Yellow
        Write-Host "  → Check Aspire Dashboard - is Fleet API running?`n" -ForegroundColor Yellow
    }
}

# Test 3: Gateway → Reservations API (Get non-existent reservation)
Write-Host "[Test 3] Testing Gateway routing to Reservations API..." -ForegroundColor Yellow
try {
    $reservationResponse = Invoke-RestMethod -Uri "http://localhost:5002/api/reservations/00000000-0000-0000-0000-000000000001" -Method Get -TimeoutSec 5 -ErrorAction Stop
    Write-Host "✗ Expected 404, but got a response!" -ForegroundColor Red
} catch {
    if ($_.Exception.Response.StatusCode.value__ -eq 404) {
        Write-Host "✓ Gateway successfully routed to Reservations API!" -ForegroundColor Green
        Write-Host "  Got expected 404 (Not Found) - routing works!`n" -ForegroundColor Gray
    } elseif ($_.Exception.Response.StatusCode.value__ -eq 503) {
        Write-Host "✗ Failed to route to Reservations API!" -ForegroundColor Red
        Write-Host "  Error: Service Unavailable (503)" -ForegroundColor Red
        Write-Host "`n  → Service discovery might not be resolving 'reservations-api'" -ForegroundColor Yellow
        Write-Host "  → Check Aspire Dashboard - is Reservations API running?`n" -ForegroundColor Yellow
    } else {
        Write-Host "✗ Unexpected error!" -ForegroundColor Red
        Write-Host "  Status Code: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
        Write-Host "  Error: $($_.Exception.Message)`n" -ForegroundColor Red
    }
}

# Test 4: Direct Fleet API call (for comparison)
Write-Host "[Test 4] Testing direct Fleet API access (bypass gateway)..." -ForegroundColor Yellow
Write-Host "  Checking Aspire Dashboard to find Fleet API port..." -ForegroundColor Gray

# Try common ports that Aspire might assign
$fleetPorts = @(5000, 5001, 5123, 5124, 7000, 7001, 7100, 7101)
$fleetFound = $false

foreach ($port in $fleetPorts) {
    try {
        $directResponse = Invoke-RestMethod -Uri "http://localhost:$port/api/vehicles?pageSize=1" -Method Get -TimeoutSec 2 -ErrorAction Stop
        Write-Host "✓ Found Fleet API directly on port $port!" -ForegroundColor Green
        Write-Host "  Direct call returned $($directResponse.totalCount) total vehicles`n" -ForegroundColor Gray
        $fleetFound = $true
        break
    } catch {
        # Port not accessible, try next
    }
}

if (-not $fleetFound) {
    Write-Host "  ℹ Could not find Fleet API on common ports" -ForegroundColor Gray
    Write-Host "  (This is OK - check Aspire Dashboard for the actual port)`n" -ForegroundColor Gray
}

# Summary
Write-Host "`n=== Test Summary ===" -ForegroundColor Cyan
Write-Host "If Tests 1-3 passed, your API Gateway service discovery is working correctly!" -ForegroundColor Green
Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "  1. Open Aspire Dashboard (check console for URL, usually http://localhost:15888)" -ForegroundColor Gray
Write-Host "  2. Verify all services show 'Running' status" -ForegroundColor Gray
Write-Host "  3. Click on each service to see their actual HTTP endpoints" -ForegroundColor Gray
Write-Host "  4. Run integration tests: cd Tests/OrangeCarRental.IntegrationTests && dotnet test`n" -ForegroundColor Gray
