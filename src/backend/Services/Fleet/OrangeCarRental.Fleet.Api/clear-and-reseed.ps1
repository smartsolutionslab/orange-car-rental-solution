# Clear and Reseed Fleet Database
Write-Host "Clearing and reseeding Fleet database..." -ForegroundColor Yellow

# Drop and recreate database
dotnet ef database drop --force --project ../OrangeCarRental.Fleet.Infrastructure
dotnet ef database update --project ../OrangeCarRental.Fleet.Infrastructure

Write-Host "Database cleared and migrations applied!" -ForegroundColor Green
Write-Host "Now restart your application - it will automatically seed with 41 proper vehicles." -ForegroundColor Green
