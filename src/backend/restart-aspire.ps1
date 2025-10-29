# Restart Aspire with API Gateway Fix
Write-Host "`n=== Stopping All Running Processes ===" -ForegroundColor Yellow

# Stop all .NET processes related to the project
$processes = Get-Process | Where-Object {
    $_.ProcessName -like "*OrangeCarRental*" -or
    ($_.ProcessName -eq "dotnet" -and $_.Path -like "*claude-orange-car-rental*")
}

if ($processes) {
    Write-Host "Found $($processes.Count) running processes" -ForegroundColor Gray
    $processes | ForEach-Object {
        Write-Host "  Stopping: $($_.ProcessName) (PID: $($_.Id))" -ForegroundColor Gray
        Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue
    }
    Write-Host "All processes stopped" -ForegroundColor Green
    Start-Sleep -Seconds 2
} else {
    Write-Host "No running processes found" -ForegroundColor Green
}

Write-Host "`n=== Building Solution ===" -ForegroundColor Yellow
Set-Location "C:\Users\heiko\claude-orange-car-rental\src\backend"
dotnet build --no-incremental

if ($LASTEXITCODE -ne 0) {
    Write-Host "`n✗ Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "`n✓ Build succeeded!" -ForegroundColor Green

Write-Host "`n=== Starting Aspire AppHost ===" -ForegroundColor Yellow
Write-Host "Watch for these lines in the output:" -ForegroundColor Gray
Write-Host "  - Fleet API URL: http://localhost:XXXXX" -ForegroundColor Cyan
Write-Host "  - Reservations API URL: http://localhost:XXXXX" -ForegroundColor Cyan
Write-Host ""

Set-Location "AppHost\OrangeCarRental.AppHost"
dotnet run
