# Orange Car Rental - Stop All Backend Services
# Run this script to kill all running OrangeCarRental processes

Write-Host "🛑 Stopping all Orange Car Rental services..." -ForegroundColor Red
Write-Host ""

# Find all OrangeCarRental processes
$processes = Get-Process | Where-Object {
    $_.ProcessName -like "*OrangeCarRental*" -or
    $_.MainWindowTitle -like "*OrangeCarRental*"
}

if ($processes.Count -eq 0) {
    Write-Host "✅ No services are currently running." -ForegroundColor Green
}
else {
    Write-Host "Found $($processes.Count) running service(s):" -ForegroundColor Yellow
    $processes | ForEach-Object {
        Write-Host "   • $($_.ProcessName) (PID: $($_.Id))" -ForegroundColor Gray
    }
    Write-Host ""

    Write-Host "Stopping processes..." -ForegroundColor Yellow
    $processes | Stop-Process -Force

    Start-Sleep -Seconds 2

    # Verify they're stopped
    $remaining = Get-Process | Where-Object {
        $_.ProcessName -like "*OrangeCarRental*" -or
        $_.MainWindowTitle -like "*OrangeCarRental*"
    }

    if ($remaining.Count -eq 0) {
        Write-Host "✅ All services stopped successfully!" -ForegroundColor Green
    }
    else {
        Write-Host "⚠️  $($remaining.Count) process(es) could not be stopped." -ForegroundColor Yellow
        Write-Host "   Try running this script again or restart your computer." -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
