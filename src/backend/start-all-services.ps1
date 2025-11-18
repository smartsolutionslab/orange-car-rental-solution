# Orange Car Rental - Start All Backend Services
# Run this script from the backend directory

Write-Host "🚀 Starting Orange Car Rental Backend Services..." -ForegroundColor Green
Write-Host ""

# Check if running from correct directory
if (-not (Test-Path "ApiGateway")) {
    Write-Host "❌ Error: Please run this script from the backend directory" -ForegroundColor Red
    Write-Host "   cd src/backend" -ForegroundColor Yellow
    exit 1
}

Write-Host "📋 Port Assignments:" -ForegroundColor Cyan
Write-Host "   API Gateway:     http://localhost:5002"
Write-Host "   Fleet API:       http://localhost:5000"
Write-Host "   Reservations:    http://localhost:5001"
Write-Host "   Customers:       http://localhost:5003"
Write-Host "   Payments:        http://localhost:5004"
Write-Host "   Notifications:   http://localhost:5005"
Write-Host "   Locations:       http://localhost:5006"
Write-Host ""

# Function to start a service in a new window
function Start-Service {
    param(
        [string]$Name,
        [string]$Project,
        [string]$Port
    )

    Write-Host "▶️  Starting $Name on port $Port..." -ForegroundColor Yellow

    $title = "OrangeCarRental - $Name"
    Start-Process pwsh -ArgumentList `
        "-NoExit", `
        "-Command", `
        "& { `$host.UI.RawUI.WindowTitle = '$title'; dotnet run --project $Project --no-launch-profile -- --urls `"http://localhost:$Port`" }"

    Start-Sleep -Milliseconds 500
}

Write-Host "🏗️  Building all projects first..." -ForegroundColor Cyan
dotnet build --nologo --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed! Please fix errors and try again." -ForegroundColor Red
    exit 1
}
Write-Host "✅ Build successful!" -ForegroundColor Green
Write-Host ""

Write-Host "🚀 Launching services in separate windows..." -ForegroundColor Cyan
Write-Host ""

# Start services in order
Start-Service "API Gateway" "ApiGateway/OrangeCarRental.ApiGateway" "5002"
Start-Service "Fleet API" "Services/Fleet/OrangeCarRental.Fleet.Api" "5000"
Start-Service "Reservations API" "Services/Reservations/OrangeCarRental.Reservations.Api" "5001"
Start-Service "Customers API" "Services/Customers/OrangeCarRental.Customers.Api" "5003"
Start-Service "Payments API" "Services/Payments/OrangeCarRental.Payments.Api" "5004"
Start-Service "Notifications API" "Services/Notifications/OrangeCarRental.Notifications.Api" "5005"
Start-Service "Locations API" "Services/Location/OrangeCarRental.Location.Api" "5006"

Write-Host ""
Write-Host "⏳ Waiting for services to start (45 seconds)..." -ForegroundColor Yellow
Write-Host ""

# Wait for services to initialize
Start-Sleep -Seconds 45

Write-Host "🔍 Checking service health..." -ForegroundColor Cyan
Write-Host ""

# Function to check health
function Test-ServiceHealth {
    param(
        [string]$Name,
        [string]$Url
    )

    try {
        $response = Invoke-RestMethod -Uri $Url -TimeoutSec 5 -ErrorAction Stop
        Write-Host "   ✅ $Name is healthy" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "   ❌ $Name is not responding" -ForegroundColor Red
        return $false
    }
}

$healthyCount = 0
$totalServices = 7

if (Test-ServiceHealth "API Gateway" "http://localhost:5002/health") { $healthyCount++ }
if (Test-ServiceHealth "Fleet API" "http://localhost:5000/health") { $healthyCount++ }
if (Test-ServiceHealth "Reservations" "http://localhost:5001/health") { $healthyCount++ }
if (Test-ServiceHealth "Customers" "http://localhost:5003/health") { $healthyCount++ }
if (Test-ServiceHealth "Payments" "http://localhost:5004/health") { $healthyCount++ }
if (Test-ServiceHealth "Notifications" "http://localhost:5005/health") { $healthyCount++ }
if (Test-ServiceHealth "Locations" "http://localhost:5006/health") { $healthyCount++ }

Write-Host ""
Write-Host "📊 Status: $healthyCount/$totalServices services healthy" -ForegroundColor $(if ($healthyCount -eq $totalServices) { "Green" } else { "Yellow" })
Write-Host ""

if ($healthyCount -eq $totalServices) {
    Write-Host "🎉 All services started successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📖 Quick Links:" -ForegroundColor Cyan
    Write-Host "   • API Gateway:        http://localhost:5002/health"
    Write-Host "   • Fleet API Docs:     http://localhost:5000/scalar/v1"
    Write-Host "   • Reservations Docs:  http://localhost:5001/scalar/v1"
    Write-Host "   • Customers Docs:     http://localhost:5003/scalar/v1"
    Write-Host "   • Payments Docs:      http://localhost:5004/scalar/v1"
    Write-Host "   • Notifications Docs: http://localhost:5005/scalar/v1"
    Write-Host "   • Locations Docs:     http://localhost:5006/scalar/v1"
    Write-Host ""
    Write-Host "💡 Next Steps:" -ForegroundColor Yellow
    Write-Host "   1. Start frontend: cd ../frontend && npm start"
    Write-Host "   2. Open browser:   http://localhost:4200"
    Write-Host "   3. Test booking flow"
    Write-Host ""
    Write-Host "🛑 To stop all services: Run .\stop-all-services.ps1" -ForegroundColor Red
}
else {
    Write-Host "⚠️  Some services failed to start. Check the individual windows for errors." -ForegroundColor Yellow
    Write-Host "   Try running .\stop-all-services.ps1 and then starting again." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Press any key to close this window..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
