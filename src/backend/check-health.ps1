# Orange Car Rental - Health Check Script
# Verifies all services are responding

Write-Host "🔍 Orange Car Rental - Service Health Check" -ForegroundColor Cyan
Write-Host ""

function Test-ServiceHealth {
    param(
        [string]$Name,
        [string]$Url,
        [int]$ExpectedStatus = 200
    )

    try {
        $response = Invoke-WebRequest -Uri $Url -TimeoutSec 3 -ErrorAction Stop
        if ($response.StatusCode -eq $ExpectedStatus) {
            Write-Host "✅ $Name" -ForegroundColor Green -NoNewline
            Write-Host " - " -NoNewline
            $content = $response.Content | ConvertFrom-Json
            if ($content.status) {
                Write-Host "$($content.status)" -ForegroundColor Gray
            } else {
                Write-Host "OK" -ForegroundColor Gray
            }
            return $true
        }
        else {
            Write-Host "⚠️  $Name - Unexpected status: $($response.StatusCode)" -ForegroundColor Yellow
            return $false
        }
    }
    catch {
        Write-Host "❌ $Name - Not responding" -ForegroundColor Red
        return $false
    }
}

$services = @(
    @{Name="API Gateway (Port 5002)"; Url="http://localhost:5002/health"},
    @{Name="Fleet API (Port 5000)"; Url="http://localhost:5000/health"},
    @{Name="Reservations API (Port 5001)"; Url="http://localhost:5001/health"},
    @{Name="Customers API (Port 5003)"; Url="http://localhost:5003/health"},
    @{Name="Payments API (Port 5004)"; Url="http://localhost:5004/health"},
    @{Name="Notifications API (Port 5005)"; Url="http://localhost:5005/health"},
    @{Name="Locations API (Port 5006)"; Url="http://localhost:5006/health"}
)

$healthyCount = 0
$totalCount = $services.Count

foreach ($service in $services) {
    if (Test-ServiceHealth -Name $service.Name -Url $service.Url) {
        $healthyCount++
    }
    Start-Sleep -Milliseconds 100
}

Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray
Write-Host ""

if ($healthyCount -eq $totalCount) {
    Write-Host "🎉 All $totalCount services are healthy!" -ForegroundColor Green
    Write-Host ""
    Write-Host "✨ System is ready for testing!" -ForegroundColor Cyan
} elseif ($healthyCount -gt 0) {
    Write-Host "⚠️  Partial: $healthyCount/$totalCount services healthy" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "💡 Try:" -ForegroundColor Yellow
    Write-Host "   1. Check individual service windows for errors"
    Write-Host "   2. Run .\stop-all-services.ps1"
    Write-Host "   3. Run .\start-all-services.ps1 again"
} else {
    Write-Host "❌ No services responding" -ForegroundColor Red
    Write-Host ""
    Write-Host "💡 Try:" -ForegroundColor Yellow
    Write-Host "   1. Run .\start-all-services.ps1"
    Write-Host "   2. Wait 45 seconds for services to start"
    Write-Host "   3. Run this script again"
}

Write-Host ""
