# ============================================================================
# Orange Car Rental - Database Seed Data Script (PowerShell)
# ============================================================================
# This script applies seed data to the PostgreSQL database
# Run after migrations have been applied
# ============================================================================

param(
    [Parameter(Mandatory=$false)]
    [string]$Host = "localhost",

    [Parameter(Mandatory=$false)]
    [int]$Port = 5432,

    [Parameter(Mandatory=$false)]
    [string]$Username = "orange_user",

    [Parameter(Mandatory=$false)]
    [string]$Password = "orange_dev_password",

    [Parameter(Mandatory=$false)]
    [string]$Database = "orange_rental"
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Orange Car Rental - Seed Data" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Host:     $Host`:$Port" -ForegroundColor Yellow
Write-Host "Database: $Database" -ForegroundColor Yellow
Write-Host "User:     $Username" -ForegroundColor Yellow
Write-Host ""

$seedFile = "scripts/db/seed-test-data.sql"

if (-not (Test-Path $seedFile)) {
    Write-Host "ERROR: Seed file not found: $seedFile" -ForegroundColor Red
    exit 1
}

Write-Host "Checking if PostgreSQL client (psql) is installed..." -ForegroundColor Cyan

try {
    $psqlVersion = & psql --version 2>&1
    Write-Host "Found: $psqlVersion" -ForegroundColor Green
}
catch {
    Write-Host "ERROR: psql not found. Please install PostgreSQL client tools." -ForegroundColor Red
    Write-Host ""
    Write-Host "Alternative: Run seed data inside Docker container:" -ForegroundColor Yellow
    Write-Host "  docker exec -i orange-rental-db psql -U $Username -d $Database < $seedFile" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "Applying seed data..." -ForegroundColor Cyan

$env:PGPASSWORD = $Password

try {
    & psql -h $Host -p $Port -U $Username -d $Database -f $seedFile

    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host "SUCCESS: Seed data applied successfully!" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Cyan
    } else {
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host "FAILED: Error applying seed data" -ForegroundColor Red
        Write-Host "========================================" -ForegroundColor Cyan
        exit 1
    }
}
catch {
    Write-Host ""
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
finally {
    Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
}
