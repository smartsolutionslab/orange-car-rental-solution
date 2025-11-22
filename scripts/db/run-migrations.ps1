# ============================================================================
# Orange Car Rental - Database Migration Script (PowerShell)
# ============================================================================
# This script runs EF Core migrations for all microservices
# Run from project root directory
# ============================================================================

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("PostgreSQL", "SQLServer")]
    [string]$Provider = "PostgreSQL",

    [Parameter(Mandatory=$false)]
    [switch]$CreateMigration,

    [Parameter(Mandatory=$false)]
    [string]$MigrationName = "NewMigration"
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Orange Car Rental - Database Migrations" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Provider: $Provider" -ForegroundColor Yellow
Write-Host ""

# Define services and their paths
$services = @(
    @{Name="Customers"; Path="src/backend/Services/Customers"},
    @{Name="Fleet"; Path="src/backend/Services/Fleet"},
    @{Name="Location"; Path="src/backend/Services/Location"},
    @{Name="Reservations"; Path="src/backend/Services/Reservations"},
    @{Name="Pricing"; Path="src/backend/Services/Pricing"},
    @{Name="Payments"; Path="src/backend/Services/Payments"},
    @{Name="Notifications"; Path="src/backend/Services/Notifications"}
)

$successCount = 0
$failureCount = 0
$failedServices = @()

foreach ($service in $services) {
    $serviceName = $service.Name
    $apiPath = Join-Path $service.Path "OrangeCarRental.$serviceName.Api"

    Write-Host "Processing: $serviceName Service..." -ForegroundColor Yellow

    if (-not (Test-Path $apiPath)) {
        Write-Host "  ERROR: Path not found: $apiPath" -ForegroundColor Red
        $failureCount++
        $failedServices += $serviceName
        continue
    }

    try {
        Push-Location $apiPath

        if ($CreateMigration) {
            Write-Host "  Creating new migration: $MigrationName" -ForegroundColor Cyan
            dotnet ef migrations add $MigrationName --verbose
        } else {
            Write-Host "  Applying migrations..." -ForegroundColor Cyan
            dotnet ef database update --verbose
        }

        if ($LASTEXITCODE -eq 0) {
            Write-Host "  SUCCESS: $serviceName" -ForegroundColor Green
            $successCount++
        } else {
            Write-Host "  FAILED: $serviceName (Exit code: $LASTEXITCODE)" -ForegroundColor Red
            $failureCount++
            $failedServices += $serviceName
        }
    }
    catch {
        Write-Host "  ERROR: $($_.Exception.Message)" -ForegroundColor Red
        $failureCount++
        $failedServices += $serviceName
    }
    finally {
        Pop-Location
    }

    Write-Host ""
}

# Summary
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Migration Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Successful: $successCount" -ForegroundColor Green
Write-Host "Failed: $failureCount" -ForegroundColor $(if ($failureCount -gt 0) { "Red" } else { "Green" })

if ($failedServices.Count -gt 0) {
    Write-Host ""
    Write-Host "Failed Services:" -ForegroundColor Red
    foreach ($failed in $failedServices) {
        Write-Host "  - $failed" -ForegroundColor Red
    }
}

Write-Host "========================================" -ForegroundColor Cyan

# Exit with appropriate code
exit $(if ($failureCount -gt 0) { 1 } else { 0 })
