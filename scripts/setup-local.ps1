# Orange Car Rental - Local Development Setup Script
# This script automates the initial setup for local development

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Orange Car Rental - Local Setup" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Check if Node.js is installed
Write-Host "Checking Node.js installation..." -ForegroundColor Yellow
try {
    $nodeVersion = node --version
    Write-Host "✓ Node.js version: $nodeVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ Node.js is not installed. Please install Node.js 20+ from https://nodejs.org/" -ForegroundColor Red
    exit 1
}

# Check if .NET is installed
Write-Host "Checking .NET installation..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "✓ .NET version: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ .NET is not installed. Please install .NET 8.0+ from https://dotnet.microsoft.com/" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Step 1: Installing Root Dependencies" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan

Set-Location $PSScriptRoot\..
npm install

if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Failed to install root dependencies" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Root dependencies installed" -ForegroundColor Green

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Step 2: Installing Playwright Browsers" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan

npx playwright install --with-deps

if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Failed to install Playwright browsers" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Playwright browsers installed" -ForegroundColor Green

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Step 3: Public Portal Setup" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan

Set-Location src\frontend\apps\public-portal
npm install

if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Failed to install public portal dependencies" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Public portal dependencies installed" -ForegroundColor Green

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Step 4: Call Center Portal Setup" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan

Set-Location ..\..\..\..\
Set-Location src\frontend\apps\call-center-portal
npm install

if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Failed to install call center portal dependencies" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Call center portal dependencies installed" -ForegroundColor Green

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Step 5: Backend Setup" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan

Set-Location ..\..\..\..\
Set-Location src\backend\Api
dotnet restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Failed to restore backend dependencies" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Backend dependencies restored" -ForegroundColor Green

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Step 6: Environment Configuration" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan

Set-Location ..\..\..

if (!(Test-Path ".env")) {
    if (Test-Path ".env.example") {
        Copy-Item ".env.example" ".env"
        Write-Host "✓ Created .env file from template" -ForegroundColor Green
        Write-Host "⚠ Please edit .env with your test credentials" -ForegroundColor Yellow
    } else {
        Write-Host "⚠ .env.example not found, skipping .env creation" -ForegroundColor Yellow
    }
} else {
    Write-Host "✓ .env file already exists" -ForegroundColor Green
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Setup Complete!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "1. Edit .env file with your test credentials" -ForegroundColor White
Write-Host "2. Start the backend: cd src\backend\Api && dotnet run" -ForegroundColor White
Write-Host "3. Start public portal: cd src\frontend\apps\public-portal && npm start" -ForegroundColor White
Write-Host "4. Start call center: cd src\frontend\apps\call-center-portal && npm start" -ForegroundColor White
Write-Host "5. Run tests: npm test" -ForegroundColor White
Write-Host ""
Write-Host "For more information, see:" -ForegroundColor Yellow
Write-Host "- QUICK-START-TESTING.md" -ForegroundColor White
Write-Host "- WHATS-NEXT.md" -ForegroundColor White
Write-Host "- README.md" -ForegroundColor White
Write-Host ""
Write-Host "Happy coding! 🚀" -ForegroundColor Green
